using System;

namespace Amplifike.Domain
{
    public class AuditLog : BaseEntity
    {
        public Guid? AdminUserId { get; private set; }
        public string Action { get; private set; }
        public string EntityType { get; private set; }
        public Guid? EntityId { get; private set; }
        public string MetadataJson { get; private set; } // Representação JSON dos dados

        public AuditLog(string action, string entityType, Guid? entityId = null, Guid? adminUserId = null, string metadataJson = "{}")
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Ação auditada é obrigatória.", nameof(action));

            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("O tipo de entidade é obrigatório.", nameof(entityType));

            Action = action;
            EntityType = entityType;
            EntityId = entityId;
            AdminUserId = adminUserId;
            MetadataJson = metadataJson;
        }
    }
}
