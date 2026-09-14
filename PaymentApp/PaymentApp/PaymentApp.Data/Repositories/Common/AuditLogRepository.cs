using Microsoft.EntityFrameworkCore;
using PaymentApp.Common;
using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentApp.Data.Repositories.Common
{
    public class AuditLogRepository : EntityFrameworkRepository<AuditLog>, IAuditLogRepository, IScopedDependency
    {
        public AuditLogRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .OrderByDescending(a => a.OperationTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<AuditLog> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Table
                .FirstOrDefaultAsync(a => a.AuditLogId == id, cancellationToken);
        }

        public async Task AddLogAsync(AuditLog auditLog, CancellationToken cancellationToken)
        {
            await AddAsync(auditLog, cancellationToken);
        }
    }
}