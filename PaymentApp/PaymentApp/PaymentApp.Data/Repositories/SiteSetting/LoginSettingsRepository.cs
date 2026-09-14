using Microsoft.EntityFrameworkCore;
using PaymentApp.Data.Contracts.SiteSetting;
using PaymentApp.Data.Contracts.Users;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Common;
using PaymentApp.Entities.SiteSetting;
using PaymentApp.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Repositories.SiteSetting
{
    public class LoginSettingsRepository : EntityFrameworkRepository<LoginSettings>, ILoginSettingsRepository, IScopedDependency
    {
        public LoginSettingsRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<LoginSettings> GetActiveLoginSetting(CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.IsActive == true && p.IsDeleted == false)
              .SingleOrDefaultAsync(cancellationToken)
              .ConfigureAwait(false);
        }
    }
}