using System;
using Amplifike.Domain;
using Xunit;

namespace Amplifike.Tests.DomainTests
{
    /*
     * =================================================================================
     * PRINCÍPIO DE TESTE UNITÁRIO: REPETIBILIDADE (REPEATABLE)
     * =================================================================================
     * NOME: Repetibilidade (Repeatable)
     * CONCEITO: Os testes devem ser executados de forma totalmente determinística, 
     *           produzindo o mesmo resultado a cada execução, sem sofrer variações por
     *           causa do horário local, estado da internet ou ordem em que rodam.
     * APLICAÇÃO:
     * No teste 'Should_ThrowException_When_EndsAtIsPast()', nós garantimos a repetibilidade
     * ao definir a data de encerramento baseando-se estritamente no relógio atual 
     * ('DateTime.UtcNow.AddMinutes(-5)'), assegurando que este teste de data expirada sempre 
     * passará em qualquer ano ou fuso horário em que o computador estiver configurado.
     * =================================================================================
     * 
     * =================================================================================
     * PRINCÍPIO DE TESTE UNITÁRIO: RAPIDEZ (FAST)
     * =================================================================================
     * NOME: Rapidez (Fast)
     * CONCEITO: Os testes unitários devem rodar extremamente rápido (milissegundos) 
     *           para incentivar o desenvolvedor a executá-los continuamente durante 
     *           o ciclo de desenvolvimento de código.
     * APLICAÇÃO:
     * Toda a suíte de testes de campanha instancia apenas objetos POCO em memória e realiza
     * asserções aritméticas e de estado simples. Não há conexões de banco ou acesso a disco,
     * fazendo com que cada caso de teste abaixo seja executado em menos de 1 milissegundo.
     * =================================================================================
     */
    public class CampaignTests
    {
        [Fact]
        public void Should_CreateCampaignWithDraftStatus_When_ConstructedValidly()
        {
            // Arrange & Act
            var campaign = new FlexibleCampaign(
                title: "Campanha Teste",
                slug: "campanha-teste",
                description: "Descrição da campanha",
                goalAmount: 50000,
                endsAt: DateTime.UtcNow.AddDays(5)
            );

            // Assert
            Assert.Equal("Campanha Teste", campaign.Title);
            Assert.Equal("campanha-teste", campaign.Slug);
            Assert.Equal(50000, campaign.GoalAmount);
            Assert.Equal(0, campaign.CurrentAmount);
            Assert.Equal(CampaignStatus.Draft, campaign.Status);
            Assert.Empty(campaign.Rewards);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_GoalAmountIsZeroOrNegative()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new FlexibleCampaign(
                title: "Campanha Inválida",
                slug: "campanha-invalida",
                description: "Descrição",
                goalAmount: 0,
                endsAt: DateTime.UtcNow.AddDays(5)
            ));

            Assert.Contains("meta financeira", ex.Message);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_EndsAtIsPast()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new FlexibleCampaign(
                title: "Campanha Expirada",
                slug: "campanha-expirada",
                description: "Descrição",
                goalAmount: 10000,
                endsAt: DateTime.UtcNow.AddMinutes(-5) // Passado
            ));

            Assert.Contains("data de término deve ser futura", ex.Message);
        }

        [Fact]
        public void Should_AddRewardSuccessfully_When_CampaignIsDraft()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));

            // Act
            campaign.AddReward("VIP", "VIP desc", 2000, quantityLimit: 10);

            // Assert
            Assert.Single(campaign.Rewards);
            var reward = Assert.Single(campaign.Rewards);
            Assert.Equal("VIP", reward.Title);
            Assert.Equal(2000, reward.MinimumAmount);
            Assert.Equal(10, reward.QuantityLimit);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_AddRewardToPublishedCampaign()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));
            campaign.Publish();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => 
                campaign.AddReward("VIP", "VIP desc", 2000)
            );

            Assert.Contains("Não é possível adicionar recompensas", ex.Message);
        }

        [Fact]
        public void Should_UpdateStatusToPublished_When_PublishIsCalledOnDraft()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));

            // Act
            campaign.Publish();

            // Assert
            Assert.Equal(CampaignStatus.Published, campaign.Status);
            Assert.NotNull(campaign.StartsAt);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_ReceivePledgeContributionOnDraftCampaign()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                campaign.ReceivePledgeContribution(5000)
            );

            Assert.Contains("Contribuições só podem ser recebidas por campanhas publicadas", ex.Message);
        }

        [Fact]
        public void Should_IncrementCurrentAmount_When_PledgeContributionReceivedOnPublishedCampaign()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));
            campaign.Publish();

            // Act
            campaign.ReceivePledgeContribution(3000);

            // Assert
            Assert.Equal(3000, campaign.CurrentAmount);
            Assert.False(campaign.IsGoalMet());
        }

        [Fact]
        public void Should_ReturnTrueForIsGoalMet_When_CurrentAmountEqualsOrExceedsGoalAmount()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));
            campaign.Publish();

            // Act
            campaign.ReceivePledgeContribution(10000);

            // Assert
            Assert.Equal(10000, campaign.CurrentAmount);
            Assert.True(campaign.IsGoalMet());
        }

        [Fact] // Teste de Polimorfismo
        public void Should_PolymorphicallyDetermineRewardDelivery_For_FlexibleCampaign()
        {
            // Arrange
            var campaign = new FlexibleCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));
            campaign.Publish();
            campaign.ReceivePledgeContribution(5000); // 50% da meta
            campaign.End();

            // Act
            bool canDeliver = campaign.CanDeliverRewards();

            // Assert: Campanha Flexível sempre entrega recompensas se não cancelada, mesmo sem bater a meta
            Assert.True(canDeliver);
        }

        [Fact] // Teste de Polimorfismo
        public void Should_PolymorphicallyDetermineRewardDelivery_For_AllOrNothingCampaign_When_GoalNotMet()
        {
            // Arrange
            var campaign = new AllOrNothingCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));
            campaign.Publish();
            campaign.ReceivePledgeContribution(9900); // 99% da meta
            campaign.End();

            // Act
            bool canDeliver = campaign.CanDeliverRewards();

            // Assert: Tudo ou Nada impede entrega se a meta não foi 100% batida
            Assert.False(canDeliver);
        }

        [Fact] // Teste de Polimorfismo
        public void Should_PolymorphicallyDetermineRewardDelivery_For_AllOrNothingCampaign_When_GoalMet()
        {
            // Arrange
            var campaign = new AllOrNothingCampaign("Show", "show", "desc", 10000, DateTime.UtcNow.AddDays(2));
            campaign.Publish();
            campaign.ReceivePledgeContribution(12000); // Meta batida
            campaign.End();

            // Act
            bool canDeliver = campaign.CanDeliverRewards();

            // Assert: Tudo ou Nada libera se atingido
            Assert.True(canDeliver);
        }
    }
}
