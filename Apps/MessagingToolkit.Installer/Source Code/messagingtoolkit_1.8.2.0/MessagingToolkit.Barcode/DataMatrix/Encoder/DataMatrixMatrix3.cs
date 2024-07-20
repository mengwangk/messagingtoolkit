using System;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class DataMatrixMatrix3
    {
        #region Fields
        double[,] _data;
        #endregion

        #region Constructors
        DataMatrixMatrix3()
        {
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src">the matrix to copy</param>
        internal DataMatrixMatrix3(DataMatrixMatrix3 src)
        {
            _data = new[,] { { src[0, 0], src[0, 1], src[0, 2] }, { src[1, 0], src[1, 1], src[1, 2] }, { src[2, 0], src[2, 1], src[2, 2] } };
        }
        #endregion

        #region internal Static Methods
        /// <summary>
        /// creates a 3x3 identitiy matrix:<para />
        /// 1 0 0<para />
        /// 0 1 0<para />
        /// 0 0 1
        /// </summary>
        internal static DataMatrixMatrix3 Identity()
        {
            return Translate(0, 0);
        }

        /// <summary>
        /// generates a 3x3 translate transformation matrix
        /// 1 0 0<para />
        /// 0 1 0<para />
        /// tx ty 1
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        internal static DataMatrixMatrix3 Translate(double tx, double ty)
        {
            DataMatrixMatrix3 result = new DataMatrixMatrix3 {_data = new[,] {{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {tx, ty, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 rotate transformation matrix
        /// cos(angle) sin(angle) 0<para />
        /// -sin(angle) cos(angle) 0<para />
        /// 0 0 1
        /// </summary>
        /// <param name="angle"></param>
        internal static DataMatrixMatrix3 Rotate(double angle)
        {
            DataMatrixMatrix3 result = new DataMatrixMatrix3 { _data = new[,] { 
                {Math.Cos(angle), Math.Sin(angle), 0.0}, 
                {-Math.Sin(angle), Math.Cos(angle), 0.0}, {0, 0, 1.0} }};
            return result;
        }

        /// <summary>
        /// generates a 3x3 scale transformation matrix
        /// sx 0 0<para />
        /// 0 sy 0<para />
        /// 0 0 1
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <returns></returns>
        internal static DataMatrixMatrix3 Scale(double sx, double sy)
        {
            DataMatrixMatrix3 result = new DataMatrixMatrix3 {_data = new[,] {{sx, 0.0, 0.0}, {0.0, sy, 0.0}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 shear transformation matrix
        /// 0 shx 0<para />
        /// shy 0 0<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DataMatrixMatrix3 Shear(double shx, double shy)
        {
            DataMatrixMatrix3 result = new DataMatrixMatrix3 {_data = new[,] {{1.0, shy, 0.0}, {shx, 1.0, 0.0}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 top line skew transformation matrix
        /// b1/b0 0 (b1-b0)/(sz * b0)<para />
        /// 0 sz/b0 0<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DataMatrixMatrix3 LineSkewTop(double b0, double b1, double sz)
        {
            if (b0 < DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("b0 must be larger than zero in top line skew transformation");
            }
            DataMatrixMatrix3 result = new DataMatrixMatrix3
                {_data = new[,] {{b1/b0, 0.0, (b1 - b0)/(sz*b0)}, {0.0, sz/b0, 0.0}, {0, 0, 1.0}}};
            return result;
        }


        /// <summary>
        /// generates a 3x3 top line skew transformation matrix (inverse)
        /// b0/b1 0 (b0-b1)/(sz * b1)<para />
        /// 0 b0/sz 0<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DataMatrixMatrix3 LineSkewTopInv(double b0, double b1, double sz)
        {
            if (b1 < DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("b1 must be larger than zero in top line skew transformation (Inverse)");
            }
            DataMatrixMatrix3 result = new DataMatrixMatrix3
                                     {_data = new[,] {{b0/b1, 0.0, (b0 - b1)/(sz*b1)}, {0.0, b0/sz, 0.0}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 side line skew transformation matrix (inverse)
        /// sz/b0 0 0<para />
        /// 0 b1/b0 (b1-b0)/(sz*b0)<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DataMatrixMatrix3 LineSkewSide(double b0, double b1, double sz)
        {
            if (b0 < DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("b0 must be larger than zero in side line skew transformation (Inverse)");
            }
            DataMatrixMatrix3 result = new DataMatrixMatrix3
                                     {_data = new[,] {{sz/b0, 0.0, 0.0}, {0.0, b1/b0, (b1 - b0)/(sz*b0)}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 side line skew transformation matrix (inverse)
        /// b0/sz 0 0<para />
        /// 0 b0/b1 (b0 - b1) / (sz * b1)<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DataMatrixMatrix3 LineSkewSideInv(double b0, double b1, double sz)
        {
            if (b1 < DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("b1 must be larger than zero in top line skew transformation (Inverse)");
            }
            DataMatrixMatrix3 result = new DataMatrixMatrix3
                                     {_data = new[,] {{b0/sz, 0.0, 0.0}, {0.0, b0/b1, (b0 - b1)/(sz*b1)}, {0, 0, 1.0}}};
            return result;
        }

        public static DataMatrixMatrix3 operator *(DataMatrixMatrix3 m1, DataMatrixMatrix3 m2)
        {
            DataMatrixMatrix3 result = new DataMatrixMatrix3 {_data = new[,] {{0.0, 0, 0}, {0, 0, 0}, {0, 0, 0}}};

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return result;
        }

        public static DataMatrixVector2 operator *(DataMatrixVector2 vector, DataMatrixMatrix3 matrix)
        {
            double w = Math.Abs(vector.X * matrix[0, 2] + vector.Y * matrix[1, 2] + matrix[2, 2]);
            if (w <= DataMatrixConstants.DataMatrixAlmostZero)
            {
                throw new ArgumentException("Multiplication of vector and matrix resulted in invalid result");
            }
            DataMatrixVector2 result = new DataMatrixVector2((vector.X*matrix[0,0] + vector.Y * matrix[1,0] + matrix[2,0])/w,
                (vector.X * matrix[0,1] + vector.Y * matrix[1,1] + matrix[2,1])/w);
            return result;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\n{3}\t{4}\t{5}\n{6}\t{7}\t{8}\n", _data[0, 0], _data[0, 1], _data[0, 2], _data[1, 0], _data[1, 1], _data[1, 2], _data[2, 0], _data[2, 1], _data[2, 2]);
        }
        #endregion

        #region Properties
        internal double this[int i, int j]
        {
            get
            {
                return _data[i, j];
            }
            set
            {
                _data[i, j] = value;
            }
        }
        #endregion
    }
}
