using PaymentApp.Common;
using PaymentApp.Data.Contracts.Payment;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Entities.Payment;

namespace PaymentApp.Data.Repositories.Payment
{
    public class ExamRepository : EntityFrameworkRepository<Exam>, IExamRepository, IScopedDependency
    {
        public ExamRepository(ApplicationDbContext context) : base(context) { }
    }
}
