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

package com.nextel.ui; // Generated package name
import javax.microedition.lcdui.Font;
import com.nextel.util.Debugger;
import com.nextel.util.Logger;

/**
 * This class represents either of the two soft keys, which are right below the
 * screen on the left and right sides.  Soft keys are pressed by the user to
 * perform actions, such as calculating a result.
 * <p>
 * <b>See the package overview and {@link com.nextel.ui.OCompositeScreen#addSoftKey}
 * for detailed information about using soft keys</b>
 * @author Glen Cordrey
 */
public class OSoftKey 
{
  /**
   * The key code provided by the KVM when the left soft key is pressed.
   */
  public final static int LEFT = -20;

  /**
   * The key code provided by the KVM when the left right key is pressed.
   */
  public final static int RIGHT = -21;

  /**
   * The font used to display the soft key label.
   */
  public final static Font FONT = OUILook.PLAIN_SMALL;

  /**
   * The maximum width, in pixels, of the soft key label.
   */
  public final static int WIDTH = FONT.stringWidth( "WWWWWWW" );

  private String label;
  private OCommandAction action;

  /**
   * Creates a new <code>OSoftKey</code> instance.
   *
   * @param label Label to display on the screen above the soft key.
   */
  public OSoftKey ( String label )
  {
    this.label = label;
  } // constructor

  /**
   * Gets the label that is displayed for this soft key.
   *
   * @return a <code>String</code> value
   */
  public String getLabel() 
  { 
    if ( Debugger.ON ) Logger.dev( "OSoftKey.getLabel CALLED" );
    return label;

  } // getLabel
  
  /**
   * Gets the action associated with the key.
   *
   * @return The OCommandAction for the key, or null if no action was specified.
   */
  public OCommandAction getAction() 
  {
    return action;
  } // getAction

  /**
   * Sets the action associated with the key.
   *
   * @param action The action to take when the key is pressed.
   */
  public void setAction( OCommandAction action ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OSoftKey.setAction ENTERED" );
    this.action = action;
    if ( Debugger.ON ) Logger.dev( "OSoftKey.setAction EXITTING" );
  } // setAction

}// OSoftKey
