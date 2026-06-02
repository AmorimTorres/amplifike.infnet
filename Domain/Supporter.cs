using System;

namespace Amplifike.Domain
{
    /*
     * =================================================================================
     * PADRÃO GRASP: ALTA COESÃO (HIGH COHESION)
     * =================================================================================
     * NOME: Alta Coesão (High Cohesion)
     * OBJETIVO: Garantir que uma classe tenha responsabilidades estreitamente relacionadas
     *           e altamente focadas em um único propósito do domínio.
     * EXPLICACÃO DO TRECHO:
     * A classe 'Supporter' possui alta coesão pois é unicamente responsável por representar
     * os dados cadastrais e as regras de um apoiador no financiamento coletivo da Amplifike. 
     * Ela não tenta gerenciar pagamentos, processar envios ou enviar e-mails de notificação. 
     * Todos os seus atributos (Name, Email, Phone, DocumentOptional) e comportamentos são 
     * focados e pertencem estritamente à entidade Apoiador, tornando-a fácil de compreender,
     * testar e manter no futuro.
     * =================================================================================
     */
    public class Supporter : BaseEntity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string? DocumentOptional { get; private set; }

        public Supporter(string name, string email, string phone, string? documentOptional = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do apoiador é obrigatório.", nameof(name));
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail do apoiador é obrigatório.", nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("O telefone do apoiador é obrigatório.", nameof(phone));

            Name = name;
            Email = email;
            Phone = phone;
            DocumentOptional = documentOptional;
        }
    }
}
