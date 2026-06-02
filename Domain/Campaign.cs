using System;
using System.Collections.Generic;

namespace Amplifike.Domain
{
    public enum CampaignStatus
    {
        Draft,
        Published,
        Ended,
        Cancelled
    }

    public abstract class Campaign : BaseEntity
    {
        private readonly List<Reward> _rewards = new();

        public string Title { get; private set; }
        public string Slug { get; private set; }
        public string Description { get; private set; }
        public int GoalAmount { get; private set; }
        public int CurrentAmount { get; private set; }
        public DateTime? StartsAt { get; private set; }
        public DateTime EndsAt { get; private set; }
        public CampaignStatus Status { get; private set; }
        public string? CoverImage { get; private set; }

        public IReadOnlyCollection<Reward> Rewards => _rewards.AsReadOnly();

        protected Campaign(string title, string slug, string description, int goalAmount, DateTime endsAt, DateTime? startsAt = null, string? coverImage = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("O título da campanha é obrigatório.", nameof(title));

            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("O slug da campanha é obrigatório.", nameof(slug));

            if (goalAmount <= 0)
                throw new ArgumentException("A meta financeira deve ser maior que zero.", nameof(goalAmount));

            if (endsAt <= DateTime.UtcNow)
                throw new ArgumentException("A data de término deve ser futura.", nameof(endsAt));

            Title = title;
            Slug = slug;
            Description = description;
            GoalAmount = goalAmount;
            CurrentAmount = 0;
            StartsAt = startsAt;
            EndsAt = endsAt;
            Status = CampaignStatus.Draft;
            CoverImage = coverImage;
        }

        public void AddReward(string title, string description, int minimumAmount, int? quantityLimit = null, bool requiresShipping = false, string? estimatedDelivery = null)
        {
            if (Status != CampaignStatus.Draft)
                throw new InvalidOperationException("Não é possível adicionar recompensas a uma campanha que já foi publicada ou encerrada.");

            var reward = new Reward(Id, title, description, minimumAmount, quantityLimit, requiresShipping, estimatedDelivery);
            _rewards.Add(reward);
        }

        public void Publish()
        {
            if (Status != CampaignStatus.Draft)
                throw new InvalidOperationException("Apenas campanhas em rascunho podem ser publicadas.");

            Status = CampaignStatus.Published;
            if (!StartsAt.HasValue)
            {
                StartsAt = DateTime.UtcNow;
            }
        }

        public void Cancel()
        {
            if (Status == CampaignStatus.Ended)
                throw new InvalidOperationException("Não é possível cancelar uma campanha que já foi encerrada.");

            Status = CampaignStatus.Cancelled;
        }

        public void End()
        {
            if (Status != CampaignStatus.Published)
                throw new InvalidOperationException("Apenas campanhas publicadas podem ser encerradas.");

            Status = CampaignStatus.Ended;
        }

        public void ReceivePledgeContribution(int amount)
        {
            if (Status != CampaignStatus.Published)
                throw new InvalidOperationException("Contribuições só podem ser recebidas por campanhas publicadas e ativas.");

            if (DateTime.UtcNow > EndsAt)
                throw new InvalidOperationException("A campanha já atingiu o prazo final de captação.");

            CurrentAmount += amount;
        }

        public bool IsGoalMet()
        {
            return CurrentAmount >= GoalAmount;
        }

        public abstract bool CanDeliverRewards();
    }
}
