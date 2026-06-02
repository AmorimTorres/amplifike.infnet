using System;

namespace Amplifike.Domain
{
    public enum PixPaymentStatus
    {
        Pending,
        Paid,
        Expired,
        Cancelled,
        Failed
    }

    public class PixPayment : BaseEntity
    {
        public Guid PledgeId { get; private set; }
        public string Provider { get; private set; }
        public string ProviderPaymentId { get; private set; }
        public string QrCode { get; private set; }
        public string CopyPasteCode { get; private set; }
        public PixPaymentStatus Status { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? PaidAt { get; private set; }

        public PixPayment(Guid pledgeId, string providerPaymentId, string qrCode, string copyPasteCode, int expirationMinutes = 15)
        {
            if (pledgeId == Guid.Empty)
                throw new ArgumentException("Id do apoio inválido.", nameof(pledgeId));

            if (string.IsNullOrWhiteSpace(providerPaymentId))
                throw new ArgumentException("Id de pagamento do provedor inválido.", nameof(providerPaymentId));

            PledgeId = pledgeId;
            Provider = "mock_pix";
            ProviderPaymentId = providerPaymentId;
            QrCode = qrCode;
            CopyPasteCode = copyPasteCode;
            Status = PixPaymentStatus.Pending;
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            PaidAt = null;
        }

        public void Confirm(DateTime paidAt)
        {
            if (Status != PixPaymentStatus.Pending)
                throw new InvalidOperationException("Apenas pagamentos pendentes podem ser confirmados.");

            Status = PixPaymentStatus.Paid;
            PaidAt = paidAt;
        }

        public void Cancel()
        {
            if (Status == PixPaymentStatus.Paid)
                throw new InvalidOperationException("Não é possível cancelar um pagamento que já foi pago.");

            Status = PixPaymentStatus.Cancelled;
        }

        public void Expire()
        {
            if (Status != PixPaymentStatus.Pending)
                throw new InvalidOperationException("Apenas pagamentos pendentes podem ser expirados.");

            Status = PixPaymentStatus.Expired;
        }

        public void MarkAsFailed()
        {
            if (Status != PixPaymentStatus.Pending)
                throw new InvalidOperationException("Apenas pagamentos pendentes podem falhar.");

            Status = PixPaymentStatus.Failed;
        }
    }
}
