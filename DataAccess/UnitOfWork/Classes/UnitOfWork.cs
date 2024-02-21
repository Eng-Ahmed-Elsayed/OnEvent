using DataAccess.Data;
using DataAccess.UnitOfWork.Interfaces;
using Models.Models;

namespace DataAccess.UnitOfWork.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private GenericRepository<EmailCraft> _emailModelRepository;
        private GenericRepository<Event> _eventRepository;
        private GenericRepository<Guest> _guestRepository;
        private GenericRepository<Invitation> _invitationRepository;
        private GenericRepository<Logistics> _logisticsRepository;
        private GenericRepository<Notification> _notificationRepository;
        private GenericRepository<RSVP> _RSVPRepository;

        public UnitOfWork(ApplicationDbContext Context)
        {
            _context = Context;
        }

        /// <summary>
        /// Lazy instantiation for ISortHelper.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private ISortHelper<T> GetSortHelper<T>()
        {
            // Implement logic to return appropriate ISortHelper instance based on the type T
            if (typeof(T) == typeof(EmailCraft))
            {
                return (ISortHelper<T>)new SortHelper<EmailCraft>();
            }
            else if (typeof(T) == typeof(Event))
            {
                return (ISortHelper<T>)new SortHelper<Event>();
            }
            else if (typeof(T) == typeof(Guest))
            {
                return (ISortHelper<T>)new SortHelper<Guest>();
            }
            else if (typeof(T) == typeof(Invitation))
            {
                return (ISortHelper<T>)new SortHelper<Invitation>();
            }
            else if (typeof(T) == typeof(Logistics))
            {
                return (ISortHelper<T>)new SortHelper<Logistics>();
            }
            else if (typeof(T) == typeof(Notification))
            {
                return (ISortHelper<T>)new SortHelper<Notification>();
            }
            else if (typeof(T) == typeof(RSVP))
            {
                return (ISortHelper<T>)new SortHelper<RSVP>();
            }
            else
            {
                throw new ArgumentException($"Sort helper for type {typeof(T)} is not supported.");
            }
        }

        public GenericRepository<EmailCraft> EmailModelRepository
        {
            get
            {
                if (_emailModelRepository == null)
                {
                    _emailModelRepository = new GenericRepository<EmailCraft>(_context, GetSortHelper<EmailCraft>());
                }
                return _emailModelRepository;
            }
        }
        public GenericRepository<Event> EventRepository
        {
            get
            {
                if (_eventRepository == null)
                {

                    _eventRepository = new(_context, GetSortHelper<Event>());
                }
                return _eventRepository;
            }
        }
        public GenericRepository<Guest> GuestRepository
        {
            get
            {
                if (_guestRepository == null)
                {
                    _guestRepository = new(_context, GetSortHelper<Guest>());
                }
                return _guestRepository;
            }
        }
        public GenericRepository<Invitation> InvitationRepository
        {
            get
            {
                if (_invitationRepository == null)
                {
                    _invitationRepository = new(_context, GetSortHelper<Invitation>());
                }
                return _invitationRepository;
            }
        }
        public GenericRepository<Logistics> LogisticsRepository
        {
            get
            {
                if (_logisticsRepository == null)
                {
                    _logisticsRepository = new(_context, GetSortHelper<Logistics>());
                }
                return _logisticsRepository;
            }
        }
        public GenericRepository<Notification> NotificationRepository
        {
            get
            {
                if (_notificationRepository == null)
                {
                    _notificationRepository = new(_context, GetSortHelper<Notification>());
                }
                return _notificationRepository;
            }
        }
        public GenericRepository<RSVP> RSVPRepository
        {
            get
            {
                if (_RSVPRepository == null)
                {
                    _RSVPRepository = new(_context, GetSortHelper<RSVP>());
                }
                return _RSVPRepository;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await _context.DisposeAsync();
                }
            }
            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

    }
}
