using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Contracts.Menus
{
    public interface IAccessMenusRepository : IEntityFrameworkRepository<UserAccessMenus>
    {
        public Task<List<UserAccessMenus>> GetUserAccessMenusByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
