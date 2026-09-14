using PaymentApp.Data.Contracts.Common;

namespace PaymentApp.Data.Contracts.Groups
{
    public interface IUserGroupRepository : IEntityFrameworkRepository<PaymentApp.Entities.Group.GroupUsers>
    {
    }
}
