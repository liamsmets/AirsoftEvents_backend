using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using AirsoftEvents.Domain.Models.Enums;
using System.Net;
using System.Security.Cryptography;

namespace AirsoftEvents.Persistance;

public class FieldRepo(AirsoftEventsAppDbContext dbContext): IFieldRepo
{
    public async Task<List<Field>> GetAllAsync()
    {
        return await dbContext.Fields.ToListAsync();
    }
    public async Task<Field?> GetByIdAsync(Guid id)
    {
        return await dbContext.Fields.FirstOrDefaultAsync(f => f.Id == id);

    }
    public async Task<List<Field>> GetApprovedFieldsAsync()
    {
        return await dbContext.Fields.Where(f => f.Status == FieldStatus.Approved).ToListAsync();
    }
    public async Task<Field> AddAsync(Field fieldToAdd)
    {
        await dbContext.Fields.AddAsync(fieldToAdd);
        await dbContext.SaveChangesAsync();
        return fieldToAdd;
    }
    public async Task UpdateAsync(Field fieldToUpdate)
    {
        dbContext.Fields.Update(fieldToUpdate);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var fieldToDelete = await dbContext.Fields.FindAsync(id);
        if(fieldToDelete != null)
        {
            dbContext.Fields.Remove(fieldToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}