/* Copyright (c) 2001 Nextel Communications, Inc.
 * All rights reserved.
 *  
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met: 
 *  
 *   - Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer. 
 *   - Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution. 
 *   - Neither the name of Nextel Communications, Inc. nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission. 
 *  
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
 * IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL NEXTEL
 * COMMUNICATIONS, INC. OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
package com.nextel.examples.expensepad;


import com.nextel.util.Logger;
import com.nextel.ui.OHandset;
import com.nextel.examples.ui.*;

/**
 * Simple thread that calls the ExpenseStore's upload() and download() methods.
 * @author Ryan Wollmuth
 */
public class UpDownThread extends Thread implements Runnable
{
  private boolean upload;
  
  /**
   * Creates an <code>UpDownThread</code> instance.
   * @param upload Flag indicating whether to upload or download.
   */
  public UpDownThread( boolean upload )
  {
    this.upload = upload;
  }
  
  /**
   * Calls upload() or download() on <code>ExpenseStore</code>, depending
   * on the value of the upload flag.
   */
  public void run()
  {
    try
    {
      if ( upload )
        ExpenseStore.getInstance().upload();
      else
      {
        // Delete all the records, then download.
        ExpenseStore.getInstance().deleteAll();
        ExpenseStore.getInstance().download();
      }
      // Thread is done, go to the Done screen...
      ScreenNavigator.goForward( new DoneScreen( upload ) );
    } catch ( Exception e ) {
      Logger.ex( e );
    }
  }
}
