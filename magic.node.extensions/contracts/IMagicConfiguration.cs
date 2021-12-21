/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

namespace magic.node.contracts
{
    /// <summary>
    /// Interface wrapping configuration settings for Magic.
    /// </summary>
    public interface IMagicConfiguration
    {
        /// <summary>
        /// Returns the specified key as a string value.
        /// </summary>
        /// <value>The string representation value of the specified key.</value>
        string this [string key] { get; set; }
    }
}
