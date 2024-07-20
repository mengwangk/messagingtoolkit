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

package com.nextel.ui; 
import javax.microedition.lcdui.*;
import javax.microedition.midlet.*;


/**
 * This class provides access to the MIDlet's environment.
 * @author Glen Cordrey
 */
public class OHandset 
{
  private static MIDlet MY_MIDLET;
  
  private OHandset()
  { /* static class */ }
  
  /**
   * The
   * {@link javax.microedition.lcdui.Display javax.microedition.lcdui.Display}
   * for the midlet.
   */
  static Display DISPLAY;

  
  /**
   * Sets the current MIDlet.  If a MIDlet calls this method then environmental
   * information which can only be accessed via the MIDlet object, such as the  
   * {@link javax.microedition.lcdui.Display javax.microedition.lcdui.Display},
   * will be accessible to other objects via this class.
   *
   * @param midlet The midlet
   */
  public static void setMIDlet( MIDlet midlet ) 
  {
    MY_MIDLET = midlet;
    DISPLAY = Display.getDisplay( midlet );
  } // setMIDlet

    /**
     * Returns the current MIDlet.
     *
     * @return The current MIDlet <b>if {@link #setMIDlet} was previously
     * called</b>, else  null.
     */
  public static MIDlet getMIDlet() 
  {
    return MY_MIDLET;
  } // getMIDlet

  /**
   * Gets the Display object associated with the midlet.  This may allow another
   * object, such as an {@link com.nextel.ui.OComponent OComponent}, to use
   * environment information which is accessible only via a MIDlet object.  For
   * example, the following code in
   {@link #beep beep} causes the handset to emit a beep.
   * <pre><code>
   * if ( OHandset.getDisplay() != null )
   *	AlertType.WARNING.playSound( OHandset.getDisplay() );
   * </code></pre>
   *
   * @return The Display object for the MIDlet, or null if the MIDlet has not
   * previously called {@link #setMIDlet setMIDlet}
   */
  public static Display getDisplay()
  {
    return DISPLAY;
  }

  /**
   * Causes the handset to emit a beep <b>if {@link #setMIDlet setMIDlet} has
   * been previously called</b>.
   *
   */
  public static void beep() 
  {
    if ( DISPLAY != null )
      AlertType.WARNING.playSound( DISPLAY );
  } // beep

}// OHandset
