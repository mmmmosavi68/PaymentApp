using Microsoft.EntityFrameworkCore;
using PaymentApp.Data.Contracts.Menu;
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

namespace PaymentApp.Data.Repositories.Menus
{
    public class AccessMenusGroupRepository : EntityFrameworkRepository<GroupAccessMenus>, IAccessMenusGroupRepository, IScopedDependency
    {
        public AccessMenusGroupRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<GroupAccessMenus>> GetGroupAccessMenusByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return await TableNoTracking.Where(u => u.GroupId.Equals(groupId)).ToListAsync(cancellationToken);
        }
    }
}
