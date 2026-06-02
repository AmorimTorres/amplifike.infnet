using System;

namespace Amplifike.Interfaces
{
    /*
     * =================================================================================
     * PRINCÍPIO SOLID: INTERFACE SEGREGATION PRINCIPLE (ISP)
     * =================================================================================
     * NOME: Princípio da Segregação de Interfaces (ISP)
     * OBJETIVO: Evitar que clientes dependam de métodos que eles não utilizam, criando 
     *           interfaces pequenas, coesas, focadas e especializadas.
     * EXPLICACÃO DO TRECHO:
     * A interface 'IAuditLogger' é segregada e possui apenas um método ('Log'). 
     * Qualquer serviço que precise registrar atividades administrativas ou operacionais só 
     * precisa implementar ou depender desta pequena interface de auditoria, sem ser forçado 
     * a herdar métodos irrelevantes de log de depuração do sistema geral ou monitoramento.
     * =================================================================================
     */
    public interface IAuditLogger
    {
        void Log(string action, string entityType, Guid? entityId = null, Guid? adminUserId = null, string metadataJson = "{}");
    }
}
