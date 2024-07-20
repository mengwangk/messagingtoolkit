using System;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class DataMatrixVector2
    {
        #region Constructors
        internal DataMatrixVector2()
        {
            this.X = 0.0;
            this.Y = 0.0;
        }

        internal DataMatrixVector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        #endregion

        #region Operators
        public static DataMatrixVector2 operator +(DataMatrixVector2 v1, DataMatrixVector2 v2)
        {
            DataMatrixVector2 result = new DataMatrixVector2(v1.X, v1.Y);
            result.X += v2.X;
            result.Y += v2.Y;
            return result;
        }

        public static DataMatrixVector2 operator -(DataMatrixVector2 v1, DataMatrixVector2 v2)
        {
            DataMatrixVector2 result = new DataMatrixVector2(v1.X, v1.Y);
            result.X -= v2.X;
            result.Y -= v2.Y;
            return result;
        }

        public static DataMatrixVector2 operator *(DataMatrixVector2 v1, double factor)
        {
            return new DataMatrixVector2(v1.X * factor, v1.Y * factor);
        }
        #endregion

        #region Methods
        internal double Cross(DataMatrixVector2 v2)
        {
            return (this.X * v2.Y - this.Y * v2.X);
        }

        internal double Norm()
        {
            double mag = Mag();
            if (mag <= DataMatrixConstants.DataMatrixAlmostZero)
            {
                return -1.0; // FIXXXME: This doesn't look clean, as noted in original dmtx source
            }
            this.X /= mag;
            this.Y /= mag;
            return mag;
        }

        internal double Dot(DataMatrixVector2 v2)
        {
            return Math.Sqrt(this.X * v2.X + this.Y * v2.Y);
        }

        internal double Mag()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        internal double DistanceFromRay2(DataMatrixRay2 ray)
        {
            if (Math.Abs(1.0 - ray.V.Mag()) > DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("DistanceFromRay2: The ray's V vector must be a unit vector");
            }
            return ray.V.Cross(this - ray.P);
        }

        internal double DistanceAlongRay2(DataMatrixRay2 ray)
        {
            if (Math.Abs(1.0 - ray.V.Mag()) > DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("DistanceAlongRay2: The ray's V vector must be a unit vector");
            }
            return (this - ray.P).Dot(ray.V);
        }

        internal bool Intersect(DataMatrixRay2 p0, DataMatrixRay2 p1)
        {
            double denominator = p1.V.Cross(p0.V);
            if (Math.Abs(denominator) < DataMatrixConstants.DataMatrixAlmostZero)
            {
                return false;
            }
            double numerator = p1.V.Cross(p1.P - p0.P);
            return PointAlongRay2(p0, numerator / denominator);
        }

        internal bool PointAlongRay2(DataMatrixRay2 ray, double t)
        {
            if (Math.Abs(1.0 - ray.V.Mag()) > DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("PointAlongRay: The ray's V vector must be a unit vector");
            }
            DataMatrixVector2 tmp = new DataMatrixVector2(ray.V.X * t, ray.V.Y * t);
            this.X = ray.P.X + tmp.X;
            this.Y = ray.P.Y + tmp.Y;
            return true;
        }
        #endregion

        #region Properties

        internal double X { get; set; }

        internal double Y { get; set; }

        #endregion
    }
}
