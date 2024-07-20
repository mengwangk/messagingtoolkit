using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using QRCodeImage = MessagingToolkit.QRCode.Codec.Data.QRCodeImage;
using QRCodeSymbol = MessagingToolkit.QRCode.Codec.Data.QRCodeSymbol;
using RsDecode = MessagingToolkit.QRCode.Crypt.RsDecode;
using DecodingFailedException = MessagingToolkit.QRCode.ExceptionHandler.DecodingFailedException;
using InvalidDataBlockException = MessagingToolkit.QRCode.ExceptionHandler.InvalidDataBlockException;
using SymbolNotFoundException = MessagingToolkit.QRCode.ExceptionHandler.SymbolNotFoundException;
using Point = MessagingToolkit.QRCode.Geom.Point;
using QRCodeDataBlockReader = MessagingToolkit.QRCode.Codec.Reader.QRCodeDataBlockReader;
using QRCodeImageReader = MessagingToolkit.QRCode.Codec.Reader.QRCodeImageReader;
using DebugCanvas = MessagingToolkit.QRCode.Helper.DebugCanvas;
using DebugCanvasAdapter = MessagingToolkit.QRCode.Helper.DebugCanvasAdapter;
using QRCodeHelper = MessagingToolkit.QRCode.Helper.QRCodeHelper;
using StringHelper = MessagingToolkit.QRCode.Helper.StringHelper;

namespace MessagingToolkit.QRCode.Codec
{
	
	public class QRCodeDecoder
	{
        internal QRCodeSymbol qrCodeSymbol;
        internal int numTryDecode;
        internal static DebugCanvas canvas;
        internal QRCodeImageReader imageReader;
        internal int numLastCorrectionFailures;

		public static DebugCanvas Canvas
		{
			get
			{
				return QRCodeDecoder.canvas;
			}
			
			set
			{
				QRCodeDecoder.canvas = value;
			}
			
		}
		virtual internal Point[] AdjustPoints
		{
			get
			{
				// note that adjusts affect dependently
				// i.e. below means (0,0), (2,3), (3,4), (1,2), (2,1), (1,1), (-1,-1)
				
				
				//		Point[] adjusts = {new Point(0,0), new Point(2,3), new Point(1,1), 
				//				new Point(-2,-2), new Point(1,-1), new Point(-1,0), new Point(-2,-2)};
                List<Point> adjustPoints = new List<Point>(10);
				for (int d = 0; d < 4; d++)
					adjustPoints.Add(new Point(1, 1));
				int lastX = 0, lastY = 0;
				for (int y = 0; y > - 4; y--)
				{
					for (int x = 0; x > - 4; x--)
					{
						if (x != y && ((x + y) % 2 == 0))
						{
							adjustPoints.Add(new Point(x - lastX, y - lastY));
							lastX = x;
							lastY = y;
						}
					}
				}
				Point[] adjusts = new Point[adjustPoints.Count];
				for (int i = 0; i < adjusts.Length; i++)
					adjusts[i] = adjustPoints[i];
				return adjusts;
			}			
		}		
				
		internal class DecodeResult
		{
            int numCorrectionFailures;
            internal sbyte[] decodedBytes;
            private QRCodeDecoder enclosingInstance;

            public DecodeResult(QRCodeDecoder enclosingInstance, sbyte[] decodedBytes, int numCorrectionFailures) 
            {
                InitBlock(enclosingInstance);
                this.decodedBytes = decodedBytes;
                this.numCorrectionFailures = numCorrectionFailures;
            }

			private void  InitBlock(QRCodeDecoder enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			virtual public sbyte[] DecodedBytes
			{
				get
				{
					return decodedBytes;
				}
				
			}

            virtual public int NumCorrectionFailures
            {
                get 
                {
			        return numCorrectionFailures;
                }
		    }
		
            virtual public bool IsCorrectionSucceeded
            {
                get 
                {
			        return enclosingInstance.numLastCorrectionFailures == 0;
                }
		    }

			
			public QRCodeDecoder EnclosingInstance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			
		}
		
		public QRCodeDecoder()
		{
			numTryDecode = 0;
			QRCodeDecoder.canvas = new DebugCanvasAdapter();
		}
			
		public virtual sbyte[] DecodeBytes(QRCodeImage qrCodeImage)
		{
			Point[] adjusts = AdjustPoints;
            List<DecodeResult> results = new List<DecodeResult>(10);
            numTryDecode = 0;
			while (numTryDecode < adjusts.Length)
			{
				try
				{
					DecodeResult result = Decode(qrCodeImage, adjusts[numTryDecode]);                                       
					if (result.IsCorrectionSucceeded)
					{
						return result.DecodedBytes;
					}
					else
					{
						results.Add(result);
						canvas.Print("Decoding succeeded but could not Correct");
						canvas.Print("all errors. Retrying..");
					}
                    
				}
				catch (DecodingFailedException dfe)
				{
					if (dfe.Message.IndexOf("Finder Pattern") >= 0)
					throw dfe;
				}
				finally
				{
					numTryDecode += 1;
				}
			}
			
			if (results.Count == 0)
				throw new DecodingFailedException("Unable to decode");
			
			int minErrorIndex = -1;
		    int minError = Int32.MaxValue;
	
			for (int i = 0; i < results.Count; i++)
			{
				DecodeResult result = results[i];

                if (result.NumCorrectionFailures < minError) {
				    minError = result.NumCorrectionFailures;
				    minErrorIndex = i;
			    }				
			}

			canvas.Print("All trials need for Correct error");
            canvas.Print("Reporting #" + (minErrorIndex)+" that,");
		    canvas.Print("corrected minimum errors (" +minError + ")");
	
			
			canvas.Print("Decoding finished.");		
            return (results[minErrorIndex]).DecodedBytes;
	
		}

        public virtual string Decode(QRCodeImage qrCodeImage, Encoding encoding)
        {
            sbyte[] data = DecodeBytes(qrCodeImage);
            byte[] byteData = new byte[data.Length];

            Buffer.BlockCopy(data, 0, byteData, 0, byteData.Length); 
            /*
            char[] decodedData = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                decodedData[i] = Convert.to(data[i]);

            }
            return new String(decodedData);
            */
            String decodedData;            
            decodedData = encoding.GetString(byteData);
            return decodedData;
        }

        public virtual String Decode(QRCodeImage qrCodeImage)
        {
            sbyte[] data = DecodeBytes(qrCodeImage);
            byte[] byteData = new byte[data.Length];
            Buffer.BlockCopy(data, 0, byteData, 0, byteData.Length);

            Encoding encoding = Encoding.GetEncoding(StringHelper.GuessEncoding(byteData));

           
            /*
            if (QRCodeHelper.IsUnicode(byteData))
            {
                encoding = Encoding.Unicode;
            }
            else
            {
                encoding = Encoding.UTF8;
            }
            */
            
            String decodedData;
            decodedData = encoding.GetString(byteData);
            return decodedData;
        }

		internal virtual DecodeResult Decode(QRCodeImage qrCodeImage, Point adjust)
		{
			try
			{
				if (numTryDecode == 0)
				{
					canvas.Print("Decoding started");
					int[][] intImage = ImageToIntArray(qrCodeImage);
					imageReader = new QRCodeImageReader();
					qrCodeSymbol = imageReader.GetQRCodeSymbol(intImage);
				}
				else
				{
					canvas.Print("--");
					canvas.Print("Decoding restarted #" + (numTryDecode));
					qrCodeSymbol = imageReader.GetQRCodeSymbolWithAdjustedGrid(adjust);
				}
			}
			catch (SymbolNotFoundException e)
			{
				throw new DecodingFailedException(e.Message);
			}
			canvas.Print("Created QRCode symbol.");
			canvas.Print("Reading symbol.");
			canvas.Print("Version: " + qrCodeSymbol.VersionReference);
			canvas.Print("Mask pattern: " + qrCodeSymbol.MaskPatternRefererAsString);
			// blocks contains all (data and RS) blocks in QR Code symbol
			int[] blocks = qrCodeSymbol.Blocks;
			canvas.Print("Correcting data errors.");
			// now blocks turn to data blocks (corrected and extracted from original blocks)
			blocks = CorrectDataBlocks(blocks);
			try
			{
				sbyte[] decodedByteArray = GetDecodedByteArray(blocks, qrCodeSymbol.Version, qrCodeSymbol.NumErrorCollectionCode);				
                return new DecodeResult(this, decodedByteArray, numLastCorrectionFailures);
			}
			catch (InvalidDataBlockException e)
			{
				canvas.Print(e.Message);
				throw new DecodingFailedException(e.Message);
			}
		}
		
		
		internal virtual int[][] ImageToIntArray(QRCodeImage image)
		{
			int width = image.Width;
			int height = image.Height;
			int[][] intImage = new int[width][];
			for (int i = 0; i < width; i++)
			{
				intImage[i] = new int[height];
			}
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					intImage[x][y] = image.GetPixel(x, y);
				}
			}
			return intImage;
		}
		
		internal virtual int[] CorrectDataBlocks(int[] blocks)
		{
			int numSucceededCorrections = 0;
            int numCorrectionFailures = 0;
			int dataCapacity = qrCodeSymbol.DataCapacity;
			int[] dataBlocks = new int[dataCapacity];
			int numErrorCollectionCode = qrCodeSymbol.NumErrorCollectionCode;
			int numRSBlocks = qrCodeSymbol.NumRSBlocks;
			int eccPerRSBlock = numErrorCollectionCode / numRSBlocks;
			if (numRSBlocks == 1)
			{
                RsDecode corrector = new RsDecode(eccPerRSBlock / 2);
                int ret = corrector.Decode(blocks);
                if (ret > 0)
                    numSucceededCorrections += ret;
                else if (ret < 0)
                    numCorrectionFailures++;
				return blocks;
			}
			else
			{
				//we have to interleave data blocks because symbol has 2 or more RS blocks
				int numLongerRSBlocks = dataCapacity % numRSBlocks;
				if (numLongerRSBlocks == 0)
				{
					//symbol has only 1 type of RS block
					int lengthRSBlock = dataCapacity / numRSBlocks;
					int[][] tmpArray = new int[numRSBlocks][];
					for (int i = 0; i < numRSBlocks; i++)
					{
						tmpArray[i] = new int[lengthRSBlock];
					}
					int[][] RSBlocks = tmpArray;
					//obtain RS blocks
					for (int i = 0; i < numRSBlocks; i++)
					{
						for (int j = 0; j < lengthRSBlock; j++)
						{
							RSBlocks[i][j] = blocks[j * numRSBlocks + i];
						}
					    RsDecode corrector = new RsDecode(eccPerRSBlock / 2);
                        int ret = corrector.Decode(RSBlocks[i]);
                        if (ret > 0)
                            numSucceededCorrections += ret;
                        else if (ret < 0)
                            numCorrectionFailures++;
					}
					//obtain only data part
					int p = 0;
					for (int i = 0; i < numRSBlocks; i++)
					{
						for (int j = 0; j < lengthRSBlock - eccPerRSBlock; j++)
						{
							dataBlocks[p++] = RSBlocks[i][j];
						}
					}
				}
				else
				{
					//symbol has 2 types of RS blocks
					int lengthShorterRSBlock = dataCapacity / numRSBlocks;
					int lengthLongerRSBlock = dataCapacity / numRSBlocks + 1;
					int numShorterRSBlocks = numRSBlocks - numLongerRSBlocks;
					int[][] tmpArray2 = new int[numShorterRSBlocks][];
					for (int i2 = 0; i2 < numShorterRSBlocks; i2++)
					{
						tmpArray2[i2] = new int[lengthShorterRSBlock];
					}
					int[][] shorterRSBlocks = tmpArray2;
					int[][] tmpArray3 = new int[numLongerRSBlocks][];
					for (int i3 = 0; i3 < numLongerRSBlocks; i3++)
					{
						tmpArray3[i3] = new int[lengthLongerRSBlock];
					}
					int[][] longerRSBlocks = tmpArray3;
					for (int i = 0; i < numRSBlocks; i++)
					{
						if (i < numShorterRSBlocks)
						{
							//get shorter RS Block(s)
							int mod = 0;
							for (int j = 0; j < lengthShorterRSBlock; j++)
							{
								if (j == lengthShorterRSBlock - eccPerRSBlock)
									mod = numLongerRSBlocks;
								shorterRSBlocks[i][j] = blocks[j * numRSBlocks + i + mod];
							}
							RsDecode corrector = new RsDecode(eccPerRSBlock / 2);
                            int ret = corrector.Decode(shorterRSBlocks[i]);
                            if (ret > 0)
                              numSucceededCorrections += ret;
                            else if (ret < 0)
                              numCorrectionFailures++;
						}
						else
						{
							//get longer RS Blocks
							int mod = 0;
							for (int j = 0; j < lengthLongerRSBlock; j++)
							{
								if (j == lengthShorterRSBlock - eccPerRSBlock)
									mod = numShorterRSBlocks;
								longerRSBlocks[i - numShorterRSBlocks][j] = blocks[j * numRSBlocks + i - mod];
							}
							
							RsDecode corrector = new RsDecode(eccPerRSBlock / 2);
                            int ret = corrector.Decode(longerRSBlocks[i - numShorterRSBlocks]);
                            if (ret > 0)
                              numSucceededCorrections += ret;
                            else if (ret < 0)
                              numCorrectionFailures++;
						}
					}
					int p = 0;
					for (int i = 0; i < numRSBlocks; i++)
					{
						if (i < numShorterRSBlocks)
						{
							for (int j = 0; j < lengthShorterRSBlock - eccPerRSBlock; j++)
							{
								dataBlocks[p++] = shorterRSBlocks[i][j];
							}
						}
						else
						{
							for (int j = 0; j < lengthLongerRSBlock - eccPerRSBlock; j++)
							{
								dataBlocks[p++] = longerRSBlocks[i - numShorterRSBlocks][j];
							}
						}
					}
				}
                if (numSucceededCorrections > 0)
				    canvas.Print(Convert.ToString(numSucceededCorrections) + " data errors corrected successfully.");
				else
					canvas.Print("No errors found.");
				numLastCorrectionFailures = numCorrectionFailures;
				return dataBlocks;
			}
		}
		
		internal virtual sbyte[] GetDecodedByteArray(int[] blocks, int version, int numErrorCorrectionCode)
		{
			sbyte[] byteArray;
			QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
			try
			{
				byteArray = reader.DataByte;
			}
			catch (InvalidDataBlockException e)
			{
				throw e;
			}
			return byteArray;
		}
		
		internal virtual String GetDecodedString(int[] blocks, int version, int numErrorCorrectionCode)
		{
			String dataString = null;
			QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
			try
			{
				dataString = reader.DataString;
			}
			catch (IndexOutOfRangeException e)
			{
				throw new InvalidDataBlockException(e.Message);
			}
			return dataString;
		}

       
	}
}