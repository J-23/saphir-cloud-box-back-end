using Anthill.Common.Data;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Repositories
{
    public class NotificationRepository : AbstractRepository<User, Role, int>, INotificationRepository
    {
        public NotificationRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public async Task Add(Notification notification)
        {
            await Context.Set<Notification>()
                .AddAsync(notification);

            await Context.SaveChangesAsync();
        }
    }
}
