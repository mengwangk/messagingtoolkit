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
    /// Context base class
    /// </summary>
    /// <typeparam name="T">Context type</typeparam>
    internal abstract class BaseContext<T>: Dictionary<string,object>, IContext
    {
        #region ==================== Private Constants ======================

        /// <summary>
        /// Result key
        /// </summary>
        private const string ResultKey = "RESULT";

        /// <summary>
        /// Data key 
        /// </summary>
        private const string DataKey = "DATA";

        #endregion ==========================================================

        #region ==================== Public Methods =========================

        /// <summary>
        /// Put data into context
        /// </summary>
        /// <param name="data">Data</param>
        public void PutData(string data)
        {
            this[DataKey] = data;
        }

        /// <summary>
        /// Get data from context
        /// </summary>
        /// <returns>Data</returns>
        public string GetData()
        {
            object result;
            if (TryGetValue(DataKey, out result))
            {
                return Convert.ToString(result);
            }
            return string.Empty;
        }


        /// <summary>
        /// Put result object into context
        /// </summary>
        /// <param name="obj">Result object</param>
        public void PutResult(object obj)
        {
            this[ResultKey] =  obj;
        }

        /// <summary>
        /// Get result string
        /// </summary>
        /// <returns>Result string</returns>
        public string GetResultString()
        {
            object result;
            if (TryGetValue(ResultKey, out result)) 
            {
                return Convert.ToString(result);
            }
            return string.Empty;
        }

        /// <summary>
        /// Get result object
        /// </summary>
        /// <returns>Result object</returns>
        public object GetResult()
        {
            object result;
            if (TryGetValue(ResultKey, out result))
            {
                return result;
            }
            return null;
        }

        #endregion ====================================================
    }
}
