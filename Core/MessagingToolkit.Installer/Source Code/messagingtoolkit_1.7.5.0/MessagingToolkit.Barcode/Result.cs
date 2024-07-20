using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode
{
	
	/// <summary> 
    /// Encapsulates the result of decoding a barcode within an image.	
	/// </summary>
	public sealed class Result
	{ 
        private readonly String text;
        private readonly byte[] rawBytes;
        private ResultPoint[] resultPoints;
        private readonly BarcodeFormat format;
        private Dictionary<ResultMetadataType, object> resultMetadata;
        private readonly long timestamp;

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        /// <returns> raw text encoded by the barcode, if applicable, otherwise <code>null</code>
        /// </returns>
		public string Text
		{
			get
			{
				return text;
			}
			
		}
        /// <summary>
        /// Gets the raw bytes.
        /// </summary>
        /// <value>The raw bytes.</value>
        /// <returns> raw bytes encoded by the barcode, if applicable, otherwise <code>null</code>
        /// </returns>
		public byte[] RawBytes
		{
			get
			{
				return rawBytes;
			}
			
		}
        /// <summary>
        /// Gets the result points.
        /// </summary>
        /// <value>The result points.</value>
        /// <returns> points related to the barcode in the image. These are typically points
        /// identifying finder patterns or the corners of the barcode. The exact meaning is
        /// specific to the type of barcode that was decoded.
        /// </returns>
		public ResultPoint[] ResultPoints
		{
			get
			{
				return resultPoints;
			}
			
		}
        /// <summary>
        /// Gets the barcode format.
        /// </summary>
        /// <value>The barcode format.</value>
        /// <returns> {@link BarcodeFormat} representing the format of the barcode that was decoded
        /// </returns>
        public BarcodeFormat BarcodeFormat
        {
            get
            {
                return format;
            }

        }

        /// <summary>
        /// Gets the result metadata.
        /// </summary>
        public Dictionary<ResultMetadataType, object> ResultMetadata
		{
			get
			{
				return resultMetadata;
			}
			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="rawBytes">The raw bytes.</param>
        /// <param name="resultPoints">The result points.</param>
        /// <param name="format">The format.</param>
        public Result(String text,
                byte[] rawBytes,
                ResultPoint[] resultPoints,
                BarcodeFormat format): this(text, rawBytes, resultPoints, format, (long)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond))
        {
           
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="rawBytes">The raw bytes.</param>
        /// <param name="resultPoints">The result points.</param>
        /// <param name="format">The format.</param>
        /// <param name="timestamp">The timestamp.</param>
		public Result(string text, byte[] rawBytes, ResultPoint[] resultPoints, BarcodeFormat format, long timestamp)
		{
			if (text == null && rawBytes == null)
			{
				throw new ArgumentException("Text and bytes are null");
			}
			this.text = text;
			this.rawBytes = rawBytes;
			this.resultPoints = resultPoints;
			this.format = format;
			this.resultMetadata = null;
            this.timestamp = timestamp;
		}
		
		public void PutMetadata(ResultMetadataType type, System.Object value)
		{
			if (resultMetadata == null)
			{
                resultMetadata = new Dictionary<ResultMetadataType, object>(3);
			}
			resultMetadata[type] = value;
		}

        public void PutAllMetadata(Dictionary<ResultMetadataType, object> metadata)
        {
            if (metadata != null)
            {
                if (resultMetadata == null)
                {
                    resultMetadata = metadata;
                }
                else
                {
                    IEnumerator<ResultMetadataType> e = metadata.Keys.GetEnumerator();
                    while (e.MoveNext())
                    {
                        ResultMetadataType key = e.Current;
                        Object val = metadata[key];
                        resultMetadata.Add(key, val);
                    }
                }
            }
        }

        public void AddResultPoints(ResultPoint[] newPoints)
        {
            if (resultPoints == null)
            {
                resultPoints = newPoints;
            }
            else if (newPoints != null && newPoints.Length > 0)
            {
                ResultPoint[] allPoints = new ResultPoint[resultPoints.Length
                        + newPoints.Length];
                System.Array.Copy((Array)(resultPoints), 0, (Array)(allPoints), 0, resultPoints.Length);
                System.Array.Copy((Array)(newPoints), 0, (Array)(allPoints), resultPoints.Length, newPoints.Length);
                resultPoints = allPoints;
            }
        }

        public long Timestamp
        {
            get
            {
                return timestamp;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			if (text == null)
			{
				return "[" + rawBytes.Length + " bytes]";
			}
			else
			{
				return text;
			}
		}
	}
}