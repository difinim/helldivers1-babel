================================================================================
  HELLDIVERS 1 BABEL - Tactical Translation & Quick Chat Assistant
  Author: difinim
  GitHub: https://github.com/difinim/helldivers1-babel
================================================================================

"Dive together. Speak as one."

Helldivers 1 Babel is an open-source, non-invasive tactical communication
and real-time chat translation assistant designed specifically for
Helldivers (2015) on Steam PC.


+------------------------------------------------------------------------------+
| COMPATIBILITY DISCLAIMER                                                     |
+------------------------------------------------------------------------------+

Helldivers 1 Babel is strictly developed for Helldivers (Arrowhead Game
Studios, 2015) on PC (Steam).
It is NOT compatible with Helldivers 2 (2024), which uses a completely
different proprietary engine and kernel-level anti-cheat (nProtect GameGuard).


+------------------------------------------------------------------------------+
| PREREQUISITE: MICROSOFT .NET 10 DESKTOP RUNTIME REQUIRED                    |
+------------------------------------------------------------------------------+

This application requires the Microsoft .NET 10 Desktop Runtime (x64).
If not already installed on your PC, download and install it before running:
  Direct Installer: https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe
  Official Portal : https://dotnet.microsoft.com/download/dotnet/10.0


+------------------------------------------------------------------------------+
| 1. QUICK START (3 STEPS)                                                     |
+------------------------------------------------------------------------------+

  [Step 1] Launch HD1-Babel.exe
           Start HD1-Babel.exe either before launching the game or while
           Helldivers is already running.

  [Step 2] Configure Settings
           Navigate to the "Settings" tab and paste your Google Gemini API key
           (obtain a free key from Google AI Studio at https://aistudio.google.com/).
           Select your target language and click "Test API Connection".

  [Step 3] Launch Helldivers on Steam
           Start Helldivers (Steam PC). Babel automatically detects the game,
           links in real time, and begins translating squad communications.


+------------------------------------------------------------------------------+
| 2. CORE FEATURES                                                             |
+------------------------------------------------------------------------------+

  * BIDIRECTIONAL REAL-TIME CHAT TRANSLATION
    Supports up to 28 target languages in alphabetical order:
    Arabic, Bengali, Chinese, Czech, Dutch, English, French, German, Greek,
    Hindi, Hungarian, Indonesian, Italian, Japanese, Korean, Malay,
    Persian (Farsi), Polish, Portuguese (PT-BR), Romanian, Russian, Spanish,
    Swedish, Tagalog (Filipino), Thai, Turkish, Ukrainian, and Vietnamese.

  * INCOMING TRANSLATION WITH COLOR PRESERVATION
    Teammates' messages in other languages are detected and translated into
    your target language in real-time, faithfully preserving player names
    and color codes (P1 Gold, P2 Blue, P3 Purple, P4 Cyan).

  * OUTGOING TRANSLATION WITH ARBITRARY LANGUAGE ROUTING
    Type in-game chat messages using either the auto-reply prefix or any
    specific language name:
      >>message            -> Auto-replies in the detected language of the
                              last teammate message.
      target language>>message  -> Translates directly into the specified language!

  * STANDARD OUTGOING MESSAGE SUPPRESSION
    Standard in-game messages typed without >> are sent directly to teammates.
    The "Don't translate standard outgoing" toggle (ON by default) prevents
    Babel from echo-translating your own messages back to you.

  * TACTICAL QUICK CHAT (10 CUSTOM SLOTS)
    10 customizable preset tactical callouts bound to hotkeys (e.g., NumPad 0-9)
    that broadcast into chat without opening the chat box, with seamless
    in-game key pass-through.

  * TACTICAL QUICK CHAT REFERENCE OVERLAY (Numpad +)
    Press Numpad + (or your configured shortcut) anytime to toggle an on-screen
    reference overlay listing active tactical callouts and their bound shortcuts.
    The overlay appears right-aligned and centered vertically, formatting items
    as [<shortcut>] <message>.
    Note: Helldivers must run in Borderless Fullscreen (or Windowed) mode.

  * PERSISTENT AND SEARCHABLE CHAT LOGS & FILE EXPORT
    Session log tracking with timestamps, player names, colors, detected
    languages, and latencies, with one-click export to a .txt file. Logs in the
    table persist across application restarts via lightweight async disk writes
    (configurable from 0 to 100 MB in Settings, with a Clear logs memory button).
    Every column in table can be filtered by the search input.

  * INTERACTIVE MESSAGE LINKS & TRUST CONFIRMATION
    Squad chat messages containing http:// or https:// URLs in the Original
    Message column are rendered as clickable links. Clicking a link prompts
    a security dialog asking if you trust the link before opening in your
    default browser.

  * GLOBAL PAUSE / RESUME HOTKEY
    Press Ctrl + F10 (or your configured shortcut) anytime to suspend or
    resume Babel without quitting. You can also use specific shortcuts to
    do the same with only the translation feature or only the quick chat.

  * ZERO TELEMETRY & COMPLETE PRIVACY
    This tool does NOT collect, log, or send any user data or telemetry data.
    All translation calls connect directly to your configured AI provider.

  * 100% NON-INVASIVE
    Zero game file modifications. Your game archives remain clean and untampered.


+------------------------------------------------------------------------------+
| 3. IN-GAME USAGE GUIDE & EXAMPLES                                            |
+------------------------------------------------------------------------------+

  1. Automatic Incoming Translation:
     When an international teammate writes in chat:
       Diveheller (English): so quiet
     Babel renders it in your chat in your chosen language:
       Diveheller: (English): mó silêncio

  2. Auto-Reply (>>message):
     Type ">>" followed by your message:
       >>sim, está calmo demais
     Broadcasted to your teammate in English:
       yes, it is too quiet

  3. Explicit Language Routing Examples (target language>>message):
     Prefix your message with any target language to direct your communication:
       russian>>bom dia!                         -> доброе утро!
       english>>minas ao norte                   -> mines to the north
       japanese>>cuidado com os tiros!           -> 射線に注意せよ！
       spanish>>preciso de munição               -> necesito munición
       german>>recuem para o ponto de extração   -> Rückzug zum Extraktionspunkt
       french>>vamos defender aqui               -> défendons cette position

  4. Tactical Quick Chat Shortcuts:
     Press your bound hotkeys (e.g. NumPad 1-9) during combat to transmit instant
     tactical callouts. Keystrokes pass through cleanly so your Helldiver
     controls remain completely responsive.

  5. Pause, Resume & Feature Toggles:
     - Master Pause / Resume (Ctrl + F10): Toggles Babel entirely between
       ACTIVE and PAUSED. When paused, both translation and Quick Chat broadcasts
       are suspended.
     - Toggle Translation (Ctrl + F11): Pauses or resumes only translation
       interception and auto-replies while keeping Quick Chat active.
     - Toggle Quick Chat (Ctrl + F12): Pauses or resumes only Quick Chat
       broadcasts while keeping translation active.
     All toggles show immediate in-game / desktop toast overlay notifications
     and can be customized with click-to-record in Settings.

  6. Exporting Chat Logs:
     Click the "Export Logs" button on the main tab to prompt for a file
     location and save your full session chat history to a formatted .txt file.

  7. Interactive Message Links:
     Clicking any detected http:// or https:// link in squad chat prompts:
       "Do you trust this link and want to open it in your default web browser?"
     Selecting Yes safely opens the link in your web browser.


+------------------------------------------------------------------------------+
| 4. HOTKEYS                                                                   |
+------------------------------------------------------------------------------+

  * Master Pause / Resume (Global):
    Ctrl + F10 (customizable with click-to-record in Settings)

  * Toggle Translation Only:
    Ctrl + F11 (customizable with click-to-record in Settings)

  * Toggle Quick Chat Only:
    Ctrl + F12 (customizable in Settings & Quick Chat tab)

  * Tactical Quick Chat Slots:
    NumPad 0 through NumPad 9 (customizable in the Quick Chat tab)

  * Tactical Quick Chat Overlay:
    NumPad + (customizable in the Quick Chat tab)


+------------------------------------------------------------------------------+
| 5. PRIVACY & SECURITY                                                        |
+------------------------------------------------------------------------------+

  * Secure Credential Storage (DPAPI): Sensitive preferences like AI provider 
    API tokens are encrypted at rest using the Windows Data Protection API
    (DPAPI, `CurrentUser` scope). Credentials are never stored in plaintext
    in configuration files and can only be decrypted on your local machine by
    your Windows account.

  * Zero Data Collection & Zero Telemetry:
    This tool DOES NOT collect, track, store, or send any user data, gameplay
    statistics, or telemetry. All communications are strictly local or sent
    directly and exclusively to your configured AI provider endpoint using
    your own personal API credentials.

  * Local Encryption (DPAPI):
    AI provider API keys are encrypted locally on your machine using Windows
    Data Protection API (DPAPI) tied to your Windows user account. Keys are
    never stored in plain text and never sent to third parties.


+------------------------------------------------------------------------------+
| 6. SYSTEM REQUIREMENTS                                                       |
+------------------------------------------------------------------------------+

  * Operating System : Windows 10 / 11 64-bit (x64)
  * Runtime          : Microsoft .NET 10 Desktop Runtime (x64)
  * Build Tools (Dev): .NET 10 SDK, LLVM Clang++, and LuaJIT 2.0+ (luajit.exe)
  * Target Game      : Helldivers (2015) on Steam PC (helldivers.exe)
  * Internet Access  : Required for AI translation API calls


+------------------------------------------------------------------------------+
| 7. CREDITS, SUPPORT & LICENSE                                                |
+------------------------------------------------------------------------------+

  * Author            : difinim
  * GitHub Repository : https://github.com/difinim/helldivers1-babel
  * Issues & Feedback : https://github.com/difinim/helldivers1-babel/issues
  * License           : MIT License

  Because Sweet Liberty sounds better when everyone understands the plan.
================================================================================
