using Amplifike.Domain;

namespace Amplifike.Interfaces
{
    /*
     * =================================================================================
     * CONCEITO DE ORIENTAÇÃO A OBJETOS: ABSTRAÇÃO
     * =================================================================================
     * NOME: Abstração (Abstraction)
     * OBJETIVO: Definir um contrato ou representação genérica de um recurso externo do mundo real
     *           sem se acoplar a uma biblioteca, banco de dados ou provedor específico.
     * EXPLICACÃO DO TRECHO:
     * A interface 'IPaymentGateway' abstrai a complexidade do processamento de pagamentos. 
     * O sistema da Amplifike sabe apenas que pode solicitar a geração de um pagamento Pix 
     * enviando um apoio ('Pledge') por meio do método 'GeneratePixPayment()'. A forma como isso 
     * é feito internamente (integrando com o Banco do Brasil, Itaú, Mercado Pago, ou usando
     * um mock de teste local) é completamente oculta para as classes de domínio.
     * =================================================================================
     */
    public interface IPaymentGateway
    {
        PixPayment GeneratePixPayment(Pledge pledge);
    }
}
