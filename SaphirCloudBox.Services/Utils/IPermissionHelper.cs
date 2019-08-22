using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Utils
{
    public interface IPermissionHelper
    {
        Task<bool> IsAvailableToChange(FileStorage fileStorage, IEnumerable<FileStorage> childFileStorages, int userId, int clientId);

        Task<(int? OwnerId, int? ClientId)> GetOwners(FileStorage parentFileStorage, int userId, int userClientId);
    }
}
