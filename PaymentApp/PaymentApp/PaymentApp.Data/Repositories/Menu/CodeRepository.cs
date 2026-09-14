using Microsoft.EntityFrameworkCore;
using PaymentApp.Data.Contracts.Menus;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Common;
using PaymentApp.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Repositories.Menu
{
    public class CodeRepository : EntityFrameworkRepository<Code>, ICodeRepository, IScopedDependency
    {
        public CodeRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<Code>> GetAllActiveCodesAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(ll => ll.IsDeleted == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Code>> GetAllCodesAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                          .Where(ll => ll.IsDeleted == false)
                          .ToListAsync(cancellationToken);
        }
    }
}
