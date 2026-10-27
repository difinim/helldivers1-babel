-- Helldivers 1 Babel - In-Game Lua Bootstrap Script
-- Intercepts text chat, handles outgoing translation tags, and injects incoming messages

local clamp = math.clamp or function(val, min_val, max_val)
    if val < min_val then return min_val end
    if val > max_val then return max_val end
    return val
end

if not _G.__babel_bootstrap_done then
    _G.__babel_bootstrap_done = true

    if _G._babel_log then
        _G._babel_log("__babel_bootstrap: Initializing Babel Lua runtime...")
    end

    -- Resilient active chat resolution helper
    _G.__babel_get_chat = function()
        if _G.__babel_active_chat then
            return _G.__babel_active_chat
        end
        if FlowCallbacks and FlowCallbacks.current_state and FlowCallbacks.current_state.text_chat then
            _G.__babel_active_chat = FlowCallbacks.current_state.text_chat
            return _G.__babel_active_chat
        end
        return nil
    end

    _G.__babel_hook_chat = function(TC)
        if not TC or TC.__babel_hooked then return end
        TC.__babel_hooked = true

        if _G._babel_log then
            _G._babel_log("__babel_hook_chat: Hooking TextChat class successfully.")
        end

        -- 1. Hook update: captures active TextChat instance every frame
        local orig_update = TC.update
        TC.update = function(self, dt)
            _G.__babel_active_chat = self
            return orig_update(self, dt)
        end

        -- 2. Hook init: captures active TextChat instance on construction
        local orig_init = TC.init
        TC.init = function(self, ...)
            _G.__babel_active_chat = self
            return orig_init(self, ...)
        end

        -- 3. Outgoing chat interception (send_text)
        local orig_send_text = TC.send_text
        TC.send_text = function(self, text)
            _G.__babel_active_chat = self
            if _babel_is_connected and _babel_is_connected() and text and type(text) == "string" then
                local typing_player = (self.get_typing_player and self:get_typing_player()) or nil
                local p_idx = (typing_player and typing_player.player_index) or 1
                local p_name = "Player"
                if typing_player then
                    if typing_player.player_go and typing_player.player_go.get then
                        p_name = typing_player.player_go:get("name") or typing_player.name or p_name
                    else
                        p_name = typing_player.name or p_name
                    end
                end

                if string.find(text, ">>") then
                    if _G._babel_log then
                        _G._babel_log("Outgoing translation intercepted: " .. tostring(text))
                    end
                    _babel_on_outgoing(p_idx, p_name, text)
                    self:stop_typing()
                    return
                else
                    -- Standard outgoing message (without >>): notify Babel to register outbound text
                    if _babel_on_outgoing then
                        _babel_on_outgoing(p_idx, p_name, text)
                    end
                end
            end
            -- Fail-open pass-through: unmodified vanilla dispatch
            orig_send_text(self, text)
        end

        -- 4. Incoming chat interception (rpc_text_chat)
        local orig_rpc_text_chat = TC.rpc_text_chat
        TC.rpc_text_chat = function(self, sender_peer_id, player_index, message)
            _G.__babel_active_chat = self
            -- Transparent Pass-Through: always let vanilla game render immediately
            orig_rpc_text_chat(self, sender_peer_id, player_index, message)

            if _babel_is_connected and _babel_is_connected() and message and type(message) == "string" then
                local p_name = "Player"
                local r, g, b = 255, 255, 255
                if self.players then
                    for _, p in ipairs(self.players) do
                        if p.player_index == player_index then
                            if p.player_go and p.player_go.get then
                                p_name = p.player_go:get("name") or p.name or p_name
                            else
                                p_name = p.name or p_name
                            end
                            break
                        end
                    end
                end
                if GameSettings and GameSettings.player_colors and GameSettings.player_colors[player_index] then
                    local c = GameSettings.player_colors[player_index]
                    r = math.floor(clamp((c.y or 180) * 1.4, 0, 255))
                    g = math.floor(clamp((c.z or 180) * 1.4, 0, 255))
                    b = math.floor(clamp((c.w or 180) * 1.4, 0, 255))
                end

                local my_peer = (Network and Network.peer_id and Network.peer_id()) or "local"
                local typing_player = (self.get_typing_player and self:get_typing_player()) or nil
                local is_local = (my_peer ~= "local" and tostring(sender_peer_id) == tostring(my_peer)) or
                                 (typing_player and typing_player.player_index == player_index) or false

                if _G._babel_log then
                    _G._babel_log("Incoming chat intercepted: [P" .. tostring(player_index) .. " " .. tostring(p_name) .. "] " .. tostring(message) .. " (local=" .. tostring(is_local) .. ")")
                end
                _babel_on_incoming(tostring(sender_peer_id), player_index, p_name, r, g, b, message, is_local)
            end
        end
    end

    -- 5. Inject translated message into in-game chat widget history
    _G.__babel_inject_chat = function(p_idx, p_name, r, g, b, msg, play_sound)
        local chat = _G.__babel_get_chat()
        if not chat or not chat.history then
            if _G._babel_log then
                _G._babel_log("__babel_inject_chat: chat widget or history not ready.")
            end
            return
        end
        local my_peer = (Network and Network.peer_id and Network.peer_id()) or "local"
        local entry = {
            peer_id = my_peer,
            always_show = true,
            avatar_unit = nil,
            player_name = p_name,
            text = p_name .. ": " .. msg,
            short_text = (chat.short_text and chat:short_text(msg)) or msg,
            time_stamp = _G.GAME_TIME or os.clock(),
            player_color = { r, g, b }
        }
        table.insert(chat.history, 1, entry)
        if #chat.history > 100 then
            chat.history[#chat.history] = nil
        end
        chat.stopped_time = _G.GAME_TIME or os.clock()
        if chat.history_hd_gui then
            chat.history_hd_gui.is_dirty = true
        end
        if play_sound and chat.play_chat_sound then
            chat:play_chat_sound()
        end
        if _G._babel_log then
            _G._babel_log("__babel_inject_chat: injected into history: " .. tostring(msg))
        end
    end

    -- 6. Macro & translated chat broadcast over network RPC
    _G.__babel_broadcast_chat = function(p_idx, msg)
        local chat = _G.__babel_get_chat()
        if not chat then
            if _G._babel_log then
                _G._babel_log("__babel_broadcast_chat: chat widget not available.")
            end
            return
        end
        local typing_p = (chat.get_typing_player and chat:get_typing_player()) or nil
        local actual_idx = (typing_p and typing_p.player_index) or p_idx or 1
        local sent = false
        if chat.network_router and chat.network_router.transmit_to_all then
            chat.network_router:transmit_to_all("rpc_text_chat", actual_idx, msg)
            sent = true
        end
        if not sent and chat.rpc_text_chat then
            local my_peer = (Network and Network.peer_id and Network.peer_id()) or "local"
            chat:rpc_text_chat(my_peer, actual_idx, msg)
            sent = true
        end
        if _G._babel_log then
            _G._babel_log("__babel_broadcast_chat: dispatched for P" .. tostring(actual_idx) .. ": " .. tostring(msg) .. " (sent=" .. tostring(sent) .. ")")
        end
    end

    -- 7. Hook NetworkSessionHandler to detect guest players joining
    _G.__babel_pending_join_names = _G.__babel_pending_join_names or {}

    local function capture_join_request(state_class)
        if not state_class or state_class.__babel_join_hooked then return end
        state_class.__babel_join_hooked = true
        local orig_rpc = state_class.rpc_on_join_session_request
        if orig_rpc then
            state_class.rpc_on_join_session_request = function(self, sender_peer_id, state_name, seed, player_name, num_local_players)
                pcall(function()
                    if sender_peer_id and player_name then
                        _G.__babel_pending_join_names[tostring(sender_peer_id)] = tostring(player_name)
                    end
                end)
                return orig_rpc(self, sender_peer_id, state_name, seed, player_name, num_local_players)
            end
        end
    end

    _G.__babel_hook_network = function(NSH)
        if not NSH or NSH.__babel_hooked then return end
        NSH.__babel_hooked = true

        if _G._babel_log then
            _G._babel_log("__babel_hook_network: Hooking NetworkSessionHandler successfully.")
        end

        local orig_add_peer = NSH.add_peer
        NSH.add_peer = function(self, peer_id)
            orig_add_peer(self, peer_id)

            pcall(function()
                if _babel_is_connected and _babel_is_connected() and peer_id then
                    local my_peer = (Network and Network.peer_id and Network.peer_id()) or "local"
                    local is_host = (NetworkGlobal and NetworkGlobal.lobby and NetworkGlobal.lobby.is_host and NetworkGlobal.lobby:is_host()) or false
                    local is_remote = (tostring(peer_id) ~= tostring(my_peer))

                    if is_host and is_remote then
                        local p_name = (_G.__babel_pending_join_names and _G.__babel_pending_join_names[tostring(peer_id)]) or "Player"
                        if _G._babel_log then
                            _G._babel_log("Guest player joined session as host: peer=" .. tostring(peer_id) .. ", name=" .. tostring(p_name))
                        end
                        if _babel_on_player_joined then
                            _babel_on_player_joined(tostring(peer_id), tostring(p_name), true)
                        end
                    end
                end
            end)
        end
    end

    -- 8. Periodic Hook Check
    _G.__babel_check_hook = function()
        if _G.TextChat and not _G.TextChat.__babel_hooked then
            _G.__babel_hook_chat(_G.TextChat)
        end
        if _G.NetworkSessionHandler and not _G.NetworkSessionHandler.__babel_hooked then
            _G.__babel_hook_network(_G.NetworkSessionHandler)
        end
        if _G.StateGame and not _G.StateGame.__babel_join_hooked then
            capture_join_request(_G.StateGame)
        end
        if _G.StateMenu and not _G.StateMenu.__babel_join_hooked then
            capture_join_request(_G.StateMenu)
        end
        if _G.StateMissionStart and not _G.StateMissionStart.__babel_join_hooked then
            capture_join_request(_G.StateMissionStart)
        end
        if _G.StateMissionEnd and not _G.StateMissionEnd.__babel_join_hooked then
            capture_join_request(_G.StateMissionEnd)
        end
    end
end

if _G.__babel_check_hook then
    _G.__babel_check_hook()
elseif _G.TextChat and not _G.TextChat.__babel_hooked then
    _G.__babel_hook_chat(_G.TextChat)
end
