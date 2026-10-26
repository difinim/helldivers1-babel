#pragma once

#include <string>
#include <vector>
#include <cstdint>
#include <atomic>

// Matches Babel.Core.Ipc.IpcPacketType enum
enum class IpcPacketType {
    Unknown = 0,
    Heartbeat = 1,
    ChatMessageIncoming = 2,
    ChatMessageOutgoing = 3,
    InjectChatMessage = 4,
    ExecuteChatBroadcast = 5,
    PlaySound = 6,
    StateUpdate = 7,
    PlayerJoined = 8
};

struct PlayerJoinedPayload {
    std::string peer_id;
    std::string player_name = "Player";
    bool is_host = false;
};

struct IncomingChatPayload {
    std::string peer_id;
    int player_index = 1;
    std::string player_name = "Player";
    uint8_t r = 255;
    uint8_t g = 255;
    uint8_t b = 255;
    std::string message;
    bool is_local = false;
};

struct OutgoingChatPayload {
    int player_index = 1;
    std::string player_name = "Player";
    std::string message;
};

struct InjectChatPayload {
    int player_index = 1;
    std::string player_name = "Player";
    uint8_t r = 255;
    uint8_t g = 255;
    uint8_t b = 255;
    std::string message;
    bool play_sound = true;
};

struct ExecuteBroadcastPayload {
    int player_index = 1;
    std::string message;
};

// Lifecycle
void ipc_client_start();
void ipc_client_stop();

// Connection and pause state
bool ipc_client_is_connected();
bool ipc_client_is_paused();

// Outbound queue (called from game thread or Lua callback - non-blocking)
void ipc_send_incoming_chat(const IncomingChatPayload& payload);
void ipc_send_outgoing_chat(const OutgoingChatPayload& payload);
void ipc_send_player_joined(const PlayerJoinedPayload& payload);
void ipc_send_heartbeat();

// Inbound queue polling (called by game thread on update loop - non-blocking)
bool ipc_poll_inject_chat(InjectChatPayload& outPayload);
bool ipc_poll_execute_broadcast(ExecuteBroadcastPayload& outPayload);
