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
import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Graphics;

/**
 * Screen that indicates when an upload/download is finished.
 * @author Ryan Wollmuth
 */
public class DoneScreen extends OCompositeScreen implements CommandListener
{
  private OLabel uploadComplete = new OLabel( "U/L complete", OUILook.PLAIN_MEDIUM );
  private OLabel downloadComplete = new OLabel( "D/L complete", OUILook.PLAIN_MEDIUM );
  
  private Command doneCmd = new Command( "DONE", Command.OK, 0 );
  /**
   * Creates a <code>DoneScreen</code> instance.
   * @param upload Flag indicating whether it is a upload/download.
   */
  public DoneScreen( boolean upload )
  {
    super( "Finished", OUILook.PLAIN_MEDIUM, 1 );
    
    addCommand( doneCmd );
    setCommandListener( this );
    if ( upload )
      add( uploadComplete, 0, 0, Graphics.HCENTER );
    else
      add( downloadComplete, 0, 0, Graphics.HCENTER );
  }
  
  /**
   * Command listener for soft keys
   * In this case, it is listening for the 'DONE' softkey.
   */
  public void commandAction( Command command, Displayable displayable )
  {
    if ( command == doneCmd ) ScreenNavigator.goHome();
  }
  
} 
  
    
