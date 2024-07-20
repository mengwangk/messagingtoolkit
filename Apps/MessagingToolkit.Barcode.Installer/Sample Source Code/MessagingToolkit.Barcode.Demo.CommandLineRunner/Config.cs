using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MessagingToolkit.Barcode.Demo.CommandLineRunner
{
    class Config
    {
        public Config()
        {
           

        }
        public Dictionary<DecodeOptions, object> DecodeOptions
        {
            get;
            set;
        }

        public bool TryHarder
        {
            get;
            set;
        }

        public bool PureBarcode
        {
            get;
            set;
        }

        public bool ProductsOnly
        {
            get;
            set;
        }

        public bool DumpResults
        {
            get;
            set;
        }

        public bool DumpBlackPoint
        {
            get;
            set;
        }


        public bool Multi
        {
            get;
            set;
        }


        public bool Brief
        {
            get;
            set;
        }

        public bool Recursive
        {
            get;
            set;
        }

        public int[] Crop
        {
            get;
            set;
        }

    }
}
