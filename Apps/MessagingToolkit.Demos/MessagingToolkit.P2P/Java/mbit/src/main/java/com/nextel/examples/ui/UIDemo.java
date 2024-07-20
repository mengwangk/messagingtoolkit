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

package com.nextel.examples.ui;

import javax.microedition.midlet.MIDlet;
import javax.microedition.midlet.MIDletStateChangeException;
import com.nextel.ui.OHandset;
import com.nextel.examples.ui.*;

/**
 * The <b>UIDemo</b> midlet provides a demonstration of the UI Components.
 * This class performs normal life-cycle methods required for midlets to include
 * starting, stopping, and pausing the application.  Please refer to the
 * {@link com.nextel.examples.ui.ComponentDemoScreen} for more details on the
 * functionality contained in this midlet.  
 * <p>
 * This demo uses the following .png files, which must reside in the root
 * directory of the MIDlet's jar file or, if an emulator is used, the
 * appropriate directory as required by the emulator
 * <pre>
 * 9x9.png
 * Phone1.png
 * Phone2.png
 * Phone3.png
 * Phone4.png
 * </pre>
 *
 * @author Anthony Paper
 */

public class UIDemo extends MIDlet
{
  private ComponentDemoScreen componentDemoScreen;

  /**
   * Method used to startup the MIDlet.  This method allocates
   * the {@link com.nextel.examples.ui.ComponentDemoScreen} instance
   * used to demonstrate UI components.  This is a require method
   * for subclasses extending {@link javax.microedition.midlet.MIDlet}.
   */
  protected void startApp() throws MIDletStateChangeException
  {
    if ( componentDemoScreen != null )
    { // after the MIDlet has been paused, so display the current screen
      OHandset.getDisplay().setCurrent( ScreenNavigator.getCurrentScreen() );
    }
    else  // 1st time through
    {
      OHandset.setMIDlet( this );
      componentDemoScreen = new ComponentDemoScreen();
      ScreenNavigator.goForward( componentDemoScreen );
    }
  }

  /**
   * Method used to pause the MIDlet.  This is a require method
   * for subclasses extending {@link javax.microedition.midlet.MIDlet}.
   */
  protected void pauseApp()
  {
  }

  /**
   * Method used to destroy the MIDlet.  This is a require method
   * for subclasses extending {@link javax.microedition.midlet.MIDlet}.  
   *
   * @param param Boolean flag indicating whether the destruction is
   *              should be conditional.
   */
  protected void destroyApp( boolean param ) throws MIDletStateChangeException
  {
  }
}
