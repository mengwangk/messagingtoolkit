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
import javax.microedition.lcdui.Font;
import java.util.Enumeration;
import com.nextel.util.*;

/**
 * An screen that contains {@link com.nextel.ui.OComponent}s.
 * <p>
 * <p>
 * Components can be laid out on the screen in a grid with a fixed number of
 * columns and a variable number of rows.
 * Columns and rows are numbered starting at 0; for example, in a grid with 3
 * columns the top left cell is at column 0 row 0, and the top right cell is at
 * column 2 row 0.
 * <p>
 * All columns will have the same width,
 * while the height of a row is the height of the tallest component displayed in
 * the row.  For example, if the physical screen is 111 pixels wide and you
 * specify 4 columns, each column will be 27 pixels wide (the remaining 3 pixels
 * will be used as margins, 1 on the left and 2 on the right). If a row
 * contains a component that is 20 pixels high and another that is 18 pixels
 * high, the row height is 20 pixels.  All components in a row are vertically
 * aligned against the top of the row - in the example above, both the 20-pixel
 * high and the 18-pixel high component will have the same y coordinate on the
 * screen, and the y coordinate of the bottom of the 20-pixel high component
 * will be 2 more than that of the bottom of the 18-pixel high component.
 * <p>
 * When you add a component to a cell you specify the horizontal alignment
 * of the component, i.e. whether the component is aligned with the left or
 * right side of the cell or centered in the cell.  If the component is wider
 * than the width of the cell then the component will expand in the appropriate
 * direction, i.e. the display of the component will not be truncated to fit
 * within the cell.  For example, if you place a component 32 pixels wide in a
 * cell that is 27 pixels wide and align the component along the right of the
 * cell, the first 5 pixel columns of the component will be drawn to the 5 pixel
 * columns to the left of the cell's left side.
 * <p>
o * the currently displayed components and a
 * down-arrow when there are off-screen components after (below)
 * the currently displayed components. 
 * <p>
 * <h2>Threaded Components</h2>
 * Components that implement {@link com.nextel.ui.OThread} will be automatically
 * started and stopped when the screen is displayed and hidden,
 * respectively. See {@link com.nextel.ui.OThread} for details.
 * <p>
 * <h2>Error Reporting</h2>
 * In keeping with the toolkit's philosophy of reducing toolkit size by
 * minimizing error checking, no check is made of whether a component's display
 * area exceeds the
 * screen's display width. For example, if on a 111-pixel wide screen a 30-pixel
 * wide component is left-aligned in a cell that starts at pixel 101, the last
 * 20 pixels in width of the component exceed the screen's display area.  It is
 * the caller's responsibility to ensure that this does not happen. 
 * @author Glen Cordrey
 */
public class OCompositeScreen extends OAbstractScreen implements  OContainer
{

  // y-coordinate offset within the body of the first component to display. for
  // example, if the screen container contains more components than can be
  // displayed at one time vertically, this offset will be adjusted to ensure
  // that the screen is scrolled to always display the currently-focused
  // component. The value of this variable is set by OFocusManager when the
  // focus changes 
  private int bodyOffset;

  private OContainerHelper container;

  // whether this is the first time the screen is being painted
  private boolean firstTime = true;

  // whether this is the first time the screen body is being painted
  private boolean firstBodyPaint = true;

  // true if all components are to be painted. Normally only a changed component
  // is (re)painted, but this value indicates that some operation has
  // possibly invalidated the display of other components
  private boolean paintAll = true;

  // Manages traversal of components in the container.
  private OFocusManager focusManager;

  // the number of columns to use for the grid layout
  private int nbrOfColumns;

  /**
   * Creates a new <code>OCompositeScreen</code> instance.
   * <p>
   * You might use this constructor when the title is dynamic but the number of
   * lines that it will occupy is known statically.
   *
   * @param nbrOfTitleLines The number of lines in the title
   * @param titleFont The font to use for the title
   * @param nbrOfColumns The number of columns to use for the screen display
   * grid on which components are positioned.
   */
  public OCompositeScreen ( int nbrOfTitleLines, Font titleFont,
                            int nbrOfColumns )
  {
    super( nbrOfTitleLines, titleFont );
    init( nbrOfColumns );
  } // OCompositeScreen

  /**
   * Creates a new <code>OCompositeScreen</code> instance.
   *
   * @param title The title, which may contain new-line characters ("\n") to
   * indicate line breaks.
   * @param titleFont The font to use for the title
   * @param nbrOfColumns The number of columns to use for the screen display
   * grid on which components are positioned.
   */
  public OCompositeScreen ( String title, Font titleFont, int nbrOfColumns )
  {
    super( title, titleFont );
    init( nbrOfColumns );
  } // OCompositeScreen

  /**
   * Creates a new <code>OCompositeScreen</code> instance.
   *
   * @param nbrOfColumns The number of columns to use for the screen display
   * grid on which components are positioned.
   */
  public OCompositeScreen ( int nbrOfColumns )
  {
    super();
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.ctor.1 ENTERED" );
    init( nbrOfColumns );
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.ctor.1 EXITTING" );
  } // OCompositeScreen

    protected void init( int nbrOfColumns )
  {
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.init ENTERED" );
    this.nbrOfColumns = nbrOfColumns;
    container = new OContainerHelper( this );

    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.init EXITTING" );
  } // init


  /**
   * Notifies the screen that it is being displayed
   * <p>
   * A flag is set to indicate that the entire screen is to be repainted, and
   * any components that implement {@link com.nextel.ui.OThread} will have their
   * {@link com.nextel.ui.OThread#start} methods called. 
   *
   */
  protected void showNotify()
  { // make sure the entire screen is painted
    paintAll = true;
    OComponent [] components = getComponents();
    // start any components, such as animations, that run threads
    for ( int idx=0; idx < components.length; idx++ )
    {
      if ( components[ idx ] instanceof OThread )
      {
    ( ( OThread ) components[ idx ] ).start();
      }
    }

  } // showNotify

  /**
   * Notifies the screen that it is being hidden.
   * <p>
   * Any components that implement {@link com.nextel.ui.OThread} will have their
   * {@link com.nextel.ui.OThread#stop} methods called to stop any threads that
   * the components may be running.
   *
   */
  protected void hideNotify()
  {
    OComponent [] components = getComponents();
    // start any components, such as animations, that run threads
    for ( int idx=0; idx < components.length; idx++ )
    {
      if ( components[ idx ] instanceof OThread )
      {
    ( ( OThread ) components[ idx ] ).stop();
      }
    }
  } // hideNotify

    protected int getScrollDirections()
  {
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.getScrollDirections ENTERED" );
    int directions = 0;
    if ( bodyOffset > 0 ) directions += OComponent.UP;
    if ( bodyOffset + getBodyHeight() < container.getHeight() )
      directions += OComponent.DOWN;
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.getScrollDirections EXITTING" );
    return directions;
  } // getScrollDirections


  /**
   * Paints the screen's body, which is all of the components that have been
   * added to the screen.
   *
   * @param g
   */
  protected void paintBody( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.paintBody ENTERED" );
    if ( firstTime && getComponentCount() > 0 )
    { // a screen may not contain components, for example if it is used only
      // for drawing strings
      focusManager = new OFocusManager( this );
    }
    firstTime = false;

    if ( getComponentCount() > 0 )
    {
      if ( firstBodyPaint )
      {
    firstBodyPaint = false;
    layoutContainer( getBodyRow() );
      }
      if ( paintAll ) paintBodyBackground( g );

      // now paint the components in the screen's container
      container.paint( g, paintAll );
    }
    paintAll = false;


    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.paintBody EXITTING" );
  } // paintBody

  protected void paintBodyBackground( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.paintBodyBackground ENTERED" );
    if ( paintAll ) super.paintBodyBackground( g );
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.paintBodyBackground EXITTING" );
  } // paintBodyBackground

  /**
   * Processes a press of a key.
   * <p>
   * Any {@link java.lang.Throwable} that is thrown within the thread the
   * processes the key press and which is not caught and consumed will cause the
   * current screen to be replaced by a screen that shows the exception.
   * <p>
   * If {@link com.nextel.util.Debugger#LOG_MEM} is <code>true</code> and the
   * menu key
   * was pressed, a screen will be displayed showing the amount of free and used
   * memory available currently and at each previous such press of the menu key.
   * The content of this screen is described in
   * {@link com.nextel.ui.OAbstractScreen#showMem }
   *
   * @param keyCode The code of the pressed key.
   */
  public void keyPressed( int keyCode )
  {
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.keyPressed ENTERED w/keyCode=" +
                keyCode );
    try
    {
      if ( keyCode == OSoftKey.LEFT || keyCode == OSoftKey.RIGHT )
      {
    OSoftKey softKey = getSoftKey( keyCode );
    if ( softKey != null )
    {
      OCommandAction action = getSoftKey( keyCode ).getAction();
      if ( action != null )
      {
        action.performAction();
        return;
      }
      // else  below the soft key will be passed to the component for
      // processing
    }
    else  // no soft key registered
    {
      OHandset.beep();
      return;
    }
      } // end of soft key processing

      // we come here if the key pressed wasn't a soft key, but also if it was
      // and an OSoftKey without an action was registered
      if ( getComponentCount() > 0 )
      {
    focusManager.keyPressed( keyCode );
      }
    }
    catch( Throwable ex )
    {
      Logger.ex( ex );
      displayEx( ex, null );
    }
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.keyPressed EXITTING" );
  } // keyPressed


  // Code for delegation of com.nextel.ui.OContainer methods to container


  /**
   * Gets the component of the specified rank.
   *
   * @param rank Rank of the desired component.
   * @return component of the specified rank
   */
  public OComponent getComponent(int rank )
  {
    return container.getComponent( rank );
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
   * @return The components in the container.
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
    return this;
  } // getScreen

  /**
   * Sets the screen that contains this container.  This method is here just to
   * satisfy the OContainer interface, since this IS the screen.  
   *
   * @param screen Any value other than this screen itself will result in a
   * RuntimeException 
   */
  public void setScreen( OCompositeScreen screen )
  {
    if ( screen != this )
    {
      throw new RuntimeException( "Attempted to change screen setting" );
    }
  } // setScreen

  /**
   * Paints this container to the screen.
   *
   * @param g The Graphics object to which the container's contents are painted.
   * @param all True if all of the container's components are to be painted,
   * false if you want to paint only those components whose appearance may
   * have changed since the screen was last painted.  Painting only those
   * changed components will reduce screen flicker. This argument does NOT force
   * a repaint of the skeleton or soft keys. 
   */
  public void paint( Graphics g, boolean all)
  {
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.paint ENTERED" );
    container.paint( g, all );
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.paint EXITTING" );
  } // paint


  /**
   * Adds a component to the screen.
   * <p>
   * For components to be displayed properly you <b>MUST</b> add them
   * from left-to-right, top-to-bottom.
   * For example,
   * <pre><code>
   * <b>CORRECT</B>             <b>INcorrect</b>
   * add( c1, 0, 0,    add( c1, 0, 0,
   * add( c2, 2, 0,    add( c3, 0, 1,
   * add( c3, 0, 1,    add( c2, 2, 0,
   * add( c4, 2, 1,    add( c4, 2, 1,
   * </code></pre>
   *
   * @param component The component to add.
   * @param gridX The grid column in which to place the component.
   * @param gridY The grid row in which to place the component
   * @param h_alignment The horizontal alignment of the component within the
   * grid cell. One of
   * {@link javax.microedition.lcdui.Graphics#LEFT Graphics.LEFT} or
   * {@link javax.microedition.lcdui.Graphics#RIGHT Graphics.RIGHT} or
   * {@link javax.microedition.lcdui.Graphics#HCENTER Graphics.HCENTER}
   */
  public void add( OComponent component,
                   int gridX, int gridY, int h_alignment )
  {
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.add ENTERED" );
    component.setContainer( this );
    if ( component instanceof OContainer )
    {
      ( ( OContainer ) component ).setScreen( this );
    }
    container.add( new OContainerItem( component,
                       gridX, gridY , h_alignment ) );

    if ( Debugger.ON ) Logger.dev( "OContainerHelper.add EXITTING" );
  } // add


  /**
   * Lays out and aligns the components at the specified grid cells.
   *
   * @param startY The y coordinate at which to start the layout.
   */
  private void layoutContainer( int startY )
  {
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.layoutContainer ENTERED" );
    int colWidth = getWidth() / nbrOfColumns;
    // use 1/2 of leftover space for left margin
    int leftMargin = ( getWidth() % nbrOfColumns ) / 2;

    int componentY = startY;
    int currentRow = 0;
    int rowHeight = 0;
    for ( Enumeration enum = container.getItems(); enum.hasMoreElements();)
    {
      OContainerItem item = ( OContainerItem ) enum.nextElement();
      if ( item.gridY > currentRow )
      { // this component is in a new row, and since row heights vary depending
    // upon the max height of the components in the row, we need to set
    // where the next row begins
    componentY += OUILook.V_GAP + rowHeight;
    currentRow = item.gridY;
    rowHeight = item.component.getHeight();
      }
      else // save the height of the tallest component in the row
      {
    rowHeight = Math.max( rowHeight, item.component.getHeight() );
      }

      // set component's x position relative to its alignment
      int componentX = leftMargin + ( item.gridX * colWidth );
      if ( item.h_alignment == Graphics.HCENTER )
      {
    componentX += ( colWidth - item.component.getWidth() ) / 2;
      }
      else  if ( item.h_alignment == Graphics.RIGHT )
      {
    componentX += ( colWidth - item.component.getWidth() - 1 );
      }
      // else  default to Left aligned
      item.component.setX( componentX );
      item.component.setY( componentY );
      if ( item.component instanceof OPanel )
      {
    OPanel panel = ( OPanel ) ( ( OContainerItem ) item ).component;

    // if the component is itself a container, then the components it
    // contains have coordinates relative to the container's, so
    // we need to correct the coordinates of those components.
    OComponent [] components = panel.getComponents();
    for ( int idx=0; idx < components.length; idx++ )
    {
      OComponent subComponent = components[idx];
      subComponent.setX( subComponent.getX() + componentX );
      subComponent.setY( subComponent.getY() + componentY );
    }
      }
    }
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.layoutContainer EXITTING" );
  } // layoutContainer


  /**
   * Method used to notify container that all
   * components need to get repainted.
   */
  public void repaintAll()
  {
    paintAll = true;
    repaint();
  }

  /**
   * Gets the focus manager that manages focus for this screen.
   *
   * @return The focus manager for this screen.
   */
  OFocusManager getFocusManager()
  {
    return focusManager;
  } // getFocusManager

    /**
     * Adjusts the screen display body's offset 
     *
     * @param component
     */
  void adjustBodyOffset( OFocusableComponent component )
  {
    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.adjustBodyOffset ENTERED" );

    int componentBegin = component.getY();
    if ( componentBegin < getBodyRow() + bodyOffset )
    { // component is above the body display area
      bodyOffset = component.getY() - getBodyRow() - LINE_SPACER;
      repaintAll();
    }
    else  if ( componentBegin + component.getHeight()  >
           bodyOffset +
           ( getBodyHeight() + getBodyRow() ) )
    { // end of component is below the viewport
      bodyOffset = component.getY() + component.getHeight() + LINE_SPACER +
    3 /* magic number for best appearance */ -
    getBodyHeight() - getBodyRow();
      repaintAll();
    }

    if ( Debugger.ON ) Logger.dev( "OCompositeScreen.adjustBodyOffset EXITTING" );
  } // adjustBodyOffset

  int getBodyOffset()
  {
    return bodyOffset;
  } // getBodyOffset

}// OCompositeScreen




