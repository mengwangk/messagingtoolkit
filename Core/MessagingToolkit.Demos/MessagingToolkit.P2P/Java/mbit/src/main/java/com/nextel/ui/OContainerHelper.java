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

import java.util.Enumeration;
import javax.microedition.lcdui.Graphics;
import java.util.Vector;
import com.nextel.util.Debugger;
import com.nextel.util.Logger;
import java.lang.Math;

/**
 *  An helper for implementing {@link com.nextel.ui.OContainer OContainer}.
 *
 */
class OContainerHelper implements OContainer
{
  /** A value is currently undefined, for instance if the user of this adapter
      has not overridden getHeight or getWidth **/
  public final static int UNDEFINED = -1;
  
  // assume nominal 6 components
  private Vector componentItems = new Vector( 6 );
  
  // all components in the container need to be painted
  private boolean needsRepaint = true;
  
  private OCompositeScreen screen;
  private int lastRowIndex;
  
  /** <b>NOTE: can accomodate a maximum of 50 rows</b> **/
  private int [] rowHeights = new int [ 50 ];
  
  /**
   * Creates a new <code>OContainerHelper</code> instance.
   */
  public OContainerHelper() 
  {
  } // OContainerHelper

  /**
   * Creates a new <code>OContainerHelper</code> instance.
   *
   * @param screen The screen that contains this container.
   */
  public OContainerHelper( OCompositeScreen screen ) 
  {
    this(  );
    this.screen = screen;
  } // OContainerHelper

    void add( OContainerItem item ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.add1 ENTERED" );
    componentItems.addElement( item );
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.add1 EXITTING" );  
  } // add

    void add( OComponent component ) 
  {
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.add2 ENTERED" );
    componentItems.addElement( component );
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.add2 EXITTING" );
  } // add
  
  /**
   * Gets the component of the specified rank.
   *
   * @param rank Rank of the desired component.
   * @return a <code>OFocusableComponent</code> value
   */
  public OComponent getComponent (int rank)
  {
    Object obj = componentItems.elementAt( rank );
    return ( obj == null ? null : ( OComponent ) obj );
  } // getComponent

  /**
   * Resets the container to it's state after all components were added.
   * Specifically, the first component in the container will have focus.
   *
   */
  public int getComponentCount ()
  {
    return componentItems.size();
  }

  /**
   * Gets the components in the container.
   *
   * @return an <code>OComponent[]</code> value
   */
  public OComponent [] getComponents() 
  {
    OComponent [] returnVal = new OComponent[ componentItems.size() ];
    int idx = 0;
    for ( Enumeration items = componentItems.elements();
	  items.hasMoreElements(); ) 
    {
      Object item = items.nextElement();
      if ( item instanceof OComponent ) 
      {
	returnVal[ idx ] = ( OComponent ) item;
      }
      else  // item instanceof OContainerItem
      {
	returnVal[ idx ] = ( ( OContainerItem ) item ).component;
      }
      idx++;
    }
    return returnVal;
    
  } // getComponents

  Enumeration getItems() 
  { 
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.getItems called" );
    return componentItems.elements();
  } // getItems

  public OCompositeScreen getScreen() 
  {
    return this.screen;
  } // getScreen
  
  public void setScreen( OCompositeScreen screen ) 
  {
    this.screen = screen;
  } // setScreen
  
  public void paint( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.paint ENTERED" );

    OFocusableComponent focusedComponent = null;
    for ( Enumeration enum=componentItems.elements();
	  enum.hasMoreElements(); ) 
    {
      OComponent component = ( ( OContainerItem ) enum.nextElement() ).component;
      paintComponent( g, component );
    }

    // Although we've already painted the focused component, we want paint it
    // painted last so that if it
    // overwrites the display areas of any other components - as may happen
    // if, for example, the focused component is a pop-up list - its
    // overwriting of those component(s) is not then undone by the painting of
    // the overwritten components. So we repaint it one final time.
    OFocusableComponent last = this.screen.getFocusManager().getFocused();
    if ( last != null )
      {
	last.setNeedsRepaint( true );
	paintComponent( g, last );
      }
    
    
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.paint EXITTING" );

  } // paint
  

  public void paint( Graphics g, boolean all ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.paint ENTERED" );
    needsRepaint = all;
    paint( g );
    needsRepaint = false;
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.paint EXITTING" );
  } // paint

  
  protected void paintComponent( Graphics g, OComponent component ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.paintComponent ENTERED" );

    int bodyY = screen.getBodyRow() + screen.getBodyOffset();
    if ( component.getY() + component.getHeight() >= bodyY && 
	 component.getY() <= bodyY + screen.getBodyHeight() )
    { // component is at least partially within viewport, so display it
      if ( ! ( component instanceof OContainer ) )
      {
	if ( needsRepaint || component.needsRepaint() )
	{
	  component.paint( g, screen.getBodyOffset() );
	  component.setNeedsRepaint( false );
	}      
      }
      else // component is also a container
      {
	OComponent [] components = ( ( OContainer ) component ).getComponents();
	for ( int idx=0; idx < components.length; idx++ )
	{ // recurse
	  paintComponent( g, components[ idx ] );
	}
      }
    }
    
    if ( Debugger.ON ) Logger.dev( "OContainerHelper.paintComponent EXITTING" );
  } // paintComponent

  public int getHeight() 
  {
    // we currently calculate the height each time. if/when the framework is
    // rearchitected so that the container knows when it's been laid out, 
    // highest-value for the 
    // coordinate of the 
    int height = 0;
    OComponent [] components = getComponents();
    OComponent thisOne = null;
    for ( int idx=0; idx < components.length; idx++ ) 
    {
      thisOne = components[idx];
      height = Math.max( height, thisOne.getY() + thisOne.getHeight() );
    }
    return height - screen.getBodyRow();
  } // getHeight

    /**
     * Returns the width of the container.
     *
     * @return {@link #UNDEFINED UNDEFINED} is returned until the container
     * has been added to a screen, after which the screen width is returned.
     */
  public int getWidth() 
  { 
    int width = 0;
    OComponent [] components = getComponents();
    for ( int idx=0; idx < components.length; idx++ ) 
    {
      OComponent thisOne = components[idx];
      width = Math.max( width, thisOne.getY() + thisOne.getWidth() );
    }    
    return width;
  } // getWidth


  
}// OContainerHelper
