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
import javax.microedition.lcdui.Graphics;
import com.nextel.util.Debugger;
import com.nextel.util.Logger;

/**
 * A component is a distinct visual element on the screen.
 * <p>
 * Components should be placed in an 
 * {@link com.nextel.ui.OContainer OContainer},
 * which will provide focus management for the component.
 *
 * @author Glen Cordrey
 */
abstract public class  OComponent
{
  /**
   * Used with {@link #drawTriangle drawTriangle}
   */
  protected final static int UP = 1;

  /**
   * Used with {@link #drawTriangle drawTriangle}
   */
  protected final static int DOWN = 2;
  
  /**
   * Gets the height, in pixels, of the component.
   *
   * @return The height, in pixels, of the component.
   */
  abstract public int getHeight();

  /**
   * Gets the width, in pixels, of the component.
   *
   * @return The width, in pixels, of the component.
   */
  abstract public int getWidth();


  /**
   * Renders the component.
   * @see javax.microedition.lcdui.Canvas#paint
   * @param g The Graphics object to which the component is rendered.
   */
  abstract public void paint( Graphics g );
  
  private boolean needsRepaint = true;
  private int x, y; // screen coordinates
  private OContainer container;

  /**
   * Paints the component to the screen, adjusting its location by a specified
   * offset. The offset supports vertical scrolling when there are more
   * components than will fit vertically on the screen.
   *
   * @param g Graphics context to use for painting the component.
   * @param yOffset The number of pixels by which the component's y coordinate
   * should be offset.
   */
  final void paint( Graphics g, int yOffset ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OComponent.paint ENTERED" );
    int oldY = getY();
    setY( oldY - yOffset );
    paint( g );
    setY( oldY );
    if ( Debugger.ON ) Logger.dev( "OComponent.paint EXITTING" );
  } // paint

  /**
   * Whether the component needs to be repainted.
   * @return True if the component needs to be repainted.
   */
  boolean needsRepaint() 
  {return this.needsRepaint;}
  
  /**
   * Sets whether the component needs to be repainted.  You should set this to
   * true after any action which should change the appearance of the screen,
   * such as when a keypress changes the text displayed in a component.
   * @param value  True if the component should be repainted.
   */
  void setNeedsRepaint(boolean  value) 
  {this.needsRepaint = value;}
  
  /**
   * Requests that the component be repainted.
   *
   */
  final public void repaint() 
  {
    if ( Debugger.ON ) Logger.dev( "OComponent.repaint ENTERED" );
    needsRepaint = true;
    if ( container != null && container.getScreen() != null ) 
    { // container or screen can be null if setText done before added to a
      // container that is on a screen
      container.getScreen().repaint();
    }
    
    if ( Debugger.ON ) Logger.dev( "OComponent.repaint EXITTING" );
  } // repaint
  
  /**
   * Requests that the ALL component(s) be repainted.
   * This is required for dynamic components that
   * change size and shape.
   *
   */
  final public void repaintAll() 
  {
    if ( Debugger.ON ) Logger.dev( "OComponent.repaintAll ENTERED" );
    needsRepaint = true;
    if ( container != null && container.getScreen() != null ) 
    { // container or screen can be null if setText done before added to a
      // container that is on a screen
      container.getScreen().repaintAll();
    }
    
    if ( Debugger.ON ) Logger.dev( "OComponent.repaintAll EXITTING" );
  } // repaint 
  
  
  /**
   * Gets the container to which the component belongs.
   *
   * @return The container in which the component was placed, or null if the
   * component has not yet been placed in a container.
   */
  public OContainer getContainer()
  { return this.container; }
  
  /**
   * Gets the x coordinate of the component.
   * <p>
   * If the component is contained
   * within an {@link com.nextel.ui.OPanel OPanel} and the OPanel has not yet
   * been added to an {@link com.nextel.ui.OCompositeScreen OCompositeScreen}, then
   * the x coordinate will be an offset from the  x coordinate of the
   * OPanel. If the component is contained in an OPanel and the OPanel
   * <b>has</b> been added to an OCompositeScreen, then the x coordinate will be
   * the physical screen coordinate of the component.
   * @return The x coordinate of the component
   */
  public int getX() 
  {return this.x;}

  /**
   * Gets the y coordinate of the component.
   * <p>
   * If the component is contained
   * within an {@link com.nextel.ui.OPanel OPanel} and the OPanel has not yet
   * been added to an {@link com.nextel.ui.OCompositeScreen OCompositeScreen}, then
   * the y coordinate will be an offset from the  y coordinate of the
   * OPanel. if the component is contained in an OPanel and the OPanel
   * <b>has</b> been added to an OCompositeScreen, then the y coordinate will be
   * the physical screen coordinate of the component.
   * @return The y coordinate of the component.
   */
  public int getY() 
  {return this.y;}
  
  /**
   * Sets the container in which the component has been placed.
   *
   * @param container The container in which the component has been placed.
   */
  public void setContainer( OContainer container )
  { this.container = container; }

  /**
   * Sets the value of x.
   * @param value  Value to assign to x.
   */
  void setX(int  value) 
  {this.x = value;}

  /**
   * Sets the value of y.
   * @param value  Value to assign to y.
   */
  void setY(int  value) 
  {this.y = value;}
  
  /**
   * Draws a filled triangle. Values are computed from the triangle tip's x
   * and y coordinates, its maximum width, and maximum height, to ensure the
   * edges of the triangle is smooth, so the actual width and height of the
   * triangle may be less than the maximums.
   *
   * @param g The Graphics context to draw to.
   * @param tipX The x coordinate of the tip of the triangle.
   * @param tipY The y coordinate of the tip of the triangle.
   * @param maxWidth The maximum width of the base of the triangle.
   * @param maxHeight The maximum height, from base to tip, of the triangle.
   * @param direction The direction to point the triangle;
   * {@link #UP UP} or {@link #DOWN DOWN}
   */
  public static void drawTriangle( Graphics g,
				   int tipX, int tipY,
				   int maxWidth, int maxHeight, 
				   int direction ) 
  {
    // must be an odd width to display correctly
    if ( maxWidth % 2 == 0 ) maxWidth--;
    int yIncrement = 1;
    if ( direction == DOWN ) yIncrement = -1;

    // start drawing at the arrow tip
    int lineWidth = 0;
    int height = 0;
    while ( lineWidth <= maxWidth &&
	    height <= maxHeight ) 
    {
      g.drawLine( tipX, tipY ,
		  tipX + lineWidth, tipY );
      tipX--; // start one pixel over
      lineWidth += 2; // make two pixels wider 

      tipY +=  yIncrement; // next row to draw
      height++;
    }
     
  } // drawTriangle

}// OComponent
