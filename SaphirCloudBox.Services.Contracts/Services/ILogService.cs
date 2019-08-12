using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface ILogService
    {
        Task Add(LogType logType, string text);
    }
}
