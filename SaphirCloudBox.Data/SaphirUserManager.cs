using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data
{
    public class SaphirUserManager : UserManager<User>
    {
        private readonly SaphirUserStore _saphirUserStore;

        public SaphirUserManager(SaphirUserStore store, IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, 
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _saphirUserStore = store;
        }

        public async Task<IEnumerable<User>> FindByClientIds(IEnumerable<int> clientIds)
        {
            return await _saphirUserStore.FindByClientIds(clientIds);
        }

        public async Task<IEnumerable<User>> FindByIds(IEnumerable<int> userIds)
        {
            return await _saphirUserStore.FindByIds(userIds);
        }
    }
}
