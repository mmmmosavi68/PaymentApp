using PaymentApp.Data.Contracts.Groups;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Common;
using PaymentApp.Entities.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApp.Data.Repositories.Groups
{
    public class UserGroupRepository : EntityFrameworkRepository<GroupUsers>, IUserGroupRepository, IScopedDependency
    {
        public UserGroupRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
