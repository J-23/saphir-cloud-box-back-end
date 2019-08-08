using Anthill.Common.Data.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace Anthill.Common.Data
{
    /// <summary>
    /// Abstract class that creates repositories and data contexts.
    /// </summary>
    public abstract class AbstractDataContextManager<TContext, TUser, TRole, TKey> : IDataContextManager
        where TContext : AbstractDataContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Keeps a list of all created repositories.
        /// </summary>
        protected List<IRepository> _repositories = new List<IRepository>();

        /// <summary>
        /// Keeps a list of all created contexts.
        /// </summary>
        private List<TContext> _contexts = new List<TContext>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextManager{TContext}" /> class.
        /// </summary>
        protected AbstractDataContextManager(IUnityContainer container)
        {
            Container = container;
        }


        /// <summary>
        /// Returns the IoC container.
        /// </summary>
        protected IUnityContainer Container { get; private set; }

        /// <summary>
        /// Creates a new data context and return a handler to this.
        /// The handler can be used to execute different operations on the context (create repository, enable logging,...).
        /// </summary>
        public ContextHandler GetNewContext()
        {
            var dataContext = CreateContext();
            _contexts.Add(dataContext);

            return new ContextHandler(dataContext.ContextId);
        }

        /// <summary>
        /// Creates a new repository by using the default context or by using a context specified by the handler.
        /// If multiple repository must be built using the same context then the same context handler must be sent.
        /// If a data context is not yet being tracked by the DataContextManager, one will be created.
        /// </summary>
        public virtual TRepository CreateRepository<TRepository>(ContextHandler handler = null)
            where TRepository : IRepository
        {
            var dataContext = GetContext(handler);

            var repository = Container.Resolve<TRepository>(new ParameterOverride("context", dataContext));
            _repositories.Add(repository);

            return repository;
        }

        /// <summary>
        /// Saves all the changes across all the contexts.
        /// </summary>
        public void Save(ContextHandler handler = null)
        {
            if (handler == null)
            {
                foreach (var context in _contexts)
                {
                    context.SaveChanges();
                }
            }
            else
            {
                var context = GetContext(handler);
                context.SaveChanges();
            }

        }

        /// <summary>
        /// Saves all the changes across all the contexts.
        /// </summary>
        public async Task SaveAsync(ContextHandler handler = null)
        {
            if (handler == null)
            {
                foreach (var context in _contexts)
                {
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                var context = GetContext(handler);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// This is part of the Dispose pattern.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This is part of the Dispose pattern.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var context in _contexts)
                {
                    if (!context.IsDisposed)
                    {
                        context.Dispose();
                    }
                }

                _contexts = new List<TContext>();
                _repositories = new List<IRepository>();
            }
        }

        /// <summary>
        /// Creates or return either the default context or a context specified by the handler.
        /// If no context if found for the specified handler the a DataContextNotFoundException is raised.
        /// </summary>
        protected TContext GetContext(ContextHandler handler = null)
        {
            TContext dataContext;

            if (handler == null)
            {
                dataContext = _contexts.FirstOrDefault();

                if (dataContext == null)
                {
                    dataContext = CreateContext();
                    _contexts.Add(dataContext);
                }
            }
            else
            {
                dataContext = _contexts.FirstOrDefault(c => c.ContextId == handler.ContextId);

                if (dataContext == null)
                {
                    throw new Exception("Data Context was not found");
                }
            }

            return dataContext;
        }

        /// <summary>
        /// Creates either a default context or any other context if no default is specified.
        /// </summary>
        protected virtual TContext CreateContext()
        {
            var context = Container.Resolve<TContext>();
            return context;
        }
    }
}
