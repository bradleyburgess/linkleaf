using LinkLeaf.Api.Data;
using LinkLeaf.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkLeaf.Api.Repositories;

public class UsersRepository(AppDbContext dbContext) : IUsersRepository
{
    private readonly AppDbContext _db = dbContext;

    public async Task<User?> AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return user;

    }

    public async Task<List<User>> GetAllAsync()
    {
        var users = await _db.Users.ToListAsync();
        return users;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> FindByEmail(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task RemoveAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is not null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}
