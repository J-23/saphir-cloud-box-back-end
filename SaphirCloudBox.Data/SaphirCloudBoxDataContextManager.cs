using Anthill.Common.Data;
using Microsoft.AspNetCore.Identity;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace SaphirCloudBox.Data
{
    public class SaphirCloudBoxDataContextManager : 
        AbstractDataContextManager<SaphirCloudBoxDataContext, User, IdentityRole<int>, int>, ISaphirCloudBoxDataContextManager
    {
        public SaphirCloudBoxDataContextManager(IUnityContainer container) 
            : base(container)
        {
        }
    }
}
