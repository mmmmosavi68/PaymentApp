using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Student;
using PaymentApp.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentApp.Data.Contracts.Students
{
    public interface IStudentOtpRepository : IEntityFrameworkRepository<StudentOtp>
    {
        Task<StudentOtp> GetValidOtpAsync(int student_ID, string code, CancellationToken cancellationToken);
        Task InvalidatePreviousOtpsAsync(int studentId,CancellationToken cancellationToken);
        Task<int> CountOtpByStudentSinceAsync(int studentId, DateTime since, CancellationToken ct);
    }
}
