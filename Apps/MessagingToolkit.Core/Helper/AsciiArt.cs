//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;

namespace MessagingToolkit.Core.Helper
{
    /// <summary>
    /// ASCII art generator
    /// </summary>
    public class AsciiArt
    {

        /// <summary>
        /// Image size and resolution
        /// </summary>
        public enum ImageSize
        {
            /// <summary>
            /// 
            /// </summary>
            UltraHighResolution,
            /// <summary>
            /// 
            /// </summary>
            HighResolution,
            /// <summary>
            /// 
            /// </summary>
            NormalResolution,
            /// <summary>
            /// 
            /// </summary>
            LowResolution,
            /// <summary>
            /// 
            /// </summary>
            UltraLowResolution    
        }


        /// <summary>
        /// Converts the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="quickRender">if set to <c>true</c> [quick render].</param>
        /// <returns></returns>
        public static string ConvertImage(Bitmap image,  ImageSize resolution, bool quickRender)
        {
            StringBuilder asciiArt = new StringBuilder();

            Rectangle bounds = new Rectangle(0, 0, image.Width, image.Height);
           
            #region greyscale image
           
            ColorMatrix _matrix = new ColorMatrix();

            _matrix[0, 0] = 1 / 3f;
            _matrix[0, 1] = 1 / 3f;
            _matrix[0, 2] = 1 / 3f;
            _matrix[1, 0] = 1 / 3f;
            _matrix[1, 1] = 1 / 3f;
            _matrix[1, 2] = 1 / 3f;
            _matrix[2, 0] = 1 / 3f;
            _matrix[2, 1] = 1 / 3f;
            _matrix[2, 2] = 1 / 3f;

            ImageAttributes _attributes = new ImageAttributes();
            _attributes.SetColorMatrix(_matrix);


            Graphics gphGrey = Graphics.FromImage(image);
            gphGrey.DrawImage(image, bounds, 0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel, _attributes);

            gphGrey.Dispose();
            #endregion

            #region ascii image

            int _pixwidth;
            switch (resolution)
            {
                case ImageSize.UltraHighResolution:
                    {
                        _pixwidth = 1;
                        break;
                    }
                case ImageSize.HighResolution:
                    {
                        _pixwidth = 2;
                        break;
                    }
                case ImageSize.LowResolution:
                    {
                        _pixwidth = 6;
                        break;
                    }
                case ImageSize.UltraLowResolution:
                    {
                        _pixwidth = 8;
                        break;
                    }
                default:
                    {
                        _pixwidth = 3;
                        break;
                    }
            }
            int _pixhight = _pixwidth * 2;
            int _pixseg = _pixwidth * _pixhight;

            for (int h = 0; h < image.Height / _pixhight; h++)
            {
                // segment hight
                int _startY = (h * _pixhight);
                // segment width
                for (int w = 0; w < image.Width / _pixwidth; w++)
                {
                    int _startX = (w * _pixwidth);
                    int _allBrightness = 0;

                    if (quickRender)
                    {
                        // each pix of this segment
                        for (int y = 0; y < _pixwidth; y++)
                        {
                            try
                            {
                                Color _c = image.GetPixel(_startX, y + _startY);
                                int _b = (int)(_c.GetBrightness() * 100);
                                _allBrightness = _allBrightness + _b;
                            }
                            catch
                            {
                                _allBrightness = (_allBrightness + 50);
                            }
                        }
                    }
                    else
                    {
                        // each pix of this segment
                        for (int y = 0; y < _pixwidth; y++)
                        {
                            for (int x = 0; x < _pixhight; x++)
                            {
                                int _cY = y + _startY;
                                int _cX = x + _startX;
                                try
                                {
                                    Color _c = image.GetPixel(_cX, _cY);
                                    int _b = (int)(_c.GetBrightness() * 100);
                                    _allBrightness = _allBrightness + _b;
                                }
                                catch
                                {
                                    _allBrightness = (_allBrightness + 50);
                                }
                            }
                        }
                    }

                    int _sb = (_allBrightness / _pixseg);
                    if (_sb < 10)
                    {
                        asciiArt.Append("#");
                    }
                    else if (_sb < 17)
                    {
                        asciiArt.Append("@");
                    }
                    else if (_sb < 24)
                    {
                        asciiArt.Append("&");
                    }
                    else if (_sb < 31)
                    {
                        asciiArt.Append("$");
                    }
                    else if (_sb < 38)
                    {
                        asciiArt.Append("%");
                    }
                    else if (_sb < 45)
                    {
                        asciiArt.Append("|");
                    }
                    else if (_sb < 52)
                    {
                        asciiArt.Append("!");
                    }
                    else if (_sb < 59)
                    {
                        asciiArt.Append(";");
                    }
                    else if (_sb < 66)
                    {
                        asciiArt.Append(":");
                    }
                    else if (_sb < 73)
                    {
                        asciiArt.Append("'");
                    }
                    else if (_sb < 80)
                    {
                        asciiArt.Append("`");
                    }
                    else if (_sb < 87)
                    {
                        asciiArt.Append(".");
                    }
                    else
                    {
                        asciiArt.Append(" ");
                    }
                }
                asciiArt.Append("\n");
            }
            #endregion

            //clean up
            image.Dispose();

            return asciiArt.ToString();

        }
    }
}