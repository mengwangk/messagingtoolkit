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
import com.nextel.util.Debugger;
import com.nextel.util.Logger;

/**
 * A container for radio buttons, which ensures that at any time only one button
 * is set.
 * @author Glen Cordrey
 */
public class OButtonGroup extends OPanel
{
  /**
   * Creates a new <code>OButtonGroup</code> instance.
   *
   */
  public OButtonGroup ( )
  {
    super();

  } // constructor

  /**
   * Creates a new <code>OButtonGroup</code> instance.
   *
   * @param title Title to be displayed for the button group.
   * @param titlePosition Position to place the title - either
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.TOP} or
   * {@link javax.microedition.lcdui.Graphics#LEFT Graphics.LEFT}. Any other
   * value will result in
   * {@link javax.microedition.lcdui.Graphics#LEFT} being used.
   */
  public OButtonGroup( OLabel title, int titlePosition )
  {
    super( title, titlePosition );
    if ( Debugger.ON ) Logger.dev( "OButtonGroup.OButtonGroup EXITTING" );
  } // OButtonGroup

  /**
   * Gets the currently-depressed radio button.
   *
   * @return The currently-depressed radio button.
   */
  public ORadioButton getSelection()
  {
    if ( Debugger.ON ) Logger.dev( "OButtonGroup.getSelection ENTERED" );
    OComponent [] components = getComponents();
    ORadioButton button;
    for ( int idx=0; idx < components.length; idx++ )
    {
      if ( components[ idx ] instanceof ORadioButton )
      { // component may not be a ORadioButton if the button group constructor
    // taking a label was used, because the label will also be on the
    // components array
    button = ( ORadioButton ) components[ idx ];
    if ( button.isSelected() )
    {
      return button;
    }
      }
    }
    return null;

  } // getSelection

  /**
   * Resets all of the radio buttons so that no button is set.
   *
   */
  public void clearAll()
  {
    if ( Debugger.ON ) Logger.dev( "OButtonGroup.clearAll ENTERED" );
    OComponent [] components = getComponents();
    for ( int idx=0; idx < components.length; idx++ )
    {
      OComponent component = components[idx];
      if ( component instanceof ORadioButton )
      { // need this check because the button group panel may also include an
    // OLabel
    ( ( ORadioButton ) component ).setSelected( false );
      }
    }

    if ( Debugger.ON ) Logger.dev( "OButtonGroup.clearAll EXITTING" );
  } // clearAll

}// OButtonGroup
