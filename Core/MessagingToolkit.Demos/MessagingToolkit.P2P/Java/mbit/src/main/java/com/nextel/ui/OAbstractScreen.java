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

import javax.microedition.lcdui.*;
import java.util.Stack;
import com.nextel.util.*;
import com.nextel.exception.*;

/**
 * An abstract definition of a screen display.
 * <p>
 * This screen has the following elements:
 * <ul>
 * <p><li>An optional, multi-line title at the top</li>
 * <p><li>If a title is specified, a horizontal line across the width of the
 * screen below the title.</li>
 * <p><li>An area at the bottom of the screen for optional labels above the
 * right and left soft keys, and scroll indicators (see below).</li>
 * <p><li>A horizontal line across the width of the screen, separating the
 * screen body from the soft-key area.</li>
 * <p><li>A body between the title and soft-key area. The body contains the
 * visual elements, such as text, radio buttons, and scroll lists.
 * </ul>
 * <p>
 * If a screen body contains more elements  than can be displayed at one time,
 * the screen will be scrolled vertically as the user navigates to the
 * off-screen elements. This will be indicated by the appearance at the center
 * bottom of the screen (between any soft key labels) of an up-arrow when there
 * are off-screen elements before (above)
 * the currently displayed elements and a
 * down-arrow when there are off-screen elements after (below)
 * the currently displayed elements. 
 * @author Glen Cordrey
 */
abstract public class OAbstractScreen extends Canvas
{
  // The following values are defined so they can be used by
  // {@link com.nextel.ui.OComponent}s without requiring access to the screen
  // the components are contained in
  
  /* The width in pixels of a screen */
  public static final int WIDTH;

    /* The HEIGHT in pixels of a screen */
  public static final int HEIGHT;

  /* The value of the left key, given by
   * {@link javax.microedition.lcdui.Canvas#getKeyCode Canvas.getKeyCode( Canvas.LEFT)}
   */ 
  public static final int LEFT_KEY;

  /* The value of the right key, given by
   * {@link javax.microedition.lcdui.Canvas#getKeyCode Canvas.getKeyCode( Canvas.RIGHT)}
   */ 
  public static final int RIGHT_KEY;

  /* The value of the up key, given by
   * {@link javax.microedition.lcdui.Canvas#getKeyCode Canvas.getKeyCode( Canvas.UP)}
   */ 
  public static final int UP_KEY;

  /* The value of the down key, given by
   * {@link javax.microedition.lcdui.Canvas#getKeyCode Canvas.getKeyCode( Canvas.DOWN)}
   */ 
  public static final int DOWN_KEY;
  // no initialize the above values
  static
  { // create a canvas for the sole purpose of getting its attributes
    Canvas dummy = new Canvas()
      {
	public void paint( Graphics g )
	{ /* do nothing*/ }
      };
    WIDTH = dummy.getWidth();
    HEIGHT = dummy.getHeight();
    LEFT_KEY = dummy.getKeyCode( Canvas.LEFT );
    RIGHT_KEY = dummy.getKeyCode( Canvas.RIGHT );
    UP_KEY = dummy.getKeyCode( Canvas.UP );
    DOWN_KEY = dummy.getKeyCode( Canvas.DOWN );
    dummy = null;
  } // static initializer
  
  // number of pixels to place between a dividing line, such as that below the
  // title or above the soft-keys, and any other visual elements
  protected static final int LINE_SPACER = 1;
  
  // maximum width of an up or down arrow drawn at the bottom of the screen to
  // indicate vertical scrolling
  private static final int ARROW_MAX_WIDTH =
    OUILook.PLAIN_SMALL.charWidth( 'W' );

  // The arrow height is dependent upon the width to ensure the arrow is fully
  // filled. for example, with a height of 5 the arrow is drawn by stacking lines
  // 5, 3, and 1 pixel wide.  This knowledge should rightly be encapsulated in a
  // method in the class that contains drawTriangle, but to save ourselves a
  // method declaration and call we calculate the height here.  Not the best
  // design, but it saves space.
  private static final int ARROW_HEIGHT =
    Math.min( OUILook.PLAIN_MEDIUM.getHeight(), ( ARROW_MAX_WIDTH + 1)/2 + 1 );

  // number of pixels to use for a margin around the scrolling arrows
  private static final int ARROW_MARGIN = 2;

  /** Number of pixels to leave at bottom for soft-key commands */
  private static final int BOTTOM_MARGIN = OSoftKey.FONT.getHeight();

  // the font to use for the title
  private Font titleFont;

  // the title
  private String title;

  // the lines comprising the title
  private String [] titleLines;

  // the number of lines in the title
  private int nbrOfTitleLines;

  // any OSoftKey instances assigned to the left soft key
  private Stack leftSoftKeys = new Stack();

  // any OSoftKey instances assigned to the right soft key
  private Stack rightSoftKeys = new Stack();

  // the currently active left OSoftKey instance, if any
  private OSoftKey leftSoftKey;

  // the currently active right OSoftKey instance, if any
  private OSoftKey rightSoftKey;

  // whether the soft key labels need to be repainted
  private boolean repaintSoftKeys = true;

  // the number of javax.microedition.lcdui.Command objects added to this screen
  private int commandCount;
  

  /**
   * Returns the directions the body can be scrolled in.
   * This should be dynamic depending upon what portion of the body area is
   * displayed. 
   *
   * @return The directions that the body area can be scrolled. Should be 0 if
   * all body elements can be displayed at once, but if not it should be an
   * appropriate bitwise or of {@link com.nextel.ui.OComponent#UP} and/or
   * {@link com.nextel.ui.OComponent#DOWN}
   */
  abstract protected int getScrollDirections() ;

  /**
   * Paints the screen's body, which is all of the components that have been
   * added to the screen.
   *
   * @param g The graphics object to draw to.
   */
  abstract protected void paintBody( Graphics g ); 

  /**
   * Creates a new <code>OAbstractScreen</code> instance.
   *
   */
  protected OAbstractScreen() 
  { /* do nothing */ } // OAbstractScreen

  /**
   * Creates a new <code>OAbstractScreen</code> instance.
   *
   * @param title The title for the screen
   * @param titleFont The font to use for the title.
   */
  protected OAbstractScreen( String title, Font titleFont ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.OAbstractScreen ENTERED" );
    setTitle( title, titleFont );
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.OAbstractScreen EXITTING" );
  } // OAbstractScreen

    /**
     * Creates a new <code>OAbstractScreen</code> instance. This form can be
     * used when the text of the title is created after the screen is
     * instantiated but the number of lines in the title is known beforehand.
     *
     * @param nbrOfTitleLines The number of lines in the title.
     * @param titleFont The font to use for the title.
     */
  protected OAbstractScreen( int nbrOfTitleLines, Font titleFont ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.OAbstractScreen ENTERED" );
    this.nbrOfTitleLines = nbrOfTitleLines;
    this.titleFont = titleFont;
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.OAbstractScreen EXITTING" );
  } // OAbstractScreen
  
  /**
   * Sets the title to display on the screen.
   *
   * @param title The screen title
   * @param titleFont The font to use for the title
   */
  final public void setTitle ( String title, Font titleFont ) 
  {
    this.title = title;
    this.titleFont = titleFont;
    if ( title != null ) 
    {
      titleLines  = StringUtils.breakIntoLines( title );
      nbrOfTitleLines = titleLines.length;
    }
  } // setTitle
  
  
  /**
   * Gets the row where the screen body begins (below the title line)
   *
   * @return the pixel y coordinate where the screen body begins.
   */
  protected int getBodyRow () 
  {
    return  getTitleLineRow() + 1 + LINE_SPACER;
  } // getBodyRow

  
  /**
   * Gets the height of the body area.
   *
   * @return an <code>int</code> value
   */
  protected int getBodyHeight() 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.getBodyHeight CALLED" );
    return getHeight() - getBodyRow() - BOTTOM_MARGIN -
      - 1 /* width of bottom line */;
  } // getBodyHeight
  
  /**
   * Gets the y-coordinate where the line below the title is drawn.
   *
   * @return an <code>int</code> value
   */
  private int getTitleLineRow () 
  {
    return ( titleFont == null ? 0 :
	     ( nbrOfTitleLines * titleFont.getHeight() ) );
  } // getTitleLineRow  


  /**
   * Displays the title on the screen
   *
   * @param g The Graphics object to which to write the title
   * @param titleLines The line(s) of the title.
   */
  protected void paintTitle ( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintTitle.1 ENTERED" );
    
    nbrOfTitleLines = titleLines.length;
    
    // paint the background for the title area
    g.setColor( OColor.TRANSPARENT );
    g.fillRect( 0, 0, getWidth(),
		nbrOfTitleLines * titleFont.getHeight() );

    // Paint the screen title
    g.setColor( OColor.BLACK );
    g.setFont( titleFont );
    for ( int idx=0; idx < nbrOfTitleLines; idx++ ) 
    { // paint each element of the array on a new row
      g.drawString( titleLines[ idx ], getWidth()/2,
		    idx * titleFont.getHeight(),
		    Graphics.TOP | Graphics.HCENTER );
    }

    if ( nbrOfTitleLines > 0 ) 
    {
      // Now, paint a line below the title
      g.setFont( OUILook.PLAIN_SMALL );
      g.drawLine( 0, getTitleLineRow(), getWidth(), getTitleLineRow() );
    }    

    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintTitle1. EXITTING" );
    
  } // paintTitle

  /**
   * Paint sscrolling indicator(s) at the bottom of the screen to indicate that
   * the display can scroll vertically.
   *
   * @param g Graphics context for writing the indicators.
   */
  protected void paintScroller( Graphics g )
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintScroller ENTERED" );
    int scrollDirections = getScrollDirections();
    int oldColor = g.getColor();
    g.setColor( OUILook.TEXT_COLOR );
    if ( ( scrollDirections & OComponent.UP ) > 0 )
    { // draw a triangle indicating the display can be scrolled up
      OComponent.drawTriangle( g,
			       (getWidth() - ARROW_MAX_WIDTH)/2 - ARROW_MARGIN,
			       getHeight() - ARROW_HEIGHT,
			       ARROW_MAX_WIDTH, ARROW_HEIGHT,
			       OComponent.UP );
    }    
    if ( ( scrollDirections & OComponent.DOWN ) > 0 )
    { // draw a triangle indicating the display can be scrolled down
      OComponent.drawTriangle( g,
			       (getWidth() + ARROW_MAX_WIDTH)/2 + ARROW_MARGIN,
			       getHeight() - 1/*spacer*/,
			       ARROW_MAX_WIDTH, getHeight() - ARROW_HEIGHT,
			       OComponent.DOWN );
    }
    g.setColor( oldColor );
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintScroller EXITTING" );
  } // paintScroller

  /**
   * Displays a screen containing the text of an exception.
   * <p>
   * The text will contain the exception name, any text returned by the
   * exception's getMessage method, and any additional text supplied as an
   * argument to this method. If the screen replaces a currently-displayed
   * screen (it may not if this method is called during MIDlet initialization),
   * the exception screen will contain a soft-key labeled BACK that returns to
   * the previous screen.
   * @param ex Exception to display
   * @param message Additional text to display; may be <code>null</code>.
   */
  public static void displayEx( Throwable ex, String message) 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.displayEx ENTERED" );
    // get the current Display object
    Display display = OHandset.getDisplay();
    if ( display != null ) 
    { // form the display text and a screen containing it
      String text = ex.toString();
      if ( message != null ) text +=  '\n' + message;
      OTextScreen screen =
	new OTextScreen( "Exception", OUILook.PLAIN_SMALL,
			 text, OUILook.PLAIN_SMALL );

      // if this screen replaces another screen, save the replaced screen and
      // create a soft-key that returns to that screen
      final Displayable currentScreen = OHandset.getDisplay().getCurrent();
      if ( currentScreen != null ) 
      {
	OSoftKey backCmd = new OSoftKey( "BACK" );
	backCmd.setAction( new OCommandAction ()
	  {
	    public void performAction()
	    {
	      OHandset.getDisplay().setCurrent( currentScreen );
	    }
	  });
	screen.addSoftKey( backCmd, OSoftKey.LEFT );

      }
      display.setCurrent( screen ); // display the exception screen
    }
    else 
    {
      System.err.println( "displayEx failed because OHandset.setMIDlet not previously called" );
    }
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.displayEx EXITTING" );
  } // displayEx

  /**
   * Adds a <code>javax.microedition.lcdui.Command</code> to the screen.
   * <p>
   * This method is overridden solely as a debugging aid to prevent the addition
   * of both <code>Command</code>s and <code>OSoftKey</code>s to the screen
   * (because doing so is an error, as <code>OSoftKey</code>s will be neither
   * displayed nor enabled if the screen contains a <code>Command</code>).
   * If the code base has been compiled with
   * {@link com.nextel.util.Debugger#CODE_ON} <code>= true</code> then this method
   * will throw an {@link com.nextel.ui.OUIError} if a <code>Command</code> is
   * added to screen that contains an <code>OSoftKey</code>. If
   * <code>CODE_ON</code> is false then this method will simply call it's parent
   * method.  
   *
   * @param cmd The Command to add to the screen.
   */
  public void addCommand( Command cmd ) 
  { 
    if ( Debugger.CODE_ON )
    {
      if ( leftSoftKeys.size() > 0 || rightSoftKeys.size() > 0 ) 
      {
	throw new OUIError( "Adding Command when OSoftKey is used" );
      }
      else 
      {
	commandCount++;
      }
    }
    
    super.addCommand( cmd );    
  } // addCommand
  
  /**
   * Removes <code>javax.microedition.lcdui.Command</code> to the screen.
   * <p>
   * This method is overridden solely as a debugging aid in conjunction with
   * {@link #addCommand}.
   *
   * @param cmd The Command to remove from the screen.
   */
  public void removeCommand( Command cmd )
  {
    super.removeCommand( cmd );
    if ( Debugger.CODE_ON ) commandCount--;
  }
  
  /**
   * Paints the screen to the display
   *
   * @param g The Graphics object to paint to.
   */
  final public void paint( Graphics g ) 
  {
    // always paint the body first, because if vertical scrolling is required
    // the paint of the top- and/or bottom-most components may overrun the
    // body area, and painting the title (top of screen) and soft keys (bottom
    // of screen) afterwords will overwrite the overrun areas
    paintBodyBackground( g );
    paintBody( g );
    if ( titleLines != null ) paintTitle( g );
    paintBottom( g );
    
  } // paint

  /**
   * Paints the background for the body area, overwriting anything else  in the
   * area. 
   *
   * @param g The graphics object to draw to. 
   */
  protected void paintBodyBackground( Graphics g ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintBodyBackground ENTERED" );
    g.setColor( OUILook.BACKGROUND_COLOR );
    g.fillRect( 0, getBodyRow() - LINE_SPACER,
		getWidth(), getBodyHeight() + LINE_SPACER ); 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintBodyBackground EXITTING" );
  } // paintBodyBackground

  /**
   * Paints the bottom of the screen, which includes the soft keys and, if
   * vertical scrolling is required, the scroll indicator(s)
   *
   * @param g The Graphics object to paint to.
   */
  protected void paintBottom( Graphics g ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintSoftKeys ENTERED" );
    // paint the background
    g.setColor( OColor.TRANSPARENT ); 
    g.fillRect( 0, getHeight() - BOTTOM_MARGIN, getWidth(), BOTTOM_MARGIN );

    // paint the line above the soft keys
    g.setColor( OColor.BLACK );
    g.drawLine( 0, getHeight() - BOTTOM_MARGIN - 1,
		getWidth(), super.getHeight() - BOTTOM_MARGIN - 1);

    // now paint the keys
    g.setFont( OSoftKey.FONT );
    if ( leftSoftKey != null )
    {
      g.drawString( leftSoftKey.getLabel(), 0, getHeight(),
		    Graphics.BOTTOM | Graphics.LEFT );
    }
    if ( rightSoftKey != null )
    {
      g.drawString( rightSoftKey.getLabel(), getWidth(), getHeight(),
		    Graphics.BOTTOM | Graphics. RIGHT );
    }
    repaintSoftKeys = false;

    paintScroller( g );
    
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.paintSoftKeys EXITTING" );
  } // paintBottom
  
  /**
   * Shows the output of {@link com.nextel.util.Debugger#getMemMsgs}.
   * <p>
   * The output is displayed in a screen with two soft keys:
   * <ul>
   * <p><li><b>OK</b> redisplays the previous screen</li>
   * <p><li><b>.gc()</b> calls <code>System.gc()</code>,
   * then redisplays the memory use.</li>
   * </ul>
   *
   * @param caller The screen that called this method.
   */
  protected static void showMem( final OAbstractScreen caller ) 
  { 
    if ( Debugger.LOG_MEM )
    { // enclosing this in test on static LOG_MEM will cause the compiler to
      // ignore this body if LOG_MEM is false, thereby saving space
    
      // log the current memory use of this class
      String className = caller.getClass().getName();
      String shortName = className.substring( className.lastIndexOf( '.' ) + 1 );
      className = null;
      Debugger.logMem( shortName );

      // Create a screen containing the logged memory use. 
      OTextScreen memScreen =
	new OTextScreen( "Memory Used", OUILook.PLAIN_SMALL,
			 Debugger.getMemMsgs().toString(), OUILook.PLAIN_SMALL );

      // Add to the screen a key to return to the previous screen
      OSoftKey okKey = new OSoftKey( "OK" );
      OCommandAction okAction =
	new OCommandAction()
	  {
	    public void performAction()
	    { // reset the screen to the original
	      OHandset.getDisplay().setCurrent( caller );
	    }
	  }; // okAction
      okKey.setAction( okAction );
      memScreen.addSoftKey( okKey, OSoftKey.LEFT );
      
      // add a key to call garbage collection and show memory use again
      OSoftKey gcKey = new OSoftKey( ".gc()" );
      OCommandAction gcAction =
	new OCommandAction()
	  {
	    public void performAction()
	    {
	      System.gc();
	      showMem( caller );
	    } 
	  }; // gcAction
      gcKey.setAction( gcAction );      
      memScreen.addSoftKey( gcKey, OSoftKey.RIGHT );

      // now display the screen
      OHandset.getDisplay().setCurrent( memScreen );
    }
  } // showMem

  /**
   * Adds a soft key to the screen display.
   * <p>
   * <b>Any screen that uses this method either directly or indirectly
   * should not also use
   * {@link javax.microedition.lcdui.Command}s - see the overview description
   * for package <code>com.nextel.ui</code> for further explanation.</b>
   * if {@link com.nextel.util.Debugger#CODE_ON} <code>= true</code> this method
   * will throw an <code>OUIError</code> if a {@link
   * javax.microedition.lcdui.Command} has already been added to the screen (see
   * {@link #addCommand} for additional information).
   * <p>
   * A soft key remains on a stack (one for the right soft key and one for the
   * left soft key) until {@link #removeSoftKey} is called, so if soft key B
   * is added after soft key A, soft key A will be reenabled when B is removed.
   * <p>
   * Here is an example of code, in an
   * <code>OAbstractScreen</code>, that enables the left soft key so that the
   * word "CANCEL" is displayed above it and pressing it results in whatever
   * processing is coded within <code>cancelAction.performAction</code>
   * <pre><code>
   * private OSoftKey cancelKey = new OSoftKey( "CANCEL" );   
   * private OCommandAction cancelAction =
   *  new OCommandAction ()
   *  {
   *	public void performAction()
   *	{ // cancel current screen and
   *	  // return to previous screen
   *	}	
   *  };
   * ...
   * cancelKey.setAction( cancelAction );
   * addSoftKey( cancelKey, OSoftKey.LEFT );
   * </code></pre>
   * <p>
   * You should not set the right soft key on screens that contain
   * {@link com.nextel.ui.ORadioButton}s,
   * {@link com.nextel.ui.OCheckBox}es, or
   * {@link com.nextel.ui.OPushButton}s, as those components utilize the right
   * soft key (see their javadoc for details).
   * <p>However, if you implement custom
   * components that need to respond to soft-key presses
   * (and if you don't implement custom components
   * you really don't need to read this),
   * you may need those
   * components to use the right soft key in a manner similar to
   * {@link com.nextel.ui.ORadioButton}s,
   * {@link com.nextel.ui.OCheckBox}es, or
   * {@link com.nextel.ui.OPushButton}s.  Here is a code extract showing how
   * {@link com.nextel.ui.ORadioButton} uses the right soft key.  Note that
   * it does not use a {@link com.nextel.ui.OCommandAction}, but rather processes
   * the press of the right soft key in its keyPressed method.  This works
   * because if <code>OAbstractScreen</code> does not find a
   * {@link com.nextel.ui.OCommandAction} for an active soft key it simply passes
   * the key to the current focused component. While
   * {@link com.nextel.ui.ORadioButton} could use a
   * {@link com.nextel.ui.OCommandAction}, using this alternative approach
   * reduces overhead and saves memory by not requiring the creation of the
   * {@link com.nextel.ui.OCommandAction} object.
   * <pre><code>
   * public class ORadioButton {
   * ...
   * private OSoftKey setKey = new OSoftKey( "SET" );
   * public void setFocus( int id ) 
   * { 
   *   super.setFocus( id );
   *   if ( id == OFocusEvent.FOCUS_GAINED ) 
   *      getContainer().getScreen().addSoftKey( setKey, OSoftKey.RIGHT  );
   *   else if ( id == OFocusEvent.FOCUS_LOST ) 
   *      getContainer().getScreen().removeSoftKey( OSoftKey.RIGHT );
   * } // setFocus
   * protected void keyPressed( int keyCode ) 
   * { 
   *    if ( keyCode == OSoftKey.RIGHT )
   *      setSelected( true );
   * </code></pre>
   *   
   * @param softKey The soft key to add.
   * @param position The position of the soft key - 
   * {@link com.nextel.ui.OSoftKey#RIGHT},
   * {@link javax.microedition.lcdui.Graphics#RIGHT},
   * {@link com.nextel.ui.OSoftKey#LEFT}, or 
   * {@link javax.microedition.lcdui.Graphics#LEFT}
   * (OSoftKey.RIGHT and Graphics.RIGHT are equivalent, as are
   * OSoftKey.LEFT and Graphics.LEFT).
   * @exception OUIError thrown if <code>position</code> is invalid
  */
  public void addSoftKey( OSoftKey softKey, int position ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.addCommand ENTERED" );
    if ( Debugger.CODE_ON && commandCount > 0 )
    {
      throw new OUIError( "Adding OSoftKey while Command is used" );
    }
    
    if ( position == Graphics.LEFT || position == OSoftKey.LEFT ) 
    {
      leftSoftKeys.push( softKey );
      leftSoftKey = softKey;
      repaintSoftKeys = true;
      repaint();
    }
    else  if ( position == Graphics.RIGHT || position == OSoftKey.RIGHT ) 
    {
      rightSoftKeys.push( softKey );
      rightSoftKey = softKey;
      repaintSoftKeys = true;
      repaint();
    }
    else  throw new OUIError( "addSoftKey: invalid position" );
    
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.addCommand EXITTING" );
  } // addCommand

  /**
   * Removes the current soft key. If the stack of soft keys for the indicated
   * <code>position</code> contains another key it will be enabled, meaning it's
   * label will be displayed above the soft key and pressing the soft key will
   * initiated the designated action.
   *
   * @param position The position of the soft key - either
   * {@link javax.microedition.lcdui.Graphics#RIGHT} or 
   * {@link javax.microedition.lcdui.Graphics#LEFT}.  No soft key will be added
   * if an invalid value is supplied, but no exception will be thrown.
   */
  public void removeSoftKey( int position ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.removeSoftKey ENTERED" );
    if ( position == Graphics.LEFT ) 
    {
      leftSoftKeys.pop();
      leftSoftKey = ( ! leftSoftKeys.empty() ?
		      ( OSoftKey ) leftSoftKeys.peek() : null );
      repaintSoftKeys = true;
      repaint();
    }
    else  if ( position == Graphics.RIGHT ) 
    {
      rightSoftKeys.pop();
      rightSoftKey = ( ! rightSoftKeys.empty() ?
		       ( OSoftKey ) rightSoftKeys.peek() : null );
      repaintSoftKeys = true;
      repaint();
    }
    
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.removeSoftKey EXITTING" );
  } // removeSoftKey

  /**
   * Processes a press of a key.
   * <p>
   * Any {@link java.lang.Throwable} that is thrown within the thread the
   * processes the key press and which is not caught and consumed will cause the
   * current screen to be replaced by a screen that shows the exception.
   * <p>
   * If {@link com.nextel.util.Debugger#LOG_MEM} is <code>true</code> and the
   * push-to-talk key
   * was pressed, a screen will be displayed showing the amount of free and used
   * memory available currently and at each previous such press of the
   * push-to-talk button.
   * The content of this screen is described in
   * {@link #showMem( OAbstractScreen ) }
   *
   * @param keyCode The code of the pressed key.
   */
  public void keyPressed( int keyCode )
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.keyPressed ENTERED w/keyCode=" +
				keyCode );
    if ( Debugger.LOG_MEM && keyCode == -50 /* -50 is the push-to-talk button */ )
    {
      showMem( this );
      return;
    }
    
    try
    {
      OCommandAction action = null;
      if ( keyCode == OSoftKey.LEFT && leftSoftKey != null ) 
      {
	action = leftSoftKey.getAction();
	if ( action != null ) action.performAction();
      }
      else  if ( keyCode == OSoftKey.RIGHT && rightSoftKey != null ) 
      {
	action = rightSoftKey.getAction();
	if ( action != null ) action.performAction();
      }
      else  OHandset.beep();
    }
    catch( Throwable ex )
    {
      Logger.ex( ex );
      displayEx( ex, null );
    }
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.keyPressed EXITTING" );
  } // keyPressed


  /**
   * Gets a soft key registered with the screen.
   *
   * @param keyPosition {@link com.nextel.ui.OSoftKey#RIGHT} or
   * {@link com.nextel.ui.OSoftKey#LEFT}
   * @return the registered soft key, or null if none is registered
   * @exception InvalidData if an invalid <code>keyPosition</code> is provided
   */
  protected OSoftKey getSoftKey( int keyPosition ) 
    throws InvalidData
  { 
    if ( Debugger.ON ) Logger.dev( "OAbstractScreen.getSoftKey ENTERED" );

    if ( keyPosition == OSoftKey.LEFT ) return leftSoftKey;
    else  if ( keyPosition == OSoftKey.RIGHT ) return rightSoftKey; 
    else 
      throw new InvalidData("getSoftKey given position neither OSoftKey.LEFT nor OSoftKey.RIGHT" );
    
  } // getSoftKey

} // OAbstractScreen

