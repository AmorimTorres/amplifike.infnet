using System;
using Amplifike.Domain;
using Amplifike.Interfaces;

namespace Amplifike.Tests.Mocks
{
    /*
     * =================================================================================
     * PRINCÍPIO DE TESTE UNITÁRIO: ISOLAMENTO (ISOLATION)
     * =================================================================================
     * NOME: Isolamento (Isolation)
     * CONCEITO: Garantir que a unidade de código sob teste não dependa ou interaja com
     *           sistemas externos (bancos de dados, rede, gateways de terceiros) para evitar
     *           que falhas nesses sistemas quebrem o teste da unidade atual.
     * APLICAÇÃO:
     * A classe 'StubPaymentGateway' é um Dublê de Teste (Test Double) do tipo STUB. 
     * Ela fornece uma resposta pré-programada (estática e controlada) para o método
     * 'GeneratePixPayment()', isolando completamente o 'PledgeService' de qualquer 
     * processamento financeiro real e garantindo um comportamento previsível.
     * =================================================================================
     */
    public class StubPaymentGateway : IPaymentGateway
    {
        public string LastGeneratedPaymentId { get; private set; } = string.Empty;

        public PixPayment GeneratePixPayment(Pledge pledge)
        {
            if (pledge == null)
                throw new ArgumentNullException(nameof(pledge));

            LastGeneratedPaymentId = $"stub_pix_{pledge.Id.ToString()[..8]}";
            
            return new PixPayment(
                pledgeId: pledge.Id,
                providerPaymentId: LastGeneratedPaymentId,
                qrCode: "https://mock-qr-code-stub.com/image.png",
                copyPasteCode: "mock-copy-paste-code-stub-12345",
                expirationMinutes: 15
            );
        }
    }
}
