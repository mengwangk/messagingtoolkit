using System;
using Line = MessagingToolkit.QRCode.Geom.Line;
using Point = MessagingToolkit.QRCode.Geom.Point;

namespace MessagingToolkit.QRCode.Helper
{
	/* 
	* This class must be a "edition independent" class for debug information controll.
	* I think it's good idea to modify this class with a adapter pattern
	*/
	public class DebugCanvasAdapter : DebugCanvas
	{
		public virtual void  Print(String string_Renamed)
		{
		}
		
		public virtual void  DrawPoint(Point point, int color)
		{
		}
		
		public virtual void  DrawCross(Point point, int color)
		{
		}
		
		public virtual void  DrawPoints(Point[] points, int color)
		{
		}
		
		public virtual void  DrawLine(Line line, int color)
		{
		}
		
		public virtual void  DrawLines(Line[] lines, int color)
		{
		}
		
		public virtual void  DrawPolygon(Point[] points, int color)
		{
		}
		
		public virtual void  DrawMatrix(bool[][] matrix)
		{
		}
		
	}
}