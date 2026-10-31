# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Scendete in campo insieme. Parlate con una sola voce.*

**Helldivers 1 Babel** è un assistente tattico di comunicazione e traduzione chat open source e non invasivo per *Helldivers (2015)*, perché la *dolce libertà suona meglio quando tutti capiscono il piano*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Dichiarazione di Compatibilità:**
> **Helldivers 1 Babel è sviluppato esclusivamente per Helldivers (2015) su PC (Steam).**
> **È compatibile con le mod.**
> **NON è compatibile con Helldivers 2 (2024)**, che è completamente diverso e utilizza un sistema anti-cheat a livello kernel.

## Indice dei Contenuti

- [1. Panoramica e Funzionalità Principali](#1-panoramica-e-funzionalità-principali)
- [2. Configurazione e Installazione](#2-configurazione-e-installazione)
  - [Requisiti](#requisiti)
  - [Configurare una Chiave API IA Gratuita](#configurare-una-chiave-api-ia-gratuita)
  - [Download e Installazione (Versione Precompilata)](#download-e-installazione-versione-precompilata)
- [3. Guida all'Uso](#3-guida-alluso)
  - [Primi Passi](#primi-passi)
  - [Comunicazione in Gioco e Casi d'Uso](#comunicazione-in-gioco-e-casi-duso)
- [4. Sviluppo (Compilazione dai Sorgenti)](#4-sviluppo-compilazione-dai-sorgenti)
  - [Requisiti per gli Sviluppatori](#requisiti-per-gli-sviluppatori)
  - [Configurazione Automatizzata dell'Ambiente](#configurazione-automatizzata-dellambiente)
  - [Compilazione Principale e Creazione Pacchetto di Rilascio](#compilazione-principale-e-creazione-pacchetto-di-rilascio)
  - [Compilazione Manuale Passo dopo Passo](#compilazione-manuale-passo-dopo-passo)
- [5. Licenza](#5-licenza)

## 1. Panoramica e Funzionalità Principali

- **Traduzione Bidirezionale della Chat in Tempo Reale:**
  - **Messaggi in Entrata:** Rileva automaticamente la lingua dei compagni di squadra, traduce i messaggi nella lingua di destinazione scelta (supporta fino a 28 lingue, tra cui italiano, inglese, spagnolo, portoghese PT-BR, russo, cinese, giapponese, francese, tedesco e altre) e li mostra **localmente** nella chat di gioco **preservando fedelmente il nome del giocatore e il relativo codice colore** (G1 Oro, G2 Blu, G3 Viola, G4 Ciano).
  - **Messaggi in Uscita:** Digita `[lingua_destinazione]>>messaggio` o semplicemente `>>messaggio` per rispondere automaticamente nella lingua dell'ultimo messaggio ricevuto da un compagno. Il testo tradotto viene trasmesso tramite RPC multiplayer in modo che i compagni internazionali possano leggerlo nella loro lingua nativa.
- **Registri Chat Persistenti ed Esportazione su File:** Cronologia completa della chat di sessione con timestamp, nomi dei giocatori, colori, lingue sorgente rilevate e latenza di traduzione. I registri persistono nella tabella tra i riavvii dell'applicazione tramite scritture asincrone su disco in batch (configurabili), con esportazione in file `.txt` con un solo clic.
- **Registri Chat con Ricerca:** Cronologia chat completamente ricercabile con filtri su qualsiasi colonna.

![Helldivers 1 Babel in azione](screenshots/screenshot.png)

- **Chat Rapida Tattica (10 Slot Personalizzabili):** 10 slot di messaggi tattici predefiniti e configurabili, ciascuno associato a tasti di scelta rapida (es. tastierino numerico 0-9 o combinazioni personalizzate) per trasmettere istantaneamente avvisi tattici nella chat multiplayer di gioco.
- **Messaggio di Benvenuto Automatico di Chat Rapida:** Trasmette automaticamente un messaggio di benvenuto personalizzabile nella chat di gioco quando ospiti una partita e si unisce un giocatore invitato.

![Chat Rapida](screenshots/quick-chat.png)

- **Overlay di Riferimento di Chat Rapida su Schermo:** Mostra o nascondi un overlay mobile di riferimento che non sottrae il focus sul lato destro dello schermo con `Numpad +` (o la scorciatoia registrata) per consultare rapidamente tutti i comandi rapidi e gli avvisi tattici attivi in partita. *(Richiede che Helldivers sia impostato su Finestra senza bordi o Finestra).*

![Overlay di Chat Rapida](screenshots/overlay.png)

- **Interfaccia Multilingue:** Supporto completo per 28 lingue.
- **Architettura Provider con Adapter:** Progettato con un'interfaccia modulare per i provider di IA. Include un adapter ottimizzato a latenza minima (zero-thinking) per **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), con contratti estensibili per OpenAI, Claude e DeepSeek (non ancora implementati).
- **Regolazione del Tono di Traduzione:** Informale: slang tipico dei videogiochi e gergo tattico da gamer / Normale: tono conversazionale naturale senza gergo videoludico.
- **Scorciatoia Globale Pausa / Riprendi:** Premi `Ctrl + F10` (o la scorciatoia configurata) in qualsiasi momento per alternare l'assistente tra **Attivo** e **In pausa**. La messa in pausa sospende in modo pulito sia l'intercettazione della chat che l'invio dei messaggi di Chat Rapida.
- **Scorciatoie Dedicate per Funzionalità:**
  - **Attiva/Disattiva Solo Traduzione:** Premi `Ctrl + F11` (o la scorciatoia registrata) per mettere in pausa o riprendere istantaneamente la traduzione della chat senza influire sulla Chat Rapida (con notifica toast su schermo).
  - **Attiva/Disattiva Solo Chat Rapida:** Premi `Ctrl + F12` (o la scorciatoia registrata) per mettere in pausa o riprendere istantaneamente i messaggi di Chat Rapida senza influire sulla traduzione (con notifica toast su schermo).

![Impostazioni](screenshots/settings.png)

- **Archiviazione Sicura delle Credenziali (DPAPI):** Le preferenze sensibili, come i token API dei provider di IA, sono crittografate a riposo tramite la Data Protection API di Windows (DPAPI, ambito `CurrentUser`). Le credenziali non vengono mai memorizzate in testo non crittografato nei file di configurazione e possono essere decifrate solo sul tuo computer locale dal tuo account Windows.
- **Zero Telemetria e Massima Privacy:** Questo strumento non raccoglie, non registra e non invia alcun dato utente o di telemetria. Tutte le richieste di traduzione comunicano direttamente ed esclusivamente con l'endpoint del provider di IA configurato utilizzando le tue credenziali API personali.
- **100% Non Invasivo (Nessuna Modifica ai File di Gioco):** Nessun archivio di gioco viene estratto o modificato. I file del gioco rimangono 100% originali e inalterati.




## 2. Configurazione e Installazione

### Requisiti
Se utilizzi la versione autonoma precompilata (`HD1-Babel.exe`):
- **Sistema Operativo:** Windows 10 / 11 a 64 bit (x64).
- **Runtime Desktop:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (programma di installazione diretto).
- **Gioco di Riferimento:** *Helldivers* (2015) su Steam (`helldivers.exe` a 64 bit).

### Configurare una Chiave API IA Gratuita
- **Credenziali del Provider IA:** Chiave API di Google Gemini (ottieni una chiave personale gratuita da [Google AI Studio](https://aistudio.google.com/) ([Guarda questo video tutorial](https://www.youtube.com/watch?v=YHFMrZD-_3g)).
- **Connessione a Internet:** Richiesta per le chiamate API di traduzione.

### Download e Installazione (Versione Precompilata)

**Non** è necessario installare strumenti di sviluppo o compilare dai sorgenti per usare Babel ([ma se preferisci](#4-sviluppo-compilazione-dai-sorgenti)):
1. Vai alla sezione **[Releases](https://github.com/difinim/helldivers1-babel/releases)** sul lato destro della pagina del progetto su GitHub.
2. Nell'ultima versione disponibile, espandi l'elenco degli **Assets**.
3. Scarica l'archivio standalone: **`Helldivers1Babel-v<versione>-win-x64.zip`**.
4. Estrai il file zip in una cartella qualsiasi del tuo computer (es. `C:\GamesTools\Helldivers1Babel`).
5. Apri la cartella ed esegui **`HD1-Babel.exe`**.

> [!NOTE]
> **Prerequisito:** Gli utenti finali necessitano unicamente del runtime gratuito [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Non è richiesto alcun compilatore C++, SDK o toolchain per eseguire la release.

> [!NOTE]
> **Nota importante:** Poiché InjectorService.cs utilizza le API standard di Windows per l'iniezione dei processi (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender o antivirus di terze parti _**potrebbero**_ attivare un rilevamento euristico (es. Generic HackTool). Questo è un comportamento del tutto normale per gli iniettori di mod e il programma può essere tranquillamente aggiunto alle esclusioni.



## 3. Guida all'Uso

### Primi Passi
1. Avvia **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Al primo avvio, apri la scheda **Impostazioni**:
   - Inserisci la tua **Chiave API Google Gemini** (memorizzata in modo sicuro tramite crittografia Windows DPAPI).
   - Seleziona la tua lingua di destinazione (es. `Italian`).
   - Fai clic su **Test Connessione API** per verificare la chiave e la latenza.
   - Personalizza la scorciatoia **Pausa / Riprendi** se lo desideri (predefinita: `Ctrl + F10`).
3. Configura i tuoi avvisi tattici nella scheda **Chat Rapida** (es. associa il tastierino numerico 0-9 ai messaggi scelti).
4. Avvia **Helldivers** su Steam. Babel rileverà automaticamente `helldivers.exe`, inietterà `BabelHook.dll` e stabilirà la comunicazione IPC ad alta velocità tramite named pipe.

### Comunicazione in Gioco e Casi d'Uso

#### 1. Traduzione Automatica in Entrata
I messaggi in arrivo dai compagni di squadra vengono rilevati e tradotti all'istante nella tua lingua di destinazione, venendo mostrati nella chat di gioco con il nome e il colore originali del giocatore:
- Compagno (Inglese): `so quiet`
- Chat in gioco: `Diveheller: (English): che silenzio`

#### 2. Risposta Automatica all'Ultima Lingua Ricevuta (`>>messaggio`)
Aggiungendo il prefisso `>>` al tuo messaggio in chat, il testo viene tradotto automaticamente nella lingua dell'ultimo messaggio ricevuto da un compagno:
- Tuo messaggio digitato in chat: `>>sì, è troppo calmo`
- Il compagno visualizza: `yes, it is too quiet`

#### 3. Selezione Esplicita / Arbitraria della Lingua (`lingua di destinazione>>messaggio`)
Invia un messaggio in una lingua specifica in qualsiasi momento anteponendo il nome della lingua seguito da `>>`:

| Comando Digitato in Chat | Lingua di Destinazione | Esempio di Risultato Trasmesso ai Compagni |
| :--- | :--- | :--- |
| `russian>>buongiorno!` | Russo | `доброе утро!` |
| `english>>mine a nord` | Inglese | `mines to the north` |
| `japanese>>attenti al fuoco amico!` | Giapponese | `射線に注意せよ！` |
| `spanish>>mi servono munizioni` | Spagnolo | `necesito munición` |
| `german>>ritirata al punto di estrazione` | Tedesco | `Rückzug zum Extraktionspunkt` |
| `french>>difendiamo questa posizione` | Francese | `défendons cette position` |

#### 4. Messaggi Standard in Uscita e Soppressione dell'Auto-Traduzione
Quando digiti un normale messaggio di chat in gioco senza `>>` (es. `need ammo`, `andiamo`), il tuo messaggio viene inviato direttamente tramite la chat multiplayer originale. Con l'opzione **"Non tradurre messaggi in uscita standard"** abilitata (attiva per impostazione predefinita sia nella barra rapida che nelle Impostazioni), Babel evita di tradurre e reinviare a te stesso il tuo stesso messaggio. Se disattivata, Babel elaborerà anche i tuoi messaggi locali standard attraverso la pipeline di traduzione in entrata.

#### 5. Scorciatoie di Chat Rapida Tattica
Premi qualsiasi tasto di scelta rapida configurato per la Chat Rapida (es. `NumPad 1`-`NumPad 9`) durante la partita per inviare istantaneamente avvisi tattici nella chat multiplayer senza aprire il prompt di testo. Tutti i tasti continuano a essere inoltrati al gioco, garantendo la massima reattività dei comandi.

#### 6. Overlay di Riferimento di Chat Rapida Tattica (`Numpad +`)
Premi `Numpad +` (o la scorciatoia configurata nella scheda **Chat Rapida**) in qualsiasi momento per visualizzare o nascondere un overlay su schermo che elenca tutti i comandi attivi della Chat Rapida. L'overlay viene mostrato allineato a destra e centrato verticalmente sullo schermo, visualizzando ciascun comando attivo nel formato `[<scorciatoia>] <messaggio>` (con messaggi troncati a 30 caratteri) utilizzando lo stesso stile grafico discreto delle notifiche su schermo.
> [!NOTE]
> **Requisito della Modalità di Visualizzazione:** Helldivers deve essere configurato in modalità **Finestra senza bordi** (o Finestra) nelle impostazioni Video del gioco affinché le finestre di overlay di Windows possano essere visualizzate sopra il gioco.

#### 7. Pausa, Riprendi e Interruttori di Funzionalità
- **Pausa / Riprendi Globale (`Ctrl + F10`):** Interruttore principale che sospende o riprende in modo pulito sia l'intercettazione della traduzione che la trasmissione dei messaggi di Chat Rapida simultaneamente.
- **Attiva/Disattiva Solo Traduzione (`Ctrl + F11`):** Attiva o disattiva unicamente la traduzione con una notifica toast su schermo. La Chat Rapida continuerà a funzionare normalmente.
- **Attiva/Disattiva Solo Chat Rapida (`Ctrl + F12`):** Attiva o disattiva unicamente l'invio dei messaggi di Chat Rapida con una notifica toast su schermo. La traduzione della chat in gioco continuerà a funzionare normalmente.
*(Tutte le scorciatoie possono essere personalizzate nelle schede **Impostazioni** e **Chat Rapida** utilizzando i pulsanti di registrazione delle scorciatoie).*

#### 8. Esportazione e Registri Chat Persistenti
Fai clic su **Esporta Registri** nella scheda Traduzione e Registri in qualsiasi momento per selezionare un percorso ed esportare la cronologia della chat (con timestamp, nomi dei giocatori, messaggi originali, traduzioni e latenze) in un file `.txt`. I messaggi della chat persistono automaticamente nella tabella tra i diversi avvii dell'app grazie a salvataggi asincroni su disco in batch (il limite di archiviazione è configurabile da 0 a 100 MB nelle Impostazioni, con un pulsante **Cancella memoria registri**).

#### 9. Link Interattivi nei Messaggi
I messaggi della chat di squadra contenenti URL `http://` o `https://` vengono mostrati come collegamenti ipertestuali selezionabili nella colonna **Messaggio Originale**. Facendo clic su di essi, una finestra di dialogo ti chiederà conferma per verificare l'attendibilità del link prima di aprirlo nel browser web predefinito.



## 4. Sviluppo (Compilazione dai Sorgenti)

### Requisiti per gli Sviluppatori
Se desideri compilare ed eseguire il build del progetto dai sorgenti:
- **Sistema Operativo:** Windows 10 / 11 a 64 bit (x64).
- **SDK .NET 10:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Toolchain MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) con i pacchetti `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` e `mingw-w64-x86_64-make` — necessari per compilare la libreria di hook nativa (`BabelHook.dll`) e compilare il sottomodulo ufficiale di LuaJIT 2.0 (`vendor/luajit`) per compilare lo script di bootstrap in-game (`babel_bootstrap.lua`) in bytecode.
- **Git:** Git per Windows (con sottomoduli ricorsivi per MinHook amalgamation e LuaJIT 2.0).

### Configurazione Automatizzata dell'Ambiente
Fai doppio clic su `setup.bat` (o esegui `setup.ps1` in PowerShell) nella directory radice del repository. Lo script verificherà e installerà automaticamente i prerequisiti mancanti:
1. Verifica l'ambiente Windows a 64 bit.
2. Controlla la presenza di .NET 10 SDK (installazione automatica tramite `winget` se mancante).
3. Controlla la toolchain MSYS2 MinGW64 (Clang++, GCC e Make). Se mancante, installa automaticamente MSYS2 tramite `winget` e scarica i pacchetti necessari (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` e `mingw-w64-x86_64-make`) tramite `pacman`.
4. Inizializza e compila il compilatore LuaJIT 2.0 dal sottomodulo (`vendor/luajit`).
5. Aggiunge automaticamente `C:\msys64\mingw64\bin` e `C:\msys64\usr\bin` al PATH di sistema.
6. Verifica le dipendenze di MinHook amalgamation ed esegue un test iniziale di compilazione.

### Compilazione Principale e Creazione Pacchetto di Rilascio
Per ripulire gli artefatti precedenti, compilare la DLL nativa di hook, eseguire tutti i test unitari e assemblare il pacchetto di rilascio:
```powershell
pwsh -File build.ps1
```
La directory di output verrà compressa in `dist/Helldivers1Babel-vX.Y.Z/`.

### Compilazione Manuale Passo dopo Passo
```powershell
# 1. Compila la DLL di hook nativa (compila automaticamente babel_bootstrap.lua con luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Compila la soluzione .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Esegui la suite di test automatizzati
dotnet test
```



## 5. Licenza

Distribuito sotto [Licenza MIT](../LICENSE).
