using System;
using QRCodeDecoder = MessagingToolkit.QRCode.Codec.QRCodeDecoder;
using MessagingToolkit.QRCode.Codec.Data;
using AlignmentPatternNotFoundException = MessagingToolkit.QRCode.ExceptionHandler.AlignmentPatternNotFoundException;
using FinderPatternNotFoundException = MessagingToolkit.QRCode.ExceptionHandler.FinderPatternNotFoundException;
using SymbolNotFoundException = MessagingToolkit.QRCode.ExceptionHandler.SymbolNotFoundException;
using InvalidVersionException = MessagingToolkit.QRCode.ExceptionHandler.InvalidVersionException;
using VersionInformationException = MessagingToolkit.QRCode.ExceptionHandler.VersionInformationException;
using MessagingToolkit.QRCode.Geom;
using MessagingToolkit.QRCode.Codec.Reader.Pattern;
using MessagingToolkit.QRCode.Helper;
using SystemUtils = MessagingToolkit.QRCode.Helper.SystemUtils;

namespace MessagingToolkit.QRCode.Codec.Reader
{
	
	public class QRCodeImageReader
	{
		internal DebugCanvas canvas;
		//boolean[][] image;
		//DP = 
		//23 ...side pixels of image will be limited maximum 255 (8 bits)
		//22 .. side pixels of image will be limited maximum 511 (9 bits)
		//21 .. side pixels of image will be limited maximum 1023 (10 bits)
		
		//I think it's good idea to use DECIMAL_POINT with type "long" too.
		
		public static int DECIMAL_POINT = 21;
		public const bool POINT_DARK = true;
		public const bool POINT_LIGHT = false;
		internal SamplingGrid samplingGrid;
		internal bool[][] bitmap;
	
		
		public QRCodeImageReader()
		{
			this.canvas = QRCodeDecoder.Canvas;
		}
	

		private class ModulePitch
		{
            public int top;
            public int left;
            public int bottom;
            public int right;

			public ModulePitch(QRCodeImageReader enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			private void  InitBlock(QRCodeImageReader enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private QRCodeImageReader enclosingInstance;

			public QRCodeImageReader EnclosingInstance
			{
				get
				{
					return enclosingInstance;
				}
				
			}		
		}
		
		
		internal virtual bool[][] applyMedianFilter(bool[][] image, int threshold)
		{
			bool[][] filteredMatrix = new bool[image.Length][];
			for (int i = 0; i < image.Length; i++)
			{
				filteredMatrix[i] = new bool[image[0].Length];
			}
			//filtering noise in image with median filter
			int numPointDark;
			for (int y = 1; y < image[0].Length - 1; y++)
			{
				for (int x = 1; x < image.Length - 1; x++)
				{
					//if (image[x][y] == true) {
					numPointDark = 0;
					for (int fy = - 1; fy < 2; fy++)
					{
						for (int fx = - 1; fx < 2; fx++)
						{
							if (image[x + fx][y + fy] == true)
							{
								numPointDark++;
							}
						}
					}
					if (numPointDark > threshold)
						filteredMatrix[x][y] = POINT_DARK;
				}
			}
			
			return filteredMatrix;
		}
		internal virtual bool[][] applyCrossMaskingMedianFilter(bool[][] image, int threshold)
		{
			bool[][] filteredMatrix = new bool[image.Length][];
			for (int i = 0; i < image.Length; i++)
			{
				filteredMatrix[i] = new bool[image[0].Length];
			}
			//filtering noise in image with median filter
			int numPointDark;
			for (int y = 2; y < image[0].Length - 2; y++)
			{
				for (int x = 2; x < image.Length - 2; x++)
				{
					//if (image[x][y] == true) {
					numPointDark = 0;
					for (int f = - 2; f < 3; f++)
					{
						if (image[x + f][y] == true)
							numPointDark++;
						
						if (image[x][y + f] == true)
							numPointDark++;
					}
					
					if (numPointDark > threshold)
						filteredMatrix[x][y] = POINT_DARK;
				}
			}
			
			return filteredMatrix;
		}
		internal virtual bool[][] FilterImage(int[][] image)
		{
			ImageToGrayScale(image);
			bool[][] bitmap = GrayScaleToBitmap(image);
			return bitmap;
		}
		
		internal virtual void  ImageToGrayScale(int[][] image)
		{
			for (int y = 0; y < image[0].Length; y++)
			{
				for (int x = 0; x < image.Length; x++)
				{
					int r = image[x][y] >> 16 & 0xFF;
					int g = image[x][y] >> 8 & 0xFF;
					int b = image[x][y] & 0xFF;
					int m = (r * 30 + g * 59 + b * 11) / 100;
					image[x][y] = m;
				}
			}
		}
		
		internal virtual bool[][] GrayScaleToBitmap(int[][] grayScale)
		{
			int[][] middle = GetMiddleBrightnessPerArea(grayScale);
			int sqrtNumArea = middle.Length;
			int areaWidth = grayScale.Length / sqrtNumArea;
			int areaHeight = grayScale[0].Length / sqrtNumArea;
			bool[][] bitmap = new bool[grayScale.Length][];
			for (int i = 0; i < grayScale.Length; i++)
			{
				bitmap[i] = new bool[grayScale[0].Length];
			}
			
			for (int ay = 0; ay < sqrtNumArea; ay++)
			{
				for (int ax = 0; ax < sqrtNumArea; ax++)
				{
					for (int dy = 0; dy < areaHeight; dy++)
					{
						for (int dx = 0; dx < areaWidth; dx++)
						{
							bitmap[areaWidth * ax + dx][areaHeight * ay + dy] = (grayScale[areaWidth * ax + dx][areaHeight * ay + dy] < middle[ax][ay])?true:false;
						}
					}
				}
			}
			return bitmap;
		}
		
		internal virtual int[][] GetMiddleBrightnessPerArea(int[][] image)
		{
			int numSqrtArea = 4;
			//obtain middle brightness((min + max) / 2) per area
			int areaWidth = image.Length / numSqrtArea;
			int areaHeight = image[0].Length / numSqrtArea;
			int[][][] minmax = new int[numSqrtArea][][];
			for (int i = 0; i < numSqrtArea; i++)
			{
				minmax[i] = new int[numSqrtArea][];
				for (int i2 = 0; i2 < numSqrtArea; i2++)
				{
					minmax[i][i2] = new int[2];
				}
			}
			for (int ay = 0; ay < numSqrtArea; ay++)
			{
				for (int ax = 0; ax < numSqrtArea; ax++)
				{
					minmax[ax][ay][0] = 0xFF;
					for (int dy = 0; dy < areaHeight; dy++)
					{
						for (int dx = 0; dx < areaWidth; dx++)
						{
							int target = image[areaWidth * ax + dx][areaHeight * ay + dy];
							if (target < minmax[ax][ay][0])
								minmax[ax][ay][0] = target;
							if (target > minmax[ax][ay][1])
								minmax[ax][ay][1] = target;
						}
					}
					//minmax[ax][ay][0] = (minmax[ax][ay][0] + minmax[ax][ay][1]) / 2;
				}
			}
			int[][] middle = new int[numSqrtArea][];
			for (int i3 = 0; i3 < numSqrtArea; i3++)
			{
				middle[i3] = new int[numSqrtArea];
			}
			for (int ay = 0; ay < numSqrtArea; ay++)
			{
				for (int ax = 0; ax < numSqrtArea; ax++)
				{
					middle[ax][ay] = (minmax[ax][ay][0] + minmax[ax][ay][1]) / 2;
					//Console.out.print(middle[ax][ay] + ",");
				}
				//Console.out.println("");
			}
			//Console.out.println("");
			
			return middle;
		}
		
		public virtual QRCodeSymbol GetQRCodeSymbol(int[][] image)
		{
			int longSide = (image.Length < image[0].Length)?image[0].Length:image.Length;
			QRCodeImageReader.DECIMAL_POINT = 23 - QRCodeHelper.Sqrt(longSide / 256);
			bitmap = FilterImage(image);
			canvas.Print("Drawing matrix.");
			canvas.DrawMatrix(bitmap);
			
			canvas.Print("Scanning Finder Pattern.");
			FinderPattern finderPattern = null;
			try
			{
				finderPattern = FinderPattern.FindFinderPattern(bitmap);
			}
			catch (FinderPatternNotFoundException e)
			{
				canvas.Print("Not found, now retrying...");
				bitmap = applyCrossMaskingMedianFilter(bitmap, 5);
				canvas.DrawMatrix(bitmap);
                /*
				for (int i = 0; i < 1000000000; i++)
					;
                */
				try
				{
					finderPattern = FinderPattern.FindFinderPattern(bitmap);
				}
				catch (FinderPatternNotFoundException e2)
				{
					throw new SymbolNotFoundException(e2.Message);
				}
				catch (VersionInformationException e2)
				{
					throw new SymbolNotFoundException(e2.Message);
				}
			}
			catch (VersionInformationException e)
			{
				throw new SymbolNotFoundException(e.Message);
			}
			
			
			canvas.Print("FinderPattern at");
			String finderPatternCoordinates = finderPattern.GetCenter(FinderPattern.UL).ToString() + finderPattern.GetCenter(FinderPattern.UR).ToString() + finderPattern.GetCenter(FinderPattern.DL).ToString();
			canvas.Print(finderPatternCoordinates);
			int[] sincos = finderPattern.GetAngle();
			canvas.Print("Angle*4098: Sin " + System.Convert.ToString(sincos[0]) + "  " + "Cos " + System.Convert.ToString(sincos[1]));
			
			int version = finderPattern.Version;
			canvas.Print("Version: " + System.Convert.ToString(version));
            if (version < 1 || version > 40)
            {                
                throw new InvalidVersionException("Invalid version: " + version);
            }
			
			AlignmentPattern alignmentPattern = null;
			try
			{
				alignmentPattern = AlignmentPattern.findAlignmentPattern(bitmap, finderPattern);
			}
			catch (AlignmentPatternNotFoundException e)
			{
				throw new SymbolNotFoundException(e.Message);
			}
			
			int matrixLength = alignmentPattern.getCenter().Length;
			canvas.Print("AlignmentPatterns at");
			for (int y = 0; y < matrixLength; y++)
			{
				String alignmentPatternCoordinates = "";
				for (int x = 0; x < matrixLength; x++)
				{
					alignmentPatternCoordinates += alignmentPattern.getCenter()[x][y].ToString();
				}
				canvas.Print(alignmentPatternCoordinates);
			}
			//for(int i = 0; i < 500000; i++) Console.out.println("");
			
			canvas.Print("Creating sampling grid.");
			//[TODO] need all-purpose method
			samplingGrid = GetSamplingGrid(finderPattern, alignmentPattern);
			canvas.Print("Reading grid.");
			bool[][] qRCodeMatrix = null;
			try
			{
				qRCodeMatrix = GetQRCodeMatrix(bitmap, samplingGrid);
			}
			catch (IndexOutOfRangeException e)
			{
				throw new SymbolNotFoundException("Sampling grid exceeded image boundary");
			}
			return new QRCodeSymbol(qRCodeMatrix);
		}
		
		public virtual QRCodeSymbol GetQRCodeSymbolWithAdjustedGrid(Point adjust)
		{
			if (bitmap == null || samplingGrid == null)
			{
				throw new SystemException("This method must be called after QRCodeImageReader.GetQRCodeSymbol() is called");
			}
			samplingGrid.Adjust(adjust);
			canvas.Print("Sampling grid adjusted d(" + adjust.X + "," + adjust.Y + ")");
			
			bool[][] qRCodeMatrix = null;
			try
			{
				qRCodeMatrix = GetQRCodeMatrix(bitmap, samplingGrid);
			}
			catch (IndexOutOfRangeException e)
			{
				throw new SymbolNotFoundException("Sampling grid exceeded image boundary");
			}
			return new QRCodeSymbol(qRCodeMatrix);
		}


        /// <summary>
        /// Generic Sampling grid method
        /// </summary>
        /// <param name="finderPattern">The finder pattern.</param>
        /// <param name="alignmentPattern">The alignment pattern.</param>
        /// <returns></returns>
		internal virtual SamplingGrid GetSamplingGrid(FinderPattern finderPattern, AlignmentPattern alignmentPattern)
		{
			
			Point[][] centers = alignmentPattern.getCenter();
			
			int version = finderPattern.Version;
			int sqrtCenters = (version / 7) + 2;
			
			centers[0][0] = finderPattern.GetCenter(FinderPattern.UL);
			centers[sqrtCenters - 1][0] = finderPattern.GetCenter(FinderPattern.UR);
			centers[0][sqrtCenters - 1] = finderPattern.GetCenter(FinderPattern.DL);
			int sqrtNumArea = sqrtCenters - 1;
			
			//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//
			SamplingGrid samplingGrid = new SamplingGrid(sqrtNumArea);
			
			Line baseLineX, baseLineY, gridLineX, gridLineY;
			
			///???
			//Point[] targetCenters;
			
			//int logicalDistance = alignmentPattern.getLogicalDistance();
			Axis axis = new Axis(finderPattern.GetAngle(), finderPattern.GetModuleSize());
			ModulePitch modulePitch;
			
			// for each area :
			for (int ay = 0; ay < sqrtNumArea; ay++)
			{
				for (int ax = 0; ax < sqrtNumArea; ax++)
				{
					modulePitch = new ModulePitch(this); /// Housing to order
					baseLineX = new Line();
					baseLineY = new Line();
					axis.ModulePitch = finderPattern.GetModuleSize();
					
					Point[][] logicalCenters = AlignmentPattern.getLogicalCenter(finderPattern);
					
					Point upperLeftPoint = centers[ax][ay];
					Point upperRightPoint = centers[ax + 1][ay];
					Point lowerLeftPoint = centers[ax][ay + 1];
					Point lowerRightPoint = centers[ax + 1][ay + 1];
					
					Point logicalUpperLeftPoint = logicalCenters[ax][ay];
					Point logicalUpperRightPoint = logicalCenters[ax + 1][ay];
					Point logicalLowerLeftPoint = logicalCenters[ax][ay + 1];
					Point logicalLowerRightPoint = logicalCenters[ax + 1][ay + 1];
					
					if (ax == 0 && ay == 0)
					// left upper corner
					{
						if (sqrtNumArea == 1)
						{
							upperLeftPoint = axis.translate(upperLeftPoint, - 3, - 3);
							upperRightPoint = axis.translate(upperRightPoint, 3, - 3);
							lowerLeftPoint = axis.translate(lowerLeftPoint, - 3, 3);
							lowerRightPoint = axis.translate(lowerRightPoint, 6, 6);
							
							logicalUpperLeftPoint.Translate(- 6, - 6);
							logicalUpperRightPoint.Translate(3, - 3);
							logicalLowerLeftPoint.Translate(- 3, 3);
							logicalLowerRightPoint.Translate(6, 6);
						}
						else
						{
							upperLeftPoint = axis.translate(upperLeftPoint, - 3, - 3);
							upperRightPoint = axis.translate(upperRightPoint, 0, - 6);
							lowerLeftPoint = axis.translate(lowerLeftPoint, - 6, 0);
							
							logicalUpperLeftPoint.Translate(- 6, - 6);
							logicalUpperRightPoint.Translate(0, - 6);
							logicalLowerLeftPoint.Translate(- 6, 0);
						}
					}
					else if (ax == 0 && ay == sqrtNumArea - 1)
					// left bottom corner
					{
						upperLeftPoint = axis.translate(upperLeftPoint, - 6, 0);
						lowerLeftPoint = axis.translate(lowerLeftPoint, - 3, 3);
						lowerRightPoint = axis.translate(lowerRightPoint, 0, 6);
						
						
						logicalUpperLeftPoint.Translate(- 6, 0);
						logicalLowerLeftPoint.Translate(- 6, 6);
						logicalLowerRightPoint.Translate(0, 6);
					}
					else if (ax == sqrtNumArea - 1 && ay == 0)
					// right upper corner
					{
						upperLeftPoint = axis.translate(upperLeftPoint, 0, - 6);
						upperRightPoint = axis.translate(upperRightPoint, 3, - 3);
						lowerRightPoint = axis.translate(lowerRightPoint, 6, 0);
						
						logicalUpperLeftPoint.Translate(0, - 6);
						logicalUpperRightPoint.Translate(6, - 6);
						logicalLowerRightPoint.Translate(6, 0);
					}
					else if (ax == sqrtNumArea - 1 && ay == sqrtNumArea - 1)
					// right bottom corner
					{
						lowerLeftPoint = axis.translate(lowerLeftPoint, 0, 6);
						upperRightPoint = axis.translate(upperRightPoint, 6, 0);
						lowerRightPoint = axis.translate(lowerRightPoint, 6, 6);
						
						logicalLowerLeftPoint.Translate(0, 6);
						logicalUpperRightPoint.Translate(6, 0);
						logicalLowerRightPoint.Translate(6, 6);
					}
					else if (ax == 0)
					// left side
					{
						upperLeftPoint = axis.translate(upperLeftPoint, - 6, 0);
						lowerLeftPoint = axis.translate(lowerLeftPoint, - 6, 0);
						
						logicalUpperLeftPoint.Translate(- 6, 0);
						logicalLowerLeftPoint.Translate(- 6, 0);
					}
					else if (ax == sqrtNumArea - 1)
					// right
					{
						upperRightPoint = axis.translate(upperRightPoint, 6, 0);
						lowerRightPoint = axis.translate(lowerRightPoint, 6, 0);
						
						logicalUpperRightPoint.Translate(6, 0);
						logicalLowerRightPoint.Translate(6, 0);
					}
					else if (ay == 0)
					// top
					{
						upperLeftPoint = axis.translate(upperLeftPoint, 0, - 6);
						upperRightPoint = axis.translate(upperRightPoint, 0, - 6);
						
						logicalUpperLeftPoint.Translate(0, - 6);
						logicalUpperRightPoint.Translate(0, - 6);
					}
					else if (ay == sqrtNumArea - 1)
					// bottom
					{
						lowerLeftPoint = axis.translate(lowerLeftPoint, 0, 6);
						lowerRightPoint = axis.translate(lowerRightPoint, 0, 6);
						
						logicalLowerLeftPoint.Translate(0, 6);
						logicalLowerRightPoint.Translate(0, 6);
					}
					
					if (ax == 0)
					{
						logicalUpperRightPoint.Translate(1, 0);
						logicalLowerRightPoint.Translate(1, 0);
					}
					else
					{
						logicalUpperLeftPoint.Translate(- 1, 0);
						logicalLowerLeftPoint.Translate(- 1, 0);
					}
					
					if (ay == 0)
					{
						logicalLowerLeftPoint.Translate(0, 1);
						logicalLowerRightPoint.Translate(0, 1);
					}
					else
					{
						logicalUpperLeftPoint.Translate(0, - 1);
						logicalUpperRightPoint.Translate(0, - 1);
					}
					
					int logicalWidth = logicalUpperRightPoint.X - logicalUpperLeftPoint.X;
					int logicalHeight = logicalLowerLeftPoint.Y - logicalUpperLeftPoint.Y;
					
					if (version < 7)
					{
						logicalWidth += 3;
						logicalHeight += 3;
					}
					modulePitch.top = GetAreaModulePitch(upperLeftPoint, upperRightPoint, logicalWidth - 1);
					modulePitch.left = GetAreaModulePitch(upperLeftPoint, lowerLeftPoint, logicalHeight - 1);
					modulePitch.bottom = GetAreaModulePitch(lowerLeftPoint, lowerRightPoint, logicalWidth - 1);
					modulePitch.right = GetAreaModulePitch(upperRightPoint, lowerRightPoint, logicalHeight - 1);
					
					baseLineX.SetP1(upperLeftPoint);
					baseLineY.SetP1(upperLeftPoint);
					baseLineX.SetP2(lowerLeftPoint);
					baseLineY.SetP2(upperRightPoint);
					
					samplingGrid.InitGrid(ax, ay, logicalWidth, logicalHeight);
					
					for (int i = 0; i < logicalWidth; i++)
					{
						gridLineX = new Line(baseLineX.GetP1(), baseLineX.GetP2());
						
						axis.Origin = gridLineX.GetP1();
						axis.ModulePitch = modulePitch.top;
						gridLineX.SetP1(axis.translate(i, 0));
						
						axis.Origin = gridLineX.GetP2();
						axis.ModulePitch = modulePitch.bottom;
						gridLineX.SetP2(axis.translate(i, 0));
						
						samplingGrid.SetXLine(ax, ay, i, gridLineX);
					}
					
					for (int i = 0; i < logicalHeight; i++)
					{
						
						gridLineY = new Line(baseLineY.GetP1(), baseLineY.GetP2());
						
						axis.Origin = gridLineY.GetP1();
						axis.ModulePitch = modulePitch.left;
						gridLineY.SetP1(axis.translate(0, i));
						
						axis.Origin = gridLineY.GetP2();
						axis.ModulePitch = modulePitch.right;
						gridLineY.SetP2(axis.translate(0, i));
						
						samplingGrid.SetYLine(ax, ay, i, gridLineY);
					}
				}
			}
			
			return samplingGrid;
		}
		
		
		
		//get module pitch in single area
		internal virtual int GetAreaModulePitch(Point start, Point end, int logicalDistance)
		{
			Line tempLine;
			tempLine = new Line(start, end);
			int realDistance = tempLine.Length;
			int modulePitch = (realDistance << DECIMAL_POINT) / logicalDistance;
			return modulePitch;
		}
		
		
		//gridLines[areaX][areaY][direction(x=0,y=1)][EachLines]	
		internal virtual bool[][] GetQRCodeMatrix(bool[][] image, SamplingGrid gridLines)
		{
			//int gridSize = gridLines.getWidth() * gridLines.getWidth(0,0);
			int gridSize = gridLines.TotalWidth;
			
			// now this is done within the SamplingGrid class...
			//		if (gridLines.getWidth() >= 2)
			//			gridSize-=1;
			
			canvas.Print("gridSize=" + gridSize);
			//canvas.println("gridLines.getWidth() * gridLines.getWidth(0,0) = "+gridLines.getWidth() * gridLines.getWidth(0,0));
			Point bottomRightPoint = null;
			bool[][] sampledMatrix = new bool[gridSize][];
			for (int i = 0; i < gridSize; i++)
			{
				sampledMatrix[i] = new bool[gridSize];
			}
			for (int ay = 0; ay < gridLines.GetHeight(); ay++)
			{
				for (int ax = 0; ax < gridLines.GetWidth(); ax++)
				{
					System.Collections.ArrayList sampledPoints = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10)); //only for visualiz;
					for (int y = 0; y < gridLines.GetHeight(ax, ay); y++)
					{
						for (int x = 0; x < gridLines.GetWidth(ax, ay); x++)
						{
							int x1 = gridLines.GetXLine(ax, ay, x).GetP1().X;
							int y1 = gridLines.GetXLine(ax, ay, x).GetP1().Y;
							int x2 = gridLines.GetXLine(ax, ay, x).GetP2().X;
							int y2 = gridLines.GetXLine(ax, ay, x).GetP2().Y;
							int x3 = gridLines.GetYLine(ax, ay, y).GetP1().X;
							int y3 = gridLines.GetYLine(ax, ay, y).GetP1().Y;
							int x4 = gridLines.GetYLine(ax, ay, y).GetP2().X;
							int y4 = gridLines.GetYLine(ax, ay, y).GetP2().Y;
							
							int e = (y2 - y1) * (x3 - x4) - (y4 - y3) * (x1 - x2);
							int f = (x1 * y2 - x2 * y1) * (x3 - x4) - (x3 * y4 - x4 * y3) * (x1 - x2);
							int g = (x3 * y4 - x4 * y3) * (y2 - y1) - (x1 * y2 - x2 * y1) * (y4 - y3);
							sampledMatrix[gridLines.GetX(ax, x)][gridLines.GetY(ay, y)] = image[f / e][g / e];
							if ((ay == gridLines.GetHeight() - 1 && ax == gridLines.GetWidth() - 1) && y == gridLines.GetHeight(ax, ay) - 1 && x == gridLines.GetWidth(ax, ay) - 1)
								bottomRightPoint = new Point(f / e, g / e);
							//calling canvas.drawPoint in loop can be very slow.
							// use canvas.drawPoints if you need
							//canvas.drawPoint(new Point(f / e,g / e), Color.RED);
						}
					}
				}
			}
			if (bottomRightPoint.X > image.Length - 1 || bottomRightPoint.Y > image[0].Length - 1)
				throw new System.IndexOutOfRangeException("Sampling grid pointed out of image");
			canvas.DrawPoint(bottomRightPoint, MessagingToolkit.QRCode.Helper.ColorFields.BLUE);
			
			return sampledMatrix;
		}
	}
}