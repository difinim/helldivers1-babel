# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Mergulhem juntos. Falem como um só.*

**Helldivers 1 Babel** é um assistente tático de comunicação e tradução de chat de código aberto e não invasivo para *Helldivers (2015)*, porque a *doce liberdade soa muito melhor quando todos entendem o plano*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Aviso de Compatibilidade:**
> **Helldivers 1 Babel foi desenvolvido estritamente para Helldivers (2015) no PC (Steam).**
> **É compatível com mods.**
> **NÃO é compatível com Helldivers 2 (2024)**, que é um jogo completamente diferente e utiliza sistema antitrapaça a nível de kernel.

## Sumário

- [1. Visão Geral e Principais Recursos](#1-visão-geral-e-principais-recursos)
- [2. Instalação e Configuração](#2-instalação-e-configuração)
  - [Requisitos](#requisitos)
  - [Configurar uma Chave de API de IA Gratuita](#configurar-uma-chave-de-api-de-ia-gratuita)
  - [Download e Instalação (Versão Pré-compilada)](#download-e-instalação-versão-pré-compilada)
- [3. Guia de Uso](#3-guia-de-uso)
  - [Primeiros Passos](#primeiros-passos)
  - [Comunicação no Jogo e Casos de Uso](#comunicação-no-jogo-e-casos-de-uso)
- [4. Desenvolvimento (Compilando a partir do Código-Fonte)](#4-desenvolvimento-compilando-a-partir-do-código-fonte)
  - [Requisitos para Desenvolvedores](#requisitos-para-desenvolvedores)
  - [Configuração Automatizada do Ambiente](#configuração-automatizada-do-ambiente)
  - [Compilação Principal e Empacotamento de Versão](#compilação-principal-e-empacotamento-de-versão)
  - [Compilação Manual Passo a Passo](#compilação-manual-passo-a-passo)
- [5. Licença](#5-licença)

## 1. Visão Geral e Principais Recursos

- **Tradução Bidirecional de Chat em Tempo Real:**
  - **Mensagens de Entrada:** Detecta automaticamente o idioma dos colegas de equipe, traduz as mensagens para o seu idioma de destino escolhido (suporta até 28 idiomas de destino, incluindo Português PT-BR, Inglês, Espanhol, Russo, Chinês, Japonês, Francês, Alemão e outros) e as renderiza **localmente** no chat do jogo, **preservando com fidelidade o nome original do jogador e sua cor** (J1 Dourado, J2 Azul, J3 Roxo, J4 Ciano).
  - **Mensagens de Saída:** Digite `[idioma_destino]>>mensagem` ou simplesmente `>>mensagem` para responder automaticamente no idioma da última mensagem recebida de um colega. O texto traduzido é transmitido via RPC multijogador para que os colegas internacionais o leiam nativamente.
- **Registros de Chat Persistentes e Exportação em Arquivo:** Histórico completo de chat da sessão com carimbos de data/hora, nomes de jogadores, cores, idiomas de origem detectados e latência de tradução. Os registros persistem na tabela entre reinicializações do aplicativo por meio de gravações assíncronas leves em lote no disco (configurável), com exportação para arquivo `.txt` em um clique.
- **Registros de Chat Pesquisáveis:** Histórico de chat totalmente pesquisável com filtragem em todas as colunas.

![Helldivers 1 Babel em ação](screenshots/screenshot.png)

- **Chat Rápido Tático (10 Slots Personalizáveis):** 10 atalhos predefinidos e configuráveis para mensagens táticas, vinculados a teclas de atalho (ex.: Teclado Numérico 0-9 ou combinações personalizadas) para transmitir avisos táticos instantaneamente no chat multijogador do jogo.

![Chat Rápido](screenshots/quick-chat.png)

- **Mensagem de Boas-Vindas Automática do Chat Rápido:** Transmite automaticamente uma mensagem de boas-vindas personalizável no chat do jogo quando você hospeda uma partida e um jogador convidado entra.
- **Overlay de Referência do Chat Rápido na Tela:** Ative ou desative uma sobreposição flutuante de referência que não rouba o foco no lado direito da tela pressionando `Numpad +` (ou seu atalho configurado) para conferir rapidamente todos os atalhos e avisos táticos ativos dentro do jogo. *(Requer que o Helldivers seja executado em modo Janela Sem Bordas ou Modo Janela).*

![Overlay de Chat Rápido](screenshots/overlay.png)

- **Suporte a Interface em Múltiplos Idiomas:** Suporte a 28 idiomas.
- **Arquitetura de Adaptador de Provedores:** Construído com uma interface modular para provedores de IA. Inclui um adaptador otimizado com zero-thinking para o **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), com contratos extensíveis para OpenAI, Claude e DeepSeek (ainda não implementados).
- **Ajuste de Tom da Tradução:** Casual: gírias informais de jogos e jargão tático de gamers / Normal: tom coloquial natural sem gírias de jogos.
- **Atalho Global de Pausa / Retomada:** Pressione `Ctrl + F10` (ou seu atalho configurado) a qualquer momento para alternar o assistente entre **Ativo** e **Pausado**. Pausar suspende de forma limpa a tradução de chat do jogo e os envios do Chat Rápido.
- **Atalhos Dedicados para Recursos Específicos:**
  - **Alternar Apenas Tradução:** Pressione `Ctrl + F11` (ou seu atalho configurado) para pausar ou retomar instantaneamente a tradução do chat sem afetar o Chat Rápido (com feedback toast na tela).
  - **Alternar Apenas Chat Rápido:** Pressione `Ctrl + F12` (ou seu atalho configurado) para pausar ou retomar instantaneamente os avisos do Chat Rápido sem afetar a tradução (com feedback toast na tela).

![Configurações](screenshots/settings.png)

- **Armazenamento Seguro de Credenciais (DPAPI):** Preferências confidenciais, como tokens de API de provedores de IA, são criptografadas em repouso usando a API de Proteção de Dados do Windows (DPAPI, escopo `CurrentUser`). As credenciais nunca são armazenadas em texto simples nos arquivos de configuração e só podem ser descriptografadas na sua máquina local pela sua conta do Windows.
- **Zero Telemetria e Privacidade Total:** Esta ferramenta não coleta, registra nem envia nenhum dado de usuário ou telemetria. Todas as requisições de tradução se comunicam de forma direta e exclusiva com o endpoint do provedor de IA configurado, utilizando suas credenciais pessoais de API.
- **100% Não Invasivo (Zero Modificação de Arquivos do Jogo):** Nenhum arquivo compactado do jogo é desempacotado ou modificado. Os arquivos do jogo permanecem 100% originais e intocados.




## 2. Instalação e Configuração

### Requisitos
Caso utilize a versão autônoma pré-compilada (`HD1-Babel.exe`):
- **Sistema Operacional:** Windows 10 / 11 64-bit (x64).
- **Ambiente de Execução Desktop:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (instalador direto).
- **Jogo Alvo:** *Helldivers* (2015) no Steam (`helldivers.exe` de 64 bits).

### Configurar uma Chave de API de IA Gratuita
- **Credenciais do Provedor de IA:** Chave de API do Google Gemini (obtenha uma chave pessoal gratuita no [Google AI Studio](https://aistudio.google.com/) ([Veja este tutorial em vídeo](https://www.youtube.com/watch?v=YHFMrZD-_3g)).
- **Acesso à Internet:** Necessário para as chamadas de API de tradução.

### Download e Instalação (Versão Pré-compilada)

Você **não** precisa instalar ferramentas de desenvolvedor nem compilar a partir do código-fonte para usar o Babel ([mas se desejar](#4-desenvolvimento-compilando-a-partir-do-código-fonte)):
1. Acesse a seção **[Releases](https://github.com/difinim/helldivers1-babel/releases)** no lado direito da página do projeto no GitHub.
2. Na versão mais recente, expanda a lista de **Assets**.
3. Baixe o pacote de distribuição autônomo: **`Helldivers1Babel-v<versão>-win-x64.zip`**.
4. Extraia o arquivo zip em qualquer local do seu computador (ex.: `C:\GamesTools\Helldivers1Babel`).
5. Abra a pasta e execute **`HD1-Babel.exe`**.

> [!NOTE]
> **Pré-requisito:** Os usuários finais só precisam do runtime gratuito [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Nenhum compilador C++, SDK ou ferramenta adicional é necessária para rodar a versão pré-compilada.

> [!NOTE]
> **Aviso importante:** Como o InjectorService.cs utiliza APIs padrão do Windows para injeção de processos (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), o Windows Defender ou antivírus de terceiros _**podem**_ disparar uma detecção heurística (ex.: Generic HackTool). Este é um comportamento normal para injetores de modificações de jogos e o arquivo pode ser colocado na lista de exceções com total segurança.



## 3. Guia de Uso

### Primeiros Passos
1. Inicie o **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. No primeiro início, abra a aba **Configurações**:
   - Insira sua **Chave de API do Google Gemini** (armazenada de forma segura com criptografia Windows DPAPI).
   - Selecione seu idioma de destino (ex.: `Portuguese (PT-BR)`).
   - Clique em **Testar Conexão da API** para verificar sua chave e latência.
   - Personalize o atalho **Pausar / Retomar**, se desejar (o padrão é `Ctrl + F10`).
3. Configure seus avisos táticos na aba **Chat Rápido** (ex.: vincule Numpad 0-9 às mensagens desejadas).
4. Inicie o **Helldivers** no Steam. O Babel detectará automaticamente o `helldivers.exe`, injetará a `BabelHook.dll` e estabelecerá a comunicação de alta velocidade via named pipe IPC.

### Comunicação no Jogo e Casos de Uso

#### 1. Tradução Automática de Entrada
Mensagens recebidas de colegas de equipe são detectadas e traduzidas instantaneamente para o seu idioma de destino, sendo exibidas no chat do jogo com o nome e a cor originais do jogador:
- Colega (Inglês): `so quiet`
- Chat no Jogo: `Diveheller: (English): mó silêncio`

#### 2. Resposta Automática no Último Idioma Recebido (`>>mensagem`)
Adicionar o prefixo `>>` à sua mensagem de chat traduz o texto automaticamente para o idioma da última mensagem recebida de um colega de equipe:
- Sua mensagem digitada no chat: `>>sim, está calmo demais`
- O colega vê: `yes, it is too quiet`

#### 3. Seleção Explícita / Arbitrária de Idioma (`idioma de destino>>mensagem`)
Direcione sua mensagem para um idioma específico a qualquer momento inserindo o nome do idioma seguido de `>>`:

| Comando Digitado no Chat | Idioma de Destino | Exemplo de Resultado Transmitido aos Colegas |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Russo | `доброе утро!` |
| `english>>minas ao norte` | Inglês | `mines to the north` |
| `japanese>>cuidado com os tiros!` | Japonês | `射線に注意せよ！` |
| `spanish>>preciso de munição` | Espanhol | `necesito munición` |
| `german>>recuem para o ponto de extração` | Alemão | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | Francês | `défendons cette position` |

#### 4. Mensagens Padrão de Saída e Supressão de Auto-Tradução
Ao digitar uma mensagem de chat padrão no jogo sem `>>` (ex.: `need ammo`, `vamos lá`), sua mensagem é transmitida diretamente no chat multijogador original. Com a opção **"Não traduzir saídas padrão"** ativada (ligada por padrão tanto na barra de controles rápidos quanto em Configurações), o Babel evita traduzir sua própria mensagem de volta para você. Se desativada, o Babel também processará suas mensagens locais padrão pelo pipeline de tradução de entrada.

#### 5. Atalhos do Chat Rápido Tático
Pressione qualquer tecla de atalho de chat rápido configurada (ex.: `NumPad 1`-`NumPad 9`) durante a partida para transmitir instantaneamente avisos táticos no chat multijogador sem precisar abrir a caixa de texto do chat. Todas as teclas continuam sendo repassadas ao jogo normalmente, garantindo que seus comandos permaneçam responsivos.

#### 6. Overlay de Referência do Chat Rápido Tático (`Numpad +`)
Pressione `Numpad +` (ou o atalho configurado na aba **Chat Rápido**) a qualquer momento para exibir ou ocultar uma sobreposição de referência na tela listando todos os atalhos ativos do Chat Rápido. O overlay aparece alinhado à direita e centralizado verticalmente na tela, mostrando cada atalho ativo no formato `[<atalho>] <mensagem>` (com mensagens truncadas em 30 caracteres) usando o mesmo estilo visual não intrusivo das notificações toast.
> [!NOTE]
> **Requisito do Modo de Exibição:** O Helldivers deve estar configurado para rodar em modo **Janela Sem Bordas** (ou Modo Janela) nas configurações de Vídeo do jogo para que janelas de sobreposição do Windows possam ser renderizadas sobre o jogo.

#### 7. Pausar, Retomar e Alternadores de Recursos
- **Pausar / Retomar Global (`Ctrl + F10`):** Interruptor mestre que suspende ou retoma de forma limpa a interceptação de tradução e a transmissão de mensagens do Chat Rápido simultaneamente.
- **Alternar Apenas Tradução (`Ctrl + F11`):** Liga ou desliga apenas o recurso de tradução com uma notificação toast na tela. O Chat Rápido continuará funcionando normalmente.
- **Alternar Apenas Chat Rápido (`Ctrl + F12`):** Liga ou desliga apenas a transmissão de mensagens do Chat Rápido com uma notificação toast na tela. A tradução do chat no jogo continuará funcionando normalmente.
*(Todos os atalhos podem ser personalizados nas abas **Configurações** e **Chat Rápido** usando os botões de captura de atalho por clique).*

#### 8. Exportação e Registros de Chat Persistentes
Clique em **Exportar Registros** na aba Tradução e Registros a qualquer momento para escolher um local e salvar o histórico de chat (com carimbos de data/hora, nomes de jogadores, mensagens originais, traduções e latências) em um arquivo `.txt`. As mensagens de chat persistem automaticamente na tabela entre inicializações do aplicativo por meio de gravações assíncronas em lote no disco (o limite de armazenamento pode ser configurado de 0 a 100 MB nas Configurações, com um botão **Limpar memória de registros** disponível).

#### 9. Links Interativos em Mensagens
Mensagens do chat de esquadrão que contenham links com URLs `http://` ou `https://` são exibidas como links clicáveis na coluna **Mensagem Original**. Ao clicar, uma caixa de diálogo de confirmação solicita que você confirme se confia no link antes de abri-lo no seu navegador web padrão.



## 4. Desenvolvimento (Compilando a partir do Código-Fonte)

### Requisitos para Desenvolvedores
Caso vá compilar e construir o projeto a partir do código-fonte:
- **Sistema Operacional:** Windows 10 / 11 64-bit (x64).
- **SDK do .NET 10:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64 Toolchain (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) com os pacotes `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` e `mingw-w64-x86_64-make` — necessários para compilar a biblioteca nativa de hook (`BabelHook.dll`) e construir o submódulo oficial do LuaJIT 2.0 (`vendor/luajit`) para compilar o script de inicialização do jogo (`babel_bootstrap.lua`) em bytecode.
- **Git:** Git for Windows (com submódulos recursivos para o MinHook amalgamation e LuaJIT 2.0).

### Configuração Automatizada do Ambiente
Dê um duplo clique em `setup.bat` (ou execute `setup.ps1` no PowerShell) na raiz do repositório. Ele verifica e instala automaticamente os pré-requisitos pendentes:
1. Verifica o ambiente Windows de 64 bits.
2. Verifica o SDK do .NET 10 (instalação automática via `winget` se ausente).
3. Verifica a toolchain MSYS2 MinGW64 (Clang++, GCC e Make). Se ausente, instala automaticamente o MSYS2 via `winget` e instala os pacotes necessários (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` e `mingw-w64-x86_64-make`) via `pacman`.
4. Inicializa e compila o compilador LuaJIT 2.0 a partir do submódulo (`vendor/luajit`).
5. Garante automaticamente que `C:\msys64\mingw64\bin` e `C:\msys64\usr\bin` sejam adicionados ao PATH.
6. Verifica as dependências do MinHook amalgamation e realiza uma verificação inicial de integridade da compilação.

### Compilação Principal e Empacotamento de Versão
Para limpar artefatos anteriores, compilar a DLL nativa de hook, executar todos os testes unitários e gerar o pacote de versão final:
```powershell
pwsh -File build.ps1
```
O diretório de saída será empacotado em `dist/Helldivers1Babel-vX.Y.Z/`.

### Compilação Manual Passo a Passo
```powershell
# 1. Compilar a DLL nativa de hook (compila automaticamente o babel_bootstrap.lua com luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Compilar a solução .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Executar as suítes de testes automatizados
dotnet test
```



## 5. Licença

Distribuído sob a [Licença MIT](../LICENSE).
