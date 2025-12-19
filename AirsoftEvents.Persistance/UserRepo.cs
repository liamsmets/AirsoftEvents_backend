using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using AirsoftEvents.Domain.Models.Enums;
using System.Net;
using System.Security.Cryptography;

namespace AirsoftEvents.Persistance;

public class UserRepo(AirsoftEventsAppDbContext dbContext): IUserRepo
{
    public async Task<List<User>> GetAllAsync()
    {
        return await dbContext.Users.ToListAsync();
    }
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<User> AddAsync(User userToAdd)
    {
        await dbContext.Users.AddAsync(userToAdd);
        await dbContext.SaveChangesAsync();
        return userToAdd;
    }
    public async Task UpdateAsync(User userToUpdate)
    {
        dbContext.Users.Update(userToUpdate);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var userToDelete = await dbContext.Users.FindAsync(id);
        if(userToDelete != null)
        {
            dbContext.Users.Remove(userToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}