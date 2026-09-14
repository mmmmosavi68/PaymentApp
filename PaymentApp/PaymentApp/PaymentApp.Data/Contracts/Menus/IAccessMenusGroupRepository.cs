using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Contracts.Menu
{
    public interface IAccessMenusGroupRepository : IEntityFrameworkRepository<GroupAccessMenus>
    {
        public Task<List<GroupAccessMenus>> GetGroupAccessMenusByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    }
}
