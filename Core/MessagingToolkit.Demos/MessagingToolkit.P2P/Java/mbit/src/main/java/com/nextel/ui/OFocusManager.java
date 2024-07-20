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

package com.nextel.ui; // Generated package na

import com.nextel.util.*;
import java.util.Vector;

class OFocusManager 
{
  // initial size of the vector of focusable components
  private final static int INITIAL_SIZE = 6;
  private Vector focusableComponents = new Vector( INITIAL_SIZE );
  
  private OCompositeScreen screen;
  private int bodyBegin;
  private int bodyEnd;
  
  // The component which currently has focus
  private int focusedComponentIndex = 0;
  private OFocusableComponent focusedComponent = null;
  
  /**
   * Creates a new <code>OFocusManager</code> instance.
   * <p>
   * <b>This constructor should only be called AFTER all components have been
   * added to the screen.  The object created by this
   * constructor will not manage focus for any components added to the screen
   * AFTER this constructor is called</b>
   *
   * @param container The container for which this object is to manage focus.
   */
  public OFocusManager ( OCompositeScreen screen )
  {
    if ( Debugger.ON ) Logger.dev( "OFocusManager.ctor.1 ENTERED" );
    
    this.screen = screen;
    bodyBegin = screen.getBodyRow();
    bodyEnd = bodyBegin + screen.getBodyHeight();
    
    // populate a vector of components which can have focus, which may be a
    // subset of the components in the screen's container
    OComponent [] components = ( ( OContainer ) screen ).getComponents();
    for ( int idx=0; idx < components.length; idx++ )
    {
      OComponent component = components[ idx ];
      if ( ! ( component instanceof OContainer ) )
      {
	if ( component instanceof OFocusableComponent )
	{
	  focusableComponents.addElement( component );
	}
      }
      else // component is itself a container, so we need to add its focusable 
      {    // components to the vector of all focusable components=
	OComponent [] subComponents =
	  ( ( OContainer ) component).getComponents();
	for ( int idx2=0; idx2 < subComponents.length; idx2++ )
	{
	  OComponent subComponent = subComponents[ idx2 ];
	  if ( subComponent instanceof OFocusableComponent ) 
	  {
	    focusableComponents.addElement( subComponent );
	  }
	}
      }
    }

    // set the focused component to the first component
    if ( focusableComponents.size() > 0 ) 
    {
      focusedComponent =
	( OFocusableComponent ) focusableComponents.
	elementAt( focusedComponentIndex );
      focusedComponent.setFocus( OFocusEvent.FOCUS_GAINED );
    }

    if ( Debugger.ON ) Logger.dev( "OFocusManager.ctor.1 EXITTING" );
  } // constructor

  /**
   * Processes a key press.
   * <p>
   * if the key pressed is the right or left key focus is changed appropriately.
   * All other keys are passed to the component which currently has focus.
   *
   * @param keyCode Code of the key pressed.
   */
  public void keyPressed( int keyCode )
  {
    if ( Debugger.ON ) Logger.dev( "OFocusManager.keyPressed CALLED w/keyCode=" +
				keyCode );
    //
    // NOTE: Not all screens will have a focusedComponent
    //       therefore we shall check for null and ensure
    //       it can handle the keypress.  Otherwise, we
    //       shall get the traverse directions
    //
    if (focusedComponent == null) return;
    int directions = focusedComponent.getTraverseDirections();

    if ( keyCode == OAbstractScreen.RIGHT_KEY
	 &&
	 ( OFocusableComponent.RIGHT & directions ) > 0 )
    {
      moveToNextField();
    }
    else  if ( keyCode == OAbstractScreen.LEFT_KEY
	       &&
	       ( OFocusableComponent.LEFT & directions ) > 0 )
    {
      moveToPreviousField();
    }    
    else  if ( keyCode == OAbstractScreen.DOWN_KEY 
	       &&
	       ( OFocusableComponent.DOWN & directions ) > 0 )
    {
      moveToNextField();
    }
    else  if ( keyCode == OAbstractScreen.UP_KEY
	       &&
	       ( OFocusableComponent.UP & directions ) > 0 )
    {
     moveToPreviousField();
    }
    else // pass the key stroke to the component with focus
    {
      focusedComponent.keyPressed( keyCode );
    }
    
    if ( Debugger.ON ) Logger.dev( "OFocusManager.keyPressed EXITTING" );
  } // keyPressed

  /**
   * Processes a right key press.  if the current component is not
   * the last component in the container then focus is passed to the component
   * of rank 1 greater than the current component.
   *
   */
  protected void moveToNextField () 
  {
    if ( Debugger.ON ) Logger.dev( "OFocusManager.processRightKeyPRessed CALLED " );
    if ( focusedComponentIndex < focusableComponents.size() - 1 )
    { // not the last component, so find the next focusable component
      OFocusableComponent nextComponent =
	( OFocusableComponent ) focusableComponents.
	elementAt( ++focusedComponentIndex );
      moveFocus( ( OFocusableComponent ) nextComponent );
    }
    else  
    { // last focusable component, so merely beep
      OHandset.beep();
    }
    if ( Debugger.ON )
      Logger.dev( "OFocusManager.processRightKeyPress EXITTING " );
  } // moveToNextField

  /**
   * Processes a left key press.  if the current component is not the first
   * component in the container then focus is passed to the component of rank
   * 1 less than the current component
   *
   */
  protected void moveToPreviousField () 
  {
    if ( Debugger.ON ) Logger.dev( "OFocusManager.processLeftKeyPress CALLED " );
    if ( focusedComponentIndex > 0 )
    {
      OFocusableComponent previousComponent =
	( OFocusableComponent ) focusableComponents.
	elementAt( --focusedComponentIndex );
      moveFocus( ( OFocusableComponent ) previousComponent );
    }
    else // first component, so merely beep
    {
      OHandset.beep();
    }
    if ( Debugger.ON ) Logger.dev( "OFocusManager.processLeftKeyPress EXITTING " );
    
    
  } // moveToPreviousField

  /**
   * Moves focus to the first component in the next row.
   *
   */
  protected void moveToNextRow () 
  {
    if ( Debugger.ON ) Logger.dev( "OFocusManager.moveToNextRow CALLED " );

    int nextRank = focusedComponentIndex + 1;
    boolean moved = false;
    OFocusableComponent gainer = null;
    int lastRank = focusableComponents.size() - 1;
    if ( nextRank <= lastRank )
    { // there are more components
      int currentY = focusedComponent.getY();
      boolean keepLooking = true;
      for ( ; keepLooking && nextRank <= lastRank; nextRank++ ) 
      {
	OFocusableComponent nextComponent =
	  ( OFocusableComponent ) focusableComponents.elementAt( nextRank );
	if ( nextComponent.getY() > currentY ) 
	{ // component is in a later row
	  gainer = ( OFocusableComponent ) nextComponent;
	  focusedComponentIndex = nextRank;
	  keepLooking = false;
	  moved = true;
	}
      }
    }
    if ( moved )
    {
      moveFocus( gainer );
    }
    else  // there is no next row
    {
      OHandset.beep();
    }

    if ( Debugger.ON )
      Logger.dev( "OFocusManager.moveToNextRow EXITTING " );
  } // moveToNextRow

  /**
   * Moves focus to the last component in the previous row.
   *
   */
  protected void moveToPreviousRow () 
  {
    if ( Debugger.ON ) Logger.dev( "OFocusManager.moveToPreviousRow CALLED " );
    int priorRank = focusedComponentIndex - 1;
    boolean moved = false;
    OFocusableComponent gainer = null;
    int firstRank = 0;
    if ( priorRank >= firstRank )
    { // there is a previous component
      int currentY = focusedComponent.getY();
      boolean keepLooking = true;
      for ( ; keepLooking && priorRank >= firstRank; priorRank-- ) 
      {
	OComponent previousComponent =
	  ( OComponent ) focusableComponents.elementAt( priorRank );
	if ( previousComponent.getY() < currentY  &&
	     previousComponent instanceof OFocusableComponent )
	{
	  gainer = ( OFocusableComponent ) previousComponent;
	  focusedComponentIndex = priorRank;
	  keepLooking = false;
	  moved = true;
	}
      }
    }
    
    if ( moved )
    {
      moveFocus( gainer );
    }
    else  // there is no prior row
    {
      OHandset.beep();
    }

    if ( Debugger.ON )
      Logger.dev( "OFocusManager.moveToPreviousRow EXITTING " );
  } // moveToPreviousRow

  /**
   * Moves the focus to the specified component.
   *
   * @param gainer The component gaining focus
   */
  protected void moveFocus( OFocusableComponent gainer )
  { 
    if ( Debugger.ON ) Logger.dev( "@@@ OFocusManager.moveFocus ENTERED" );
    focusedComponent.setFocus( OFocusEvent.FOCUS_LOST );
    gainer.setFocus( OFocusEvent.FOCUS_GAINED );
    focusedComponent = gainer;
    screen.adjustBodyOffset( gainer );
    if ( Debugger.ON ) Logger.dev( "OFocusManager.moveFocus EXITTING" );
  } // moveFocus

    /**
     * Gets the component that has focus.
     *
     * @return The component that has focus.
     */
  OFocusableComponent getFocused() 
  {
    return focusedComponent;
  } // getFocused

}// OFocusManager





