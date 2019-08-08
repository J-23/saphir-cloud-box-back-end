namespace Anthill.Common.Data.Contracts
{
    public interface IConnectionConfiguration
    {
        /// <summary>
        /// Returns a connection string.
        /// The implementer should either return a connection string from a configuration file or it could be a mock object if is used for testing.
        /// </summary>
        string GetConnectionString();
    }
}
