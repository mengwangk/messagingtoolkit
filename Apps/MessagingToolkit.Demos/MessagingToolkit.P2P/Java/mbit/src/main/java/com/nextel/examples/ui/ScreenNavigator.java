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
import javax.microedition.lcdui.*;
import javax.microedition.midlet.*;
import java.util.*;
import com.nextel.ui.OHandset;

/**
 * Supports navigation among the screens.
 * @author Glen Cordrey
 */
public class ScreenNavigator 
{
  // A stack of the path to the currently-displayed screen.
  private static Stack SCREENS = new Stack();

  /**
   * Creates a new <code>ScreenNavigator</code> instance.
   *
   */
  private ScreenNavigator ()
  { } // constructor

  /**
   * Display a new screen, and the new screen supports going back to the
   * previous screen.
   *
   * @param newScreen The new screen to display
   */
  public static void goForward ( Displayable newScreen ) 
  {    
    SCREENS.push( newScreen );
    OHandset.getDisplay().setCurrent( newScreen );
  } // goForward

  /**
   * Makes the previously displayed screen the currently displayed screen.
   */
  public static void goBack () 
  {
    SCREENS.pop();
    OHandset.getDisplay().setCurrent( ( Displayable ) SCREENS.peek() );
  } // removeScreen

    /**
     * Gets the current screen (the screen at the top of the stack).
     *
     * @return a <code>OCompositeScreen</code> value
     */
  public static Displayable getCurrentScreen()
  {
    Displayable current = null;
    if ( ! SCREENS.empty() ) 
    {
      current = ( Displayable ) SCREENS.peek();
    }
    return current;
  } // getCurrentScreen

  /**
   * Goes back to the first (home) screen.
   *
   */
  public static void goHome () 
  {
    Displayable home = null;
    while ( ! SCREENS.empty() ) 
    {
      home = ( Displayable ) SCREENS.pop();
    }
    OHandset.getDisplay().setCurrent( home );
    SCREENS.push( home );
  } // goHome


}// ScreenNavigator







