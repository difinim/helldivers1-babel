# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Plongez ensemble. Parlez d'une seule voix.*

**Helldivers 1 Babel** est un assistant de communication tactique et de traduction de discussion open-source et non invasif pour *Helldivers (2015)*, car la *douce liberté résonne mieux quand tout le monde comprend le plan*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Avertissement de compatibilité :**
> **Helldivers 1 Babel est strictement développé pour Helldivers (2015) sur PC (Steam).**
> **Il est compatible avec les mods.**
> **Il n'est PAS compatible avec Helldivers 2 (2024)**, qui est totalement différent et utilise un anti-triche au niveau du noyau (kernel).

## Table des matières

- [1. Présentation et fonctionnalités clés](#1-présentation-et-fonctionnalités-clés)
- [2. Installation et configuration](#2-installation-et-configuration)
  - [Prérequis](#prérequis)
  - [Obtenir une clé API d'IA gratuite](#obtenir-une-clé-api-dia-gratuite)
  - [Téléchargement et installation (Version précompilée)](#téléchargement-et-installation-version-précompilée)
- [3. Guide d'utilisation](#3-guide-dutilisation)
  - [Prise en main](#prise-en-main)
  - [Communication en jeu et cas d'usage](#communication-en-jeu-et-cas-dusage)
- [4. Développement (Compilation depuis les sources)](#4-développement-compilation-depuis-les-sources)
  - [Prérequis pour les développeurs](#prérequis-pour-les-développeurs)
  - [Configuration automatisée de l'environnement](#configuration-automatisée-de-lenvironnement)
  - [Compilation principale et création de paquet](#compilation-principale-et-création-de-paquet)
  - [Compilation manuelle étape par étape](#compilation-manuelle-étape-par-étape)
- [5. Licence](#5-licence)

## 1. Présentation et fonctionnalités clés

- **Traduction bidirectionnelle du chat en temps réel :**
  - **Messages entrants :** Détecte automatiquement la langue de vos coéquipiers, traduit les messages dans votre langue cible choisie (prend en charge jusqu'à 28 langues cibles, dont le français, l'anglais, l'espagnol, le portugais PT-BR, le russe, le chinois, le japonais, l'allemand, etc.) et les affiche **localement** dans le chat en jeu tout en **préservant fidèlement le nom et le code couleur d'origine du coéquipier** (J1 Or, J2 Bleu, J3 Violet, J4 Cyan).
  - **Messages sortants :** Tapez `[langue_cible]>>message` ou simplement `>>message` pour répondre automatiquement dans la langue du dernier message reçu d'un coéquipier. Le texte traduit est diffusé via le RPC multijoueur afin que vos coéquipiers internationaux le lisent dans leur langue maternelle.
- **Journaux de chat persistants et export de fichiers :** Suivi complet de l'historique du chat de session avec horodatage, noms des joueurs, couleurs, langues sources détectées et latence de traduction. Les journaux sont conservés dans le tableau lors des redémarrages de l'application grâce à des écritures légères et asynchrones par lots sur le disque (configurables), avec un export en fichier `.txt` en un clic.
- **Recherche dans les journaux de chat :** Historique de chat entièrement consultable avec filtrage sur n'importe quelle colonne.

![Helldivers 1 Babel en action](screenshots/screenshot.png)

- **Chat Rapide tactique (10 emplacements personnalisables) :** 10 emplacements de messages tactiques prédéfinis et configurables, chacun lié à un raccourci clavier (ex. pavé numérique 0-9 ou combinaisons personnalisées) pour transmettre instantanément des signaux tactiques dans le chat multijoueur en jeu.

![Chat Rapide](screenshots/quick-chat.png)

- **Message de bienvenue automatique du Chat Rapide :** Diffuse automatiquement un message de bienvenue personnalisable dans le chat du jeu lorsque vous hébergez une partie et qu'un joueur invité la rejoint.
- **Overlay de référence du Chat Rapide à l'écran :** Affichez ou masquez un overlay flottant de référence qui ne vole pas le focus sur le côté droit de l'écran avec `Numpad +` (ou votre raccourci enregistré) pour consulter rapidement tous vos raccourcis et signaux tactiques actifs en cours de partie. *(Nécessite que Helldivers soit exécuté en mode Plein écran fenêtré sans bordure ou en mode Fenêtré).*

![Overlay de Chat Rapide](screenshots/overlay.png)

- **Interface utilisateur multilingue :** Prise en charge de 28 langues.
- **Architecture d'adaptateurs de fournisseurs :** Conçu avec une interface modulaire de fournisseurs d'IA. Comprend un adaptateur optimisé sans délai de réflexion (zero-thinking) pour **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), avec des contrats extensibles pour OpenAI, Claude et DeepSeek (pas encore implémentés).
- **Réglage du ton de la traduction :** Décontracté : argot informel de jeu et expressions tactiques de gamers / Normal : ton conversationnel naturel sans jargon vidéoludique.
- **Raccourci global Pause / Reprise :** Appuyez à tout moment sur `Ctrl + F10` (ou votre raccourci configuré) pour basculer l'assistant entre **Actif** et **En pause**. La pause suspend proprement l'interception de la traduction et la diffusion des messages du Chat Rapide.
- **Raccourcis dédiés par fonctionnalité :**
  - **Basculer uniquement la traduction :** Appuyez sur `Ctrl + F11` (ou votre raccourci enregistré) pour suspendre ou reprendre instantanément la traduction du chat sans affecter le Chat Rapide (avec notification toast à l'écran).
  - **Basculer uniquement le Chat Rapide :** Appuyez sur `Ctrl + F12` (ou votre raccourci enregistré) pour suspendre ou reprendre instantanément les messages du Chat Rapide sans affecter la traduction (avec notification toast à l'écran).

![Paramètres](screenshots/settings.png)

- **Stockage sécurisé des identifiants (DPAPI) :** Les préférences sensibles, telles que les jetons d'API des fournisseurs d'IA, sont chiffrées au repos à l'aide de l'API de protection des données de Windows (DPAPI, portée `CurrentUser`). Les identifiants ne sont jamais stockés en clair dans les fichiers de configuration et ne peuvent être déchiffrés que sur votre ordinateur local par votre compte Windows.
- **Zéro télémétrie et respect total de la vie privée :** Cet outil ne collecte, n'enregistre ni n'envoie aucune donnée utilisateur ou de télémétrie. Toutes les requêtes de traduction communiquent directement et exclusivement avec le point de terminaison de votre fournisseur d'IA configuré à l'aide de vos identifiants d'API personnels.
- **100 % non invasif (aucune modification des fichiers du jeu) :** Aucune archive du jeu n'est extraite ou modifiée. Les fichiers de votre jeu restent 100 % d'origine et intacts.




## 2. Installation et configuration

### Prérequis
Si vous utilisez la version autonome précompilée (`HD1-Babel.exe`) :
- **Système d'exploitation :** Windows 10 / 11 64 bits (x64).
- **Runtime de bureau :** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (installateur direct).
- **Jeu cible :** *Helldivers* (2015) sur Steam (`helldivers.exe` 64 bits).

### Obtenir une clé API d'IA gratuite
- **Identifiants du fournisseur d'IA :** Clé API Google Gemini (obtenez une clé personnelle gratuite sur [Google AI Studio](https://aistudio.google.com/) ([Voir ce tutoriel vidéo](https://www.youtube.com/watch?v=YHFMrZD-_3g)).
- **Accès Internet :** Requis pour les appels d'API de traduction.

### Téléchargement et installation (Version précompilée)

Vous n'avez **pas** besoin d'installer des outils de développement ou de compiler à partir des sources pour utiliser Babel ([mais si vous le souhaitez](#4-développement-compilation-depuis-les-sources)) :
1. Rendez-vous dans la section **[Releases](https://github.com/difinim/helldivers1-babel/releases)** sur le côté droit de la page GitHub du projet.
2. Sous la dernière version, développez la liste des **Assets**.
3. Téléchargez l'archive de distribution autonome : **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Extrayez le fichier zip n'importe où sur votre ordinateur (ex. `C:\GamesTools\Helldivers1Babel`).
5. Ouvrez le dossier et lancez **`HD1-Babel.exe`**.

> [!NOTE]
> **Prérequis :** Les utilisateurs finaux ont uniquement besoin du runtime gratuit [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Aucun compilateur C++, SDK ou chaîne d'outils n'est requis pour exécuter la version finale.

> [!NOTE]
> **À noter :** Parce que InjectorService.cs utilise les API standards d'injection de processus Windows (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender ou des antivirus tiers _**peuvent**_ déclencher une détection heuristique (par ex. Generic HackTool). Il s'agit d'un comportement normal pour les injecteurs de mods de jeux et l'exécutable peut être ajouté aux exceptions en toute sécurité.



## 3. Guide d'utilisation

### Prise en main
1. Lancez **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Lors du premier lancement, ouvrez l'onglet **Paramètres** :
   - Saisissez votre **Clé API Google Gemini** (stockée en toute sécurité via le chiffrement Windows DPAPI).
   - Sélectionnez votre langue cible (ex. `French`).
   - Cliquez sur **Tester la connexion API** pour vérifier votre clé et la latence.
   - Personnalisez le raccourci **Pause / Reprise** si vous le souhaitez (`Ctrl + F10` par défaut).
3. Configurez vos signaux tactiques dans l'onglet **Chat Rapide** (ex. associez le pavé numérique 0-9 aux messages souhaités).
4. Lancez **Helldivers** sur Steam. Babel détecte automatiquement `helldivers.exe`, injecte `BabelHook.dll` et établit une communication IPC haute vitesse via named pipe.

### Communication en jeu et cas d'usage

#### 1. Traduction automatique entrante
Les messages entrants de vos coéquipiers sont détectés et traduits à la volée dans votre langue cible, puis affichés dans le chat du jeu avec le nom et la couleur d'origine du joueur :
- Coéquipier (anglais) : `so quiet`
- Chat en jeu : `Diveheller: (English): trop calme`

#### 2. Réponse automatique dans la langue du dernier message entrant (`>>message`)
Préfixer votre message de chat avec `>>` traduit automatiquement votre texte dans la langue du dernier message reçu d'un coéquipier :
- Votre saisie dans le chat : `>>oui, c'est trop calme`
- Le coéquipier voit : `yes, it is too quiet`

#### 3. Sélection explicite / arbitraire de la langue (`langue cible>>message`)
Dirigez votre message vers une langue spécifique à tout moment en le préfixant par le nom de la langue suivi de `>>` :

| Commande saisie dans le chat | Langue cible | Exemple de résultat diffusé aux coéquipiers |
| :--- | :--- | :--- |
| `russian>>bonjour !` | Russe | `доброе утро!` |
| `english>>mines au nord` | Anglais | `mines to the north` |
| `japanese>>attention aux tirs !` | Japonais | `射線に注意せよ！` |
| `spanish>>besoin de munitions` | Espagnol | `necesito munición` |
| `german>>repliez-vous vers le point d'extraction` | Allemand | `Rückzug zum Extraktionspunkt` |
| `french>>défendons cette position` | Français | `défendons cette position` |

#### 4. Messages sortants standard et suppression de l'auto-traduction
Lorsque vous tapez un message normal dans le chat en jeu sans `>>` (par ex. `need ammo`, `en route`), votre message est diffusé directement sur le chat multijoueur d'origine. Avec l'option **« Ne pas traduire les messages sortants standard »** activée (activée par défaut dans la barre d'accès rapide et dans les Paramètres), Babel évite de traduire en retour votre propre message. Si vous la désactivez, Babel traitera également vos messages locaux standard via le pipeline de traduction entrante.

#### 5. Raccourcis du Chat Rapide tactique
Appuyez sur n'importe quel raccourci configuré du Chat Rapide (par ex. `NumPad 1`-`NumPad 9`) pendant la partie pour diffuser instantanément vos signaux tactiques prédéfinis dans le chat multijoueur sans avoir à ouvrir l'invite de chat. Toutes les frappes de touches continuent d'être transmises au jeu sans aucune interruption, garantissant une réactivité totale de vos commandes.

#### 6. Overlay de référence du Chat Rapide tactique (`Numpad +`)
Appuyez sur `Numpad +` (ou sur votre raccourci configuré dans l'onglet **Chat Rapide**) à tout moment pour afficher ou masquer un overlay de référence à l'écran répertoriant tous les raccourcis actifs du Chat Rapide. L'overlay s'affiche aligné à droite et centré verticalement, présentant chaque raccourci actif selon le format `[<raccourci>] <message>` (avec des messages tronqués à 30 caractères) en utilisant le même style visuel non intrusif que les notifications à l'écran.
> [!NOTE]
> **Condition relative au mode d'affichage :** Helldivers doit être configuré en mode **Plein écran fenêtré sans bordure** (ou en mode Fenêtré) dans les paramètres vidéo du jeu pour que les fenêtres d'overlay Windows puissent s'afficher par-dessus le jeu.

#### 7. Pause, reprise et bascules de fonctionnalités
- **Pause / Reprise globale (`Ctrl + F10`) :** Interrupteur principal qui suspend ou reprend proprement l'interception de la traduction et la diffusion des messages du Chat Rapide en même temps.
- **Basculer uniquement la traduction (`Ctrl + F11`) :** Active ou désactive uniquement la traduction avec une notification toast à l'écran. Le Chat Rapide continue de fonctionner normalement.
- **Basculer uniquement le Chat Rapide (`Ctrl + F12`) :** Active ou désactive uniquement la diffusion des messages du Chat Rapide avec une notification toast à l'écran. La traduction du chat en jeu continue de fonctionner normalement.
*(Tous les raccourcis peuvent être personnalisés dans les onglets **Paramètres** et **Chat Rapide** à l'aide des boutons d'enregistrement par clic).*

#### 8. Exportation et persistance des journaux de chat
Cliquez sur **Exporter les journaux** dans l'onglet Traduction et Journaux à tout moment pour choisir un emplacement et exporter votre historique de chat (avec horodatage, noms des joueurs, messages originaux, traductions et latences) vers un fichier `.txt`. Les messages de discussion sont automatiquement conservés dans le tableau d'une session à l'autre grâce à des écritures asynchrones par lots sur le disque (la limite de stockage est réglable de 0 à 100 Mo dans les Paramètres, avec un bouton **Effacer la mémoire des journaux**).

#### 9. Liens interactifs dans les messages
Les messages du chat d'escouade contenant des URL en `http://` ou `https://` s'affichent sous forme de liens cliquables dans la colonne **Message d'origine**. Lorsque vous cliquez dessus, une boîte de dialogue de confirmation vous invite à vérifier si vous faites confiance au lien avant de l'ouvrir dans votre navigateur web par défaut.



## 4. Développement (Compilation depuis les sources)

### Prérequis pour les développeurs
Si vous compilez et construisez le projet à partir des sources :
- **Système d'exploitation :** Windows 10 / 11 64 bits (x64).
- **SDK .NET 10 :** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Chaîne d'outils MSYS2 MinGW64 (Clang++, GCC, Make) :** [MSYS2](https://www.msys2.org/) avec les paquets `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` et `mingw-w64-x86_64-make` — requis pour compiler la bibliothèque de hook native (`BabelHook.dll`) et construire le sous-module officiel LuaJIT 2.0 (`vendor/luajit`) afin de compiler le script de démarrage en jeu (`babel_bootstrap.lua`) en bytecode.
- **Git :** Git for Windows (avec sous-modules récursifs pour l'amalgamation MinHook et LuaJIT 2.0).

### Configuration automatisée de l'environnement
Double-cliquez sur `setup.bat` (ou lancez `setup.ps1` dans PowerShell) à la racine du dépôt. Il vérifie et installe automatiquement les prérequis manquants :
1. Vérifie l'environnement Windows 64 bits.
2. Vérifie la présence du SDK .NET 10 (installation automatique via `winget` s'il est manquant).
3. Vérifie la chaîne d'outils MSYS2 MinGW64 (Clang++, GCC et Make). Si elle est manquante, installe automatiquement MSYS2 via `winget` et installe les paquets requis (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` et `mingw-w64-x86_64-make`) via `pacman`.
4. Initialise et compile le compilateur LuaJIT 2.0 à partir du sous-module (`vendor/luajit`).
5. S'assure automatiquement que `C:\msys64\mingw64\bin` et `C:\msys64\usr\bin` sont ajoutés au PATH.
6. Vérifie les dépendances de l'amalgamation MinHook et exécute un test de compilation initial.

### Compilation principale et création de paquet
Pour nettoyer les artefacts précédents, compiler la DLL de hook native, exécuter tous les tests unitaires et assembler le paquet final :
```powershell
pwsh -File build.ps1
```
Le répertoire de sortie sera empaqueté dans `dist/Helldivers1Babel-vX.Y.Z/`.

### Compilation manuelle étape par étape
```powershell
# 1. Compiler la DLL de hook native (compile automatiquement babel_bootstrap.lua avec luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Compiler la solution .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Exécuter les suites de tests automatisés
dotnet test
```



## 5. Licence

Distribué sous la [Licence MIT](../LICENSE).
