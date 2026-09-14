using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Menu;
using PaymentApp.Entities.SiteSetting;
using PaymentApp.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Contracts.SiteSetting
{
    public interface ILoginSettingsRepository : IEntityFrameworkRepository<LoginSettings>
    {
        Task<LoginSettings> GetActiveLoginSetting(CancellationToken cancellationToken);
    }
}
