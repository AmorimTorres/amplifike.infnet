using System;

namespace Amplifike.Domain
{
    /*
     * =================================================================================
     * CONCEITO DE ORIENTAÇÃO A OBJETOS: ENCAPSULAMENTO
     * =================================================================================
     * NOME: Encapsulamento (Encapsulation)
     * OBJETIVO: Esconder o estado interno de um objeto e exigir que todas as interações
     *           ocorram por meio de métodos públicos bem definidos, protegendo a integridade
     *           dos dados e mantendo a consistência do objeto.
     * EXPLICACÃO DO TRECHO:
     * Na classe 'Reward', todas as propriedades possuem setters privados ('private set').
     * Isso impede que agentes externos alterem arbitrariamente a quantidade de recompensas
     * resgatadas ('QuantityClaimed') ou o limite de estoque ('QuantityLimit'). Toda alteração
     * de estado ocorre estritamente através dos métodos públicos 'Claim()' e 'Release()', 
     * que validam as regras de negócio internas da recompensa.
     * =================================================================================
     * 
     * =================================================================================
     * PADRÃO GRASP: ESPECIALISTA NA INFORMAÇÃO (INFORMATION EXPERT)
     * =================================================================================
     * NOME: Especialista na Informação (Information Expert)
     * OBJETIVO: Atribuir a responsabilidade de executar um comportamento à classe que
     *           possui as informações necessárias para executá-lo.
     * EXPLICACÃO DO TRECHO:
     * O método 'IsAvailable()' e os métodos 'Claim()'/'Release()' aplicam este padrão. 
     * A classe 'Reward' é quem detém as informações sobre seu limite de estoque ('QuantityLimit') 
     * e a quantidade já reivindicada ('QuantityClaimed'). Portanto, ela própria deve ser a 
     * especialista responsável por decidir se está disponível e por atualizar seu próprio 
     * contador de estoque, ao invés de deixar que uma classe de serviço de fora faça essa conta.
     * =================================================================================
     */
    public class Reward : BaseEntity
    {
        public Guid CampaignId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int MinimumAmount { get; private set; } // Representado em centavos (ex: R$ 10,00 = 1000)
        public int? QuantityLimit { get; private set; }
        public int QuantityClaimed { get; private set; }
        public bool RequiresShipping { get; private set; }
        public string? EstimatedDelivery { get; private set; }

        public Reward(Guid campaignId, string title, string description, int minimumAmount, int? quantityLimit = null, bool requiresShipping = false, string? estimatedDelivery = null)
        {
            if (campaignId == Guid.Empty)
                throw new ArgumentException("Id da campanha inválido.", nameof(campaignId));
            
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("O título da recompensa é obrigatório.", nameof(title));

            if (minimumAmount <= 0)
                throw new ArgumentException("O valor mínimo de apoio deve ser maior que zero.", nameof(minimumAmount));

            if (quantityLimit.HasValue && quantityLimit.Value <= 0)
                throw new ArgumentException("O limite de quantidade, se definido, deve ser maior que zero.", nameof(quantityLimit));

            CampaignId = campaignId;
            Title = title;
            Description = description;
            MinimumAmount = minimumAmount;
            QuantityLimit = quantityLimit;
            RequiresShipping = requiresShipping;
            EstimatedDelivery = estimatedDelivery;
            QuantityClaimed = 0;
        }

        public bool IsAvailable()
        {
            if (!QuantityLimit.HasValue)
                return true;

            return QuantityClaimed < QuantityLimit.Value;
        }

        public bool IsAmountSufficient(int pledgeAmount)
        {
            return pledgeAmount >= MinimumAmount;
        }

        public void Claim()
        {
            if (!IsAvailable())
                throw new InvalidOperationException("Esta recompensa atingiu o limite de unidades disponíveis.");

            QuantityClaimed++;
        }

        public void Release()
        {
            if (QuantityClaimed > 0)
            {
                QuantityClaimed--;
            }
        }
    }
}
