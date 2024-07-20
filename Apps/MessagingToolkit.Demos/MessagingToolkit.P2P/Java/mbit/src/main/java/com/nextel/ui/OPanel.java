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
import javax.microedition.lcdui.Graphics;
import com.nextel.ui.OUILook;
import java.lang.Math;
import com.nextel.ui.OContainerHelper;
import com.nextel.ui.OComponent;

/**
 * A panel is a component that is itself a composition of one or more
 * components.
 * <p>
 * Components are laid out in the panel starting with the top left
 * corner of the panel. Components are added to a panel in either a flow
 * direction or via relative coordinates.
 * <p>
 * In a flow, components are either horizontally or vertically
 * adjacent, and are added via
 * {@link #add( OComponent )}. For example, if you want a panel
 * to contain three fields sided-by-side you would use horizontal flow,
 * but if you want a panel to contain three fields one above the other you would
 * use a vertical flow.
 * <p>
 * The default flow for a panel is horizontal; to change
 * the flow use {@link #setFlow setFlow}.
 * <p>
 * If you change flows within a panel
 * the components added with the new flow start at the left hand side.  For
 * example,  the code
 * <pre><code>
 * Panel myPanel = new Panel();
 * myPanel.add( componentA );
 * myPanel.add( componentB );
 * myPanel.setFlow( VERTICAL );
 * myPanel.add( componentC );
 * myPanel.add( componentD );
 * </code></pre>
 * would result in a layout like
 * <pre>
 * componentA componentB
 * componentC
 * componentD
 * </pre>
 *<p>
 * You can use {@link #add( OComponent, int, int )} to position components more
 * precisely by adding them with relative coordinates. If in one panel
 * you mix adding components by flow and by relative coordinates you are
 * responsible for ensuring that component display regions do not overlap
 * and are positioned correctly.
 * @author Glen Cordrey
 */
public class OPanel extends OComponent implements OContainer
{
  /**
   * Used with {@link #setFlow} to set the flow direction to horizontal.
   */
  public final static int HORIZONTAL = 1;

  /**
   * Used with {@link #setFlow} to set the flow direction to vertical.
   */
  public final static int VERTICAL = -1;
  
  private int flowDirection = HORIZONTAL;
  
  private OContainerHelper container;
  
  private int nextX = 0;
  private int nextY = 0;

  // maximum x and y positions of the components in the panel
  private int maxX, maxY;
  
  /**
   * Creates a new <code>OPanel</code> instance.
   *
   */
  public OPanel() 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.OPanel1  ENTERED" );
    container = new OContainerHelper(  );
    if ( Debugger.ON ) Logger.dev( "OPanel.OPanel1 EXITTING" );
  } // OPanel
  
  /**
   * Creates a new <code>OPanel</code> instance.
   *
   * @param title Title for the panel
   * @param titlePosition Position to place the title - either
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.TOP} or
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.LEFT}. Any other
   * value will result in
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.LEFT} being used.
   */
  public OPanel( OLabel title, int titlePosition )
  {
    if ( Debugger.ON ) Logger.dev( "OPanel.OPanel2 ENTERED" );
    title.setX( 0 );
    title.setY( 0 );
    container = new OContainerHelper( );
    container.add( title );

    if ( titlePosition == Graphics.TOP )
    {
      nextY = maxY = title.getHeight();
    }
    else  // Graphics.LEFT
    {
      nextX = maxX = title.getWidth();
    }
    if ( Debugger.ON ) Logger.dev( "OPanel.OPanel2 EXITTING" );
  } // OPanel

  /**
   * Sets the flow direction for adding components
   *
   * @param direction {@link #HORIZONTAL} or {@link #VERTICAL}
   */
  public void setFlow( int direction ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.setFlow ENTERED" );
    if ( direction == VERTICAL )
    {
      nextY = maxY + OUILook.V_GAP;
    }
    else  if ( direction != HORIZONTAL )
    {
      throw new OUIError( "OPanel.setFlow: invalid direction " + direction );
    }
    nextX = 0;
    flowDirection = direction;
    
    if ( Debugger.ON ) Logger.dev( "OPanel.setFlow EXITTING" );
  } // setFlow
  
  /**
   * Adds a component to the panel horizontally or vertically adjacent to the
   * last component. It is the caller's responsibility to ensure that the
   * display area of the component will not exceed the display area of the
   * screen. 
   *
   * @param component The component to add to the panel
   * <p><b>Nested containers are not allowed; adding an OContainer (such as an
   * OPanel) to OPanel will cause an
   * {@link com.nextel.ui.OUIError OUIError}</b>
   */
  public void add( OComponent component ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.add ENTERED" );
    if ( component instanceof OContainer ) 
    {
      throw new OUIError( "Nested panels disallowed" );
    }
    
    add( component, nextX, nextY );
    
    if ( flowDirection == HORIZONTAL ) 
    {
      nextX += component.getWidth() + OUILook.H_GAP;
    }
    else  //  ( flowDirection == VERTICAL ) 
    {
      nextY += component.getHeight() + OUILook.V_GAP;
    }
    
    if ( Debugger.ON ) Logger.dev( "OPanel.add EXITTING" );
    
  } // add

    /**
     * Adds a component to the panel, with the component offset from the panel's
     * top left corner by a specified number of pixels.
     *
     * @param component The component to add
     * <p><b>Nested containers are not allowed; adding an OContainer will cause an
     * {@link com.nextel.ui.OUIError OUIError}</b>
     * @param xOffset The number of pixels from the top left corner of the panel
     * to horizontally offset the top left corner of the component.
     * @param yOffset The number of pixels from the top left corner of the panel
     * to vertically offset the top left corner of the component.
     */
  public void add( OComponent component, int xOffset, int yOffset ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.add ENTERED" );
    if ( component instanceof OContainer ) 
    {
      throw new OUIError( "Nested panels disallowed" );
    }
    component.setX( xOffset );
    component.setY( yOffset );
    component.setContainer( this );

    // determine if t his component has the maximum x and/or y coordinates
    maxX = Math.max( maxX, nextX + component.getWidth() );
    maxY = Math.max( maxY, nextY + component.getHeight() );
    
    container.add( component );
 
    if ( Debugger.ON ) Logger.dev( "OPanel.add EXITTING" );
  } // add

  
  /**
   * Gets the height in pixels of the panel.
   *
   * @return an <code>int</code> value
   */
  public int getHeight() 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.getHeight CALLED, returning " +
				this.maxY );
    return this.maxY;

  }
  
  /**
   * Gets the width in pixels of the panel.  
   *
   * @return The width in pixels of the panel.
   */
  public int getWidth() 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.getWidth CALLED, maxX=" + maxX );
    return maxX;
  }

  
  // Code for delegation of com.nextel.ui.OContainerImpl methods to container
  
  /**
   * Paints the panel to the display.
   * @param Graphics The Graphics object to which the panel is painted
   */
  public void paint( Graphics g ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.paint ENTERED" );

    container.paint( g );
    
    if ( Debugger.ON ) Logger.dev( "OPanel.paint EXITTING" );
  } // paint

  /**
   * Paints the panel to the display.
   * 
   * @param Graphics The Graphics object to which the panel is painted
   * @param all true if all components are to painted, false if only those
   * components whose appearance may have changed are painted
   */
  public void paint( Graphics g, boolean all ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OPanel.paint ENTERED" );
    container.paint( g, all );
    if ( Debugger.ON ) Logger.dev( "OPanel.paint EXITTING" );
  } // paint

  /**
   * Gets the component of the specified rank.
   * <p>
   * The rank is the order in which the component was added to the container.
   *
   * @param rank Rank of the desired component.
   * @return The component of the specified rank.
   */
  public OComponent getComponent(int rank)
  {
    return container.getComponent( rank);
  }
  
  /**
   * Gets the number of components in the container.
   *
   * @return The number of components in the container
   */
  public int getComponentCount()
  {
    return container.getComponentCount();
  }
  
  /**
   * Gets the components in the container
   *
   * @return an <code>Enumeration</code> of the components in the container
   */
  public OComponent [] getComponents()
  {
    return container.getComponents();
  } // getComponents

  /**
   * Gets the screen that contains this container.
   *
   * @return The screen containing this container, or a null if this container
   * has not yet been added to a screen.
   */
  public OCompositeScreen getScreen() 
  {
    if ( Debugger.ON ) Logger.dev( "OPanel.getScreen CALLED, container=" +
				container ); 
    
    return container.getScreen();
  } // getScreen

  /**
   * Sets the screen that contains this container.
   *
   * @param screen The screen that contains this container.
   */
  public void setScreen( OCompositeScreen screen ) 
  {
    container.setScreen( screen );
  } // setScreen


}// OPanel
