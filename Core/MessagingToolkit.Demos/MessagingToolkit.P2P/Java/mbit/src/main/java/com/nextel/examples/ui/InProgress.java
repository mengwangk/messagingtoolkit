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

package com.nextel.examples.ui;

import javax.microedition.midlet.*;
import javax.microedition.lcdui.*;
import com.nextel.ui.*;
import com.nextel.util.*;
import java.util.Hashtable;
import java.io.IOException;

/**
 *  This MIDlet demonstrates the use of {@link com.nextel.ui.OAnimation} to
 *  create a screen displaying an in-progress indicator while processing occurs
 *  in a separate thread.
 * <p>
 * The in-progress indicator is an inverting hourglass generated from files
 * <code>hourglass1.png, hourglass2.png,</code> and <code>hourglass3.png</code>,
 * which are expected to be in the top-level directory in the jar file, or in
 * the appropriate directory if using an emulator.
 * <p>
 * The MIDlet first displays a screen that contains text and has the right soft
 * key enabled and labeled DO IT.
 * When the DO IT button is pressed a new screen showing an
 * animated hourglass is displayed, and a separate thread is initiated that
 * waits 10 seconds before replacing the animated hourglass screen. In your
 * application you would replace this wait with whatever processing you need to
 * occur.
 *
 * @author Glen Cordrey
 */
public class InProgress extends MIDlet
{
  // Images showing an hourglass in different states
  private  static String [] HOURGLASSES =
  { "/hourglass1.png", "/hourglass2.png", "/hourglass3.png" };

  // The screen showing the initial text
  private OTextScreen textScreen;
  
  /**
   * Creates a new <code>InProgress</code> instance.
   *
   */
  public InProgress() 
  {
    // Store a reference to this MIDlet in OHandset so other objects can
    // access objects that are only accessible via a MIDlet reference
    OHandset.setMIDlet( this );
  } // constructor
  
  /**
   * Called when the application is started.
   *
   * @exception MIDletStateChangeException if an error occurs
   */
  protected void startApp() throws MIDletStateChangeException
  {
    if ( textScreen == null ) 
    { // we only create the  screen once because this method may be
      // called to resume the MIDlet after it has paused, in which case we don't
      // want to create a new screen
      try 
      {
	// create an initial screen with a DO IT key to initiate an action
	textScreen = 
	  new OTextScreen( "Do Something", OUILook.PLAIN_SMALL,
			"Press the DO IT button to initiate an action",
			OUILook.PLAIN_SMALL );

	OSoftKey doItCmd = new OSoftKey( "DO IT" );

	// define the action to take when the soft key is pressed by
	// instantiating an anonymous class
	
	doItCmd.setAction( new OCommandAction ()
	  {
	    // define a Runnable that can be run in a separate
	    // thread while the hourglass screen is displayed
	    Runnable task = new Runnable()
	      {
		public void run()
		{ // you would replace this .sleep with whatever processing you
		  // want to do while the hourglass screen is displayed
		  try { Thread.sleep( 10000 );}
		  catch (InterruptedException e) { /* do nothing */ }
		  ScreenNavigator.
		    goForward( new OTextScreen( "DONE", OUILook.PLAIN_SMALL,
					     "Processing complete",
					     OUILook.PLAIN_SMALL ) );
		}
	      }; // end Runnable

	    // now define the action that initiates the processing and the
	    // in-progress indication
	    public void performAction()
	    { // create screen showing hourglass
	      OCompositeScreen hourglassScreen =
		new OCompositeScreen( "Patience", OUILook.PLAIN_SMALL, 1 );
	      int row=0;
	      hourglassScreen.add( new OLabel( "Processing...",
					       OUILook.PLAIN_SMALL ),
				   0, row++, Graphics.HCENTER );
	      try 
	      {
		hourglassScreen.add( new OAnimation( HOURGLASSES, 1500 ), 0,
				     row++, Graphics.HCENTER );
		 
	      } catch (IOException e) 
	      { e.printStackTrace(); } // end of try-catch
	      
	      // display the hourglass screen
	      ScreenNavigator.goForward( hourglassScreen );

	      // initiate the processing that is to occur
	      Thread activity = new Thread( task );
	      activity.start();
	    }	
	  } /* end of anonymous doItCmd.setAction */ );

	// add to the screen a key to start the processing that initiates the
	// in-progress indicator
	textScreen.addSoftKey( doItCmd, Graphics.RIGHT );

	// ScreenNavigator is a utility class that we use to navigate our way
	// forwards and backwards between screens
	ScreenNavigator.goForward( textScreen );
      }
      catch ( Throwable ex ) 
      { // if an exception is thrown display and log it
	OCompositeScreen.displayEx( ex, null );
	Logger.ex( ex );
	return;
      } // end of catch
    }
    else  // resuming after a pause, so restore the last screen
    {
      Display.getDisplay( this ).
	setCurrent( ScreenNavigator.getCurrentScreen() );
    }
  } // startApp
  
  /**
   * Does nothing except satisfy the need to implement it because it is abstract
   * in the base class.
   *
   */
  protected void pauseApp()
  { /* there is nothing we need to do*/ }
  
  /**
   * Does nothing except satisfy the need to implement it because it is abstract
   * in the base class.
   *
   */
  // Called when the application exits.
  protected void destroyApp(boolean param) throws MIDletStateChangeException
  { /* we have no resources to free, so do nothing */ }


} // InProgress









  
