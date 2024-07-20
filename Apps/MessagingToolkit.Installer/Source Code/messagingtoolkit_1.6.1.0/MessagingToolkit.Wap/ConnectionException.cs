using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Wap
{
    /// <summary>
    /// Socket exception
    /// </summary>
    [global::System.Serializable]
    public class ConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new object of SocketException class.
        /// </summary>
        public ConnectionException()
        {            
        }

        /// <summary>
        /// Initializes a new object of SocketException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new object of SocketException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public ConnectionException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new object of SocketException class.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:Exception.HResult"/> is zero (0).
        /// </exception>
        protected ConnectionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

