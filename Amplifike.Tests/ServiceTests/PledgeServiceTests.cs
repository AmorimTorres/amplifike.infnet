using System;
using Amplifike.Domain;
using Amplifike.Services;
using Amplifike.Tests.Mocks;
using Xunit;

namespace Amplifike.Tests.ServiceTests
{
    /*
     * =================================================================================
     * PRINCÍPIO DE TESTE UNITÁRIO: ISOLAMENTO (ISOLATION)
     * =================================================================================
     * NOME: Isolamento (Isolation)
     * CONCEITO: Isolar a classe sob teste (PledgeService) injetando dublês de testes
     *           (stubs e spies) ao invés de usar serviços reais, assegurando que o teste
     *           teste estritamente a orquestração do PledgeService.
     * =================================================================================
     * 
     * =================================================================================
     * PRINCÍPIO DE TESTE UNITÁRIO: ABRANGÊNCIA (THOROUGH)
     * =================================================================================
     * NOME: Abrangência/Abrupticidade (Thorough)
     * CONCEITO: Testar não apenas os caminhos felizes ("happy path"), mas cobrir
     *           exaustivamente cenários alternativos e negativos (exceções, limites de dados,
     *           estados inválidos) para garantir a segurança da aplicação sob estresse.
     * =================================================================================
     */
    public class PledgeServiceTests
    {
        private readonly StubPaymentGateway _stubGateway;
        private readonly SpyAuditLogger _spyLogger;
        private readonly PledgeService _service;

        public PledgeServiceTests()
        {
            // Criar dublês de teste leves (Isolation / Fast)
            _stubGateway = new StubPaymentGateway();
            _spyLogger = new SpyAuditLogger();

            // PledgeService recebe as dependências abstratas (DIP)
            _service = new PledgeService(_stubGateway, _spyLogger);
        }

        [Fact]
        public void Should_CreatePledgeSuccessfully_When_CampaignIsPublishedAndAmountIsSufficient()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Album", "album", "desc", 20000, DateTime.UtcNow.AddDays(10));
            campaign.AddReward("Vinil", "Vinil desc", 5000, quantityLimit: 10);
            campaign.Publish(); // Campanha publicada (ativa)

            var reward = GetRewardByTitle(campaign, "Vinil");
            var supporter = new Supporter("Rodrigo", "rodrigo@email.com", "1234");

            // Act
            var (pledge, payment) = _service.CreatePledge(
                campaign: campaign,
                supporter: supporter,
                amount: 6000, // Oferece R$ 60,00 (mínimo é R$ 50,00)
                reward: reward
            );

            // Assert
            Assert.NotNull(pledge);
            Assert.Equal(PledgeStatus.AwaitingPix, pledge.Status);
            Assert.Equal(reward!.Id, pledge.RewardId);
            Assert.Equal(6000, pledge.Amount);
            
            // Garantir que a quantidade de recompensas foi decrementada (Information Expert)
            Assert.Equal(1, reward.QuantityClaimed);

            // Verificar se o stub gateway retornou o Pix mockado determinístico
            Assert.NotNull(payment);
            Assert.Equal(pledge.Id, payment.PledgeId);
            Assert.Equal(_stubGateway.LastGeneratedPaymentId, payment.ProviderPaymentId);
            Assert.Equal(PixPaymentStatus.Pending, payment.Status);

            // Auto-verificação (Self-Validating): Assegurar que o log de criação foi gerado
            Assert.True(_spyLogger.HasLogged("PLEDGE_CREATED", pledge.Id));
            Assert.Single(_spyLogger.RecordedCalls);
        }

        [Fact]
        public void Should_ConfirmPaymentAndAddCampaignBalance_When_ValidPixConfirmed()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Album", "album", "desc", 20000, DateTime.UtcNow.AddDays(10));
            campaign.Publish();
            var supporter = new Supporter("Rodrigo", "rodrigo@email.com", "1234");

            var (pledge, payment) = _service.CreatePledge(campaign, supporter, 5000);
            Assert.Equal(0, campaign.CurrentAmount);

            // Act
            _service.ConfirmPayment(pledge, payment, campaign);

            // Assert
            Assert.Equal(PledgeStatus.Paid, pledge.Status);
            Assert.NotNull(pledge.PaidAt);
            Assert.Equal(PixPaymentStatus.Paid, payment.Status);
            Assert.Equal(5000, campaign.CurrentAmount); // Saldo atualizado da campanha

            // Auto-verificação: Garantir log de pagamento disparado
            Assert.True(_spyLogger.HasLogged("PLEDGE_PAID", pledge.Id));
            Assert.Equal(2, _spyLogger.RecordedCalls.Count); // 1. Criado, 2. Pago
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_CampaignIsDraft()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Album", "album", "desc", 20000, DateTime.UtcNow.AddDays(10));
            // Deixado intencionalmente em rascunho (Draft)
            
            var supporter = new Supporter("Rodrigo", "rodrigo@email.com", "1234");

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.CreatePledge(campaign, supporter, 5000)
            );

            Assert.Contains("publicada e ativa", ex.Message);
            Assert.Empty(_spyLogger.RecordedCalls); // Nenhum log gravado
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_CampaignIsExpired()
        {
            // Arrange
            // Criar uma campanha com data de encerramento extremamente curta (100 milissegundos)
            var expiredCampaign = new FlexibleCampaign("Album", "album", "desc", 20000, DateTime.UtcNow.AddMilliseconds(100));
            expiredCampaign.Publish();
            
            var supporter = new Supporter("Rodrigo", "rodrigo@email.com", "1234");

            // Aguardar 150 milissegundos para expirar a campanha naturalmente
            System.Threading.Thread.Sleep(150);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.CreatePledge(expiredCampaign, supporter, 5000)
            );

            Assert.Contains("período de captação", ex.Message);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_PledgeAmountIsLessThanRewardMinimum()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Album", "album", "desc", 20000, DateTime.UtcNow.AddDays(10));
            campaign.AddReward("Vinil", "Vinil", 5000);
            campaign.Publish();
            var reward = GetRewardByTitle(campaign, "Vinil");
            var supporter = new Supporter("Rodrigo", "rodrigo@email.com", "1234");

            // Act & Assert: R$ 40,00 oferecido (Mínimo R$ 50,00)
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.CreatePledge(campaign, supporter, 4000, reward)
            );

            Assert.Contains("insuficiente para esta recompensa", ex.Message);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_RewardBelongsToAnotherCampaign()
        {
            // Arrange
            var campaign1 = new FlexibleCampaign("Album 1", "album-1", "desc", 20000, DateTime.UtcNow.AddDays(10));
            campaign1.Publish();

            var campaign2 = new FlexibleCampaign("Album 2", "album-2", "desc", 20000, DateTime.UtcNow.AddDays(10));
            campaign2.AddReward("Reward de Outra", "desc", 1000);
            var rewardOfCampaign2 = GetRewardByTitle(campaign2, "Reward de Outra");

            var supporter = new Supporter("Rodrigo", "rodrigo@email.com", "1234");

            // Act & Assert: Rodrigo tenta apoiar campanha1 usando recompensa da campanha2
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.CreatePledge(campaign1, supporter, 2000, rewardOfCampaign2)
            );

            Assert.Contains("não pertence a esta campanha", ex.Message);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_ConfirmPaymentOnMismatchedPaymentAndPledge()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Album", "album", "desc", 20000, DateTime.UtcNow.AddDays(10));
            campaign.Publish();
            var supporter = new Supporter("Rodrigo", "rodrigo@email.com", "1234");

            var (pledge1, payment1) = _service.CreatePledge(campaign, supporter, 5000);
            var (pledge2, payment2) = _service.CreatePledge(campaign, supporter, 3000);

            // Act & Assert: Tentar cruzar pagamento do apoio 2 com apoio 1
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.ConfirmPayment(pledge1, payment2, campaign)
            );

            Assert.Contains("não corresponde a este apoio", ex.Message);
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
    }
}
