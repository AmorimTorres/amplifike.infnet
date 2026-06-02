using System;
using Amplifike.Domain;
using Amplifike.Services;

namespace Amplifike
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.Title = "Amplifike Crowdfunding - Simulação C# MVP";
            
            PrintHeader();

            // 1. Inicializar Serviços de Apoio e Infraestrutura
            // Injeção de dependências demonstrando DIP e Baixo Acoplamento
            var auditLogger = new AuditLogger();
            var paymentGateway = new MockPixPaymentGateway();
            var pledgeService = new PledgeService(paymentGateway, auditLogger);

            Console.WriteLine("\n[INFRA] Serviços de Auditoria e Gateway de Pagamentos inicializados.");
            Console.WriteLine("--------------------------------------------------------------------------------");

            // 2. Criar as Campanhas (Polimorfismo / Herança)
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[ETAPA 1] Criando Campanhas e Recompensas...");
            Console.ResetColor();

            // Campanha Flexível (Keep-It-All)
            var flexibleCampaign = new FlexibleCampaign(
                title: "Festival da Comunidade Amplifika 2026",
                slug: "festival-amplifika-2026",
                description: "Festival cultural para promover artistas locais e diversidade.",
                goalAmount: 1000000, // R$ 10.000,00 (representado em centavos)
                endsAt: DateTime.UtcNow.AddDays(30)
            );

            // Campanha Tudo ou Nada (All-or-Nothing)
            var allOrNothingCampaign = new AllOrNothingCampaign(
                title: "Gravação do Novo Álbum - Banda Amplifika",
                slug: "novo-album-amplifika",
                description: "Financiamento coletivo para gravação do nosso primeiro álbum de estúdio.",
                goalAmount: 2500000, // R$ 25.000,00 (representado em centavos)
                endsAt: DateTime.UtcNow.AddDays(45)
            );

            // 3. Criar Recompensas via classe Campaign (Padrão GRASP Creator)
            // Campanha Flexível
            flexibleCampaign.AddReward(
                title: "Ingresso Simples",
                description: "Dá direito a 1 dia de festival na pista comum.",
                minimumAmount: 5000, // R$ 50,00
                quantityLimit: 100 // Limite de 100 ingressos
            );
            
            flexibleCampaign.AddReward(
                title: "Ingresso VIP + Camiseta",
                description: "Ingresso para todos os dias + área VIP + camiseta exclusiva.",
                minimumAmount: 20000, // R$ 200,00
                quantityLimit: 20, // Limite restrito de 20 VIPs
                requiresShipping: true,
                estimatedDelivery: "Julho/2026"
            );

            // Campanha Tudo ou Nada
            allOrNothingCampaign.AddReward(
                title: "Álbum Digital",
                description: "Download digital em alta fidelidade com encarte digital.",
                minimumAmount: 3000 // R$ 30,00
            );

            allOrNothingCampaign.AddReward(
                title: "Vinil Autografado Especial",
                description: "Cópia física em vinil colorido autografado pela banda.",
                minimumAmount: 35000, // R$ 350,00
                quantityLimit: 5, // Apenas 5 cópias!
                requiresShipping: true,
                estimatedDelivery: "Outubro/2026"
            );

            Console.WriteLine($"-> Campanha Criada (Flexível): '{flexibleCampaign.Title}' | Meta: R$ {flexibleCampaign.GoalAmount / 100.0:F2}");
            Console.WriteLine($"-> Campanha Criada (Tudo ou Nada): '{allOrNothingCampaign.Title}' | Meta: R$ {allOrNothingCampaign.GoalAmount / 100.0:F2}");

            // 4. Publicar Campanhas
            flexibleCampaign.Publish();
            allOrNothingCampaign.Publish();
            Console.WriteLine("\n[INFO] Ambas as campanhas foram marcadas como PUBLICADAS.");
            Console.WriteLine("--------------------------------------------------------------------------------");

            // 5. Cadastrar Apoiadores (Alta Coesão)
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[ETAPA 2] Cadastrando Apoiadores...");
            Console.ResetColor();

            var supporter1 = new Supporter("Rodrigo Amorim", "rodrigo@email.com", "(11) 98888-7777", "123.456.789-00");
            var supporter2 = new Supporter("Beatriz Torres", "beatriz@email.com", "(21) 97777-6666");

            Console.WriteLine($"-> Apoiador Cadastrado: {supporter1.Name} ({supporter1.Email})");
            Console.WriteLine($"-> Apoiador Cadastrado: {supporter2.Name} ({supporter2.Email})");
            Console.WriteLine("--------------------------------------------------------------------------------");

            // 6. Simular Checkout e Geração do Pix (Fluxo Central do MVP)
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[ETAPA 3] Iniciando Checkout (Criando Apoio com Recompensa VIP)...");
            Console.ResetColor();

            // Obter recompensa VIP da campanha flexível
            var vipReward = GetRewardByTitle(flexibleCampaign, "Ingresso VIP + Camiseta");

            // Rodrigo apoia a campanha flexível com R$ 250,00 (o mínimo é R$ 200,00)
            int pledgeAmount1 = 25000; // R$ 250,00
            
            Console.WriteLine($"\n[PROCESSANDO] {supporter1.Name} tentando apoiar '{flexibleCampaign.Title}' com R$ {pledgeAmount1 / 100.0:F2}...");
            
            var (pledge1, payment1) = pledgeService.CreatePledge(
                campaign: flexibleCampaign,
                supporter: supporter1,
                amount: pledgeAmount1,
                reward: vipReward
            );

            // Exibir dados do apoio criado e Pix gerado
            Console.WriteLine("\n================== DETALHES DO APOIO (CRIADO) ==================");
            Console.WriteLine($"Apoio ID:         {pledge1.Id}");
            Console.WriteLine($"Status do Apoio:  {pledge1.Status}");
            Console.WriteLine($"Recompensa:       {vipReward!.Title}");
            Console.WriteLine($"Envios de VIP:    {vipReward.QuantityClaimed} / {vipReward.QuantityLimit} resgatados.");
            Console.WriteLine($"Valor do Apoio:   R$ {pledge1.Amount / 100.0:F2}");
            Console.WriteLine("---------------- DETALHES DO PIX GERADO ----------------");
            Console.WriteLine($"Provedor Pix:     {payment1.Provider}");
            Console.WriteLine($"ID Transação Pix: {payment1.ProviderPaymentId}");
            Console.WriteLine($"PIX Copia e Cola: {payment1.CopyPasteCode}");
            Console.WriteLine($"QR Code URL:      {payment1.QrCode}");
            Console.WriteLine($"Vencimento em:    {payment1.ExpiresAt.ToLocalTime()}");
            Console.WriteLine("==================================================================");

            // 7. Simular Pagamento do Pix (Webhook Trigger)
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[ETAPA 4] Simulando Webhook de Recebimento de Pagamento Pix...");
            Console.ResetColor();

            Console.WriteLine($"[WEBHOOK] Simulação de notificação de pagamento recebido para {payment1.ProviderPaymentId}...");
            
            // Confirmando pagamento através do serviço coeso (PledgeService)
            pledgeService.ConfirmPayment(pledge1, payment1, flexibleCampaign);

            Console.WriteLine("\n================== DETALHES DO APOIO (PAGO) ==================");
            Console.WriteLine($"Status do Apoio:    {pledge1.Status}");
            Console.WriteLine($"Horário do Pago:    {pledge1.PaidAt}");
            Console.WriteLine($"Status de Entrega:  {pledge1.DeliveryStatus} (Preparando envio físico)");
            Console.WriteLine($"Pix Pago em:        {payment1.PaidAt}");
            Console.WriteLine($"Status Pix:         {payment1.Status}");
            Console.WriteLine($"Campanha Saldo:     R$ {flexibleCampaign.CurrentAmount / 100.0:F2} arrecadados de R$ {flexibleCampaign.GoalAmount / 100.0:F2}");
            Console.WriteLine("==================================================================");
            Console.WriteLine("--------------------------------------------------------------------------------");

            // 8. Teste de Validação / Tratamento de Regra de Negócio (MinimumAmount)
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[ETAPA 5] Validando Regras de Negócio e Tratamento de Erro...");
            Console.ResetColor();

            Console.WriteLine("[TESTE] Tentando apoiar R$ 150,00 selecionando a recompensa VIP (Mínimo R$ 200,00)...");
            try
            {
                // Beatriz tenta apoiar com valor abaixo do mínimo exigido pela recompensa VIP
                pledgeService.CreatePledge(
                    campaign: flexibleCampaign,
                    supporter: supporter2,
                    amount: 15000, // R$ 150,00
                    reward: vipReward
                );
            }
            catch (InvalidOperationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[REGRA ATENDIDA] Lançou exceção esperada com sucesso: \"{ex.Message}\"");
                Console.ResetColor();
            }
            Console.WriteLine("--------------------------------------------------------------------------------");

            // 9. Simulação de Herança e Polimorfismo na verificação de sucesso das Campanhas
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[ETAPA 6] Demonstrando Herança e Polimorfismo nos Tipos de Campanhas...");
            Console.ResetColor();

            // Apoiar Campanha Tudo ou Nada com R$ 26.000,00 (Meta R$ 25.000,00 - META ATINGIDA)
            int pledgeAmount2 = 2600000; // R$ 26.000,00
            Console.WriteLine($"\n[PROCESSO] {supporter2.Name} apoia Campanha Tudo ou Nada com R$ {pledgeAmount2 / 100.0:F2}...");
            var digitalReward = GetRewardByTitle(allOrNothingCampaign, "Álbum Digital");
            var (pledge2, payment2) = pledgeService.CreatePledge(allOrNothingCampaign, supporter2, pledgeAmount2, digitalReward);
            
            // Confirmar o pagamento
            pledgeService.ConfirmPayment(pledge2, payment2, allOrNothingCampaign);

            // Simular encerramento das duas campanhas para avaliar o status final de sucesso
            flexibleCampaign.End();
            allOrNothingCampaign.End();

            Console.WriteLine("\nEncerrando as campanhas e avaliando resultados polimorficamente:");
            
            // Lista contendo campanhas genéricas para aplicar o Polimorfismo
            var campaignsList = new List<Campaign> { flexibleCampaign, allOrNothingCampaign };

            foreach (var campaign in campaignsList)
            {
                Console.WriteLine("\n------------------------------------------------------------------");
                Console.WriteLine($"Campanha:           {campaign.Title}");
                Console.WriteLine($"Tipo de Campanha:   {campaign.GetType().Name}");
                Console.WriteLine($"Meta:               R$ {campaign.GoalAmount / 100.0:F2}");
                Console.WriteLine($"Total Arrecadado:   R$ {campaign.CurrentAmount / 100.0:F2}");
                Console.WriteLine($"Meta Atingida?      {(campaign.IsGoalMet() ? "SIM" : "NÃO")}");
                
                // Chamada polimórfica ao método abstrato 'CanDeliverRewards()'
                bool canDeliver = campaign.CanDeliverRewards();
                Console.ForegroundColor = canDeliver ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"POLIMORFISMO -> Pode entregar recompensas? {canDeliver} (Regra do {campaign.GetType().Name})");
                Console.ResetColor();
                Console.WriteLine("------------------------------------------------------------------");
            }

            // Exibir lista final de auditoria acumulada para o administrador
            PrintAuditLogs(auditLogger);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n🎉 Simulação finalizada com sucesso!");
            Console.ResetColor();
        }

        private static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
    ===================================================================
      _                 _ _  __ _ _           ___ ___   ___ _     ___ 
     /_\  _ __  _ __   | (_)/ _(_) |_____    / __/ __| | _ \ |   / __|
    / _ \| '  \| '_ \  | | |  _| | / / -_)  | (__\__ \ |  _/ |__| (__ 
   /_/ \_\_|_|_| .__/  |_|_|_| |_|_\_\___|   \___|___/ |_| |____|\___|
               |_|                                                    
                         MVP C# CROWDFUNDING ENGINE
    ===================================================================");
            Console.ResetColor();
        }

        private static Reward? GetRewardByTitle(Campaign campaign, string title)
        {
            foreach (var reward in campaign.Rewards)
            {
                if (reward.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                {
                    return reward;
                }
            }
            return null;
        }

        private static void PrintAuditLogs(AuditLogger logger)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n========================= LOGS DE AUDITORIA GERADOS =========================");
            int index = 1;
            foreach (var log in logger.Logs)
            {
                Console.WriteLine($"{index:00}. Ação: {log.Action,-15} | Entidade: {log.EntityType,-8} | ID: {log.EntityId} | Criado Em: {log.CreatedAt}");
                index++;
            }
            Console.WriteLine("=============================================================================");
            Console.ResetColor();
        }
    }
}
