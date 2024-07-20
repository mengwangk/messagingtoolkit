using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417
{
    public sealed class Pdf417ResultMetadata
    {

        private int segmentIndex;
        private string fileId;
        private int[] optionalData;
        private bool lastSegment;

        public int SegmentIndex
        {
            get
            {
                return segmentIndex;
            }
            set
            {
                this.segmentIndex = value;
            }
        }


        public string FileId
        {
            get
            {
                return fileId;
            }
            set
            {
                this.fileId = value;
            }
        }


        public int[] OptionalData
        {
            get
            {
                return optionalData;
            }
            set
            {
                this.optionalData = value;
            }
        }


        public bool LastSegment
        {
            get
            {
                return lastSegment;
            }
            set
            {
                this.lastSegment = value;
            }
        }
    }
}
