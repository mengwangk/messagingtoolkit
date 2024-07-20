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
import javax.microedition.lcdui.Graphics;
import com.nextel.util.Logger;
import com.nextel.util.Debugger;
import com.nextel.ui.OFocusEvent;

/**
 * A radio button is a button which may be set or unset.
 * <p><b>Example:</b><img src="doc-files/RadioButtons.jpg">
 * <p>
 * Radio buttons are typically grouped in an
 * {@link com.nextel.ui.OButtonGroup} so that only one radio button may be
 * set at a time.
 * <p>
 * When an unset radio button has focus the string "SET" is displayed above the
 * right soft key, and the key is enabled so that pressing it results in the
 * radio button being set and the previously set radio button being unset.
 * Therefore, because the radio button uses the right soft key, any screen that
 * contains radio buttons <b>should not</b> use
 * {@link javax.microedition.lcdui.Command}, as that would cause conflicts as
 * described in the javadoc for this package (<code>com.nextel.ui</code>).
 * @author Glen Cordrey
 */
public class ORadioButton extends OFocusableComponent 
{
  private Font labelFont;

  private String label;
  
  private int circleDiameter;

  private boolean selected = false;

  private boolean softKeyVisible = false;
  
  private OSoftKey setKey = new OSoftKey( "SET" );

  /**
   * Creates a new <code>ORadioButton</code> instance.
   *
   * @param label The label for the button
   * @param labelFont The font for the label
   */
  public ORadioButton ( String label, Font labelFont  )
  {
    this.label = label;
    this.labelFont = labelFont;
    this.circleDiameter =
      labelFont.getHeight() - 2 /* slightly smaller than text height */;
  } // constructor

  /**
   * Sets the button's focus state to that indicated by <code>id</code>.
   *
   * @param id {@link com.nextel.ui.OFocusEvent#FOCUS_GAINED} or 
   * {@link com.nextel.ui.OFocusEvent#FOCUS_LOST}
   */
  public void setFocus( int id ) 
  { 
    if ( Debugger.ON ) Logger.dev( "ORadioButton.setFocus ENTERED" );
    super.setFocus( id );
    if ( id == OFocusEvent.FOCUS_GAINED && ( ! isSelected() ) ) 
    {
      getContainer().getScreen().addSoftKey( setKey, Graphics.RIGHT  );
      softKeyVisible = true;
    }
    else if ( id == OFocusEvent.FOCUS_LOST && softKeyVisible )
	      
    {
      getContainer().getScreen().removeSoftKey( Graphics.RIGHT );
      softKeyVisible = false;
    }
    
    if ( Debugger.ON ) Logger.dev( "ORadioButton.setFocus EXITTING" );
  } // setFocus

  /**
   * Gets whether the button is selected (i.e., set).
   * @return True if the button is set.
   */
  public boolean isSelected() 
  {return this.selected;}
   
  /**
   * Sets whether the button is to beselected (i.e., set).
   * @param True if the button is to be set.
   */
  public void setSelected(boolean  value) 
  {
    if ( value )
    {
      OContainer container = getContainer();
      if ( container instanceof OButtonGroup ) 
      {
	( ( OButtonGroup ) container ).clearAll();
      }
    }
    this.selected = value;
	
    repaint();
  }

  /**
   * Gets the height of the button.
   *
   * @return the height
   */
  public int getHeight() 
  { 
    if ( Debugger.ON ) Logger.dev( "ORadioButton.getHeight CALLED" );
    return OUILook.STRING_SPACER_HEIGHT + labelFont.getHeight();
  } // getHeight

  /**
   * Gets the width of the button
   *
   * @return the width
   */
  public int getWidth() 
  { 
    if ( Debugger.ON ) Logger.dev( "ORadioButton.getWidth CALLED" );
    return OUILook.STRING_SPACER_WIDTH + labelFont.stringWidth( label ) +
      OUILook.V_GAP + circleDiameter;
  } // getWidth

  /**
   * Paints the button to the screen.
   *
   * @param g The Graphics object to render to.
   */
  public void paint( Graphics g) 
  { 
    if ( Debugger.ON ) Logger.dev( "ORadioButton.paint ENTERED" );
    Font oldFont = g.getFont();
    int oldColor = g.getColor();

    int x = getX();
    int y = getY();
    
    // draw the circle 
    //g.setColor( OUILook.TEXT_COLOR );
    //g.drawArc( x, y, circleDiameter, circleDiameter, 0, 360 );

    // now fill in the center of the button
    // the button detent (the part of the button that is pushed-in) is separated
    // from the button outline by 1 pixel. The following use of hardcoded magic
    // numbers ensures that the detent is centered properly, so don't change
    // them without checking the results of the change
    int fillDiameter = circleDiameter - 3;
    g.setColor( OUILook.TEXT_COLOR );
    if ( ! isSelected() ) g.setColor( OColor.TRANSPARENT );
    g.fillArc( x + 2, y + 2, fillDiameter, fillDiameter, 0, 360 );

    // draw the circle 
    g.setColor( OUILook.TEXT_COLOR );
    g.drawArc( x, y, circleDiameter, circleDiameter, 0, 360 );

    // draw the label background
    x += circleDiameter + OUILook.V_GAP;
    int color = OUILook.BACKGROUND_COLOR; 
    if ( hasFocus() ) color = SELECTED_BACKGROUND_W_FOCUS;
    g.setColor( color );
    g.fillRect( x, y, labelFont.stringWidth( label ) +
		OUILook.STRING_SPACER_WIDTH, getHeight() );

    // draw the label text
    if ( hasFocus() ) color = SELECTED_FOREGROUND;
    else  color = OUILook.TEXT_COLOR;
    g.setColor( color );
    g.setFont( labelFont );    
    g.drawString( label, x + OUILook.STRING_SPACER_WIDTH,
		  y + OUILook.STRING_SPACER_HEIGHT,
		  Graphics.LEFT | Graphics.TOP );
    
    // reset to original values
    g.setFont( oldFont );
    g.setColor( oldColor );
    if ( Debugger.ON ) Logger.dev( "ORadioButton.paint EXITTING" );
  } // paint

  /**
   * Overrides the parent method and does nothing.
   * This ensures  that the background for
   * the entire component will not be reverse-videoed when the component has
   * focus. Instead, only the label background will be reverse-videoed.
   *
   * @param g The Graphics object to render to
   * @param x The x coordinate of the component
   * @param y The y coordinate of the component
   * @param width The width of the component
   * @param height The height of the component
   */
  protected void paintBox( Graphics g, int x, int y, int width,
			   int height ) 
  { 
    if ( Debugger.ON ) Logger.dev( "ORadioButton.paintBox ENTERED" );
    // we don't need a box for radio buttons, so we override and do nothing
    if ( Debugger.ON ) Logger.dev( "ORadioButton.paintBox EXITTING" );
  } // paintBox

  /**
   * Gets the label for the button.
   *
   * @return The button
   */
  public String getLabel() 
  {
    return label;
  } // getLabel

  /**
   * Responds to key presses when the button has focus.
   * The only key that results in action is the right soft key, which results in
   * the radio button being set.
   *
   * @param keyCode The pressed key.
   */
  protected void keyPressed( int keyCode ) 
  { 
    if ( Debugger.ON ) Logger.dev( "ORadioButton.keyPressed ENTERED" );
    if ( keyCode == OSoftKey.RIGHT && ( ! isSelected() ) ) 
    {
      setSelected( true );
      getContainer().getScreen().removeSoftKey( Graphics.RIGHT );
      softKeyVisible = false;
    }
    
    if ( Debugger.ON ) Logger.dev( "ORadioButton.keyPressed EXITTING" );
  } // keyPressed

}// ORadioButton
