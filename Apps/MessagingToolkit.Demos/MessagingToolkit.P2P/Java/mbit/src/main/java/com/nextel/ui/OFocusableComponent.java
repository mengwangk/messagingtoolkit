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
import java.util.Enumeration;
import java.util.Vector;
import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import java.util.Hashtable;

/**
 * A component which can have focus, meaning it responds to key presses.
 * @author Glen Cordrey
 */
abstract public class OFocusableComponent extends OComponent 
{
  /** Background for a component that has focus */
  protected static final int SELECTED_BACKGROUND_W_FOCUS = OColor.BLACK;

  /** the color of the text of a selected component */
  protected static final int SELECTED_FOREGROUND = OColor.TRANSPARENT;
  
  /** the top of the 4-way navigation key, used with
      {@link #setTraverseDirections setTraverseDirections}**/

  public static final int UP = 1;
  /** the bottom of the 4-way navigation key, used with
      {@link #setTraverseDirections setTraverseDirections}**/

  public static final int DOWN = 2;
  /** the right side of the 4-way navigation key, used with
      {@link #setTraverseDirections setTraverseDirections}**/
  public static final int RIGHT = 4;

  /** the left side of the 4-way navigation key, used with
      {@link #setTraverseDirections setTraverseDirections}**/
  public static final int LEFT = 8;
  
  /** Width in pixels of box perimeters */
  protected static final int PERIMETER_WIDTH = 1;
  
  /** Background color for a box when it doesn't have focus */
   protected static final int UNFOCUSED_BOX_COLOR = OColor.LT_GRAY;
  
  // Whether the component has focus, i.e. it is the currently active component
  // in which data may be entered or selected
  private boolean focus = false;

  // Vector of objects to notify when this component's focus changes
  private Vector focusListeners = new Vector();
  
  private int focusedComponentBackground = OColor.BLACK;

  // The container in which the component is placed.
  //private OContainer container;

  // Directions in which focus can be lost
  private int traverseDirections = RIGHT | LEFT | UP | DOWN;
  
  /**
   * Creates a new <code>OFocusableComponent</code> instance.
   *
   */
  protected OFocusableComponent ()
  {
  }

  /**
   * Adds an object which is to be notified when this component either gains or
   * loses focus.
   *
   * @param listener The object to notify
   */
  public void addFocusListener ( OFocusListener listener ) 
  {
    focusListeners.addElement( listener );
    return; 
  }

  /**
   * Draws a box for the component. If the component has focus the box
   * background is dark, else  it is light. If a subclass desires this behavior
   * it should call this method from its {@link #paint paint} method, and if it
   * doesn't desire this behavior it should override this method.
   *
   * @param g The graphics context for the component.
   * @param x The pixel column of the component.
   * @param y The pixel row of the component.
   * @param width The width of the component in number of pixels.
   * @param height The height of the component in number of pixels.
   */
  protected void paintBox ( Graphics g, int x, int y, int width,
			    int height ) 
  {
    // save
    int oldColor = g.getColor();
    Font oldFont = g.getFont();

    // first fill the box
    g.setColor( hasFocus() ? focusedComponentBackground : UNFOCUSED_BOX_COLOR );
    g.fillRect( x, y, width, height );

    // now draw the perimeter
    g.setColor( OColor.BLACK );
    g.drawRect( x, y, width, height );

    // restore
    g.setFont( oldFont );
    g.setColor( oldColor );
  }
  
  
  /**
   * Gets the directions in which focus traversal out of this component can
   * occur.  For examp
   * @return A bitwise-OR of any combination of
   * {@link #UP UP}, {@link #DOWN DOWN}, {@link #RIGHT RIGHT}, and/or
   * {@link #LEFT LEFT}, indicating which directions focus traversal out of
   * this component is enabled.
   */
  public int getTraverseDirections() 
  {return this.traverseDirections;}

  /**
   * Returns true if the component has focus.
   *
   * @return true if the component has focus
   */
  public boolean hasFocus( )
  {
    return this.focus;
  }
  
  /**
   * Processes the press of a key while the component has focus.
   *
   * @param keyCode Code of the key which was pressed
   */
  abstract protected void keyPressed ( int keyCode );

  /**
   * Notifies all listeners that the component has gained focus.
   *
   * @param event Event signalling the gain.
   */
  protected void notifyListenersOfGain ( OFocusEvent event ) 
  {
    for ( Enumeration enum=focusListeners.elements(); enum.hasMoreElements(); ) 
    {
      OFocusListener listener = ( OFocusListener ) enum.nextElement();
      listener.focusGained( event );
    }
    
    return; 
  }

  /**
   * Notifies all listeners that the component has lost focus.
   *
   * @param event Event signalling the loss.
   */
  protected void notifyListenersOfLoss ( OFocusEvent event ) 
  {
    for ( Enumeration enum=focusListeners.elements(); enum.hasMoreElements(); ) 
    {
      OFocusListener listener = ( OFocusListener ) enum.nextElement();
      listener.focusLost( event );
    }
    
    return; 
  }

  /**
   * Paints the component on the display.
   *
   * @param g The component's graphics context
   */
  abstract public void paint ( Graphics g );

  /**
   * Sets the focus state. 
   *
   * @param id
   * {@link com.nextel.ui.OFocusEvent#FOCUS_GAINED OFocusEvent.FOCUS_GAINED} or
   * {@link com.nextel.ui.OFocusEvent#FOCUS_LOST OFocusEvent.FOCUS_LOST}
   */
  public void setFocus( int id )
  {
    OFocusEvent event = new OFocusEvent( this, id );
    if ( ( ! focus ) && ( id == OFocusEvent.FOCUS_GAINED ) )
    {
      focus = true;
      repaint( );
      notifyListenersOfGain( event );
    }
    else if ( focus && ( id == OFocusEvent.FOCUS_LOST ) )
    {
      focus = false;
      repaint();
      notifyListenersOfLoss( event );
    }    
  } // setFocus

  /**
   * Sets the focus state without initiating a focus event.
   * This method can be used when one component contains and/or delegates to 
   * another component. For example, {@link com.nextel.ui.ODropDownList}
   * contains, and delegates key events to, {@link com.nextel.ui.OScrollList} 
   * when the former has focus.
   * 
   * @param focus true if the component has focus
   */
  protected void setFocus(boolean focus)
  {
      this.focus = focus;
  }

  /**
   * Sets the color to give to the background of a component that has focus.
   * @param color  Color to give the background.
   */
  protected void setFocusedComponentBackground(int  color ) 
  {this.focusedComponentBackground = color;}  

  /**
   * Sets the 4-way navigation key directions which can cause this component
   * to lose focus. 
   * By default,
   * <ul>
   * <p><li>pressing the right side of the key moves to the next
   * focusable component</li>
   * <p><li>pressing the left side of the key moves to the previous
   * focusable component</li>
   * <p><li>pressing the top moves to the last focusable component
   * on the previous line</li>
   * <p><li>pressing the bottom moves to the first focusable
   * component on the next line</li>
   *</ul>
   * <p>You can use this method to override this behavior if you want your
   * component to process any of these 4-way navigation key presses.  for
   * example,
   * a scrollable component might want to intercept the up and down key presses
   * and use them to initiate scrolling rather than to change focus.
   * @param directions  A bitwise-OR of any combination of
   * {@link #UP UP}, {@link #DOWN DOWN}, {@link #RIGHT RIGHT}, and/or
   * {@link #LEFT LEFT}, indicating which directions focus traversal out of
   * this component is enabled.
   */
  public void setTraverseDirections(int  directions ) 
  {this.traverseDirections = directions;}
  
}// OFocusableComponent
