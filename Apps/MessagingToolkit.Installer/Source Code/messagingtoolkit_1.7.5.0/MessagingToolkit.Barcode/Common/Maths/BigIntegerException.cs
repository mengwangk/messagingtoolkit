using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Common.Maths
{
    /// <summary>
    /// BigInteger-related exception class.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class BigIntegerException : Exception
    {
        /// <summary>
        /// BigIntegerException constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public BigIntegerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
