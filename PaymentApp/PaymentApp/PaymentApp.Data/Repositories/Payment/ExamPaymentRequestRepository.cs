using PaymentApp.Common;
using PaymentApp.Data.Contracts.Payment;
using PaymentApp.Data.Repositories.Common;
using PaymentApp.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApp.Data.Repositories.Payment
{
    public class ExamPaymentRequestRepository : EntityFrameworkRepository<ExamPaymentRequest>, IExamPaymentRequestRepository, IScopedDependency
    {
        public ExamPaymentRequestRepository(ApplicationDbContext context) : base(context) { }
    }
}
