using Microsoft.EntityFrameworkCore;
using PaymentApp.Data.Contracts.Users;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Common;
using PaymentApp.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Repositories.Users
{
    public class UserRoleRepository : EntityFrameworkRepository<UserRole>, IUserRoleRepository, IScopedDependency
    {
        public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public UserRole GetUserRoleByUserId(Guid userId)
        {
            return TableNoTracking.Where(ur => ur.IsDeleted == false && ur.UserId == userId).FirstOrDefault();
        }
    }
}
