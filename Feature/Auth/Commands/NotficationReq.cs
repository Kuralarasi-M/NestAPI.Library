using FirebaseAdmin.Messaging;
using LibraryApp.Data;
using LibraryApp.Feature.Auth.Repository.Interface;
using LibraryApp.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Feature.Auth.Commands
{
    public class NotficationReq:IRequest<IEnumerable<NotificationRes>>
    {

    }
    public class NotificationRes
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class NotificationHandler : IRequestHandler<NotficationReq, IEnumerable<NotificationRes>>
    {
        private readonly AppDBContext _dbContext;
        public NotificationHandler(AppDBContext _dbContext)
        {
            this._dbContext= _dbContext;
        }
        public async Task<IEnumerable<NotificationRes>> Handle(NotficationReq query, CancellationToken cancellationToken)
        {
           
            var notifications = await _dbContext.Notifications
    .Where(x => x.UserId == 1)
    .Select(x => new NotificationRes
    {
        Id = x.Id,
        Title = x.Title,
        Message = x.Message,
        IsRead = x.IsRead,
        CreatedAt = x.CreatedAt
    })
    .ToListAsync();

            return notifications;
        }
    }
}
