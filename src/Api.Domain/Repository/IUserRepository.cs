using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
         Task<UserEntity> FindByLogin(string email);
    }
}
