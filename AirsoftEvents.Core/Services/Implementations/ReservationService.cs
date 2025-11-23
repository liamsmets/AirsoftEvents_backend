using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Exceptions;
using AirsoftEvents.Core.Persistence.Interfaces;
using AirsoftEvents.Core.Services.Interfaces;
using AirsoftEvents.Core.Services.Mapping;

namespace AirsoftEvents.Core.Services.Implementations;

public class ReservationService : IReservationService
{
    private readonly IReservationRepo _reservationRepo;
    private readonly IEventRepo _eventRepo;

    public ReservationService(IReservationRepo reservationRepo, IEventRepo eventRepo)
    {
        _reservationRepo = reservationRepo;
        _eventRepo = eventRepo;
    }

    public async Task<ReservationResponseContract> CreateReservationAsync(ReservationRequestContract request)
    {
        var eventModel = await _eventRepo.GetByIdAsync(request.EventId);
        if (eventModel == null)
        {
            throw new KeyNotFoundException("Event niet gevonden");
        }

        var currentReservations = await _reservationRepo.GetByEventIdAsync(request.EventId);
        if (currentReservations.Count >= eventModel.MaxPlayers)
        {
            throw new CapacityExceededException("Sorry, dit event zit vol!");
        }

        var reservation = request.AsModel();
        reservation.Id = Guid.NewGuid();
        reservation.ReservedAt = DateTime.UtcNow;

        var created = await _reservationRepo.AddAsync(reservation);
        return created.AsContract();
    }
    public async Task<ReservationResponseContract?> GetReservationByIdAsync(Guid id)
    {
        var reservation = await _reservationRepo.GetByIdAsync(id);
        return reservation?.AsContract();
    }
}