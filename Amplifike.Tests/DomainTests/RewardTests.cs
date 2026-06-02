using System;
using Amplifike.Domain;
using Xunit;

namespace Amplifike.Tests.DomainTests
{
    public class RewardTests
    {
        [Fact]
        public void Should_CreateRewardCorrectly_When_ValidParametersProvided()
        {
            // Arrange
            var campaignId = Guid.NewGuid();

            // Act
            var reward = new Reward(
                campaignId: campaignId,
                title: "Camiseta Oficial",
                description: "Camiseta oficial da banda",
                minimumAmount: 8000,
                quantityLimit: 50,
                requiresShipping: true,
                estimatedDelivery: "Dezembro/2026"
            );

            // Assert
            Assert.Equal(campaignId, reward.CampaignId);
            Assert.Equal("Camiseta Oficial", reward.Title);
            Assert.Equal(8000, reward.MinimumAmount);
            Assert.Equal(50, reward.QuantityLimit);
            Assert.Equal(0, reward.QuantityClaimed);
            Assert.True(reward.RequiresShipping);
            Assert.Equal("Dezembro/2026", reward.EstimatedDelivery);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_MinimumAmountIsZeroOrNegative()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Reward(
                campaignId: Guid.NewGuid(),
                title: "Camiseta",
                description: "Camiseta",
                minimumAmount: 0
            ));

            Assert.Contains("valor mínimo", ex.Message);
        }

        [Fact]
        public void Should_ReturnTrueForIsAmountSufficient_When_PledgeAmountEqualsOrExceedsMinimum()
        {
            // Arrange
            var reward = new Reward(Guid.NewGuid(), "CD", "desc", 3000);

            // Act & Assert
            Assert.True(reward.IsAmountSufficient(3000));
            Assert.True(reward.IsAmountSufficient(5000));
            Assert.False(reward.IsAmountSufficient(2900));
        }

        [Fact]
        public void Should_AlwaysBeAvailable_When_NoQuantityLimitIsSet()
        {
            // Arrange
            var reward = new Reward(Guid.NewGuid(), "Download", "desc", 1000, quantityLimit: null);

            // Act & Assert
            Assert.True(reward.IsAvailable());
            reward.Claim();
            Assert.Equal(1, reward.QuantityClaimed);
            Assert.True(reward.IsAvailable()); // Ainda disponível
        }

        [Fact]
        public void Should_IncrementQuantityClaimed_When_ClaimIsCalled()
        {
            // Arrange
            var reward = new Reward(Guid.NewGuid(), "Ingresso", "desc", 5000, quantityLimit: 2);

            // Act
            Assert.True(reward.IsAvailable());
            reward.Claim();
            Assert.Equal(1, reward.QuantityClaimed);
            
            Assert.True(reward.IsAvailable());
            reward.Claim();
            Assert.Equal(2, reward.QuantityClaimed);

            // Assert - Atingiu o limite de estoque
            Assert.False(reward.IsAvailable());
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_ClaimCalledOnSoldOutReward()
        {
            // Arrange
            var reward = new Reward(Guid.NewGuid(), "Ingresso VIP", "desc", 10000, quantityLimit: 1);
            reward.Claim(); // Consome o único disponível

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => reward.Claim());
            Assert.Contains("atingiu o limite", ex.Message);
        }

        [Fact]
        public void Should_DecrementQuantityClaimed_When_ReleaseIsCalled()
        {
            // Arrange
            var reward = new Reward(Guid.NewGuid(), "VIP", "desc", 10000, quantityLimit: 1);
            reward.Claim();
            Assert.False(reward.IsAvailable());

            // Act
            reward.Release();

            // Assert
            Assert.Equal(0, reward.QuantityClaimed);
            Assert.True(reward.IsAvailable()); // Volta a estar disponível
        }

        [Fact]
        public void Should_DoNothing_When_ReleaseIsCalledAndQuantityClaimedIsZero()
        {
            // Arrange
            var reward = new Reward(Guid.NewGuid(), "VIP", "desc", 10000);

            // Act
            reward.Release();

            // Assert
            Assert.Equal(0, reward.QuantityClaimed);
        }
    }
}
