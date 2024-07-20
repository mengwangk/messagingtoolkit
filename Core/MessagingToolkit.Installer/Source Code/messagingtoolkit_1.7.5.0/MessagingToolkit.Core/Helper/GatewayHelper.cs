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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Diagnostics;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Ras;
using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Core.Helper
{
    /// <summary>
    /// Gateway helper class
    /// </summary>
    public static class GatewayHelper
    {
        #region =========== Declaration ================================================================

        [StructLayout(LayoutKind.Sequential)]
        struct COMMPROP
        {
            public short wPacketLength;
            public short wPacketVersion;
            public int dwServiceMask;
            public int dwReserved1;
            public int dwMaxTxQueue;
            public int dwMaxRxQueue;
            public int dwMaxBaud;
            public int dwProvSubType;
            public int dwProvCapabilities;
            public int dwSettableParams;
            public int dwSettableBaud;
            public short wSettableData;
            public short wSettableStopParity;
            public int dwCurrentTxQueue;
            public int dwCurrentRxQueue;
            public int dwProvSpec1;
            public int dwProvSpec2;
            public string wcProvChar;
        }

        [DllImport("kernel32.dll")]
        static extern bool GetCommProperties(IntPtr hFile, ref COMMPROP lpCommProp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CreateFile(string lpFileName, int dwDesiredAccess,
                   int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition,
                   int dwFlagsAndAttributes, IntPtr hTemplateFile);


        #endregion =========== Declaration ==================================================================

        #region =========== Public Static Methods ===========================================================



        /// <summary>
        /// Gets the modem configuration. The device name must be provided.
        /// The baud rate and COM port are retrieved.
        /// </summary>
        /// <param name="configuration">The mobile gateway configuration object.</param>
        /// <returns>
        /// true if configuration can be derived successfully
        /// </returns>
        public static bool GetDeviceConfiguration(MobileGatewayConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.DeviceName)) return false;

            /*
                class Win32_POTSModem : CIM_PotsModem
                {
                  uint16   AnswerMode;
                  string   AttachedTo;
                  uint16   Availability;
                  string   BlindOff;
                  string   BlindOn;
                  string   Caption;
                  string   CompatibilityFlags;
                  uint16   CompressionInfo;
                  string   CompressionOff;
                  string   CompressionOn;
                  uint32   ConfigManagerErrorCode;
                  boolean  ConfigManagerUserConfig;
                  string   ConfigurationDialog;
                  string   CountriesSupported[];
                  string   CountrySelected;
                  string   CreationClassName;
                  string   CurrentPasswords[];
                  uint8    DCB[];
                  uint8    Default[];
                  string   Description;
                  string   DeviceID;
                  string   DeviceLoader;
                  string   DeviceType;
                  uint16   DialType;
                  datetime DriverDate;
                  boolean  ErrorCleared;
                  string   ErrorControlForced;
                  uint16   ErrorControlInfo;
                  string   ErrorControlOff;
                  string   ErrorControlOn;
                  string   ErrorDescription;
                  string   FlowControlHard;
                  string   FlowControlOff;
                  string   FlowControlSoft;
                  string   InactivityScale;
                  uint32   InactivityTimeout;
                  uint32   Index;
                  datetime InstallDate;
                  uint32   LastErrorCode;
                  uint32   MaxBaudRateToPhone;
                  uint32   MaxBaudRateToSerialPort;
                  uint16   MaxNumberOfPasswords;
                  string   Model;
                  string   ModemInfPath;
                  string   ModemInfSection;
                  string   ModulationBell;
                  string   ModulationCCITT;
                  uint16   ModulationScheme;
                  string   Name;
                  string   PNPDeviceID;
                  string   PortSubClass;
                  uint16   PowerManagementCapabilities[];
                  boolean  PowerManagementSupported;
                  string   Prefix;
                  uint8    Properties[];
                  string   ProviderName;
                  string   Pulse;
                  string   Reset;
                  string   ResponsesKeyName;
                  uint8    RingsBeforeAnswer;
                  string   SpeakerModeDial;
                  string   SpeakerModeOff;
                  string   SpeakerModeOn;
                  string   SpeakerModeSetup;
                  string   SpeakerVolumeHigh;
                  uint16   SpeakerVolumeInfo;
                  string   SpeakerVolumeLow;
                  string   SpeakerVolumeMed;
                  string   Status;
                  uint16   StatusInfo;
                  string   StringFormat;
                  boolean  SupportsCallback;
                  boolean  SupportsSynchronousConnect;
                  string   SystemCreationClassName;
                  string   SystemName;
                  string   Terminator;
                  datetime TimeOfLastReset;
                  string   Tone;
                  string   VoiceSwitchFeature;
                };
             */
            try
            {
                ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher("Select * from Win32_POTSModem");
                ManagementObjectCollection objectCollection = objectSearcher.Get();

                foreach (ManagementBaseObject obj in objectCollection)
                {
                    string name = (string)obj["Name"];
                    if (configuration.DeviceName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        configuration.BaudRate = (PortBaudRate)Enum.Parse(typeof(PortBaudRate), Convert.ToString(obj["MaxBaudRateToSerialPort"]));
                        configuration.PortName = Convert.ToString(obj["AttachedTo"]);

                        // Using the port name, get the serial port information
                        /*
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_PnPEntity");
                        foreach (ManagementObject sp in searcher.Get())
                        {                            
                            string portName = Convert.ToString(sp.GetPropertyValue("Name"));
                            //string deviceId = Convert.ToString(sp.GetPropertyValue("DeviceId"));
                            if (portName.Contains(configuration.PortName)) //|| deviceId.Equals(configuration.PortName, StringComparison.OrdinalIgnoreCase))
                            {
                                // Get the rest of the port information
                                string dataBits = Convert.ToString(sp.GetPropertyValue("SettableDataBits"));
                            }
                        }
                        */

                        /*
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_SerialPort");
                        foreach (ManagementObject sp in searcher.Get())
                        {
                            string portName = Convert.ToString(sp.GetPropertyValue("Name"));
                            //string deviceId = Convert.ToString(sp.GetPropertyValue("DeviceId"));
                            if (portName.Contains(configuration.PortName)) //|| deviceId.Equals(configuration.PortName, StringComparison.OrdinalIgnoreCase))
                            {
                                // Get the rest of the port information
                                string dataBits = Convert.ToString(sp.GetPropertyValue("SettableDataBits"));

                            }                            
                        }
                        */

                        /*
                        Logger.LogThis("Device name: " + obj["Name"], LogLevel.Verbose);
                        Logger.LogThis("Model: " + obj["Model"], LogLevel.Verbose);
                        Logger.LogThis("Attached to: " + obj["AttachedTo"], LogLevel.Verbose);
                        Logger.LogThis("Max baud rate: " + obj["MaxBaudRateToSerialPort"], LogLevel.Verbose);
                        Logger.LogThis("Error control forced: " + obj["ErrorControlForced"], LogLevel.Verbose);
                        Logger.LogThis("Error control off: " + obj["ErrorControlOff"], LogLevel.Verbose);
                        Logger.LogThis("Error control on: " + obj["ErrorControlOn"], LogLevel.Verbose);
                        Logger.LogThis("Flow control hard: " + obj["FlowControlHard"], LogLevel.Verbose);
                        Logger.LogThis("Flow control off: " + obj["FlowControlOff"], LogLevel.Verbose);
                        Logger.LogThis("Flow control soft: " + obj["FlowControlSoft"], LogLevel.Verbose);
                        Logger.LogThis("Inactivity scale: " + obj["InactivityScale"], LogLevel.Verbose);
                        Logger.LogThis("Inactivity time out: " + obj["InactivityTimeout"], LogLevel.Verbose);
                        */

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogThis("Deriving configuration: " + e.Message, LogLevel.Verbose);
            }
            return false;
        }


        /// <summary>
        /// Return a list of RAS device names which are present
        /// </summary>
        /// <returns>List of device names</returns>
        public static List<string> GetActiveDevices()
        {
            List<string> devices = new List<string>(1);
            try
            {
                ReadOnlyCollection<RasDevice> rasDevices = RasDevice.GetDevices();
                foreach (RasDevice device in rasDevices)
                {
                    if (device.DeviceType.Equals("modem", StringComparison.OrdinalIgnoreCase))
                        if (!string.IsNullOrEmpty(device.Name)) devices.Add(device.Name);
                }
            }
            catch (Exception e)
            {
                Logger.LogThis("Unable to retrieve list of active devices: " + e.Message, LogLevel.Error);
            }
            return devices;
        }

        /// <summary>
        /// Return a list of RAS device names, regardless if they are present
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDevices()
        {
            List<string> devices = new List<string>(1);

            try
            {
                ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher("Select * from Win32_POTSModem");
                ManagementObjectCollection objectCollection = objectSearcher.Get();

                if (objectCollection != null)
                {
                    foreach (ManagementBaseObject obj in objectCollection)
                    {
                        string name = (string)obj["Name"];
                        if (!string.IsNullOrEmpty(name)) devices.Add(name);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogThis("Unable to retrieve list of devices: " + e.Message, LogLevel.Error);
            }
            return devices;
        }

        /// <summary>
        /// Gets the communication port properties.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        public static int GetPortBaudRate(string portName)
        {
            try
            {
                COMMPROP commProp = new COMMPROP();
                IntPtr hFile = CreateFile(@"\\.\" + portName, 0, 0, IntPtr.Zero, 3, 0x80, IntPtr.Zero);
                GetCommProperties(hFile, ref commProp);
                return commProp.dwSettableBaud;
                //return commProp.dwMaxBaud;
            }
            catch (Exception ex)
            {
                Logger.LogThis(string.Format("Error getting baud rate for {0}:{1}", portName, ex.Message), LogLevel.Info);
            }
            return 0;
            /*
            try
            {
                SerialPort port = new SerialPort(portName);
                port.Open();
                object p = port.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(port.BaseStream);
                Int32 bv = (Int32)p.GetType().GetField("dwSettableBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);
                port.Close();
                return bv;
            }
            catch (Exception ex)
            {
            }
            return 0;
            */

        }

        /// <summary>
        /// Generates a unique identifier.
        /// </summary>
        /// <returns>A unique identifier string</returns>
        public static string GenerateUniqueIdentifier()
        {
            return System.Guid.NewGuid().ToString("N");
        }


        /// <summary>
        /// Converts image to gray scale
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="newFileName">New name of the file.</param>
        public static void ConvertGrayScale(string fileName, string newFileName)
        {
            System.Drawing.Image tempImage = System.Drawing.Image.FromFile(fileName);
            System.Drawing.Imaging.ImageFormat imageFormat = tempImage.RawFormat;
            System.Drawing.Bitmap tempBitmap = new System.Drawing.Bitmap(tempImage, tempImage.Width, tempImage.Height);

            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(tempBitmap.Width, tempBitmap.Height);
            System.Drawing.Graphics newGraphics = System.Drawing.Graphics.FromImage(newBitmap);
            float[][] FloatColorMatrix ={
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                };

            System.Drawing.Imaging.ColorMatrix newColorMatrix = new System.Drawing.Imaging.ColorMatrix(FloatColorMatrix);
            System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            attributes.SetColorMatrix(newColorMatrix);
            newGraphics.DrawImage(tempBitmap, new System.Drawing.Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), 0, 0, tempBitmap.Width, tempBitmap.Height, System.Drawing.GraphicsUnit.Pixel, attributes);
            newGraphics.Dispose();
            newBitmap.Save(newFileName, imageFormat);
        }

        /// <summary>
        /// Converts image to black and white.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="newFileName">New name of the file.</param>
        public static bool ConvertBlackAndWhite(string fileName, string newFileName)
        {
            try
            {
                Bitmap img = (Bitmap)Image.FromFile(fileName);
                // Ensure that it's a 32 bit per pixel file
                if (img.PixelFormat != PixelFormat.Format32bppPArgb)
                {
                    Bitmap temp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb);
                    Graphics g = Graphics.FromImage(temp);
                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                    img.Dispose();
                    g.Dispose();
                    img = temp;
                }
                // lock the bits of the original bitmap
                BitmapData bmdo = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                // and the new 1bpp bitmap
                Bitmap bm = new Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
                BitmapData bmdn = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);


                //scan through the pixels Y by X
                int x, y;
                for (y = 0; y < img.Height; y++)
                {
                    for (x = 0; x < img.Width; x++)
                    {
                        //generate the address of the colour pixel
                        int index = y * bmdo.Stride + (x * 4);
                        //check its brightness
                        if (Color.FromArgb(Marshal.ReadByte(bmdo.Scan0, index + 2),
                                        Marshal.ReadByte(bmdo.Scan0, index + 1),
                                        Marshal.ReadByte(bmdo.Scan0, index)).GetBrightness() > 0.6f)
                            SetIndexedPixel(x, y, bmdn, true); //set it if its bright.
                    }
                }

                //tidy up
                bm.UnlockBits(bmdn);
                img.UnlockBits(bmdo);
                bm.Save(newFileName);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogThis(string.Format("Error converting image to black and white: {1}", ex.Message), LogLevel.Info);
                return false;
            }
        }



        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="originalFile">The original file.</param>
        /// <param name="newFile">The new file.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <param name="onlyResizeIfWider">if set to <c>true</c> [only resize if wider].</param>
        public static void ResizeImage(string originalFile, string newFile, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {
            System.Drawing.Image fullsizeImage = System.Drawing.Image.FromFile(originalFile);

            // Prevent using images internal thumbnail
            fullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            fullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (onlyResizeIfWider)
            {
                if (fullsizeImage.Width <= newWidth)
                {
                    newWidth = fullsizeImage.Width;
                }
            }

            int newHeight = fullsizeImage.Height * newWidth / fullsizeImage.Width;
            if (newHeight > maxHeight)
            {
                // Resize with height instead
                newWidth = fullsizeImage.Width * maxHeight / fullsizeImage.Height;
                newHeight = maxHeight;
            }

            System.Drawing.Image newImage = fullsizeImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            fullsizeImage.Dispose();

            // Save resized picture
            newImage.Save(newFile);
        }


        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="fullSizeImage">The full size image.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Bitmap fullSizeImage, int newWidth, int maxHeight)
        {
            
            // Prevent using images internal thumbnail
            fullSizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            fullSizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            bool onlyResizeIfWider = true;

            if (onlyResizeIfWider)
            {
                if (fullSizeImage.Width <= newWidth)
                {
                    newWidth = fullSizeImage.Width;
                }
            }

            int newHeight = fullSizeImage.Height * newWidth / fullSizeImage.Width;
            if (newHeight > maxHeight)
            {
                // Resize with height instead
                newWidth = fullSizeImage.Width * maxHeight / fullSizeImage.Height;
                newHeight = maxHeight;
            }

            System.Drawing.Bitmap newImage = (Bitmap)fullSizeImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            fullSizeImage.Dispose();

            return newImage;
        }

        /// <summary>
        /// Converts image to black and white.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static Bitmap ConvertBlackAndWhite(Bitmap image)
        {
            try
            {                
                // Ensure that it's a 32 bit per pixel file
                if (image.PixelFormat != PixelFormat.Format32bppPArgb)
                {
                    Bitmap temp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppPArgb);
                    Graphics g = Graphics.FromImage(temp);
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                    image.Dispose();
                    g.Dispose();
                    image = temp;
                }
                // lock the bits of the original bitmap
                BitmapData bmdo = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                // and the new 1bpp bitmap
                Bitmap bm = new Bitmap(image.Width, image.Height, PixelFormat.Format1bppIndexed);
                BitmapData bmdn = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);


                //scan through the pixels Y by X
                int x, y;
                for (y = 0; y < image.Height; y++)
                {
                    for (x = 0; x < image.Width; x++)
                    {
                        //generate the address of the colour pixel
                        int index = y * bmdo.Stride + (x * 4);
                        //check its brightness
                        if (Color.FromArgb(Marshal.ReadByte(bmdo.Scan0, index + 2),
                                        Marshal.ReadByte(bmdo.Scan0, index + 1),
                                        Marshal.ReadByte(bmdo.Scan0, index)).GetBrightness() > 0.6f)
                            SetIndexedPixel(x, y, bmdn, true); //set it if its bright.
                    }
                }

                //tidy up
                bm.UnlockBits(bmdn);
                image.UnlockBits(bmdo);
                return bm;
            }
            catch (Exception ex)
            {
                Logger.LogThis(string.Format("Error converting image to black and white: {1}", ex.Message), LogLevel.Info);                
            }

            return null;
        }


        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="imgToResize">The img to resize.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static Image ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
                        
            Bitmap b = new Bitmap(destWidth, destHeight);            
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }


        /// <summary>
        /// Determines whether [is black white] [the specified file name].
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// 	<c>true</c> if [is black white] [the specified file name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBlackAndWhite(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && !File.Exists(fileName)) return false;

            Bitmap image = new Bitmap(fileName);
            if (image.PixelFormat == PixelFormat.Format1bppIndexed) 
                return true;

            return false;
        }


        /// <summary>
        /// Determines whether [is black white] [the specified file name].
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// 	<c>true</c> if [is black white] [the specified file name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBlackAndWhite(Bitmap image)
        {
            if (image == null) return false;

            if (image.PixelFormat == PixelFormat.Format1bppIndexed)
                return true;

            return false;
        }

        #endregion ============================================================================================



        #region =========== Internal Static Methods ===========================================================

        /// <summary>
        /// Sets the indexed pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="bmd">The BMD.</param>
        /// <param name="pixel">if set to <c>true</c> [pixel].</param>
        internal static void SetIndexedPixel(int x, int y, BitmapData bmd, bool pixel)
        {

            int index = y * bmd.Stride + (x >> 3);
            byte p = Marshal.ReadByte(bmd.Scan0, index);
            byte mask = (byte)(0x80 >> (x & 0x7));
            if (pixel)
                p |= mask;
            else
                p &= (byte)(mask ^ 0xff);

            Marshal.WriteByte(bmd.Scan0, index, p);
        }

        /// <summary>
        /// Hexes to int.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        internal static byte[] HexToInt(string s)
        {
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < (s.Length / 2); i++)
            {
                string str = s.Substring(i * 2, 2);
                buffer[i] = Convert.ToByte(str, 0x10);
            }
            return buffer;
        }

        /// <summary>
        /// Encodes the semi octets.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static string EncodeSemiOctets(string data)
        {
            if ((data.Length % 2) != 0)
            {
                data = data + "F";
            }
            string str = string.Empty;
            for (int i = 0; i < data.Length; i += 2)
            {
                str = str + data.Substring(i + 1, 1) + data.Substring(i, 1);
            }
            return str;
        }

        /// <summary>
        /// Connects the gateway.
        /// </summary>
        /// <param name="rasEntryName">Name of the RAS entry.</param>
        /// <param name="apnAccount">The APN account.</param>
        /// <param name="apnPassword">The APN password.</param>
        /// <param name="gatewayAddress">The gateway address.</param>
        /// <returns>
        /// IP address of the connection. Return a empty string if cannot connect to the gateway
        /// </returns>
        internal static string ConnectGateway(string rasEntryName, string apnAccount, string apnPassword, string gatewayAddress)
        {
            RasDialer rasDialer = new RasDialer();
            rasDialer.EntryName = rasEntryName;
            rasDialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);


            // NOTE: The entry MUST be in the phone book before the connection can be dialed.
            // Begin dialing the connection; this will raise events from the dialer instance.
            //rasDialer.Dial(new NetworkCredential(apnAccount, apnPassword));

            // Set the credentials the dialer should use.
            rasDialer.Credentials = new NetworkCredential(apnAccount, apnPassword);
            rasDialer.Dial();


            // Get the active IP
            string ipAddress = GatewayHelper.GetActiveIPAddress(rasEntryName);
            if (!string.IsNullOrEmpty(ipAddress))
            {
                Logger.LogThis("Creating routing entry", LogLevel.Verbose);
                // Add to the routing table
                CreateRoutingEntry(gatewayAddress, ipAddress);
                Logger.LogThis("Routing entry completed", LogLevel.Verbose);
                return ipAddress;
            }

            return string.Empty;
        }

        /// <summary>
        /// Disconnects the gateway.
        /// </summary>
        /// <param name="rasEntryName">Name of the RAS entry.</param>
        /// <returns></returns>
        internal static bool DisconnectGateway(string rasEntryName)
        {
            try
            {
                if (string.IsNullOrEmpty(rasEntryName)) return false;
                RasDialer rasDialer = new RasDialer();
                foreach (RasConnection connection in RasConnection.GetActiveConnections())
                {
                    if (rasEntryName.Equals(connection.EntryName, StringComparison.OrdinalIgnoreCase))
                    {
                        /*
                        if (connection.HangUp())
                        {
                            return true;
                        }
                        */
                        connection.HangUp();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogThis("Unable to disconnect " + rasEntryName + ": " + e.Message, LogLevel.Error);
            }
            return false;
        }

        /// <summary>
        /// Gets the active IP address.
        /// </summary>
        /// <param name="rasEntryName">Name of the RAS entry.</param>
        /// <returns>
        /// Return the IP address. If IP address cannot be retrieved, return a empty string
        /// </returns>
        internal static string GetActiveIPAddress(string rasEntryName)
        {
            try
            {
                if (string.IsNullOrEmpty(rasEntryName)) return string.Empty;
                RasDialer rasDialer = new RasDialer();
                foreach (RasConnection connection in RasConnection.GetActiveConnections())
                {
                    Logger.LogThis("Connection name: " + connection.EntryName, LogLevel.Verbose);
                    Logger.LogThis("Id: " + connection.EntryId, LogLevel.Verbose);

                    if (rasEntryName.Equals(connection.EntryName, StringComparison.OrdinalIgnoreCase))
                    {
                        RasIPInfo ipAddress = (RasIPInfo)connection.GetProjectionInfo(RasProjectionType.IP);
                        if (ipAddress != null)
                        {
                            Logger.LogThis("Client IP: " + ipAddress.IPAddress.ToString(), LogLevel.Verbose);
                            Logger.LogThis("Server IP: " + ipAddress.ServerIPAddress.ToString(), LogLevel.Verbose);
                            return ipAddress.IPAddress.ToString();
                        }
                        else
                        {
                            Logger.LogThis("Unable to get the IP address", LogLevel.Info);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogThis("Retrieving IP address for " + rasEntryName + ": " + rasEntryName, LogLevel.Error);
            }
            return string.Empty;
        }



        /// <summary>
        /// Creates the routing entry.
        /// </summary>
        /// <param name="destinationAddress">The destination address.</param>
        /// <param name="gateway">The gateway.</param>
        /// <returns>
        /// true if the routing entry is created successfully, otherwise return false
        /// </returns>
        internal static bool CreateRoutingEntry(string destinationAddress, string gateway)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = "route";
                process.StartInfo.Arguments = string.Format("add {0} mask 255.255.255.255 {1}", destinationAddress, gateway);
                process.Start();
                process.Close();
            }
            catch (Exception e)
            {
                Logger.LogThis(string.Format("Creating routing entry for {0} using gateway {1}", destinationAddress, gateway), LogLevel.Error);
                Logger.LogThis("Message: " + e.Message, LogLevel.Error);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromRecord">From record.</param>
        /// <param name="toRecord">To record.</param>
        /// <returns></returns>
        internal static T SetProperties<T>(T fromRecord, T toRecord)
        {
            foreach (PropertyInfo field in typeof(T).GetProperties())
            {
                field.SetValue(toRecord, field.GetValue(fromRecord, null), null);
            }
            return toRecord;
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="fromRecord">From record.</param>
        /// <param name="toRecord">To record.</param>
        /// <returns></returns>
        internal static T SetProperties<T, U>(U fromRecord, T toRecord)
        {
            foreach (PropertyInfo fromField in typeof(U).GetProperties())
            {
                if (fromField.Name != "Id")
                {
                    foreach (PropertyInfo toField in typeof(T).GetProperties())
                    {
                        if (fromField.Name == toField.Name)
                        {
                            toField.SetValue(toRecord, fromField.GetValue(fromRecord, null), null);
                            break;
                        }
                    }
                }
            }
            return toRecord;
        }


        /// <summary>
        /// Sets the fields.
        /// </summary>
        /// <typeparam name="T">To record data type</typeparam>
        /// <typeparam name="U">From record data type</typeparam>
        /// <param name="fromRecord">From record.</param>
        /// <param name="toRecord">To record.</param>
        /// <returns>The to record</returns>
        internal static T SetFields<T, U>(U fromRecord, T toRecord)
        {
            foreach (FieldInfo fromField in typeof(U).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (FieldInfo toField in typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (fromField.Name == toField.Name)
                    {
                        toField.SetValue(toRecord, fromField.GetValue(fromRecord));
                        break;
                    }
                }
            }
            return toRecord;
        }


        /// <summary>
        /// Generates a random id number
        /// </summary>
        /// <returns>An integer value for the random id</returns>
        internal static string GenerateRandomId()
        {
            /*
            Random random = new Random();
            return random.Next(1, 10000000);
            */
            return System.Guid.NewGuid().ToString("N");
        }      

        #endregion ============================================================================================

    }
}
