using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Group;
using System.Collections.Generic;

namespace PaymentApp.Data.Contracts.Groups
{
    public interface IGroupRepository : IEntityFrameworkRepository<Group>
    {
        IReadOnlyList<Group> GetAllGroups();
    }

}
