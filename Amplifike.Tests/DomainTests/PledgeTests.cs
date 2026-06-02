using System;
using Amplifike.Domain;
using Xunit;

namespace Amplifike.Tests.DomainTests
{
    public class PledgeTests
    {
        [Fact]
        public void Should_CreatePledgeWithInitialStates_When_ConstructedValidly()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var supporterId = Guid.NewGuid();
            var rewardId = Guid.NewGuid();

            // Act
            var pledge = new Pledge(campaignId, supporterId, 5000, rewardId);

            // Assert
            Assert.Equal(campaignId, pledge.CampaignId);
            Assert.Equal(supporterId, pledge.SupporterId);
            Assert.Equal(5000, pledge.Amount);
            Assert.Equal(rewardId, pledge.RewardId);
            Assert.Equal(PledgeStatus.Created, pledge.Status);
            Assert.Equal(DeliveryStatus.NotStarted, pledge.DeliveryStatus);
            Assert.Null(pledge.PaidAt);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_PledgeAmountIsZeroOrNegative()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Pledge(
                campaignId: Guid.NewGuid(),
                supporterId: Guid.NewGuid(),
                amount: 0
            ));

            Assert.Contains("maior que zero", ex.Message);
        }

        [Fact]
        public void Should_UpdateStatusToAwaitingPix_When_MarkAsAwaitingPaymentCalled()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 1000);

            // Act
            pledge.MarkAsAwaitingPayment();

            // Assert
            Assert.Equal(PledgeStatus.AwaitingPix, pledge.Status);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_MarkAsAwaitingPaymentCalledOnAwaitingPixPledge()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 1000);
            pledge.MarkAsAwaitingPayment();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => pledge.MarkAsAwaitingPayment());
            Assert.Contains("estado inicial", ex.Message);
        }

        [Fact]
        public void Should_ConfirmPaymentAndSetPaidAt_When_ConfirmPaymentCalledOnAwaitingPix()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);
            pledge.MarkAsAwaitingPayment();

            // Act
            pledge.ConfirmPayment();

            // Assert
            Assert.Equal(PledgeStatus.Paid, pledge.Status);
            Assert.NotNull(pledge.PaidAt);
            Assert.Equal(DeliveryStatus.Preparing, pledge.DeliveryStatus);
            Assert.True((DateTime.UtcNow - pledge.PaidAt.Value).TotalSeconds < 5); // Tempo recente
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_ConfirmPaymentCalledOnCancelledPledge()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);
            pledge.Cancel();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => pledge.ConfirmPayment());
            Assert.Contains("Pagamento só pode ser confirmado", ex.Message);
        }

        [Fact]
        public void Should_CancelPledgeSuccessfully_When_PledgeIsUnpaid()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);
            pledge.MarkAsAwaitingPayment();

            // Act
            pledge.Cancel();

            // Assert
            Assert.Equal(PledgeStatus.Cancelled, pledge.Status);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_CancelCalledOnPaidPledge()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);
            pledge.MarkAsAwaitingPayment();
            pledge.ConfirmPayment();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => pledge.Cancel());
            Assert.Contains("Não é possível cancelar um apoio que já foi pago", ex.Message);
        }

        [Fact]
        public void Should_ExpirePledgeSuccessfully_When_AwaitingPix()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);
            pledge.MarkAsAwaitingPayment();

            // Act
            pledge.Expire();

            // Assert
            Assert.Equal(PledgeStatus.Expired, pledge.Status);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_ExpireCalledOnPaidPledge()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);
            pledge.MarkAsAwaitingPayment();
            pledge.ConfirmPayment();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => pledge.Expire());
            Assert.Contains("Apenas apoios aguardando pagamento podem expirar", ex.Message);
        }

        [Fact]
        public void Should_UpdateDeliveryStatusSuccessfully_When_PledgeIsPaid()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);
            pledge.MarkAsAwaitingPayment();
            pledge.ConfirmPayment();

            // Act
            pledge.UpdateDelivery(DeliveryStatus.Shipped);

            // Assert
            Assert.Equal(DeliveryStatus.Shipped, pledge.DeliveryStatus);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_UpdateDeliveryStatusCalledOnUnpaidPledge()
        {
            // Arrange
            var pledge = new Pledge(Guid.NewGuid(), Guid.NewGuid(), 5000);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => pledge.UpdateDelivery(DeliveryStatus.Shipped));
            Assert.Contains("Não é possível atualizar o status de entrega", ex.Message);
        }
    }
}
