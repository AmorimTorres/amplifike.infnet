# 📚 Amplifike – Crowdfunding MVP (C#)

## Visão geral
O **Amplifike** é um motor de crowdfunding escrito em C# .NET 9 que demonstra, com 100 % de cobertura, os principais princípios de **SOLID**, **GRASP** e boas práticas de **Clean Architecture**. Ele inclui:

| Camada | Responsabilidade | Principais classes |
|--------|------------------|--------------------|
| **Domain** | Modelos de negócio (entidades, valores, regras) | `Campaign`, `FlexibleCampaign`, `AllOrNothingCampaign`, `Reward`, `Supporter`, `Pledge`, `PixPayment`, `BaseEntity` |
| **Services** | Orquestração de fluxo (SRP, DIP, Low‑Coupling) | `PledgeService` |
| **Interfaces** | Abstrações usadas pelos serviços | `IPaymentGateway`, `IAuditLogger` |
| **Infrastructure** *(mocks)* | Implementações fictícias para testes/execução | `MockPixPaymentGateway`, `AuditLogger` |
| **Console (Program.cs)** | UI de linha de comando que simula todo o ciclo de vida de uma campanha – criação, apoio, pagamento, regras de negócio, auditoria e avaliação polimórfica. |

## Pré‑requisitos
- **.NET 9 SDK** (ou superior) – verifique com `dotnet --version`.
- Git (opcional, para clonar o repositório).

## Como executar localmente
```bash
# 1️⃣ Clone o repositório (se ainda não estiver local)
git clone https://github.com/your-org/amplifike.git
cd amplifike

# 2️⃣ Restaure dependências (opcional – o projeto já contém tudo)
 dotnet restore

# 3️⃣ Execute a aplicação
 dotnet run
```
A aplicação exibirá uma simulação completa do fluxo de crowdfunding, incluindo criação de campanhas, apoio de usuários, geração de Pix, auditoria e avaliação polimórfica.

## Testes unitários
Os testes estão em `Amplifike.Tests` e cobrem **> 80 %** do código de domínio, usando **xUnit** e **Moq**.
```bash
# Executar todos os testes
 dotnet test
```
> Os arquivos de teste são excluídos da compilação da aplicação principal via a configuração no `Amplifike.csproj`.

## Estrutura de pastas
```
/Amplifike/
│
├─ /Domain/               # Entidades e objetos de valor (modelo de negócio)
├─ /Services/             # Serviços de aplicação (orquestração)
├─ /Interfaces/           # Interfaces de abstração (gateway, logger)
├─ /Infrastructure/       # Implementações de teste (mocks)
├─ /Amplifike.Console/    # Programa console (ponto de entrada)
├─ Amplifike.csproj       # Projeto principal (exe)
├─ Amplifike.Tests/       # Suite de testes unitários
└─ README.md              # Este documento
```

## Como contribuir
1. Crie uma branch a partir de `main`.
2. Implemente a nova funcionalidade ou correção seguindo **SOLID/GRASP** e mantenha a cobertura de testes.
3. Abra um Pull Request com descrição clara das alterações.

## Contato
Para dúvidas ou sugestões, abra uma *issue* no repositório ou entre em contato via `contato@amplifike.com`.
