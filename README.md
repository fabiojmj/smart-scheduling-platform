# smart-scheduling-platform

> Plataforma SaaS de agendamento inteligente via WhatsApp com IA — atende clientes por texto e áudio, agenda automaticamente e oferece painel completo para estabelecimentos e funcionários.

[![Build](https://github.com/fabiojmj/smart-scheduling-platform/actions/workflows/ci.yml/badge.svg)](https://github.com/fabiojmj/smart-scheduling-platform/actions)
[![Coverage](https://img.shields.io/badge/coverage-80%25-brightgreen)](https://github.com/fabiojmj/smart-scheduling-platform)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com)
[![React](https://img.shields.io/badge/React-18-61DAFB)](https://react.dev)

---

## Visão geral

O **smart-scheduling-platform** permite que estabelecimentos (salões, clínicas, barbearias etc.) ofereçam agendamento automatizado aos seus clientes diretamente pelo WhatsApp, sem que o cliente precise baixar nenhum app.

O cliente envia uma mensagem de texto ou áudio → a IA interpreta a intenção → verifica disponibilidade → confirma o agendamento. O proprietário gerencia tudo por um painel web.

```
Cliente (WhatsApp)
       │  texto ou áudio
       ▼
  Meta Cloud API
       │  webhook
       ▼
  Worker Service (.NET)
       │  transcrição (Whisper) + classificação (GPT-4o)
       ▼
  Motor de Agendamento (ASP.NET Core)
       │  verifica disponibilidade + salva
       ▼
  Confirmação via WhatsApp  ←→  Painel Admin (React)
```

---

## Funcionalidades

### Bot WhatsApp
- Recebe mensagens de **texto e áudio** (transcrição automática via Whisper)
- Classifica intenção: agendar, cancelar, consultar, saudação
- Extrai entidades: serviço desejado, funcionário preferido, data e horário
- Conversa multi-turno: pede dados faltantes até ter tudo para agendar
- Envia **confirmação e lembrete automático** 24h antes

### Módulo administrativo
- Dashboard com agenda do dia e semana por funcionário
- Gestão de funcionários, serviços e horários de trabalho
- Calendário visual com arrastar para reagendar
- Relatórios de agendamentos, cancelamentos e receita estimada
- Notificações em tempo real via SignalR

### Portal do cliente
- Acesso por magic link (sem senha)
- Visualização de próximos agendamentos e histórico
- Cancelamento com confirmação
- Listagem de serviços e profissionais disponíveis

---

## Stack tecnológica

| Camada | Tecnologias |
|---|---|
| Back-end | .NET 8, ASP.NET Core, Entity Framework Core, SignalR |
| Processamento assíncrono | Worker Service, Hangfire, Azure Service Bus |
| Front-end | React 18, TypeScript, Vite, Tailwind CSS, React Query, FullCalendar |
| Banco de dados | SQL Server |
| IA & Integrações | Meta Cloud API (WhatsApp), OpenAI Whisper (STT), GPT-4o |
| Observabilidade | Serilog, Seq / Application Insights |
| Infraestrutura | Azure App Service, GitHub Actions (CI/CD) |

---

## Arquitetura

O back-end segue **Clean Architecture** com separação clara de responsabilidades:

```
src/
├── SmartScheduling.Domain/          # Entidades, regras de negócio, interfaces
├── SmartScheduling.Application/     # Use cases, CQRS (MediatR), DTOs
├── SmartScheduling.Infrastructure/  # EF Core, WhatsApp API, OpenAI, Hangfire
├── SmartScheduling.API/             # Controllers, webhooks, SignalR hubs
└── SmartScheduling.Worker/          # Processamento de mensagens, jobs agendados

tests/
├── SmartScheduling.Domain.Tests/    # Unit tests (xUnit + FluentAssertions)
└── SmartScheduling.API.Tests/       # Integration tests (WebApplicationFactory)

frontend/
├── admin/                           # Painel do proprietário (React)
└── client/                          # Portal do cliente (React)
```

### Fluxo de mensagem WhatsApp

O webhook responde imediatamente à Meta (< 20s) e delega o processamento para uma fila assíncrona, evitando timeout:

```
Webhook (API)  →  Service Bus  →  Worker Service  →  GPT-4o  →  Resposta WhatsApp
      └── 200 OK imediato
```

### Modelo de dados principal

```
Estabelecimento  1──N  Funcionario  N──N  Servico
                              │
                              └──N  Agendamento ──1  Cliente
                                          │
                                          └── status: Pendente | Confirmado | Concluído | Cancelado
```

---

## Como rodar localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [SQL Server](https://www.microsoft.com/sql-server) ou Docker
- Conta na [Meta for Developers](https://developers.facebook.com) (WhatsApp Cloud API)
- Chave de API da [OpenAI](https://platform.openai.com)
- [ngrok](https://ngrok.com) para expor o webhook localmente

### 1. Clonar o repositório

```bash
git clone https://github.com/fabiojmj/smart-scheduling-platform.git
cd smart-scheduling-platform
```

### 2. Configurar variáveis de ambiente

```bash
cp .env.example .env
```

Edite o `.env` com suas credenciais (veja a seção [Variáveis de ambiente](#variáveis-de-ambiente) abaixo).

### 3. Banco de dados

```bash
cd src/SmartScheduling.API
dotnet ef database update
```

### 4. Back-end

```bash
# API
cd src/SmartScheduling.API
dotnet run

# Worker Service (terminal separado)
cd src/SmartScheduling.Worker
dotnet run
```

### 5. Front-end

```bash
cd frontend/admin
npm install
npm run dev
```

### 6. Expor webhook para o WhatsApp

```bash
ngrok http 5000
# Copie a URL https gerada e configure no painel da Meta
```

---

## Variáveis de ambiente

Copie `.env.example` e preencha com seus valores. **Nunca commite o `.env` real.**

```env
# Banco de dados
DATABASE_CONNECTION_STRING=Server=localhost;Database=SmartScheduling;...

# JWT
JWT_SECRET=sua-chave-secreta-minimo-32-caracteres
JWT_EXPIRATION_MINUTES=60

# WhatsApp (Meta Cloud API)
WHATSAPP_TOKEN=seu-token-de-acesso
WHATSAPP_PHONE_NUMBER_ID=seu-phone-number-id
WHATSAPP_VERIFY_TOKEN=token-para-validar-webhook
WHATSAPP_APP_SECRET=seu-app-secret

# OpenAI
OPENAI_API_KEY=sk-...

# Azure Service Bus (ou RabbitMQ)
SERVICE_BUS_CONNECTION_STRING=Endpoint=sb://...

# Hangfire
HANGFIRE_DASHBOARD_USER=admin
HANGFIRE_DASHBOARD_PASSWORD=senha-forte
```

---

## Testes

```bash
# Todos os testes
dotnet test

# Apenas unit tests
dotnet test tests/SmartScheduling.Domain.Tests

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## CI/CD

O pipeline roda automaticamente em todo Pull Request e merge na `main`:

- Build e restauração de dependências
- Execução de todos os testes
- Análise de formatação (`dotnet format --verify-no-changes`)
- Deploy automático para staging (merge na `develop`)
- Deploy para produção (merge na `main` com aprovação)

Veja [`.github/workflows/ci.yml`](.github/workflows/ci.yml) para detalhes.

---

## Contribuindo

1. Fork o projeto
2. Crie sua branch: `git checkout -b feature/nome-da-feature`
3. Commit seguindo [Conventional Commits](https://www.conventionalcommits.org): `feat: adiciona cancelamento pelo portal`
4. Abra um Pull Request descrevendo o que foi feito e por quê

Veja [CONTRIBUTING.md](CONTRIBUTING.md) para o guia completo.

---

## Decisões técnicas

| Decisão | Escolha | Motivo |
|---|---|---|
| Arquitetura back-end | Clean Architecture + CQRS | Separação clara de responsabilidades, testabilidade, escalabilidade |
| Mensageria | Azure Service Bus | Desacoplar webhook do processamento de IA (evitar timeout de 20s da Meta) |
| STT | OpenAI Whisper | Melhor acurácia para português com áudio OGG do WhatsApp |
| Classificação | GPT-4o | Robustez em linguagem natural coloquial brasileira |
| Jobs agendados | Hangfire | Integração nativa com .NET, dashboard incluso, persistência no SQL Server |
| Calendário | FullCalendar | Componente maduro com drag-and-drop e visualização por recurso (funcionário) |

---

## Roadmap

Acompanhe o progresso no [GitHub Projects](https://github.com/fabiojmj/smart-scheduling-platform/projects/1).

- [x] Fase 1 — Fundação e arquitetura
- [ ] Fase 2 — Core API e domínio
- [ ] Fase 3 — Integração WhatsApp + IA
- [ ] Fase 4 — Módulo administrativo
- [ ] Fase 5 — Portal do cliente e lançamento

---

## Licença

Distribuído sob a licença MIT. Veja [LICENSE](LICENSE) para mais informações.

---

## Autor

Feito por **[Fábio Mendonça](https://github.com/fabiojmj)** · [LinkedIn](https://linkedin.com/in/fabio-mendonca-jr)
