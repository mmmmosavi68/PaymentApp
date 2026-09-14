using AutoMapper;
using PaymentApp.Entities.User;

namespace WebFramework.CustomMapping
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile profile);
    }
}
