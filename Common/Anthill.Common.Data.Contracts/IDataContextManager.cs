using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthill.Common.Data.Contracts
{

    public interface IDataContextManager
    {
        /// <summary>
        /// Factory method to create repositories.
        /// All the repositories will share the same context specified by the 'handler' if one is passed as a parameter.
        /// If no 'handler' is specified all the repositories will share the same 'default' context.
        /// If a 'handler' is specified then this repository will be created in the specified context.
        /// </summary>
        TRepository CreateRepository<TRepository>(ContextHandler handler = null)
            where TRepository : IRepository;


        /// <summary>
        /// Creates a new data context and returns a handler to it.
        /// This handler can then be used to execute other actions or to create repositories on this context.
        /// </summary>
        ContextHandler GetNewContext();

        /// <summary>
        /// Saves all the data modified in all the contexts tracked by the current IDataContextManager.
        /// </summary>
        void Save(ContextHandler handler = null);

        /// <summary>
        /// Saves all the data modified in all the contexts tracked by the current IDataContextManager.
        /// </summary>
        Task SaveAsync(ContextHandler handler = null);
    }
}
