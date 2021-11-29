/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace magic.node.extensions
{
    /// <summary>
    /// Exception type thrown from Hyperlambda using [throw], and other parts,
    /// allowing you to return semantic errors to caller to explain details
    /// about what was wrong.
    /// </summary>
    [Serializable]
    public class HyperlambdaException : Exception, ISerializable
    {
        /// <summary>
        /// Creates a new instance of exception.
        /// </summary>
        public HyperlambdaException()
        {
            IsPublic = false;
            Status = 500;
            FieldName = "";
        }

        /// <summary>
        /// Creates a new instance of exception.
        /// </summary>
        /// <param name="message">Friendly message, that might or might not be returned back to client.</param>
        public HyperlambdaException(string message)
            : base(message)
        {
            IsPublic = false;
            Status = 500;
            FieldName = "";
        }

        /// <summary>
        /// Creates a new instance of exception
        /// </summary>
        /// <param name="message">Friendly message, that might or might not be returned back to client.</param>
        /// <param name="innerException">Inner exception</param>
        public HyperlambdaException(string message, Exception innerException)
            : base(message, innerException)
        {
            IsPublic = false;
            Status = 500;
            FieldName = "";
        }

        /// <summary>
        /// Constructs a new instance of a Hyperlambda exception.
        /// </summary>
        /// <param name="message">Exception error text.</param>
        /// <param name="isPublic">Whether or not exception message should propagate to client in release builds.</param>
        /// <param name="status">Status code returned to client.</param>
        public HyperlambdaException(string message, bool isPublic, int status)
            : base(message)
        {
            IsPublic = isPublic;
            Status = status;
            FieldName = "";
        }

        /// <summary>
        /// Constructs a new instance of a Hyperlambda exception.
        /// </summary>
        /// <param name="message">Exception error text.</param>
        /// <param name="isPublic">Whether or not exception message should propagate to client in release builds.</param>
        /// <param name="status">Status code returned to client.</param>
        /// <param name="fieldName">Field that triggered exception, if any.</param>
        public HyperlambdaException(string message, bool isPublic, int status, string fieldName)
            : base(message)
        {
            IsPublic = isPublic;
            Status = status;
            FieldName = fieldName;
        }

        /// <summary>
        /// Constructs a new instance of a Hyperlambda exception.
        /// </summary>
        /// <param name="message">Exception error text.</param>
        /// <param name="isPublic">Whether or not exception message should propagate to client in release builds.</param>
        /// <param name="status">Status code returned to client.</param>
        /// <param name="fieldName">Field that triggered exception, if any.</param>
        /// <param name="innerException">Inner exception</param>
        public HyperlambdaException(string message, bool isPublic, int status, string fieldName, Exception innerException)
            : base(message, innerException)
        {
            IsPublic = isPublic;
            Status = status;
            FieldName = fieldName;
        }

        /// <summary>
        /// Whether ot not exception will propagate to client in release builds.
        /// </summary>
        /// <value>Returns true if exception is visible to the client.</value>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Status code to return to client.
        /// </summary>
        /// <value>HTTP status code to return to client.</value>
        public int Status { get; set; }

        /// <summary>
        /// Name of field that triggered exception, if any.
        /// </summary>
        /// <value>Field name that triggered exception.</value>
        public string FieldName { get; set; }

        #region [ -- Serialization implementation -- ]

        /// <inheritdoc/>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected HyperlambdaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsPublic = (bool)info.GetValue("IsPublic", typeof(bool));
            this.Status = (int)info.GetValue("Status", typeof(int));
            this.FieldName = (string)info.GetValue("FieldName", typeof(string));
        }

        /// <inheritdoc/>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            info.AddValue("IsPublic", IsPublic, typeof(bool));
            info.AddValue("Status", Status, typeof(int));
            info.AddValue("FieldName", FieldName, typeof(string));
            base.GetObjectData(info, context);
        }

        #endregion
    }
}
