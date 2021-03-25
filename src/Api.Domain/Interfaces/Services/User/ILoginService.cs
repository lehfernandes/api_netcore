using System.Threading.Tasks;
using Api.Data.Dtos;

namespace Api.Domain.Interfaces.Services.User
{
    public interface ILoginService
    {
         Task<object> FindByLogin(LoginDto user);
    }
}
