using System;
using Amplifike.Domain;
using Xunit;

namespace Amplifike.Tests.DomainTests
{
    public class PixPaymentTests
    {
        [Fact]
        public void Should_CreatePixPaymentWithPendingStatus_When_ConstructedValidly()
        {
            // Arrange
            var pledgeId = Guid.NewGuid();

            // Act
            var payment = new PixPayment(pledgeId, "provider-id-123", "qr-code-url", "copia-e-cola-code", expirationMinutes: 20);

            // Assert
            Assert.Equal(pledgeId, payment.PledgeId);
            Assert.Equal("mock_pix", payment.Provider);
            Assert.Equal("provider-id-123", payment.ProviderPaymentId);
            Assert.Equal("qr-code-url", payment.QrCode);
            Assert.Equal("copia-e-cola-code", payment.CopyPasteCode);
            Assert.Equal(PixPaymentStatus.Pending, payment.Status);
            Assert.Null(payment.PaidAt);
            Assert.True(payment.ExpiresAt > DateTime.UtcNow.AddMinutes(19));
        }

        [Fact]
        public void Should_ConfirmPixPaymentSuccessfully_When_Pending()
        {
            // Arrange
            var payment = new PixPayment(Guid.NewGuid(), "provider-id-123", "qr-code", "copia-cola");
            var paidTime = DateTime.UtcNow;

            // Act
            payment.Confirm(paidTime);

            // Assert
            Assert.Equal(PixPaymentStatus.Paid, payment.Status);
            Assert.Equal(paidTime, payment.PaidAt);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_ConfirmCalledOnAlreadyPaidPix()
        {
            // Arrange
            var payment = new PixPayment(Guid.NewGuid(), "provider-id-123", "qr-code", "copia-cola");
            payment.Confirm(DateTime.UtcNow);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => payment.Confirm(DateTime.UtcNow));
            Assert.Contains("Apenas pagamentos pendentes", ex.Message);
        }

        [Fact]
        public void Should_CancelPixPaymentSuccessfully_When_Pending()
        {
            // Arrange
            var payment = new PixPayment(Guid.NewGuid(), "provider-id-123", "qr-code", "copia-cola");

            // Act
            payment.Cancel();

            // Assert
            Assert.Equal(PixPaymentStatus.Cancelled, payment.Status);
        }

        [Fact] // Teste Negativo
        public void Should_ThrowException_When_CancelCalledOnPaidPix()
        {
            // Arrange
            var payment = new PixPayment(Guid.NewGuid(), "provider-id-123", "qr-code", "copia-cola");
            payment.Confirm(DateTime.UtcNow);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => payment.Cancel());
            Assert.Contains("Não é possível cancelar um pagamento que já foi pago", ex.Message);
        }

        [Fact]
        public void Should_ExpirePixPaymentSuccessfully_When_Pending()
        {
            // Arrange
            var payment = new PixPayment(Guid.NewGuid(), "provider-id-123", "qr-code", "copia-cola");

            // Act
            payment.Expire();

            // Assert
            Assert.Equal(PixPaymentStatus.Expired, payment.Status);
        }

        [Fact]
        public void Should_MarkAsFailedSuccessfully_When_Pending()
        {
            // Arrange
            var payment = new PixPayment(Guid.NewGuid(), "provider-id-123", "qr-code", "copia-cola");

            // Act
            payment.MarkAsFailed();

            // Assert
            Assert.Equal(PixPaymentStatus.Failed, payment.Status);
        }
    }
}
