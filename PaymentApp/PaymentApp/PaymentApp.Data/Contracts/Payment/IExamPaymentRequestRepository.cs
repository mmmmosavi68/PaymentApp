using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApp.Data.Contracts.Payment
{
    public interface IExamPaymentRequestRepository : IEntityFrameworkRepository<ExamPaymentRequest>
    {
    }
}
