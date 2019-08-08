using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthill.Common.Data.Contracts
{
    /// <summary>
    /// This is a handler to an actual data context.
    /// It is used to perform different actions on the DataContext without exposing a context outside the Data Access Layer.
    /// </summary>
    public class ContextHandler
    {
        /// <summary>
        /// Stores the contextId associated with the data context.
        /// </summary>
        private readonly string _contextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextHandler" /> class.
        /// </summary>
        public ContextHandler(string contextId)
        {
            _contextId = contextId;
        }

        /// <summary>
        /// Gets the current contextId associated with the data context.
        /// </summary>
        public string ContextId
        {
            get { return _contextId; }
        }
    }
}
