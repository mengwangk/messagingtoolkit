using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.OneD.Rss
{

    public class DataCharacter
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

    }
}
