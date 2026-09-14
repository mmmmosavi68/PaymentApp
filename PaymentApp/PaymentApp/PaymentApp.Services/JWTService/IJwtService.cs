using PaymentApp.Entities.User;
using System.Threading.Tasks;

namespace PaymentApp.Services.JWTService
{
    public interface IJwtService
    {
        Task<string> GenerateAsync(User user, UserToken tokens);
    }
}