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

namespace MessagingToolkit.Pdu.Ie
{
    /// <summary>
    /// Information element factory
    /// </summary>
    public class InformationElementFactory
    {
        /// <summary>
        /// Used  to determine what InformationElement to use based on bytes from a UDH
        /// assumes the supplied bytes are correct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static InformationElement CreateInformationElement(int id, byte[] data)
        {
            byte iei = (byte)(id & 0xFF);
            switch (iei)
            {
                case (byte)(ConcatInformationElement.Concat8BitRef):
                case (byte)(ConcatInformationElement.Concat16BitRef):
                    return new ConcatInformationElement(iei, data);

                case (byte)(PortInformationElement.Port16Bit):
                    return new PortInformationElement(iei, data);

                default:
                    return new InformationElement(iei, data);
            }
        }

        public static ConcatInformationElement GenerateConcatInfo(int mpRefNo, int partNo)
        {
            ConcatInformationElement concatInfo = new ConcatInformationElement(ConcatInformationElement.DefaultConcatType, mpRefNo, 1, partNo);
            return concatInfo;
        }

        public static ConcatInformationElement GenerateConcatInfo(int identifier, int mpRefNo, int partNo)
        {
            ConcatInformationElement concatInfo = new ConcatInformationElement(identifier, mpRefNo, 1, partNo);
            return concatInfo;
        }

        public static PortInformationElement GeneratePortInfo(int destPort, int srcPort)
        {
            PortInformationElement portInfo = new PortInformationElement(PortInformationElement.Port16Bit, destPort, srcPort);
            return portInfo;
        }
    }
}