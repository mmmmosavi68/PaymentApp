using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.User;
using System;

namespace PaymentApp.Data.Contracts.Users
{
    public interface IUserRoleRepository : IEntityFrameworkRepository<UserRole>
    {
        public UserRole GetUserRoleByUserId(Guid userId);
    }
}
