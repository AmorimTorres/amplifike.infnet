using System;

namespace Amplifike.Domain
{
    public enum PledgeStatus
    {
        Created,
        AwaitingPix,
        Paid,
        Expired,
        Cancelled
    }

    public enum DeliveryStatus
    {
        NotStarted,
        Preparing,
        Shipped,
        Delivered,
        DeliveryIssue
    }

    /*
     * =================================================================================
     * CONCEITO DE ORIENTAÇÃO A OBJETOS: ENCAPSULAMENTO
     * =================================================================================
     * NOME: Encapsulamento (Encapsulation)
     * OBJETIVO: Garantir que a transição de estados de um apoio (Pledge) ocorra
     *           somente por meio de caminhos de transição válidos determinados pelas regras
     *           de negócio, protegendo as propriedades internas.
     * EXPLICACÃO DO TRECHO:
     * O estado 'Status' do apoio começa como 'Created' e só pode avançar para outros estados 
     * específicos (como 'Paid', 'Expired' ou 'Cancelled') através dos respectivos métodos públicos 
     * ('ConfirmPayment()', 'Cancel()', 'Expire()'). A propriedade 'PaidAt' só é preenchida
     * automaticamente quando o pagamento é de fato confirmado, mantendo a coesão e segurança.
     * =================================================================================
     */
    public class Pledge : BaseEntity
    {
        public Guid CampaignId { get; private set; }
        public Guid? RewardId { get; private set; }
        public Guid SupporterId { get; private set; }
        public int Amount { get; private set; } // Em centavos
        public PledgeStatus Status { get; private set; }
        public DeliveryStatus DeliveryStatus { get; private set; }
        public DateTime? PaidAt { get; private set; }

        public Pledge(Guid campaignId, Guid supporterId, int amount, Guid? rewardId = null)
        {
            if (campaignId == Guid.Empty)
                throw new ArgumentException("Id da campanha inválido.", nameof(campaignId));

            if (supporterId == Guid.Empty)
                throw new ArgumentException("Id do apoiador inválido.", nameof(supporterId));

            if (amount <= 0)
                throw new ArgumentException("O valor do apoio deve ser maior que zero.", nameof(amount));

            CampaignId = campaignId;
            SupporterId = supporterId;
            Amount = amount;
            RewardId = rewardId;
            Status = PledgeStatus.Created;
            DeliveryStatus = DeliveryStatus.NotStarted;
            PaidAt = null;
        }

        public void MarkAsAwaitingPayment()
        {
            if (Status != PledgeStatus.Created)
                throw new InvalidOperationException("Um apoio só pode aguardar pagamento se estiver no estado inicial.");

            Status = PledgeStatus.AwaitingPix;
        }

        public void ConfirmPayment()
        {
            if (Status != PledgeStatus.AwaitingPix && Status != PledgeStatus.Created)
                throw new InvalidOperationException("Pagamento só pode ser confirmado para apoios recém-criados ou aguardando Pix.");

            Status = PledgeStatus.Paid;
            PaidAt = DateTime.UtcNow;
            DeliveryStatus = DeliveryStatus.Preparing;
        }

        public void Cancel()
        {
            if (Status == PledgeStatus.Paid)
                throw new InvalidOperationException("Não é possível cancelar um apoio que já foi pago.");

            Status = PledgeStatus.Cancelled;
        }

        public void Expire()
        {
            if (Status != PledgeStatus.AwaitingPix)
                throw new InvalidOperationException("Apenas apoios aguardando pagamento podem expirar.");

            Status = PledgeStatus.Expired;
        }

        public void UpdateDelivery(DeliveryStatus newStatus)
        {
            if (Status != PledgeStatus.Paid)
                throw new InvalidOperationException("Não é possível atualizar o status de entrega de um apoio que não foi pago.");

            DeliveryStatus = newStatus;
        }
    }
}
