using System;

namespace Amplifike.Domain
{
    /*
     * =================================================================================
     * CONCEITO DE ORIENTAÇÃO A OBJETOS: HERANÇA E POLIMORFISMO
     * =================================================================================
     * NOME: Herança e Polimorfismo
     * OBJETIVO: Reutilizar a base da superclasse e modificar o comportamento dinamicamente 
     *           por meio da sobrescrita de métodos para suportar variações no domínio.
     * EXPLICACÃO DO TRECHO:
     * A classe 'FlexibleCampaign' estende 'Campaign'. Ela implementa uma regra de negócio 
     * diferente para a liberação de recompensas no método 'CanDeliverRewards()'. 
     * Ao contrário do Tudo ou Nada, a campanha "Flexível" permite que a Amplifike capture 
     * todos os apoios e entregue as recompensas independentemente de atingir a meta financeira.
     * Portanto, ela simplesmente retorna 'true' se não estiver cancelada, alterando o 
     * comportamento polimorficamente em tempo de execução.
     * =================================================================================
     */
    public class FlexibleCampaign : Campaign
    {
        public FlexibleCampaign(string title, string slug, string description, int goalAmount, DateTime endsAt, DateTime? startsAt = null, string? coverImage = null)
            : base(title, slug, description, goalAmount, endsAt, startsAt, coverImage)
        {
        }

        public override bool CanDeliverRewards()
        {
            // Na regra Flexível, as recompensas e recursos captados são garantidos e serão entregues,
            // desde que a campanha não tenha sido cancelada.
            return Status != CampaignStatus.Cancelled;
        }
    }
}
