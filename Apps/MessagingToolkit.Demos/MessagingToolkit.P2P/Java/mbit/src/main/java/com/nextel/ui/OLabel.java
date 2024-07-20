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
import com.nextel.util.StringUtils;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import com.nextel.util.Debugger;
import com.nextel.util.Logger;

/**
 * Provides a text label for display on the screen.  A label is not focusable,
 * meaning that it does not receive key press events.
 *
 * @author Glen Cordrey
 */
public class OLabel extends OComponent 
{
  private int height;
  private int width;
   
  private String [] label;
  private Font labelFont;

  /**
   * Creates a new <code>OLabel</code> instance.
   *
   * @param text The text for the label.  The text can be multiple lines, with
   * \n characters used to indicate line breaks. It is up to the caller to
   * ensure that lines do not exceed the screen width, as this class does not
   * validate the width.
   * @param textFont The font to use for the label text.
   */
  public OLabel ( String text, Font textFont )
  {
    labelFont = textFont;
    label = StringUtils.breakIntoLines( text );
    for ( int idx=0; idx < label.length; idx++ ) 
    {
      int lineWidth = textFont.stringWidth( label[ idx ] ); 
      if ( lineWidth > width ) 
      {
	width = lineWidth;
      }
    }
    height = label.length * textFont.getHeight() +
      OUILook.STRING_SPACER_HEIGHT;
      
  } // constructor
  
  /**
   * Paints the label to the screen.
   *
   * @param g The Graphics object to render to.
   */
  public void paint( Graphics g ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OLabel.paint ENTERED" );

    Font oldFont = g.getFont();
    int oldColor = g.getColor();
    g.setFont( labelFont );
    g.setColor( OUILook.TEXT_COLOR );
    int fontHeight = labelFont.getHeight();
    int y = getY() + OUILook.STRING_SPACER_HEIGHT;
    for ( int idx=0; idx < label.length; idx++ ) 
    {
      g.drawString( label[ idx ], getX(), y,
		    Graphics.TOP | Graphics.LEFT );
      y += fontHeight;
    }

    g.setFont( oldFont );
    g.setColor( oldColor );
    if ( Debugger.ON ) Logger.dev( "OLabel.paint EXITTING" );
  } // paint

  /**
   * Gets the height of the label.
   *
   * @return an <code>int</code> value
   */
  public int getHeight() 
  { 
    if ( Debugger.ON ) Logger.dev( "OLabel.getHeight ENTERED" );
    return height;
  } // getHeight
  
  /**
   * Gets the width of the label.  If the label contains multiple lines then the
   * width of the widest line is the width of the labe.
   *
   * @return an <code>int</code> value
   */
  public int getWidth() 
  { 
    if ( Debugger.ON ) Logger.dev( "OLabel.getWidth ENTERED" );
    return width;
  } // getWidth
  
}// OLabel
