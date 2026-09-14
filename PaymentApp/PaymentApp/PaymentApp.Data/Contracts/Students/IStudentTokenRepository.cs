using PaymentApp.Data.Contracts.Common;
using PaymentApp.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PaymentApp.Entities.User;

namespace PaymentApp.Data.Contracts.Students
{
    public interface IStudentTokenRepository : IEntityFrameworkRepository<StudentToken>
    {
        Task<List<StudentToken>> GetUserStudentTokensByStudentIdAsync(int studentId, CancellationToken cancellationToken);
        Task AddStudentTokenAsync(StudentToken token, CancellationToken cancellationToken, bool saveNow = true);
        Task DeleteStudentTokensByStudentIdAsync(int studentId, CancellationToken cancellationToken);
    }
}
