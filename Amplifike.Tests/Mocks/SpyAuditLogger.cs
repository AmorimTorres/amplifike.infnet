using System;
using System.Collections.Generic;
using Amplifike.Interfaces;

namespace Amplifike.Tests.Mocks
{
    public struct AuditLogCall
    {
        public string Action { get; }
        public string EntityType { get; }
        public Guid? EntityId { get; }
        public Guid? AdminUserId { get; }
        public string MetadataJson { get; }

        public AuditLogCall(string action, string entityType, Guid? entityId, Guid? adminUserId, string metadataJson)
        {
            Action = action;
            EntityType = entityType;
            EntityId = entityId;
            AdminUserId = adminUserId;
            MetadataJson = metadataJson;
        }
    }

    /*
     * =================================================================================
     * PRINCÍPIO DE TESTE UNITÁRIO: AUTO-VERIFICAÇÃO (SELF-VALIDATING)
     * =================================================================================
     * NOME: Auto-verificação (Self-Validating)
     * CONCEITO: O teste unitário deve ser capaz de determinar de forma totalmente 
     *           automatizada e binária (Pass/Fail) se o comportamento do código sob teste
     *           está correto, sem depender de inspeção manual visual de logs em tela.
     * APLICAÇÃO:
     * A classe 'SpyAuditLogger' é um Dublê de Teste (Test Double) do tipo SPY.
     * Ela captura as chamadas de auditoria recebidas e as armazena na lista pública 'RecordedCalls'.
     * Isso permite que os métodos de asserção nos testes unitários verifiquem programaticamente
     * se as chamadas de auditoria ocorreram com as ações corretas (ex: 'PLEDGE_PAID'),
     * garantindo a auto-verificação automática do fluxo.
     * =================================================================================
     */
    public class SpyAuditLogger : IAuditLogger
    {
        public List<AuditLogCall> RecordedCalls { get; } = new();

        public void Log(string action, string entityType, Guid? entityId = null, Guid? adminUserId = null, string metadataJson = "{}")
        {
            RecordedCalls.Add(new AuditLogCall(action, entityType, entityId, adminUserId, metadataJson));
        }

        public bool HasLogged(string action, Guid entityId)
        {
            return RecordedCalls.Exists(call => call.Action == action && call.EntityId == entityId);
        }
    }
}
