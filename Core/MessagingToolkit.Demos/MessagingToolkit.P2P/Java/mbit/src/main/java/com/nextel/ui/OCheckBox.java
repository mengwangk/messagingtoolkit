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

/**
 * Focusable component used to display a checkbox with a text label.  
 * <p><b>Example:</b> <img src="doc-files/CheckBoxes.jpg">
 * <p>
 *<ul>
 *<li>
 * The label for a focused checkbox  is displayed using reverse-video.  
 *</li>
 *<li>
 *All other checkboxes have normal video.
 *</li>
 *<li>
 *Selected checkboxes shall draw "X" lines within boxed area to indicate a selection.
 *</li>
 * <li>
 *Not selected checkboxes shall NOT draw "X" lines within box area to indicate a non-selection.
 *</li>
 *<li>
 * Checkbox labels with multiple lines are are not supported.
 *</li>
 *<li>
 *For text labels, the pixel height and width are calculated based on the Font size, label string, and border area.
 *</li>
 *</ul>
 * <p>
 * When an unchecked check box has focus the string "CHECK" is displayed
 * above the
 * right soft key, and the key is enabled so that pressing it results in the
 * checkbox being checked. Conversely, a checked checkbox that has focus will
 * have the word "UNCHECK" displayed above the right soft key, and pressing the
 * key will result in the box being unchecked.
 * <p>
 * Because the checkbox uses the right soft key, any screen that
 * contains checkboxes <b>should not</b> use
 * {@link javax.microedition.lcdui.Command}, as that would cause conflicts as
 * described in the javadoc for this package (<code>com.nextel.ui</code>).
 * @author Anthony Paper
 */
public class OCheckBox extends OFocusableComponent
{
 
  // reduction in size of the button to make it look proportional to the text
  private final static int CIRCLE_HEIGHT_REDUCTION = 2;
  private String label;
  private Font   labelFont;
  private int labelWidth;
  private int circleDiameter;

  private boolean selected = false;

  private OSoftKey checkKey = new OSoftKey( "CHECK" );
  private OSoftKey uncheckKey = new OSoftKey( "UNCHECK" );
  private boolean softKeyVisible = false;
   
  /** Constructor method used to create a checkbox using the specified label and font.  
   *  Multiple text lines are not currently supported.
   * @param label Contains the label value for a single line
   * @param myLabelFont Contains the Font used to format and display the label string.
   */      
  public OCheckBox ( String label, Font labelFont )
  {
    this.label = label;
    this.labelFont =  labelFont;
    this.labelWidth = labelFont.stringWidth( label );
    this.circleDiameter = labelFont.getHeight() - CIRCLE_HEIGHT_REDUCTION;
  } // constructor

  /**
   * Sets the checkbox's focus state to that indicated by <code>id</code>.
   * When the checkbox has focus the right soft key will be enabled for
   * checking or unchecking the box.
   *
   * @param id {@link com.nextel.ui.OFocusEvent#FOCUS_GAINED} or 
   * {@link com.nextel.ui.OFocusEvent#FOCUS_LOST}
   */
  public void setFocus( int id ) 
  { 
    if ( Debugger.ON ) Logger.dev( "ORadioButton.setFocus ENTERED" );
    super.setFocus( id );
    if ( id == OFocusEvent.FOCUS_GAINED ) 
    {
      OSoftKey softKey = ( ! isSelected() ? checkKey : uncheckKey );
      getContainer().getScreen().addSoftKey( softKey, Graphics.RIGHT  );
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
   * Gets whether the box is checked.
   * @return True if the box is checked
   */
  public boolean isSelected() 
  {return this.selected;}
   
  /**
   * Sets whether the box is to be checked.
   * @param value  True if the box is to be checked.
   */
  public void setSelected(boolean  value) 
  {
    this.selected = value;

    if ( hasFocus() )
    { // the right soft key needs to be updated

      if ( softKeyVisible )
      { 
	getContainer().getScreen().removeSoftKey( Graphics.RIGHT );
      }
      OSoftKey softKey = ( ! isSelected() ? checkKey : uncheckKey );
      getContainer().getScreen().addSoftKey( softKey, Graphics.RIGHT  );
      softKeyVisible = true;
    }
	
    this.repaint();
  }
 
  /** Method used to return the pixel height of the checkbox.
   * @return Height value in pixels
   */   
  public int getHeight() 
  { 
    if ( Debugger.ON ) Logger.dev( "OCheckBox.getHeight CALLED" );
    return labelFont.getHeight() + OUILook.STRING_SPACER_HEIGHT;
  } // getHeight
   
  /** Method used to return the pixel width of the checkbox.
   * @return Width value in pixels
   */   
  public int getWidth() 
  { 
    if ( Debugger.ON ) Logger.dev( "OCheckBox.getWidth CALLED" );
    return labelFont.stringWidth( label ) + OUILook.V_GAP + circleDiameter;
  } // getWidth
   
  /** Method used to paint the checkbox.
   * @param g Contains the graphics object used to repaint the checkbox
   */   
  public void paint( Graphics g) 
  { 
    if ( Debugger.ON ) Logger.dev( "OCheckBox.paint ENTERED" );

    Font oldFont = g.getFont();
    int oldColor = g.getColor();
    
    int col = getX() + circleDiameter + OUILook.V_GAP;
    int row = getY();

    int backgroundColor = OUILook.BACKGROUND_COLOR;
    int foregroundColor = OUILook.TEXT_COLOR;
    if ( hasFocus() ) 
    { // fill the background if we have focus
      backgroundColor = SELECTED_BACKGROUND_W_FOCUS;
      foregroundColor = SELECTED_FOREGROUND;
    }

    // draw the label background
    g.setColor( backgroundColor );
    g.fillRect( col, row, labelWidth, getHeight() );
      
    // draw the label text
    g.setColor( foregroundColor );
    g.setFont( labelFont );    
    g.drawString( label, col, getY() + OUILook.STRING_SPACER_HEIGHT,
		  Graphics.LEFT | Graphics.TOP );

    // now prepare to draw the button

    col = getX();
    g.setColor( OUILook.TEXT_COLOR );
    g.drawRect(col,row,circleDiameter,circleDiameter);

    //
    // Let's draw lines based on whether OR NOT the
    // checkbox is currently selected.
    //
    if (isSelected()) 
    { 
      g.drawLine(col,row,col+circleDiameter,row + circleDiameter);
      g.drawLine(col,row + circleDiameter,col+circleDiameter,row);
    }
    else
    {
      g.setColor(OUILook.BACKGROUND_COLOR);
      g.drawLine(col,row,col+circleDiameter,row + circleDiameter);
      g.drawLine(col,row + circleDiameter,col+circleDiameter,row);
    }
    //
    // Let's redraw the box to make sure the
    // edges are displayed properly
    //
    g.setColor( OUILook.TEXT_COLOR );
    g.drawRect(col,row,circleDiameter,circleDiameter);

    // reset to original values
    g.setFont( oldFont );
    g.setColor( oldColor );
    if ( Debugger.ON ) Logger.dev( "OCheckBox.paint EXITTING" );
  } // paint

  /** 
   * Overrides the parent method and does nothing.
   * This ensures  that the background for
   * the entire component will not be reverse-videoed when the component has
   * focus. Instead, only the label background will be reverse-videoed.
   * @param g Parameter is not currently used.
   * @param x Parameter is not currently used.
   * @param y Parameter is not currently used.
   * @param width Parameter is not currently used.
   * @param height Parameter is not currently used.
   */   
  protected void paintBox( Graphics g, int x, int y, int width,
			   int height ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OCheckBox.paintBox ENTERED" );
    // we don't need a perimeter for radio buttons
    if ( Debugger.ON ) Logger.dev( "OCheckBox.paintBox EXITTING" );
  } // paintBox
   
  /** Method used to return the label value of the checkbox.
   * @return label value
   */
  public String getLabel() 
  {
    return label;
  } // getLabel

  /**
   * Responds to key presses when the checkbox has focus.
   * The only key that results in action is the right soft key, which results in
   * the checkbox being checked or unchecked, as appropriate.
   *
   * @param keyCode The pressed key.
   */
  protected void keyPressed( int keyCode ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OCheckBox.keyPressed ENTERED" );
    if ( keyCode == OSoftKey.RIGHT ) 
    {
      setSelected( ! isSelected() );
    }
    
    if ( Debugger.ON ) Logger.dev( "OCheckBox.keyPressed EXITTING" );
  } // keyPressed

}// OCheckBox
