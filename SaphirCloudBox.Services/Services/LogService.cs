using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class LogService : AbstractService, ILogService
    {
        public LogService(IUnityContainer container, ISaphirCloudBoxDataContextManager dataContextManager) : base(container, dataContextManager)
        {
        }

        public void Add(LogType logType, string text)
        {
            var logRepository = DataContextManager.CreateRepository<ILogRepository>();

            var log = new Log
            {
                LogType = logType,
                Text = text
            };

            logRepository.Add(log);
        }
    }
}
