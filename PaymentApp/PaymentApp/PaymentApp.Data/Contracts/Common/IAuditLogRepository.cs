using PaymentApp.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentApp.Data.Contracts.Common
{
    public interface IAuditLogRepository : IEntityFrameworkRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken);
        Task<AuditLog> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddLogAsync(AuditLog auditLog, CancellationToken cancellationToken);
    }
}
