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
using ErrorCorrectionLevel = MessagingToolkit.Barcode.QRCode.Decoder.ErrorCorrectionLevel;

namespace MessagingToolkit.Barcode.Common
{

    /// <summary> 
    /// Encapsulates the result of decoding a matrix of bits. This typically
    /// applies to 2D barcode formats. For now it contains the raw bytes obtained,
    /// as well as a String interpretation of those bytes, if applicable.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal sealed class DecoderResult
    {

        private readonly byte[] rawBytes;
        private readonly String text;
        private readonly List<byte[]> byteSegments;
        private readonly String ecLevel;

        public DecoderResult(byte[] rawBytes, String text, List<byte[]> byteSegments, String ecLevel)
        {
            this.rawBytes = rawBytes;
            this.text = text;
            this.byteSegments = byteSegments;
            this.ecLevel = ecLevel;
        }

        public byte[] RawBytes
        {
            get
            {
                return rawBytes;
            }
        }

        public String Text
        {
            get
            {
                return text;
            }
        }

        public List<byte[]> ByteSegments
        {
            get
            {
                return byteSegments;
            }
        }

        public String ECLevel
        {
            get
            {
                return ecLevel;
            }
        }
	

    }
}