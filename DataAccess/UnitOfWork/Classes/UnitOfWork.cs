using DataAccess.Data;
using DataAccess.UnitOfWork.Interfaces;
using Models.Models;

namespace DataAccess.UnitOfWork.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private GenericRepository<EmailModel> _emailModelRepository;
        private ISortHelper<EmailModel> _emailModelSortHelper;

        private GenericRepository<Event> _eventRepository;
        private ISortHelper<Event> _eventSortHelper;

        private GenericRepository<Guest> _guestRepository;
        private ISortHelper<Guest> _guestSortHelper;

        private GenericRepository<Invitation> _invitationRepository;
        private ISortHelper<Invitation> _invitationSortHelper;

        private GenericRepository<Logistics> _logisticsRepository;
        private ISortHelper<Logistics> _logisticsSortHelper;

        private GenericRepository<Notification> _notificationRepository;
        private ISortHelper<Notification> _notificationSortHelper;

        private GenericRepository<RSVP> _RSVPRepository;
        private ISortHelper<RSVP> _RSVPSortHelper;

        public UnitOfWork(ApplicationDbContext Context)
        {
            _context = Context;
        }

        public GenericRepository<EmailModel> EmailModelRepository
        {
            get
            {
                if (_emailModelRepository == null)
                {
                    _emailModelRepository = new GenericRepository<EmailModel>(_context, _emailModelSortHelper);
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
                    _eventRepository = new(_context, _eventSortHelper);
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
                    _guestRepository = new(_context, _guestSortHelper);
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
                    _invitationRepository = new(_context, _invitationSortHelper);
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
                    _logisticsRepository = new(_context, _logisticsSortHelper);
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
                    _notificationRepository = new(_context, _notificationSortHelper);
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
                    _RSVPRepository = new(_context, _RSVPSortHelper);
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
