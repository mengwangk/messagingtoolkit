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
import com.nextel.util.*;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;

/**
 * A bar for indicating that a display contains more items than are displayed.
 * <p>
 * The bar contains up and down arrows at its ends, and a slider between the
 * ends. The slider indicates both where the top of the items displayed
 * is relative to the total number of items and what percentage of the total
 * number of items is currently visible.  For example, if the scrollbar is for a
 * set of 100 items of which only 7 are visible at a time, and the first of the
 * 7 currently visible items is the 43rd item, then the slider will occupy 7% of
 * the available scrollbar region and the top of the slider will start 43% of
 * the way down from the top of the scrollbar region.
 * @author Glen Cordrey
 */
public class OScrollBar
{
  /** font to use for calculating scrollbar size */
  private static final Font SCROLLBAR_FONT = OUILook.PLAIN_MEDIUM;

  /** width of a vertical scrollbar */
  private static  int SCROLLBAR_WIDTH = SCROLLBAR_FONT.charWidth( 'W' );
  static
  { // ensure scrollbar width is odd so arrow centers
    if ( SCROLLBAR_WIDTH % 2 == 0 ) SCROLLBAR_WIDTH--;
  }
  
  /** Height of the end boxes in a vertical scrollbar */
  private static final int SCROLLBAR_END_BOX_HEIGHT =
    SCROLLBAR_FONT.getHeight( ) / 2;

  /** Width of an arrow in a vertical scrollbar */
  private static  int SCROLLBAR_ARROW_WIDTH = SCROLLBAR_WIDTH - 2;
  private static int SCROLLBAR_ARROW_HEIGHT = SCROLLBAR_END_BOX_HEIGHT - 2;
  
  /** Color of scrollbar arrow */
  private static final int SCROLLBAR_ARROW_COLOR = OColor.BLACK;

  /** Color of scrollbar slider */
  private static final int SCROLLBAR_SLIDER_COLOR = OColor.BLACK;

  // The height of the scroll bar in number of pixels
  private int height;
  
  /** 
      Indicates whether the scrollbar should only draw the DOWN Arrow.
      The default value is false and both arrows shall be displayed.
   */
  private boolean downArrowOnlyFlag = false;
  
  public void setDownArrowOnlyFlag(boolean flag)
  {
      downArrowOnlyFlag = flag;
  }
  /**
   * Creates a new <code>OScrollBar</code> instance.
   *
   */
  public OScrollBar ()
  {
    super();
    if ( Debugger.ON ) Logger.dev( "OScrollBar ctor CALLED " );
  } // constructor

  /**
   * Paints the scroll bar on the screen.
   *
   * @param g The graphics context of the scroll bar.
   * @param x The pixel column of the scroll bar.
   * @param y The pixel row of the scroll bar.
   * @param height The height of the scroll bar in pixels
   * @param sliderBegin The percentage of the scroll bar
   * where the slider is to begin.
   * @param sliderSize The percentage of the scroll bar which the slider is to
   * occupy. 
   */
  public void paint ( Graphics g, int x, int y, int height,
		      int sliderBegin, int sliderSize  )
  {
    if ( Debugger.ON ) Logger.dev( "OScrollBar. paint ENTERED " );
    this.height = height;
    
    int oldColor = g.getColor();
    Font oldFont = g.getFont();

    // fill the scrollbar background
    g.setColor( OUILook.BACKGROUND_COLOR );
    g.fillRect( x, y, SCROLLBAR_WIDTH, height );
    
    int scrollBarCol = x;
    int scrollBarRow = y;
    
    // draw the up and down arrows
    g.setColor( SCROLLBAR_ARROW_COLOR );
    int arrowPointColumn = x + ( SCROLLBAR_WIDTH / 2 );
    
    //
    // For DropDownList(s) we only want to draw the DOWN arrow.
    //
    if (downArrowOnlyFlag == false) {
        OComponent.drawTriangle( g, arrowPointColumn, y + OUILook.V_GAP,
                                SCROLLBAR_ARROW_WIDTH,
                                SCROLLBAR_ARROW_HEIGHT, OComponent.UP );
    }
    
    OComponent.drawTriangle( g, arrowPointColumn, y + height - OUILook.V_GAP,
			     SCROLLBAR_ARROW_WIDTH,
			     SCROLLBAR_ARROW_HEIGHT, OComponent.DOWN );

    // outline the scrollbar 
    int scrollBarLeftEdge = x - 1;
    g.drawRect( scrollBarLeftEdge, y, SCROLLBAR_WIDTH, height );

    // draw line below top box end
    int lineRow = scrollBarRow + SCROLLBAR_END_BOX_HEIGHT + 1;
    g.drawLine( scrollBarLeftEdge, lineRow,
		scrollBarLeftEdge + SCROLLBAR_WIDTH, lineRow );

    // draw line above bottom end
    lineRow = y + this.height - SCROLLBAR_END_BOX_HEIGHT - 1;
    g.drawLine( scrollBarLeftEdge, lineRow,
		scrollBarLeftEdge + SCROLLBAR_WIDTH, lineRow );
		
    //
    // For DropDownList(s) we only want to draw the DOWN arrow.
    //
    if (downArrowOnlyFlag == false) {
       paintSlider( g, scrollBarCol, scrollBarRow, sliderBegin, sliderSize );
    }
    
    g.setColor( oldColor );
    g.setFont( oldFont );
    if ( Debugger.ON ) Logger.dev( "OScrollBar.paint EXITTING" );

  } // paint


  /**
   * Paints the slider on the scrollbar.
   *
   * @param g The graphics context for the scrollbar.
   * @param scrollBarX The pixel column of the scrollbar.
   * @param scrollBarY The pixel row of the scrollbar.
   * @param sliderBegin The percentage of the scroll bar
   * where the slider is to begin.
   * @param sliderSize The percentage of the slider area that the slider is to
   * occupy. 
   */
  protected void paintSlider ( Graphics g, 
			       int scrollBarX, int scrollBarY,
			       int sliderBegin, int sliderSize  ) 
  {
    if ( Debugger.ON ) Logger.dev( "OScrollBar.paintSlider ENTERED " );
    int oldColor = g.getColor();
    
    int maxHeight = this.height - ( SCROLLBAR_END_BOX_HEIGHT * 2 );
    int beginRow = scrollBarY + SCROLLBAR_END_BOX_HEIGHT + 
      Math.max( 1, ( sliderBegin * maxHeight ) / 100 );
    
    // NOTE: Below, we adjust the slider length to ensure that if it is 
    // at the bottom, there is no visible line separating the slider from the
    // bottom end box
    int sliderLength = ( ( sliderSize * maxHeight ) / 100 );// + 1;
    if ( sliderLength == 0 ) 
    {
      sliderLength = 2;
    }
    else  if ( sliderLength > maxHeight ) 
    {
      sliderLength = maxHeight;
    }
    
    g.setColor( SCROLLBAR_SLIDER_COLOR );
    g.fillRect( scrollBarX, beginRow,
		SCROLLBAR_WIDTH, sliderLength );
    
    g.setColor( oldColor );
    if ( Debugger.ON ) Logger.dev( "OScrollBar.paintSlider EXITTING" );
  } // paintSlider

  /**
   * Gets the width in pixels of the scrollbar.
   *
   * @return The width in pixels
   */
  public int getWidth () 
  {
    if ( Debugger.ON ) Logger.dev( "OScrollBar.getWidth CALLED, returning " +
				SCROLLBAR_WIDTH );
    return SCROLLBAR_WIDTH;
  } // getWidth

  /**
   * Gets the height in pixels of the scrollbar
   *
   * @return The height in pixels
   */
  public int getHeight () 
  {
    if ( Debugger.ON ) Logger.dev( "OScrollBar.getHeight CALLED, returning "
				+ this.height );
    return this.height;
  } // getHeight


}// OScrollBar

