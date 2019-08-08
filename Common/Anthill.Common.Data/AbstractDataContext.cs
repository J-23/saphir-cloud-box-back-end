using Anthill.Common.Data.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthill.Common.Data
{
    /// <summary>
    /// This is a wrapper for EF <see cref="DbContext" /> class.
    /// Must be inherited in order to define all the entities tracked by this context.
    /// </summary>
    public abstract class AbstractDataContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey> 
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IConnectionConfiguration _configuration;

        /// <summary>
        /// Stores the generated contextId. This will be used to match a ContextHandler to its DataContext.
        /// </summary>
        private readonly string _contextId;


        /// <summary>
        /// This is true if the DataContext has been disposed.
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext" /> class.
        /// </summary>
        protected AbstractDataContext(IConnectionConfiguration configuration)
            : base()
        {
            _configuration = configuration;
            _contextId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Stores the generated contextId. This will be used to match a ContextHandler to its DataContext.
        /// </summary>
        public string ContextId
        {
            get { return _contextId; }
        }

        /// <summary>
        /// This is true if the DataContext has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString());
            }
        }
    }
}
