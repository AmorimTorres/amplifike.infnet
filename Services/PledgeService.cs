using System;
using Amplifike.Domain;
using Amplifike.Interfaces;

namespace Amplifike.Services
{
    /*
     * =================================================================================
     * PRINCÍPIO SOLID: SINGLE RESPONSIBILITY PRINCIPLE (SRP)
     * =================================================================================
     * NOME: Princípio de Responsabilidade Única (SRP)
     * OBJETIVO: Separar o fluxo de processamento de apoios da lógica interna das entidades.
     * EXPLICACÃO DO TRECHO:
     * A classe 'PledgeService' é responsável única e exclusivamente por coordenar o fluxo 
     * de apoio e checkout. Ela orquestra a validação da campanha, o resgate de recompensas, 
     * a persistência do apoio, a chamada do gateway de pagamentos e a auditoria dos eventos.
     * Ela não cuida dos detalhes internos da regra Pix e nem das regras de estoque da recompensa, 
     * delegando-as às suas respectivas classes.
     * =================================================================================
     * 
     * =================================================================================
     * PRINCÍPIO SOLID: DEPENDENCY INVERSION PRINCIPLE (DIP)
     * =================================================================================
     * NOME: Princípio da Inversão de Dependência (DIP)
     * OBJETIVO: Garantir que módulos de alto nível não dependam de módulos de baixo nível,
     *           mas sim de abstrações. Abstrações não devem depender de detalhes; detalhes
     *           devem depender de abstrações.
     * EXPLICACÃO DO TRECHO:
     * O 'PledgeService' (módulo de alto nível) depende estritamente de 'IPaymentGateway' e 
     * 'IAuditLogger' (abstrações). Ele não possui acoplamento rígido com a classe 
     * 'MockPixPaymentGateway' (módulo de baixo nível). Isso permite que possamos trocar 
     * o gateway por uma integração real com a API da Juno, Stone ou Stripe apenas trocando
     * a injeção via construtor, sem quebrar nenhuma linha de código da lógica de negócio de apoios.
     * =================================================================================
     * 
     * =================================================================================
     * PADRÃO GRASP: BAIXO ACOPLAMENTO (LOW COUPLING)
     * =================================================================================
     * NOME: Baixo Acoplamento (Low Coupling)
     * OBJETIVO: Reduzir a dependência mútua entre as classes, tornando o sistema mais 
     *           flexível a mudanças e facilitando a manutenção e os testes unitários.
     * EXPLICACÃO DO TRECHO:
     * Ao receber as dependências de auditoria e gateway de pagamento via construtor (Injeção de 
     * Dependência), a classe 'PledgeService' permanece altamente isolada. Se um novo requisito 
     * exigir que as regras de pagamento mudem, as classes consumidoras de 'PledgeService' não 
     * são afetadas, garantindo o baixo acoplamento.
     * =================================================================================
     */
    public class PledgeService
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IAuditLogger _auditLogger;

        public PledgeService(IPaymentGateway paymentGateway, IAuditLogger auditLogger)
        {
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
            _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        }

        public (Pledge pledge, PixPayment payment) CreatePledge(
            Campaign campaign, 
            Supporter supporter, 
            int amount, 
            Reward? reward = null, 
            ShippingAddress? shippingAddress = null)
        {
            if (campaign == null) throw new ArgumentNullException(nameof(campaign));
            if (supporter == null) throw new ArgumentNullException(nameof(supporter));

            // Validar se a campanha está ativa para receber apoios
            if (campaign.Status != CampaignStatus.Published)
            {
                throw new InvalidOperationException("A campanha deve estar publicada e ativa para receber apoios.");
            }

            // Validar se a campanha expirou
            if (DateTime.UtcNow > campaign.EndsAt)
            {
                throw new InvalidOperationException("O período de captação desta campanha já se encerrou.");
            }

            // Se selecionou recompensa, validar se pertence à campanha e se o valor é suficiente
            if (reward != null)
            {
                if (reward.CampaignId != campaign.Id)
                {
                    throw new InvalidOperationException("A recompensa selecionada não pertence a esta campanha.");
                }

                if (!reward.IsAmountSufficient(amount))
                {
                    throw new InvalidOperationException($"O valor de R$ {amount / 100.0:F2} é insuficiente para esta recompensa. O mínimo é R$ {reward.MinimumAmount / 100.0:F2}.");
                }

                // Especialista na informação: A recompensa decide se há estoque e consome a unidade
                reward.Claim();
            }

            // Instanciar o Apoio (Pledge)
            var pledge = new Pledge(campaign.Id, supporter.Id, amount, reward?.Id);

            // Gerar o pagamento Pix via Gateway Abstrato (DIP / Abstração)
            pledge.MarkAsAwaitingPayment();
            var pixPayment = _paymentGateway.GeneratePixPayment(pledge);

            // Gravar log de auditoria via Logger Abstrato (ISP / DIP)
            string metaJson = $"{{\"supporter\":\"{supporter.Name}\",\"amount\":{amount},\"rewardId\":\"{reward?.Id}\"}}";
            _auditLogger.Log("PLEDGE_CREATED", "Pledge", pledge.Id, null, metaJson);

            return (pledge, pixPayment);
        }

        public void ConfirmPayment(Pledge pledge, PixPayment payment, Campaign campaign)
        {
            if (pledge == null) throw new ArgumentNullException(nameof(pledge));
            if (payment == null) throw new ArgumentNullException(nameof(payment));
            if (campaign == null) throw new ArgumentNullException(nameof(campaign));

            if (payment.PledgeId != pledge.Id)
            {
                throw new InvalidOperationException("O pagamento Pix não corresponde a este apoio.");
            }

            // 1. Confirmar o pagamento Pix
            payment.Confirm(DateTime.UtcNow);

            // 2. Confirmar o apoio (muda para Pago e altera status de entrega)
            pledge.ConfirmPayment();

            // 3. Atualizar o valor arrecadado da campanha (Encapsulamento)
            campaign.ReceivePledgeContribution(pledge.Amount);

            // 4. Gravar log de auditoria
            string metaJson = $"{{\"campaignSlug\":\"{campaign.Slug}\",\"amount\":{pledge.Amount}}}";
            _auditLogger.Log("PLEDGE_PAID", "Pledge", pledge.Id, null, metaJson);
        }
    }
}
