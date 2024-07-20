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

import com.nextel.ui.*;
import com.nextel.examples.ui.*;
import javax.microedition.lcdui.Graphics;
import com.nextel.util.Logger;

/**
 * This screen is the "wait" screen for uploading/downloading records.
 * It uses the {@link com.nextel.ui.OAnimation} class to show the 
 * image of a phone "transmitting" information. It then starts the thread that
 * will upload/download the records.
 * @author Ryan Wollmuth
 */
public class NetworkScreen extends OCompositeScreen
{
  private OSoftKey okSoftKey = new OSoftKey( "OK" );
  private OCommandAction okAction = new OCommandAction()
  {
    public void performAction()
    {
      ScreenNavigator.goHome();
    }
  };
  
  private String[] imageArray = { "/Phone1.png", "/Phone2.png",
                                  "/Phone3.png", "/Phone4.png" };
  private OAnimation phoneAnimation;
  
  /**
   * Creates a <code>NetworkScreen</code> instance.
   * @param upload Flag indicating whether it is a upload/download.
   */
  public NetworkScreen( boolean upload )
  {
    super( "Please wait.", OUILook.PLAIN_MEDIUM, 1 );
        
    try
    {
      phoneAnimation =  new OAnimation( imageArray, 250 );
    }
    catch ( Exception e )
    {
      Logger.ex( e );
    }
    add( phoneAnimation, 0, 0, Graphics.HCENTER );
    okSoftKey.setAction( okAction );

    UpDownThread upDownThread = new UpDownThread( upload );
    upDownThread.start();

  }
}
