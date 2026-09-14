using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Data.Contracts.Menus
{
    public interface ICodeRepository : IEntityFrameworkRepository<Code>
    {
        public Task<List<Code>> GetAllCodesAsync(CancellationToken cancellationToken);
        public Task<List<Code>> GetAllActiveCodesAsync(CancellationToken cancellationToken);

    }
}