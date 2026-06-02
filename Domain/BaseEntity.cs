using System;

namespace Amplifike.Domain
{
    /*
     * =================================================================================
     * CONCEITO DE ORIENTAÇÃO A OBJETOS: ABSTRAÇÃO
     * =================================================================================
     * NOME: Abstração (Abstraction)
     * OBJETIVO: Focar apenas nos aspectos essenciais de uma entidade do mundo real, 
     *           escondendo detalhes irrelevantes e reduzindo a complexidade do sistema.
     * EXPLICACÃO DO TRECHO:
     * A classe abstract 'BaseEntity' serve como um modelo conceitual genérico para todas
     * as entidades persistíveis do sistema (Campanha, Apoiador, Apoio, Recompensa, etc.).
     * Ela encapsula os atributos comuns 'Id' e 'CreatedAt', impedindo que esta classe
     * seja instanciada diretamente (visto que é abstrata), e fornecendo um contrato de base
     * robusto para a herança de outras classes do domínio.
     * =================================================================================
     */
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
