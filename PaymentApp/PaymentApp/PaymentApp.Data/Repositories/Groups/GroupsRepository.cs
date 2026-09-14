using Microsoft.EntityFrameworkCore;
using PaymentApp.Data.Contracts.Groups;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Common;
using PaymentApp.Entities.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PaymentApp.Data.Repositories.Groups
{
    public class GroupRepository : EntityFrameworkRepository<Group>, IGroupRepository, IScopedDependency
    {
        public GroupRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public IReadOnlyList<Group> GetAllGroups()
        {
            //        return TableNoTracking.Include(ur => ur.UserRoles)
            //                    .ThenInclude(r => r.Role)
            //                    .Include(up => up.UserProfiles)
            //                    .Where(u => u.IsDeleted == false)
            //                    .ToList()
            //.ConfigureAwait(false);

            return TableNoTracking
                    .Where(g => g.IsDeleted != true)
                    .OrderByDescending(g => g.CreatedDate) // مرتب‌سازی براساس CreatedDate به‌صورت نزولی
                    .Include(g => g.Creator)
                    .ToList();
        }
    }
}
