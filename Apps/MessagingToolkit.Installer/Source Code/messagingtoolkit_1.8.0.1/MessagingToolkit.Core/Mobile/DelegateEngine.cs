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
using System.Windows.Forms;
using System.Threading;
using System.Runtime.CompilerServices;

using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Delegation engine 
    /// </summary>
    internal sealed class DelegateEngine
    {
        #region ===================== Private Variables ================================

        /// <summary>
        /// Delegate asynchronous method caller
        /// </summary>
        private delegate IContext AsyncMethodCaller(ref IFeature feature, ref Exception thrownException);


        #endregion ================================================================


        #region ===================== Private Constants ===========================

        /// <summary>
        /// Interval in milliseconds to wait for its turn for execution
        /// </summary>
        private const int ExecutionWaitInterval = 100;


        /// <summary>
        /// Number of retries to get its execution cycle
        /// </summary>
        private const int ExecutionWaitRetries = 5;


        #endregion ================================================================

        #region ===================== Constructor =================================

        /// <summary>
        /// Private constructor
        /// </summary>
        private DelegateEngine()
        {
            // Do nothing
        }

        #endregion ================================================================

        #region ===================== Private Metods ==============================

        /// <summary>
        /// Synchronous feature execution
        /// </summary>
        /// <param name="feature">Feature to be executed</param>       
        private IContext ExecuteSynchronous(IFeature feature)
        {
            try
            {
                AsyncMethodCaller asynchDelegate = new AsyncMethodCaller(Execute);

                // Asynchronously invoke the method
                Exception thrownException = null;
                IAsyncResult result = asynchDelegate.BeginInvoke(ref feature, ref thrownException, null, null);

                while (!result.IsCompleted)
                {
                    // Do any work you can do before waiting.                    
                    //result.AsyncWaitHandle.WaitOne(ExecutionWaitInterval, false);
                    Application.DoEvents();
                    Thread.Sleep(10);
                    
                }

                // The asynchronous operation has completed.
                // Obtain the result.
                IContext context = asynchDelegate.EndInvoke(ref feature, ref thrownException, result);

                if (thrownException != null) throw thrownException;

                return context;
            } 
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        #endregion ================================================================

        #region ===================== Private Methods =======================

        /// <summary>
        ///  Method to execute the feature
        /// </summary>
        /// <param name="feature">Feature to be executed</param>
        /// <param name="thrownException">Any exception thrown</param>
        /// <returns>Result context</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private IContext Execute(ref IFeature feature, ref Exception thrownException) 
        {
            string errorMessage = string.Empty;
            IContext context = new Context();
            try
            {
                if (feature.Execute(context))
                    return context;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            thrownException = new GatewayException(String.Format(Resources.UnsupportedFeature, feature.ToString(),errorMessage));
            return context;
        }

        #endregion ================================================================

        #region ===================== Public Method ================================

        /// <summary>
        /// Execute the feature in a separate thread. Execution can be synchronous or asynchronous
        /// </summary>
        /// <param name="feature">Feature to be executed</param>
        /// <returns>The result context</returns>
        public IContext Run(IFeature feature)
        {   
            try
            {
                if (feature.ExecutionBehavior == ExecutionBehavior.Synchronous)
                {
                    // Synchronous execution
                    return ExecuteSynchronous(feature);
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }

            throw new GatewayException(string.Format(Resources.ExecutionException, feature.ToString()));
        }

        #endregion ================================================================



        #region ===================== Static Factory Method =======================

        /// <summary>
        /// Return a new instance of the delegation engine
        /// </summary>
        /// <returns>A new delegation engine instance</returns>
        public static DelegateEngine NewInstance()
        {
            return new DelegateEngine();
        }

        #endregion ================================================================
    }

   
}
