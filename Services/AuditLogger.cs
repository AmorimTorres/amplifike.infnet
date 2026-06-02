using System;
using System.Collections.Generic;
using Amplifike.Domain;
using Amplifike.Interfaces;

namespace Amplifike.Services
{
    /*
     * =================================================================================
     * PRINCÍPIO SOLID: SINGLE RESPONSIBILITY PRINCIPLE (SRP)
     * =================================================================================
     * NOME: Princípio de Responsabilidade Única (SRP)
     * OBJETIVO: Garantir que uma classe tenha apenas um único motivo para mudar,
     *           ou seja, que ela tenha uma única responsabilidade no ecossistema de software.
     * EXPLICACÃO DO TRECHO:
     * O 'AuditLogger' é unicamente responsável por capturar, formatar e armazenar (ou exibir)
     * os registros de auditoria ('AuditLog') das ações administrativas e críticas.
     * Se mudarmos a forma de log (por exemplo, salvar em banco de dados SQL ou enviar para um
     * serviço em nuvem), apenas esta classe será alterada, não afetando a lógica das campanhas
     * ou dos apoios.
     * =================================================================================
     */
    public class AuditLogger : IAuditLogger
    {
        private readonly List<AuditLog> _logs = new();
        public IReadOnlyCollection<AuditLog> Logs => _logs.AsReadOnly();

        public void Log(string action, string entityType, Guid? entityId = null, Guid? adminUserId = null, string metadataJson = "{}")
        {
            var auditLog = new AuditLog(action, entityType, entityId, adminUserId, metadataJson);
            _logs.Add(auditLog);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[AUDIT-LOG] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | Ação: {action} | Entidade: {entityType} | ID: {entityId} | Meta: {metadataJson}");
            Console.ResetColor();
        }
    }
}
