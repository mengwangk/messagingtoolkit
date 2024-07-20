using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.OneD.Rss
{

    internal class DataCharacter
    {

        private readonly int val;
        private readonly int checksumPortion;

        public DataCharacter(int value, int checksumPortion)
        {
            this.val = value;
            this.checksumPortion = checksumPortion;
        }

        public int Value
        {
            get
            {
                return val;
            }
        }

        public int ChecksumPortion
        {
            get
            {
                return checksumPortion;
            }
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        override public String ToString()
        {
            return Value + "(" + ChecksumPortion + ')';
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        override public bool Equals(Object o)
        {
            if (!(o is DataCharacter))
            {
                return false;
            }
            DataCharacter that = (DataCharacter)o;
            return Value == that.Value && ChecksumPortion == that.ChecksumPortion;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        override public int GetHashCode()
        {
            return Value ^ ChecksumPortion;
        }
    }
}