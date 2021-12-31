/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

namespace magic.node.contracts
{
    /// <summary>
    /// Common interface for creating services dynamically without having to resort to
    /// the service locator anti-pattern.
    /// </summary>
    public interface IServiceCreator<out T> where T : class
    {
        /// <summary>
        /// Creates a new service for you of the specified type.
        /// </summary>
        /// <returns>The newly created service.</returns>
        T Create();
    }
}
