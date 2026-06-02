using System;

namespace Amplifike.Domain
{
    /*
     * =================================================================================
     * CONCEITO DE ORIENTAÇÃO A OBJETOS: HERANÇA
     * =================================================================================
     * NOME: Herança (Inheritance)
     * OBJETIVO: Criar uma nova classe (subclasse) baseada em uma classe existente (superclasse), 
     *           compartilhando e estendendo seus atributos e comportamentos para promover o 
     *           reúso e a especialização de código.
     * EXPLICACÃO DO TRECHO:
     * A classe 'AllOrNothingCampaign' herda diretamente de 'Campaign' (usando a sintaxe : Campaign).
     * Ela reutiliza todas as propriedades básicas como 'Title', 'GoalAmount', 'CurrentAmount' 
     * e o comportamento comum de gerenciar recompensas ('AddReward'), adicionando apenas as 
     * particularidades e especializações deste tipo específico de campanha.
     * =================================================================================
     * 
     * =================================================================================
     * CONCEITO DE ORIENTAÇÃO A OBJETOS: POLIMORFISMO
     * =================================================================================
     * NOME: Polimorfismo (Polymorphism)
     * OBJETIVO: Permitir que um mesmo método apresente comportamentos diferentes quando 
     *           chamado em diferentes subclasses da mesma hierarquia.
     * EXPLICACÃO DO TRECHO:
     * Sobrescrevemos o método abstrato 'CanDeliverRewards()' usando a palavra-chave 'override'.
     * Em uma campanha "Tudo ou Nada", os apoiadores só receberão suas recompensas se a meta 
     * de arrecadação for 100% atingida até o prazo final. O polimorfismo permite que o 
     * 'PledgeService' invoque 'CanDeliverRewards()' em uma campanha genérica sem precisar 
     * saber o tipo exato dela, e o C# chamará dinamicamente esta implementação específica.
     * =================================================================================
     */
    public class AllOrNothingCampaign : Campaign
    {
        public AllOrNothingCampaign(string title, string slug, string description, int goalAmount, DateTime endsAt, DateTime? startsAt = null, string? coverImage = null)
            : base(title, slug, description, goalAmount, endsAt, startsAt, coverImage)
        {
        }

        public override bool CanDeliverRewards()
        {
            // Na regra Tudo ou Nada, as recompensas são liberadas apenas se o prazo expirou ou
            // a campanha terminou, E o valor arrecadado é maior ou igual à meta estipulada.
            return IsGoalMet();
        }
    }
}
