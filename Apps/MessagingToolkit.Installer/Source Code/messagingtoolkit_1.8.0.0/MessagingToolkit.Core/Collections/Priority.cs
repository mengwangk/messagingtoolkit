using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Collections
{
    public sealed class Priority : IComparable<Priority>
    {
        public static Priority Low = new Priority(0);
        public static Priority Normal = new Priority(1);
        public static Priority High = new Priority(2);


        private Priority(int level)
        {
            this.Level = level;
        }

        public int Level
        {
            get;
            private set;
        }

        public int CompareTo(Priority other)
        {
            // If other is not a valid object reference, this instance is greater. 
            if (other == null) return 1;

            return other.Level.CompareTo(this.Level);
        }

    }
}
