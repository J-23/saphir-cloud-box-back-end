using Anthill.Common.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Repositories
{
    public class ClientRepository : AbstractRepository<User, Role, int>, IClientRepository
    {
        public ClientRepository(SaphirCloudBoxDataContext context) 
            : base(context)
        {
        }

        public async Task Add(Client client)
        {
            await Context.Set<Client>()
                .AddAsync(client);

            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Client>> GetAll()
        {
            return await Context.Set<Client>()
                .OrderByDescending(ord => ord.CreateDate)
                .ThenByDescending(ord => ord.UpdateDate)
                .ToListAsync();
        }

        public async Task<Client> GetById(int clientId)
        {
            return await Context.Set<Client>().FirstOrDefaultAsync(x => x.Id == clientId);
        }

        public async Task<Client> GetByName(string clientName)
        {
            return await Context.Set<Client>().FirstOrDefaultAsync(x => x.Name.Equals(clientName));
        }

        public async Task Remove(Client client)
        {
            Context.Set<Client>().Remove(client);
            await Context.SaveChangesAsync();
        }

        public async Task Update(Client client)
        {
            Context.Entry(client).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }
}
