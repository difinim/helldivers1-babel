#include "hook_engine.h"
#include "ipc_client.h"
#include "hook_log.h"
#include "babel_bootstrap_bytecode.h"
#include "MinHook.h"
#include <windows.h>
#include <cstdint>
#include <cstdio>
#include <cstring>
#include <string>

// Lua 5.1 / LuaJIT Type Definitions
struct lua_State;
typedef int (*lua_CFunction)(lua_State* L);

// Lua C API Function Signatures
typedef int (*luaL_loadbuffer_t)(lua_State* L, const char* buff, size_t sz, const char* name);
typedef int (*luaL_loadbufferx_t)(lua_State* L, const char* buff, size_t sz, const char* name, const char* mode);
typedef int (*luaL_loadstring_t)(lua_State* L, const char* s);
typedef int (*lua_pcall_t)(lua_State* L, int nargs, int nresults, int errfunc);
typedef void (*lua_pushcclosure_t)(lua_State* L, lua_CFunction fn, int n);
typedef void (*lua_setfield_t)(lua_State* L, int idx, const char* k);
typedef void (*lua_getfield_t)(lua_State* L, int idx, const char* k);
typedef const char* (*lua_tolstring_t)(lua_State* L, int idx, size_t* len);
typedef int64_t (*lua_tointeger_t)(lua_State* L, int idx);
typedef int (*lua_toboolean_t)(lua_State* L, int idx);
typedef void (*lua_pushboolean_t)(lua_State* L, int b);
typedef void (*lua_pushstring_t)(lua_State* L, const char* s);
typedef void (*lua_pushnumber_t)(lua_State* L, double n);
typedef void (*lua_settop_t)(lua_State* L, int idx);
typedef int (*lua_gettop_t)(lua_State* L);
typedef int (*lua_type_t)(lua_State* L, int idx);
typedef void (*lua_set_load_protection_t)(lua_State* L, int disallow_bytecode, int disallow_text);

static lua_set_load_protection_t p_lua_set_load_protection = nullptr;
static luaL_loadbuffer_t p_luaL_loadbuffer = nullptr;
static luaL_loadbufferx_t p_luaL_loadbufferx = nullptr;
static luaL_loadstring_t p_luaL_loadstring = nullptr;
static lua_pcall_t p_lua_pcall = nullptr;
static lua_pushcclosure_t p_lua_pushcclosure = nullptr;
static lua_setfield_t p_lua_setfield = nullptr;
static lua_getfield_t p_lua_getfield = nullptr;
static lua_tolstring_t p_lua_tolstring = nullptr;
static lua_tointeger_t p_lua_tointeger = nullptr;
static lua_toboolean_t p_lua_toboolean = nullptr;
static lua_pushboolean_t p_lua_pushboolean = nullptr;
static lua_pushstring_t p_lua_pushstring = nullptr;
static lua_pushnumber_t p_lua_pushnumber = nullptr;
static lua_settop_t p_lua_settop = nullptr;
static lua_gettop_t p_lua_gettop = nullptr;
static lua_type_t p_lua_type = nullptr;

// Internal State
static uintptr_t g_base = 0;
static lua_State* g_L = nullptr;
static bool g_hooks_installed = false;
static bool g_lua_initialized = false;
static int g_frame_counter = 0;

// Helper: Run precompiled LuaJIT bytecode safely with proper stack discipline and loader unlock
static int run_lua_bytecode(lua_State* L, const unsigned char* bytecode, size_t size, const char* name) {
    hook_log("run_lua_bytecode: Step 1 - pointers check (L=%p, loadbuffer=%p, pcall=%p, protect=%p)",
        L, (void*)p_luaL_loadbuffer, (void*)p_lua_pcall, (void*)p_lua_set_load_protection);

    if (!p_luaL_loadbuffer || !p_lua_pcall || !L) {
        hook_log("run_lua_bytecode: missing required function pointers!");
        return -1;
    }

    // Step 2: Temporarily unlock Stingray anti-tamper loader flags (G(L)+0x198 and G(L)+0x199)
    hook_log("run_lua_bytecode: Step 2 - unlocking load protection...");
    if (p_lua_set_load_protection) {
        p_lua_set_load_protection(L, 0, 0);
    }

    // Step 3: Load bytecode buffer (0x1ae240 is luaL_loadbuffer with 4 args)
    hook_log("run_lua_bytecode: Step 3 - calling luaL_loadbuffer (size=%zu)...", size);
    int status = p_luaL_loadbuffer(L, (const char*)bytecode, size, name ? name : "=@babel");
    hook_log("run_lua_bytecode: Step 4 - luaL_loadbuffer returned status=%d", status);

    // Step 5: Restore load protection flags
    if (p_lua_set_load_protection) {
        p_lua_set_load_protection(L, 1, 1);
    }
    hook_log("run_lua_bytecode: Step 5 - re-locked load protection");

    if (status != 0) {
        size_t len = 0;
        const char* err = p_lua_tolstring ? p_lua_tolstring(L, -1, &len) : "";
        hook_log("run_lua_bytecode: loadbuffer failed with status %d: %s", status, err ? err : "");
        if (p_lua_settop) p_lua_settop(L, -2);
        return status;
    }

    // Step 6: Execute the loaded bytecode closure via pcall
    hook_log("run_lua_bytecode: Step 6 - calling lua_pcall(L, 0, 0, 0)...");
    int pcall_status = p_lua_pcall(L, 0, 0, 0);
    hook_log("run_lua_bytecode: Step 7 - lua_pcall returned status=%d", pcall_status);

    if (pcall_status != 0) {
        size_t len = 0;
        const char* err = p_lua_tolstring ? p_lua_tolstring(L, -1, &len) : "";
        hook_log("run_lua_bytecode: pcall failed with status %d: %s", pcall_status, err ? err : "");
        if (p_lua_settop) p_lua_settop(L, -2);
        return pcall_status;
    }

    hook_log("run_lua_bytecode: Step 8 - bootstrap bytecode loaded and executed with success!");
    return 0;
}

// ============================================================================
// Lua C Callbacks (Exposed to Helldivers Lua VM via _G)
// ============================================================================

static int l_babel_is_connected(lua_State* L) {
    bool active = ipc_client_is_connected() && !ipc_client_is_paused();
    if (p_lua_pushboolean) {
        p_lua_pushboolean(L, active ? 1 : 0);
    }
    return 1;
}

static int l_babel_log(lua_State* L) {
    if (!p_lua_tolstring) return 0;
    const char* msg = p_lua_tolstring(L, 1, nullptr);
    hook_log("[Lua] %s", msg ? msg : "");
    return 0;
}

static int l_babel_on_incoming(lua_State* L) {
    if (!ipc_client_is_connected() || ipc_client_is_paused()) return 0;
    if (!p_lua_tolstring || !p_lua_tointeger) return 0;

    IncomingChatPayload payload;
    const char* peer = p_lua_tolstring(L, 1, nullptr);
    payload.peer_id = peer ? peer : "";
    payload.player_index = (int)p_lua_tointeger(L, 2);
    const char* name = p_lua_tolstring(L, 3, nullptr);
    payload.player_name = name ? name : "Player";
    payload.r = (uint8_t)p_lua_tointeger(L, 4);
    payload.g = (uint8_t)p_lua_tointeger(L, 5);
    payload.b = (uint8_t)p_lua_tointeger(L, 6);
    const char* msg = p_lua_tolstring(L, 7, nullptr);
    payload.message = msg ? msg : "";
    if (p_lua_gettop && p_lua_toboolean && p_lua_gettop(L) >= 8) {
        payload.is_local = (p_lua_toboolean(L, 8) != 0);
    }

    hook_log("l_babel_on_incoming: [P%d] %s (local=%d): %s", payload.player_index, payload.player_name.c_str(), payload.is_local ? 1 : 0, payload.message.c_str());
    ipc_send_incoming_chat(payload);
    return 0;
}

static int l_babel_on_outgoing(lua_State* L) {
    if (!ipc_client_is_connected() || ipc_client_is_paused()) return 0;
    if (!p_lua_tolstring || !p_lua_tointeger) return 0;

    OutgoingChatPayload payload;
    payload.player_index = (int)p_lua_tointeger(L, 1);
    const char* name = p_lua_tolstring(L, 2, nullptr);
    payload.player_name = name ? name : "Player";
    const char* msg = p_lua_tolstring(L, 3, nullptr);
    payload.message = msg ? msg : "";

    hook_log("l_babel_on_outgoing: [P%d] %s: %s", payload.player_index, payload.player_name.c_str(), payload.message.c_str());
    ipc_send_outgoing_chat(payload);
    return 0;
}

static int l_babel_on_player_joined(lua_State* L) {
    if (!ipc_client_is_connected() || ipc_client_is_paused()) return 0;
    if (!p_lua_tolstring) return 0;

    PlayerJoinedPayload payload;
    const char* peer = p_lua_tolstring(L, 1, nullptr);
    payload.peer_id = peer ? peer : "";
    const char* name = (p_lua_gettop && p_lua_gettop(L) >= 2) ? p_lua_tolstring(L, 2, nullptr) : "Player";
    payload.player_name = name ? name : "Player";
    if (p_lua_gettop && p_lua_toboolean && p_lua_gettop(L) >= 3) {
        payload.is_host = (p_lua_toboolean(L, 3) != 0);
    } else {
        payload.is_host = true;
    }

    hook_log("l_babel_on_player_joined: peer=%s, name=%s, is_host=%d", payload.peer_id.c_str(), payload.player_name.c_str(), payload.is_host ? 1 : 0);
    ipc_send_player_joined(payload);
    return 0;
}

// ============================================================================
// Lua Hook Injection Script
// ============================================================================

static void setup_lua_environment(lua_State* L) {
    if (!L || !p_lua_pushcclosure || !p_lua_setfield) {
        hook_log("setup_lua_environment: Missing required Lua C API pointers!");
        return;
    }

    hook_log("setup_lua_environment: Registering C callbacks in globals...");

    // Register C callbacks in global environment (LUA_GLOBALSINDEX = -10002)
    p_lua_pushcclosure(L, l_babel_log, 0);
    p_lua_setfield(L, -10002, "_babel_log");

    p_lua_pushcclosure(L, l_babel_is_connected, 0);
    p_lua_setfield(L, -10002, "_babel_is_connected");

    p_lua_pushcclosure(L, l_babel_on_incoming, 0);
    p_lua_setfield(L, -10002, "_babel_on_incoming");

    p_lua_pushcclosure(L, l_babel_on_outgoing, 0);
    p_lua_setfield(L, -10002, "_babel_on_outgoing");

    p_lua_pushcclosure(L, l_babel_on_player_joined, 0);
    p_lua_setfield(L, -10002, "_babel_on_player_joined");

    hook_log("setup_lua_environment: C callbacks registered. Running precompiled bootstrap bytecode...");

    int status = run_lua_bytecode(L, g_babel_bootstrap_bytecode, g_babel_bootstrap_bytecode_size, "=@babel_bootstrap");
    if (status == 0) {
        hook_log("setup_lua_environment: Precompiled Lua bootstrap bytecode executed successfully.");
    } else {
        hook_log("setup_lua_environment: Bootstrap bytecode failed with status %d", status);
    }
}

// Drain inbound queues and execute on the main game thread safely
static void process_incoming_game_thread_queues(lua_State* L) {
    if (!L || !p_lua_getfield || !p_lua_pcall) return;

    // 1. Process InjectChatMessage queue
    InjectChatPayload inject;
    while (ipc_poll_inject_chat(inject)) {
        p_lua_getfield(L, -10002, "__babel_inject_chat");
        int t = p_lua_type ? p_lua_type(L, -1) : 6;
        if (t == 6) { // LUA_TFUNCTION == 6
            p_lua_pushnumber(L, (double)inject.player_index);
            p_lua_pushstring(L, inject.player_name.c_str());
            p_lua_pushnumber(L, (double)inject.r);
            p_lua_pushnumber(L, (double)inject.g);
            p_lua_pushnumber(L, (double)inject.b);
            p_lua_pushstring(L, inject.message.c_str());
            p_lua_pushboolean(L, inject.play_sound ? 1 : 0);
            int res = p_lua_pcall(L, 7, 0, 0);
            if (res != 0) {
                hook_log("process_incoming: __babel_inject_chat error %d", res);
                if (p_lua_settop) p_lua_settop(L, -2);
            }
        } else {
            hook_log("process_incoming: __babel_inject_chat not callable (type=%d)", t);
            if (p_lua_settop) p_lua_settop(L, -2);
        }
    }

    // 2. Process ExecuteChatBroadcast queue
    ExecuteBroadcastPayload broadcast;
    while (ipc_poll_execute_broadcast(broadcast)) {
        p_lua_getfield(L, -10002, "__babel_broadcast_chat");
        int t = p_lua_type ? p_lua_type(L, -1) : 6;
        if (t == 6) { // LUA_TFUNCTION == 6
            p_lua_pushnumber(L, (double)broadcast.player_index);
            p_lua_pushstring(L, broadcast.message.c_str());
            int res = p_lua_pcall(L, 2, 0, 0);
            if (res != 0) {
                hook_log("process_incoming: __babel_broadcast_chat error %d", res);
                if (p_lua_settop) p_lua_settop(L, -2);
            }
        } else {
            hook_log("process_incoming: __babel_broadcast_chat not callable (type=%d)", t);
            if (p_lua_settop) p_lua_settop(L, -2);
        }
    }
}

// ============================================================================
// MinHook Detours
// ============================================================================

typedef void (*Engine_update_t)(void* world, float dt);
static Engine_update_t o_Engine_update = nullptr;

static void hk_Engine_update(void* world, float dt) {
    // 1. Transparent pass-through: execute original update first
    if (o_Engine_update) {
        o_Engine_update(world, dt);
    }

    if (!world) return;

    // In Bitsquid x64, world + 0x118 is LuaContext, and *(lua_State**)(world + 0x118) is lua_State*
    lua_State** ppL = (lua_State**)((uintptr_t)world + 0x118);
    if (!ppL) return;
    lua_State* L = *ppL;
    if (!L) return;

    // 2. Initialize Lua environment once the VM is active
    if (!g_lua_initialized || L != g_L) {
        g_L = L;
        g_lua_initialized = true;
        hook_log("hk_Engine_update: captured valid lua_State* = %p from world = %p. Initializing environment...", L, world);
        setup_lua_environment(L);
    }

    // Periodically verify TextChat class is hooked (every ~60 frames)
    if (++g_frame_counter >= 60) {
        g_frame_counter = 0;
        if (p_lua_getfield && p_lua_pcall && p_lua_settop) {
            p_lua_getfield(L, -10002, "__babel_check_hook");
            int t = p_lua_type ? p_lua_type(L, -1) : 6;
            if (t == 6) { // LUA_TFUNCTION == 6
                int res = p_lua_pcall(L, 0, 0, 0);
                if (res != 0) {
                    size_t len = 0;
                    const char* err = p_lua_tolstring ? p_lua_tolstring(L, -1, &len) : "unknown error";
                    hook_log("hk_Engine_update: __babel_check_hook failed (%d): %s", res, err ? err : "");
                    p_lua_settop(L, -2);
                }
            } else {
                p_lua_settop(L, -2);
            }
        }
    }

    // 3. Process queued incoming packets on the main game thread
    process_incoming_game_thread_queues(L);
}

// ============================================================================
// Hook Engine Lifecycle
// ============================================================================

void hook_engine_install() {
    g_base = (uintptr_t)GetModuleHandleA(NULL);
    if (!g_base) {
        hook_log("hook_engine_install: GetModuleHandleA returned NULL!");
        return;
    }

    char modPath[MAX_PATH];
    GetModuleFileNameA((HMODULE)g_base, modPath, sizeof(modPath));
    hook_log("hook_engine_install: Process base = %p, module = %s", (void*)g_base, modPath);

    if (strstr(modPath, "helldivers") == nullptr && strstr(modPath, "Helldivers") == nullptr) {
        hook_log("hook_engine_install: Not running inside Helldivers, skipping hooks safely.");
        return;
    }

    // Resolve Lua C API functions
    p_lua_set_load_protection = (lua_set_load_protection_t)(g_base + 0x1a8b50);
    p_luaL_loadbuffer = (luaL_loadbuffer_t)(g_base + 0x1ae240);
    p_luaL_loadbufferx = (luaL_loadbufferx_t)(g_base + 0x1ae190);
    p_luaL_loadstring = (luaL_loadstring_t)(g_base + 0x1ae260);
    p_lua_pcall = (lua_pcall_t)(g_base + 0x1a7ec0);
    p_lua_pushcclosure = (lua_pushcclosure_t)(g_base + 0x1a70f0);
    p_lua_setfield = (lua_setfield_t)(g_base + 0x1a7a90);
    p_lua_getfield = (lua_getfield_t)(g_base + 0x1a7610);
    p_lua_tolstring = (lua_tolstring_t)(g_base + 0x1a6ae0);
    p_lua_tointeger = (lua_tointeger_t)(g_base + 0x1a69e0);
    p_lua_toboolean = (lua_toboolean_t)(g_base + 0x1a6ac0);
    p_lua_pushboolean = (lua_pushboolean_t)(g_base + 0x1a6f20);
    p_lua_pushstring = (lua_pushstring_t)(g_base + 0x1a6fc0);
    p_lua_pushnumber = (lua_pushnumber_t)(g_base + 0x1a6ed0);
    p_lua_settop = (lua_settop_t)(g_base + 0x1a6260);
    p_lua_gettop = (lua_gettop_t)(g_base + 0x1a6250);
    p_lua_type = (lua_type_t)(g_base + 0x1a64c0);

    // Install MinHook detour on Engine_update (RVA 0x167f20)
    void* target_update = (void*)(g_base + 0x167f20);

    MH_STATUS s1 = MH_CreateHook(target_update, (void*)&hk_Engine_update, (void**)&o_Engine_update);
    MH_STATUS s2 = MH_EnableHook(target_update);
    hook_log("hook_engine_install: Hook on Engine_update (0x%p): create=%d, enable=%d", target_update, (int)s1, (int)s2);

    g_hooks_installed = true;
}

void hook_engine_uninstall() {
    if (!g_hooks_installed) return;
    hook_log("hook_engine_uninstall: Removing all hooks...");
    MH_DisableHook(MH_ALL_HOOKS);
    MH_RemoveHook(MH_ALL_HOOKS);
    g_hooks_installed = false;
    g_lua_initialized = false;
    g_L = nullptr;
    hook_log("hook_engine_uninstall: Hooks removed.");
}
