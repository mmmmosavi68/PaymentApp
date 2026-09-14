using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Contracts.Users
{
    public interface IUserOtpRepository : IEntityFrameworkRepository<UserOtp>
    {
        Task<UserOtp> GetValidOtpAsync(Guid user_ID, string code, CancellationToken cancellationToken);
    }
}
