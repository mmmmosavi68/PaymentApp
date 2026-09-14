using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Payment;


namespace PaymentApp.Data.Contracts.Payment
{
    public interface IExamRepository : IEntityFrameworkRepository<Exam>
    {
    }
}
