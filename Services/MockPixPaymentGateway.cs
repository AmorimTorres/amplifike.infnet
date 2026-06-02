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
     * OBJETIVO: Atribuir uma única responsabilidade à classe.
     * EXPLICACÃO DO TRECHO:
     * A classe 'MockPixPaymentGateway' é responsável puramente por simular a integração 
     * com o provedor Pix da Amplifike. Ela gera o QR Code e o código Pix copia-e-cola 
     * baseando-se no ID do apoio. Isso separa a complexidade tecnológica externa do núcleo
     * das entidades de negócio.
     * =================================================================================
     */
    public class MockPixPaymentGateway : IPaymentGateway
    {
        public PixPayment GeneratePixPayment(Pledge pledge)
        {
            if (pledge == null)
                throw new ArgumentNullException(nameof(pledge));

            string mockPaymentId = $"mock_pix_{pledge.Id.ToString()[..8]}";
            string mockQrCode = $"https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=00020126360014BR.GOV.BCB.PIX0114contato@amplifike0214{mockPaymentId}";
            string mockCopyPaste = $"00020126360014BR.GOV.BCB.PIX0114contato@amplifike0214{mockPaymentId}5204000053039865405{pledge.Amount / 100.0:F2}5802BR5909AMPLIFIKE6009SaoPaulo62070503***6304D1B8";

            return new PixPayment(pledge.Id, mockPaymentId, mockQrCode, mockCopyPaste, expirationMinutes: 15);
        }
    }
}
