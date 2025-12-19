using Microsoft.EntityFrameworkCore;

namespace AirsoftEvents.Persistance.Entities;

public class AirsoftEventsAppDbContext: DbContext
{
    public AirsoftEventsAppDbContext(DbContextOptions<AirsoftEventsAppDbContext> options) : base(options) { }
    public virtual DbSet<Event> Events {get;set;}
    public virtual DbSet<Field> Fields {get;set;}
    public virtual DbSet<Reservation> Reservations {get;set;}
    public virtual DbSet<User> Users {get;set;}

}