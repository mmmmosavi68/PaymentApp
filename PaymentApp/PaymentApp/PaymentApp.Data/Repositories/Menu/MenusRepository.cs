using Microsoft.EntityFrameworkCore;
using PaymentApp.Data.Contracts.Menus;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Common;
using PaymentApp.Entities.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Repositories.Menus
{
    public class MenusRepository : EntityFrameworkRepository<Entities.Menu.Menu>, IMenusRepository, IScopedDependency
    {
        public MenusRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<Entities.Menu.Menu>> GetAllActiveMenusAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(ll => ll.IsDeleted == false)
                .Include(rr => rr.MenusAccesses)
                .OrderBy(ll => ll.PageCode) // مرتب‌سازی براساس CreatedDate به‌صورت نزولی
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Entities.Menu.Menu>> GetAllMenusAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                          .Where(ll => ll.IsDeleted == false)
                          .Include(rr => rr.MenusAccesses)
                          .OrderBy(ll => ll.PageCode) // مرتب‌سازی براساس CreatedDate به‌صورت نزولی
                          .ToListAsync(cancellationToken);
        }
    }
}
