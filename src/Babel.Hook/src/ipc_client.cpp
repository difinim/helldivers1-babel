#include "ipc_client.h"
#include "hook_log.h"
#include <windows.h>
#include <string>
#include <vector>
#include <mutex>
#include <thread>
#include <atomic>
#include <chrono>
#include <unordered_map>
#include <cstdint>
#include <cstdio>

// Thread control and pipe state
static std::atomic<bool> g_ipc_running{false};
static std::atomic<bool> g_is_connected{false};
static std::atomic<bool> g_is_paused{false};

static HANDLE g_hPipe = INVALID_HANDLE_VALUE;
static std::mutex g_pipe_mutex;
static std::thread g_worker_thread;

// Outgoing queue (Hook -> C# Desktop App)
static std::mutex g_out_mutex;
static std::vector<std::string> g_outgoing_queue;

// Incoming queues (C# Desktop App -> Hook)
static std::mutex g_inject_mutex;
static std::vector<InjectChatPayload> g_inject_queue;

static std::mutex g_broadcast_mutex;
static std::vector<ExecuteBroadcastPayload> g_broadcast_queue;

// Forward declaration of worker thread function
static void ipc_worker_thread_proc();

// ============================================================================
// JSON Serialization & Parsing Helpers
// ============================================================================

static std::string json_escape(const std::string& input) {
    std::string out;
    out.reserve(input.size() + 16);
    for (unsigned char c : input) {
        switch (c) {
            case '\"': out += "\\\""; break;
            case '\\': out += "\\\\"; break;
            case '\b': out += "\\b"; break;
            case '\f': out += "\\f"; break;
            case '\n': out += "\\n"; break;
            case '\r': out += "\\r"; break;
            case '\t': out += "\\t"; break;
            default:
                if (c < 0x20) {
                    char buf[8];
                    snprintf(buf, sizeof(buf), "\\u%04x", c);
                    out += buf;
                } else {
                    out += (char)c;
                }
                break;
        }
    }
    return out;
}

static std::string json_unescape(const std::string& input) {
    std::string out;
    out.reserve(input.size());
    for (size_t i = 0; i < input.size(); ++i) {
        if (input[i] == '\\' && i + 1 < input.size()) {
            char next = input[++i];
            switch (next) {
                case '\"': out += '\"'; break;
                case '\\': out += '\\'; break;
                case '/':  out += '/';  break;
                case 'b':  out += '\b'; break;
                case 'f':  out += '\f'; break;
                case 'n':  out += '\n'; break;
                case 'r':  out += '\r'; break;
                case 't':  out += '\t'; break;
                case 'u': {
                    if (i + 4 < input.size()) {
                        std::string hex_str = input.substr(i + 1, 4);
                        i += 4;
                        unsigned int codepoint = 0;
                        try {
                            codepoint = (unsigned int)std::stoul(hex_str, nullptr, 16);
                        } catch (...) { codepoint = 0; }

                        if (codepoint <= 0x7F) {
                            out += (char)codepoint;
                        } else if (codepoint <= 0x7FF) {
                            out += (char)(0xC0 | ((codepoint >> 6) & 0x1F));
                            out += (char)(0x80 | (codepoint & 0x3F));
                        } else {
                            out += (char)(0xE0 | ((codepoint >> 12) & 0x0F));
                            out += (char)(0x80 | ((codepoint >> 6) & 0x3F));
                            out += (char)(0x80 | (codepoint & 0x3F));
                        }
                    }
                    break;
                }
                default:
                    out += '\\';
                    out += next;
                    break;
            }
        } else {
            out += input[i];
        }
    }
    return out;
}

static void skip_ws(const std::string& s, size_t& i) {
    while (i < s.size() && (s[i] == ' ' || s[i] == '\t' || s[i] == '\r' || s[i] == '\n')) {
        i++;
    }
}

static std::unordered_map<std::string, std::string> parse_json_object(const std::string& json) {
    std::unordered_map<std::string, std::string> map;
    size_t i = 0;
    skip_ws(json, i);
    if (i >= json.size() || json[i] != '{') return map;
    i++; // skip '{'

    while (i < json.size()) {
        skip_ws(json, i);
        if (i >= json.size() || json[i] == '}') break;

        // Expect key string
        if (json[i] != '\"') break;
        i++; // skip '"'
        std::string key_raw;
        while (i < json.size()) {
            if (json[i] == '\\' && i + 1 < json.size()) {
                key_raw += json[i];
                key_raw += json[i + 1];
                i += 2;
            } else if (json[i] == '\"') {
                i++; // skip closing '"'
                break;
            } else {
                key_raw += json[i++];
            }
        }
        std::string key = json_unescape(key_raw);

        skip_ws(json, i);
        if (i >= json.size() || json[i] != ':') break;
        i++; // skip ':'
        skip_ws(json, i);

        // Read value
        std::string val;
        if (i < json.size() && json[i] == '\"') {
            i++; // skip '"'
            std::string val_raw;
            while (i < json.size()) {
                if (json[i] == '\\' && i + 1 < json.size()) {
                    val_raw += json[i];
                    val_raw += json[i + 1];
                    i += 2;
                } else if (json[i] == '\"') {
                    i++; // skip closing '"'
                    break;
                } else {
                    val_raw += json[i++];
                }
            }
            val = json_unescape(val_raw);
        } else if (i < json.size() && (json[i] == '{' || json[i] == '[')) {
            char open_char = json[i];
            char close_char = (open_char == '{') ? '}' : ']';
            int depth = 0;
            bool in_str = false;
            size_t start = i;
            while (i < json.size()) {
                if (in_str) {
                    if (json[i] == '\\' && i + 1 < json.size()) {
                        i += 2;
                    } else if (json[i] == '\"') {
                        in_str = false;
                        i++;
                    } else {
                        i++;
                    }
                } else {
                    if (json[i] == '\"') {
                        in_str = true;
                        i++;
                    } else if (json[i] == open_char) {
                        depth++;
                        i++;
                    } else if (json[i] == close_char) {
                        depth--;
                        i++;
                        if (depth == 0) break;
                    } else {
                        i++;
                    }
                }
            }
            val = json.substr(start, i - start);
        } else {
            // Read primitive until ',' or '}'
            size_t start = i;
            while (i < json.size() && json[i] != ',' && json[i] != '}' && json[i] != ' ' && json[i] != '\t' && json[i] != '\r' && json[i] != '\n') {
                i++;
            }
            val = json.substr(start, i - start);
        }

        map[key] = val;

        skip_ws(json, i);
        if (i < json.size() && json[i] == ',') {
            i++; // skip ','
        }
    }
    return map;
}

static std::string format_incoming_chat_packet(const IncomingChatPayload& p) {
    std::string inner = "{";
    inner += "\"peer_id\":\"" + json_escape(p.peer_id) + "\",";
    inner += "\"player_index\":" + std::to_string(p.player_index) + ",";
    inner += "\"player_name\":\"" + json_escape(p.player_name) + "\",";
    inner += "\"r\":" + std::to_string(p.r) + ",";
    inner += "\"g\":" + std::to_string(p.g) + ",";
    inner += "\"b\":" + std::to_string(p.b) + ",";
    inner += "\"message\":\"" + json_escape(p.message) + "\",";
    inner += "\"is_local\":" + std::string(p.is_local ? "true" : "false");
    inner += "}";

    std::string outer = "{\"type\":\"ChatMessageIncoming\",\"payload\":\"" + json_escape(inner) + "\"}\n";
    return outer;
}

static std::string format_outgoing_chat_packet(const OutgoingChatPayload& p) {
    std::string inner = "{";
    inner += "\"player_index\":" + std::to_string(p.player_index) + ",";
    inner += "\"player_name\":\"" + json_escape(p.player_name) + "\",";
    inner += "\"message\":\"" + json_escape(p.message) + "\"";
    inner += "}";

    std::string outer = "{\"type\":\"ChatMessageOutgoing\",\"payload\":\"" + json_escape(inner) + "\"}\n";
    return outer;
}

static std::string format_player_joined_packet(const PlayerJoinedPayload& p) {
    std::string inner = "{";
    inner += "\"peer_id\":\"" + json_escape(p.peer_id) + "\",";
    inner += "\"player_name\":\"" + json_escape(p.player_name) + "\",";
    inner += "\"is_host\":" + std::string(p.is_host ? "true" : "false");
    inner += "}";

    std::string outer = "{\"type\":\"PlayerJoined\",\"payload\":\"" + json_escape(inner) + "\"}\n";
    return outer;
}

static std::string format_heartbeat_packet() {
    return "{\"type\":\"Heartbeat\",\"payload\":\"\"}\n";
}

static void handle_received_packet_line(const std::string& line) {
    if (line.empty()) return;
    auto outer = parse_json_object(line);
    auto itType = outer.find("type");
    if (itType == outer.end()) return;

    const std::string& type_str = itType->second;
    std::string payload_str;
    auto itPayload = outer.find("payload");
    if (itPayload != outer.end()) {
        payload_str = itPayload->second;
    }

    if (type_str == "InjectChatMessage") {
        auto inner = parse_json_object(payload_str);
        InjectChatPayload p;
        if (inner.count("player_index")) {
            try { p.player_index = std::stoi(inner["player_index"]); } catch (...) {}
        }
        if (inner.count("player_name")) p.player_name = inner["player_name"];
        if (inner.count("r")) {
            try { p.r = (uint8_t)std::stoi(inner["r"]); } catch (...) {}
        }
        if (inner.count("g")) {
            try { p.g = (uint8_t)std::stoi(inner["g"]); } catch (...) {}
        }
        if (inner.count("b")) {
            try { p.b = (uint8_t)std::stoi(inner["b"]); } catch (...) {}
        }
        if (inner.count("message")) p.message = inner["message"];
        if (inner.count("play_sound")) {
            p.play_sound = (inner["play_sound"] == "true" || inner["play_sound"] == "1");
        }

        hook_log("ipc_client: received InjectChatMessage for P%d (%s): %s", p.player_index, p.player_name.c_str(), p.message.c_str());

        std::lock_guard<std::mutex> lock(g_inject_mutex);
        if (g_inject_queue.size() < 100) {
            g_inject_queue.push_back(p);
        }
    } else if (type_str == "ExecuteChatBroadcast") {
        auto inner = parse_json_object(payload_str);
        ExecuteBroadcastPayload p;
        if (inner.count("player_index")) {
            try { p.player_index = std::stoi(inner["player_index"]); } catch (...) {}
        }
        if (inner.count("message")) p.message = inner["message"];

        hook_log("ipc_client: received ExecuteChatBroadcast for P%d: %s", p.player_index, p.message.c_str());

        std::lock_guard<std::mutex> lock(g_broadcast_mutex);
        if (g_broadcast_queue.size() < 100) {
            g_broadcast_queue.push_back(p);
        }
    } else if (type_str == "StateUpdate") {
        auto inner = parse_json_object(payload_str);
        if (inner.count("is_paused")) {
            bool paused = (inner["is_paused"] == "true" || inner["is_paused"] == "1");
            g_is_paused.store(paused);
            hook_log("ipc_client: StateUpdate is_paused=%d", paused ? 1 : 0);
        }
    } else if (type_str == "Heartbeat") {
        // Heartbeat received, no action needed
    }
}

// ============================================================================
// Worker Thread Loop (Fail-Safe Disconnect & Hot-Plugging Auto-Reconnect)
// ============================================================================

static void ipc_worker_thread_proc() {
    const LPCWSTR pipe_name = L"\\\\.\\pipe\\helldivers1_babel";
    std::string read_buffer;
    read_buffer.reserve(4096);

    auto last_heartbeat = std::chrono::steady_clock::now();

    while (g_ipc_running.load()) {
        // Connection attempt loop
        if (g_hPipe == INVALID_HANDLE_VALUE) {
            // Wait for pipe with timeout to prevent blocking thread exit
            if (!WaitNamedPipeW(pipe_name, 500)) {
                // Pipe not available yet, wait and retry
                std::this_thread::sleep_for(std::chrono::milliseconds(500));
                continue;
            }

            if (!g_ipc_running.load()) break;

            HANDLE h = CreateFileW(
                pipe_name,
                GENERIC_READ | GENERIC_WRITE,
                0,
                NULL,
                OPEN_EXISTING,
                0, // synchronous I/O, non-blocking via PeekNamedPipe
                NULL
            );

            if (h == INVALID_HANDLE_VALUE) {
                g_is_connected.store(false);
                std::this_thread::sleep_for(std::chrono::milliseconds(1000));
                continue;
            }

            // Pipe connected successfully!
            {
                std::lock_guard<std::mutex> lock(g_pipe_mutex);
                g_hPipe = h;
            }
            g_is_connected.store(true);
            read_buffer.clear();

            // Clear any stale outbound queue upon connection
            {
                std::lock_guard<std::mutex> lock(g_out_mutex);
                g_outgoing_queue.clear();
            }

            // Send initial heartbeat
            ipc_send_heartbeat();
            last_heartbeat = std::chrono::steady_clock::now();
        }

        // Active connection loop
        bool connection_healthy = true;

        // 1. Non-blocking read check via PeekNamedPipe
        DWORD bytes_avail = 0;
        if (!PeekNamedPipe(g_hPipe, NULL, 0, NULL, &bytes_avail, NULL)) {
            DWORD err = GetLastError();
            if (err == ERROR_BROKEN_PIPE || err == ERROR_PIPE_NOT_CONNECTED || err == ERROR_INVALID_HANDLE) {
                connection_healthy = false;
            }
        } else if (bytes_avail > 0) {
            char temp[2048];
            DWORD to_read = (bytes_avail < sizeof(temp)) ? bytes_avail : sizeof(temp);
            DWORD bytes_read = 0;
            if (ReadFile(g_hPipe, temp, to_read, &bytes_read, NULL) && bytes_read > 0) {
                read_buffer.append(temp, bytes_read);

                // Split by newline and dispatch packets
                size_t nl_pos;
                while ((nl_pos = read_buffer.find('\n')) != std::string::npos) {
                    std::string line = read_buffer.substr(0, nl_pos);
                    read_buffer.erase(0, nl_pos + 1);

                    if (!line.empty() && line.back() == '\r') {
                        line.pop_back();
                    }
                    if (!line.empty()) {
                        handle_received_packet_line(line);
                    }
                }
            } else {
                DWORD err = GetLastError();
                if (err == ERROR_BROKEN_PIPE || err == ERROR_PIPE_NOT_CONNECTED) {
                    connection_healthy = false;
                }
            }
        }

        // 2. Periodic Heartbeat (every 5 seconds)
        auto now = std::chrono::steady_clock::now();
        if (connection_healthy && std::chrono::duration_cast<std::chrono::seconds>(now - last_heartbeat).count() >= 5) {
            ipc_send_heartbeat();
            last_heartbeat = now;
        }

        // 3. Process outbound packet queue
        if (connection_healthy) {
            std::vector<std::string> to_send;
            {
                std::lock_guard<std::mutex> lock(g_out_mutex);
                if (!g_outgoing_queue.empty()) {
                    to_send.swap(g_outgoing_queue);
                }
            }

            for (const auto& pkt : to_send) {
                DWORD bytes_written = 0;
                if (!WriteFile(g_hPipe, pkt.data(), (DWORD)pkt.size(), &bytes_written, NULL)) {
                    DWORD err = GetLastError();
                    if (err == ERROR_BROKEN_PIPE || err == ERROR_PIPE_NOT_CONNECTED) {
                        connection_healthy = false;
                        break;
                    }
                }
            }
        }

        // 4. Fail-Safe Disconnect Protocol: handle broken pipe
        if (!connection_healthy) {
            {
                std::lock_guard<std::mutex> lock(g_pipe_mutex);
                if (g_hPipe != INVALID_HANDLE_VALUE) {
                    CloseHandle(g_hPipe);
                    g_hPipe = INVALID_HANDLE_VALUE;
                }
            }
            g_is_connected.store(false);

            // Discard stale outbound queue
            {
                std::lock_guard<std::mutex> lock(g_out_mutex);
                g_outgoing_queue.clear();
            }

            // Low-frequency polling loop (2.0s via 100ms intervals)
            for (int i = 0; i < 20 && g_ipc_running.load(); ++i) {
                std::this_thread::sleep_for(std::chrono::milliseconds(100));
            }
            continue;
        }

        // Yield CPU to prevent busy spinning
        std::this_thread::sleep_for(std::chrono::milliseconds(10));
    }

    // Clean shutdown on thread termination
    {
        std::lock_guard<std::mutex> lock(g_pipe_mutex);
        if (g_hPipe != INVALID_HANDLE_VALUE) {
            CloseHandle(g_hPipe);
            g_hPipe = INVALID_HANDLE_VALUE;
        }
    }
    g_is_connected.store(false);
}

// ============================================================================
// Public API Implementation
// ============================================================================

void ipc_client_start() {
    if (g_ipc_running.load()) return;
    g_ipc_running.store(true);
    g_worker_thread = std::thread(ipc_worker_thread_proc);
}

void ipc_client_stop() {
    if (!g_ipc_running.load()) return;
    g_ipc_running.store(false);

    {
        std::lock_guard<std::mutex> lock(g_pipe_mutex);
        if (g_hPipe != INVALID_HANDLE_VALUE) {
            // Cancel pending I/O and close handle to unblock thread
            CancelIoEx(g_hPipe, NULL);
            CloseHandle(g_hPipe);
            g_hPipe = INVALID_HANDLE_VALUE;
        }
    }

    if (g_worker_thread.joinable()) {
        g_worker_thread.join();
    }
    g_is_connected.store(false);
}

bool ipc_client_is_connected() {
    return g_is_connected.load();
}

bool ipc_client_is_paused() {
    return g_is_paused.load();
}

void ipc_send_incoming_chat(const IncomingChatPayload& payload) {
    if (!g_is_connected.load() || g_is_paused.load()) return;

    std::string packet = format_incoming_chat_packet(payload);
    std::lock_guard<std::mutex> lock(g_out_mutex);
    if (g_outgoing_queue.size() < 200) {
        g_outgoing_queue.push_back(packet);
    }
}

void ipc_send_outgoing_chat(const OutgoingChatPayload& payload) {
    if (!g_is_connected.load() || g_is_paused.load()) return;

    std::string packet = format_outgoing_chat_packet(payload);
    std::lock_guard<std::mutex> lock(g_out_mutex);
    if (g_outgoing_queue.size() < 200) {
        g_outgoing_queue.push_back(packet);
    }
}

void ipc_send_player_joined(const PlayerJoinedPayload& payload) {
    if (!g_is_connected.load() || g_is_paused.load()) return;

    std::string packet = format_player_joined_packet(payload);
    std::lock_guard<std::mutex> lock(g_out_mutex);
    if (g_outgoing_queue.size() < 200) {
        g_outgoing_queue.push_back(packet);
    }
}

void ipc_send_heartbeat() {
    if (!g_is_connected.load()) return;

    std::string packet = format_heartbeat_packet();
    std::lock_guard<std::mutex> lock(g_out_mutex);
    if (g_outgoing_queue.size() < 200) {
        g_outgoing_queue.push_back(packet);
    }
}

bool ipc_poll_inject_chat(InjectChatPayload& outPayload) {
    std::unique_lock<std::mutex> lock(g_inject_mutex, std::try_to_lock);
    if (!lock.owns_lock() || g_inject_queue.empty()) {
        return false;
    }
    outPayload = g_inject_queue.front();
    g_inject_queue.erase(g_inject_queue.begin());
    return true;
}

bool ipc_poll_execute_broadcast(ExecuteBroadcastPayload& outPayload) {
    std::unique_lock<std::mutex> lock(g_broadcast_mutex, std::try_to_lock);
    if (!lock.owns_lock() || g_broadcast_queue.empty()) {
        return false;
    }
    outPayload = g_broadcast_queue.front();
    g_broadcast_queue.erase(g_broadcast_queue.begin());
    return true;
}
