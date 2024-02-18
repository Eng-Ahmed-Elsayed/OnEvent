using DataAccess.UnitOfWork.Classes;
using Models.Models;

namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task SaveChangesAsync();
        GenericRepository<EmailModel> EmailModelRepository { get; }
        GenericRepository<Event> EventRepository { get; }
        GenericRepository<Guest> GuestRepository { get; }
        GenericRepository<Invitation> InvitationRepository { get; }
        GenericRepository<Logistics> LogisticsRepository { get; }
        GenericRepository<Notification> NotificationRepository { get; }
        GenericRepository<RSVP> RSVPRepository { get; }
    }
}
