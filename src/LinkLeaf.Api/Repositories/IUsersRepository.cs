using LinkLeaf.Api.Models;

namespace LinkLeaf.Api.Repositories;

public interface IUsersRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> FindByEmail(string email);
    Task<User?> AddAsync(User user);
    Task RemoveAsync(Guid id);
}
