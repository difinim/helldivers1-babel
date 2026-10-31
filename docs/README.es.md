# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Desciendan juntos. Hablen como uno solo.*

**Helldivers 1 Babel** es un asistente táctico de comunicación y traducción de chat no invasivo y de código abierto para *Helldivers (2015)*, porque la *dulce libertad suena mejor cuando todos entienden el plan*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Aviso de Compatibilidad:**
> **Helldivers 1 Babel está desarrollado estrictamente para Helldivers (2015) en PC (Steam).**
> **Es compatible con mods.**
> **NO es compatible con Helldivers 2 (2024)**, ya que es un juego completamente diferente y utiliza un sistema antitrampas a nivel de kernel.

## Índice de Contenidos

- [1. Visión General y Características Principales](#1-visión-general-y-características-principales)
- [2. Configuración e Instalación](#2-configuración-e-instalación)
  - [Requisitos](#requisitos)
  - [Configurar una Clave API de IA Gratuita](#configurar-una-clave-api-de-ia-gratuita)
  - [Descarga e Instalación (Versión Precompilada)](#descarga-e-instalación-versión-precompilada)
- [3. Guía de Uso](#3-guía-de-uso)
  - [Primeros Pasos](#primeros-pasos)
  - [Comunicación en el Juego y Casos de Uso](#comunicación-en-el-juego-y-casos-de-uso)
- [4. Desarrollo (Compilación desde el Código Fuente)](#4-desarrollo-compilación-desde-el-código-fuente)
  - [Requisitos para Desarrolladores](#requisitos-para-desarrolladores)
  - [Configuración Automatizada del Entorno](#configuración-automatizada-del-entorno)
  - [Compilación Principal y Empaquetado de Versión](#compilación-principal-y-empaquetado-de-versión)
  - [Compilación Manual Paso a Paso](#compilación-manual-paso-a-paso)
- [5. Licencia](#5-licencia)

## 1. Visión General y Características Principales

- **Traducción Bidireccional de Chat en Tiempo Real:**
  - **Mensajes Entrantes:** Detecta automáticamente el idioma de los compañeros de equipo, traduce los mensajes a tu idioma de destino seleccionado (admite hasta 28 idiomas de destino, incluidos español, inglés, portugués PT-BR, ruso, chino, japonés, francés, alemán y más) y los muestra **localmente** en el chat del juego, **conservando fielmente el nombre original del jugador y su código de color** (J1 Dorado, J2 Azul, J3 Morado, J4 Cian).
  - **Mensajes Salientes:** Escribe `[idioma_destino]>>mensaje` o simplemente `>>mensaje` para responder automáticamente en el idioma del último mensaje recibido de un compañero. El texto traducido se transmite a través de RPC multijugador para que los compañeros internacionales lo lean de forma nativa.
- **Registros de Chat Persistentes y Exportación de Archivos:** Historial completo del chat de la sesión con marcas de tiempo, nombres de jugadores, colores, idiomas de origen detectados y latencia de traducción. Los registros persisten en la tabla entre reinicios de la aplicación mediante escrituras asíncronas por lotes en disco (configurables), con exportación a un archivo `.txt` con un solo clic.
- **Registros de Chat con Búsqueda:** Historial de chat con búsqueda completa que permite filtrar por cualquiera de las columnas.

![Helldivers 1 Babel en acción](screenshots/screenshot.png)

- **Chat Rápido Táctico (10 Ranuras Personalizables):** 10 ranuras de mensajes tácticos predefinidos y configurables, cada una asignada a teclas de acceso rápido (p. ej. teclado numérico 0-9 o combinaciones personalizadas) para transmitir avisos tácticos al instante en el chat multijugador del juego.
- **Mensaje de Bienvenida Automático de Chat Rápido:** Transmite automáticamente un mensaje de bienvenida personalizable al chat del juego cuando eres anfitrión de una sala y se une un jugador invitado.

![Chat Rápido](screenshots/quick-chat.png)

- **Superposición de Referencia de Chat Rápido en Pantalla:** Activa o desactiva una superposición flotante de referencia en el lado derecho de la pantalla con `Numpad +` (o tu atajo configurado) para consultar rápidamente todos los avisos y atajos tácticos activos durante la partida. *(Requiere que Helldivers se ejecute en pantalla completa sin bordes o modo ventana).*

![Superposición de Chat Rápido](screenshots/overlay.png)

- **Soporte de Interfaz Multiidioma:** Compatible con 28 idiomas.
- **Arquitectura de Adaptadores de Proveedor:** Diseñado con una interfaz modular de proveedores de IA. Incluye un adaptador optimizado sin tiempo de reflexión (zero-thinking) para **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), con contratos extensibles para OpenAI, Claude y DeepSeek (aún no implementados).
- **Ajuste de Tono de Traducción:** Casual: jerga informal de juegos y terminología gamer / Normal: tono conversacional natural sin jerga de videojuegos.
- **Atajo Global de Pausa / Reanudación:** Presiona `Ctrl + F10` (o tu atajo configurado) en cualquier momento para alternar el asistente entre **Activo** y **Pausado**. Pausar suspende de manera limpia tanto la traducción del chat en el juego como los avisos de Chat Rápido.
- **Atajos Dedicados para Funciones Específicas:**
  - **Alternar Solo Traducción:** Presiona `Ctrl + F11` (o tu atajo configurado) para pausar o reanudar instantáneamente la traducción del chat sin afectar el Chat Rápido (con notificación emergente en pantalla).
  - **Alternar Solo Chat Rápido:** Presiona `Ctrl + F12` (o tu atajo configurado) para pausar o reanudar instantáneamente los avisos de Chat Rápido sin afectar la traducción (con notificación emergente en pantalla).

![Ajustes](screenshots/settings.png)

- **Almacenamiento Seguro de Credenciales (DPAPI):** Las preferencias confidenciales, como los tokens API del proveedor de IA, se cifran en reposo utilizando la API de protección de datos de Windows (DPAPI, ámbito `CurrentUser`). Las credenciales nunca se guardan en texto sin formato en los archivos de configuración y solo pueden descifrarse en tu equipo local mediante tu cuenta de Windows.
- **Cero Telemetría y Privacidad Absoluta:** Esta herramienta no recopila, registra ni envía datos de usuario o de telemetría. Todas las solicitudes de traducción se comunican de forma directa y exclusiva con el endpoint de tu proveedor de IA configurado utilizando tus credenciales de API personales.
- **100% No Invasivo (Cero Modificación de Archivos del Juego):** Ningún archivo comprimido del juego es desempaquetado o modificado. Los archivos de tu juego permanecen 100% intactos y originales.




## 2. Configuración e Instalación

### Requisitos
Si utilizas la versión precompilada independiente (`HD1-Babel.exe`):
- **Sistema Operativo:** Windows 10 / 11 de 64 bits (x64).
- **Entorno de Ejecución:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (instalador directo).
- **Juego Destino:** *Helldivers* (2015) en Steam (`helldivers.exe` de 64 bits).

### Configurar una Clave API de IA Gratuita
- **Credenciales del Proveedor de IA:** Clave API de Google Gemini (obtén una clave personal gratuita en [Google AI Studio](https://aistudio.google.com/) ([Ver este video tutorial](https://www.youtube.com/watch?v=YHFMrZD-_3g)).
- **Acceso a Internet:** Requerido para las llamadas a la API de traducción.

### Descarga e Instalación (Versión Precompilada)

**No** necesitas instalar herramientas de desarrollo ni compilar desde el código fuente para usar Babel ([pero si lo deseas](#4-desarrollo-compilación-desde-el-código-fuente)):
1. Dirígete a la sección **[Releases](https://github.com/difinim/helldivers1-babel/releases)** en la parte derecha de la página del proyecto en GitHub.
2. En la última versión, despliega la lista de **Assets**.
3. Descarga el archivo de distribución independiente: **`Helldivers1Babel-v<versión>-win-x64.zip`**.
4. Extrae el archivo zip en cualquier lugar de tu equipo (p. ej. `C:\GamesTools\Helldivers1Babel`).
5. Abre la carpeta y ejecuta **`HD1-Babel.exe`**.

> [!NOTE]
> **Prerrequisito:** Los usuarios finales solo necesitan el runtime gratuito [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). No se requiere ningún compilador C++, SDK ni herramientas de desarrollo para ejecutar la versión publicada.

> [!NOTE]
> **A tener en cuenta**: Debido a que InjectorService.cs utiliza las APIs estándar de inyección de procesos de Windows (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender o antivirus de terceros _**podrían**_ activar una detección heurística (p. ej. Generic HackTool). Este es un comportamiento normal en inyectores de modificaciones de juegos y el archivo puede incluirse en la lista de exclusiones de forma segura.



## 3. Guia de Uso

### Primeros Pasos
1. Inicia **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. En el primer inicio, abre la pestaña **Ajustes**:
   - Introduce tu **Clave API de Google Gemini** (almacenada de forma segura mediante cifrado Windows DPAPI).
   - Selecciona tu idioma de destino (p. ej. `Spanish`).
   - Haz clic en **Probar Conexión API** para verificar tu clave y latencia.
   - Personaliza el atajo de **Pausa / Reanudación** si lo deseas (por defecto `Ctrl + F10`).
3. Configura tus avisos tácticos en la pestaña **Chat Rápido** (p. ej. asigna Numpad 0-9 a tus mensajes preferidos).
4. Inicia **Helldivers** en Steam. Babel detecta automáticamente `helldivers.exe`, inyecta `BabelHook.dll` y establece la comunicación IPC de alta velocidad mediante named pipes.

### Comunicación en el Juego y Casos de Uso

#### 1. Traducción Automática Entrante
Los mensajes entrantes de compañeros de equipo se detectan y traducen a tu idioma de destino sobre la marcha, mostrándose en el chat del juego con el nombre y color original del jugador:
- Compañero (Inglés): `so quiet`
- Chat en el Juego: `Diveheller: (English): qué silencio`

#### 2. Respuesta Automática al Último Idioma Recibido (`>>mensaje`)
Al anteponer `>>` a tu mensaje de chat, el texto se traduce automáticamente al idioma del último mensaje entrante recibido de un compañero:
- Tu entrada en el chat: `>>sí, está demasiado tranquilo`
- El compañero ve: `yes, it is too quiet`

#### 3. Selección Explícita / Arbitraria de Idioma (`idioma de destino>>mensaje`)
Dirige tu mensaje a un idioma específico en cualquier momento anteponiendo el nombre del idioma seguido de `>>`:

| Comando de Entrada en Chat | Idioma de Destino | Ejemplo de Resultado Transmitido a Compañeros |
| :--- | :--- | :--- |
| `russian>>buenos días!` | Ruso | `доброе утро!` |
| `english>>minas al norte` | Inglés | `mines to the north` |
| `japanese>>cuidado con los disparos!` | Japonés | `射線に注意せよ！` |
| `spanish>>preciso de munição` | Español | `necesito munición` |
| `german>>retirada al punto de extracción` | Alemán | `Rückzug zum Extraktionspunkt` |
| `french>>defendamos esta posición` | Francés | `défendons cette position` |

#### 4. Mensajes Salientes Estándar y Supresión de Auto-Traducción
Al escribir un mensaje de chat normal en el juego sin `>>` (p. ej. `need ammo`, `vamos`), tu mensaje se transmite directamente a través del chat multijugador original. Con la opción **"No traducir mensajes salientes estándar"** activada (activada por defecto tanto en la barra de control rápido como en Ajustes), Babel suprime la traducción de tu propio mensaje de vuelta hacia ti. Si se desactiva, Babel también procesará tus mensajes locales estándar mediante el canal de traducción entrante.

#### 5. Atajos de Chat Rápido Táctico
Presiona cualquier tecla de acceso rápido configurada de chat rápido (p. ej. `NumPad 1`-`NumPad 9`) mientras juegas para transmitir de inmediato avisos tácticos al chat multijugador sin abrir la interfaz de texto. Todas las pulsaciones de teclas continúan pasando al juego con total normalidad, garantizando una respuesta fluida de tus controles.

#### 6. Superposición de Referencia de Chat Rápido Táctico (`Numpad +`)
Presiona `Numpad +` (o tu atajo configurado en la pestaña **Chat Rápido**) en cualquier momento para mostrar u ocultar una superposición de referencia en pantalla con todos los atajos activos de Chat Rápido. La superposición aparece alineada a la derecha y centrada verticalmente, mostrando cada aviso activo con el formato `[<atajo>] <mensaje>` (con mensajes truncados a 30 caracteres) utilizando el mismo estilo visual no intrusivo de las notificaciones emergentes.
> [!NOTE]
> **Requisito del Modo de Pantalla:** Helldivers debe estar configurado para ejecutarse en **Pantalla completa sin bordes** (o Ventana) en los ajustes de Vídeo del juego para que las ventanas de superposición de Windows puedan mostrarse sobre el juego.

#### 7. Pausa, Reanudación y Alternadores de Funciones
- **Pausa / Reanudación Global (`Ctrl + F10`):** Interruptor maestro que suspende o reanuda limpiamente tanto la interceptación de traducción como los avisos de Chat Rápido simultáneamente.
- **Alternar Solo Traducción (`Ctrl + F11`):** Activa o desactiva únicamente la función de traducción con un aviso emergente en pantalla. El Chat Rápido continúa funcionando normalmente.
- **Alternar Solo Chat Rápido (`Ctrl + F12`):** Activa o desactiva únicamente el envío de mensajes de Chat Rápido con un aviso emergente en pantalla. La traducción de chat en el juego continúa funcionando normalmente.
*(Todos los atajos se pueden personalizar en las pestañas **Ajustes** y **Chat Rápido** utilizando los botones de captura de atajo por clic).*

#### 8. Exportación y Registros de Chat Persistentes
Haz clic en **Exportar Registros** en la pestaña Traducción y Registros en cualquier momento para elegir una ubicación y exportar tu historial de chat (con marcas de tiempo, nombres de jugadores, mensajes originales, traducciones y latencias) a un archivo `.txt`. Los mensajes persisten automáticamente en la tabla entre inicios de la aplicación mediante operaciones asíncronas por lotes en disco (el límite de almacenamiento es configurable de 0 a 100 MB en Ajustes, con un botón para **Limpiar memoria de registros**).

#### 9. Enlaces Interactivos en Mensajes
Los mensajes del chat del escuadrón que contengan enlaces `http://` o `https://` se muestran como hipervínculos interactivos en la columna **Mensaje Original**. Al hacer clic, un cuadro de confirmación te solicita verificar si confías en el enlace antes de abrirlo en tu navegador web predeterminado.



## 4. Desarrollo (Compilación desde el Código Fuente)

### Requisitos para Desarrolladores
Si deseas compilar y generar el proyecto desde el código fuente:
- **Sistema Operativo:** Windows 10 / 11 de 64 bits (x64).
- **SDK de .NET 10:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Cadena de Herramientas MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) con los paquetes `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` y `mingw-w64-x86_64-make` — requeridos para compilar la biblioteca de hook nativa (`BabelHook.dll`) y compilar el submódulo oficial de LuaJIT 2.0 (`vendor/luajit`) para compilar el script de inicio en el juego (`babel_bootstrap.lua`) a bytecode.
- **Git:** Git for Windows (con submódulos recursivos para MinHook amalgamation y LuaJIT 2.0).

### Configuración Automatizada del Entorno
Haz doble clic en `setup.bat` (o ejecuta `setup.ps1` en PowerShell) en la raíz del repositorio. Verificará e instalará automáticamente los prerrequisitos faltantes:
1. Verifica el entorno Windows de 64 bits.
2. Comprueba la presencia del SDK de .NET 10 (se instala automáticamente mediante `winget` si falta).
3. Comprueba la cadena de herramientas MSYS2 MinGW64 (Clang++, GCC y Make). Si falta, instala automáticamente MSYS2 mediante `winget` e instala los paquetes necesarios (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` y `mingw-w64-x86_64-make`) mediante `pacman`.
4. Inicializa y compila el compilador LuaJIT 2.0 desde el submódulo (`vendor/luajit`).
5. Asegura automáticamente la inclusión de `C:\msys64\mingw64\bin` y `C:\msys64\usr\bin` en la variable PATH.
6. Verifica las dependencias de MinHook amalgamation y realiza una comprobación inicial de compilación.

### Compilación Principal y Empaquetado de Versión
Para limpiar artefactos anteriores, compilar la DLL nativa de hook, ejecutar todas las pruebas unitarias y generar el paquete de distribución final:
```powershell
pwsh -File build.ps1
```
El directorio de salida se empaquetará en `dist/Helldivers1Babel-vX.Y.Z/`.

### Compilación Manual Paso a Paso
```powershell
# 1. Compilar la DLL nativa de hook (compila automáticamente babel_bootstrap.lua con luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Compilar la solución .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Ejecutar la suite de pruebas automatizadas
dotnet test
```



## 5. Licencia

Distribuido bajo la [Licencia MIT](../LICENSE).
