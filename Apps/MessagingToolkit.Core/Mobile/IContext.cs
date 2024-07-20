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

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Context for result passing.
    /// </summary>
    internal interface IContext: IDictionary<string, object>
    {
        #region ==================== Methods =========================

        /// <summary>
        /// Put data into the context
        /// </summary>
        /// <param name="data">Data</param>
        void PutData(string data);

        /// <summary>
        /// Get data from context
        /// </summary>
        /// <returns>Data</returns>
        string GetData();


        /// <summary>
        /// Put result into context
        /// </summary>
        /// <param name="obj">Result</param>
        void PutResult(object obj);


        /// <summary>
        /// Get result string
        /// </summary>
        /// <returns>Result string</returns>
        string GetResultString();

        /// <summary>
        /// Get result object
        /// </summary>
        /// <returns>Result object</returns>
        object GetResult();

        #endregion ====================================================
    }
}
