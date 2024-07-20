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
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;

/**
 * The component displays a 3-Dimmensional bordered list of scrollable items.
 * <p>
 * The component shall automatically display a vertical scroll bar to the right of
 * the list.  The scroll bar is enabled based on the available pixel height, fonts,
 * and items to populate.  If all items are displayed (i.e., all items fit within
 * the height available for the display) then no scroll bar is displayed.
 * <p>
 * The currently selected item is reverse-videoed while unselected items have
 * normal video.  The currently selected item may optionally be in a different
 * font than the unselected items.
 *
 * @author Anthony Paper
 */
public class O3DScrollList extends OScrollList
{
  /**
   * Contains the shadow thickness value used to render 
   * the right and bottom 3-dimmensional border
   */
   public final static int shadowThickness = 2;

  /**
   * Contains the shadow indentation value used to render 
   * the right and bottom 3-dimmensional border
   */
   public final static int shadowIndent = 2;

  /**
   * Creates a new <code>O3DScrollList</code> instance.
   *
   * @param width The width of the scroll list.  If any items in the list are
   * wider than this their display will be truncated.
   * @param maxHeight The height available for the scroll list.  The
   * actual height of the list may be less than this, and is derived from the
   * number of lines which can be displayed in the maxHeight.
   * @param selectedFont Font to use for the selected item
   * @param unselectedFont Font to use for the unselected items
   */
  public O3DScrollList ( int width,
                         int maxHeight,
                         Font selectedFont,
                         Font unselectedFont )
  {
    super(width, maxHeight, selectedFont, unselectedFont);
    if ( Debugger.ON ) Logger.dev( "O3DScrollList.ctor.3 ENTERED" );

    if ( Debugger.ON ) Logger.dev( "O3DScrollList.init EXITTING" );

  } // O3DScrollList

  /**
   * Gets the height of the scroll list.  This height includes
   * the 3D border at the bottom of the component.
   * @return Height of the scroll list. The height will be 0 until
   * {@link #populate populate} is called.
   */
  public int getHeight()
  {
    if ( Debugger.ON ) Logger.dev( "O3DScrollList.getHeight CALLED, returning " +
                super.getHeight() + shadowThickness );
    return (super.getHeight() + shadowThickness);
  }

  /**
   * Gets the width of the scroll list, including the scroll bar and the
   * 3D border for the right and bottom.
   * @return Width of the scroll list.
   */
  public int getWidth()
  {
    if ( Debugger.ON ) Logger.dev( "O3DScrollList.getWidth CALLED, returning " +
                super.getHeight() + shadowThickness);
    return (super.getWidth() + shadowThickness);
  }

  /**
   * Paints the scroll list on the screen.
   *
   * @param g The graphics context
   */
  public void paint ( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "O3DScrollList.paint ENTERED" );

    int oldColor = g.getColor(); // save so we can restore it
    Font oldFont = g.getFont();
    //
    // Paint the scrolled list
    //
    super.paint(g);

    //
    // Paint the 3D border
    //
    int x = getX();
    int y = getY();
    int w = getWidth();
    int h = getHeight();

    //
    // For the number of lines to draw based on the shadow thickness
    //
    for (int index=0; index < shadowThickness; index++)
    {
       // 
       // Draw Right border for index
       //
       g.drawLine(x+w-index,y+shadowIndent,x+w-index,y+h);

       //
       // Draw Left border for index
       //
       g.drawLine(x+shadowIndent,y+h-index,x+w,y+h-index);
    }


    g.setColor( oldColor );
    g.setFont( oldFont );
    if ( Debugger.ON ) Logger.dev( "O3DScrollList.paint EXITTING" );

  } // paint

}// O3SDcrollList
