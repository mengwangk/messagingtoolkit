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
package com.nextel.examples.expensepad;

import com.nextel.ui.*;
import com.nextel.examples.ui.*;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.Display;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Displayable;
import com.nextel.rms.OURIStore;

/**
 * This is the main screen for the ExpensePad.
 * It's options include creating a new expense, listing all of the current
 * expenses on the phone, deleting all expenses on the phone, uploading and
 * downloading expense records to a remote server and setting the network
 * configuration.
 * @author Ryan Wollmuth
 */
public class ExpensePadMainScreen extends OCompositeScreen
  implements CommandListener
{   
  private ExpensePadInputScreen inputScreen;
  
  private Command exitCmd = new Command( "EXIT", Command.EXIT, 1 );
  private Command selectCmd = new Command( "SELECT", Command.OK, 1 );
  
  private OScrollList scrollList;
  
  // This is the option list. If you ever want to add an option and have
  // it show up in the scrolllist, this is the place.
  private String[] options = 
  { "New Expense", "List All", "Delete All", "Net Config", "Upload", "Download",  };
  
  public static final int NEW_EXPENSE = 0;
  public static final int LIST_ALL = 1;
  public static final int DELETE_ALL = 2;
  public static final int CONFIG = 3;
  public static final int UPLOAD = 4;
  public static final int DOWNLOAD = 5;
  
  /**
   * Creates a new <code>ExpensePadMainScreen</code> instance.
   */
  public ExpensePadMainScreen() 
  {
    super( "ExpensePad", OUILook.TEXT_FONT, 1 );
    init();
  }
  
  // Initializes the screen.
  private void init()
  {
    setCommandListener( this );
    addCommand( selectCmd );
    addCommand( exitCmd );
    
    scrollList = new OScrollList( getWidth(), getBodyHeight(), 
				  OUILook.PLAIN_MEDIUM, OUILook.PLAIN_MEDIUM );
    scrollList.populate( options );
    add( scrollList, 0, 0, Graphics.HCENTER );
  }
  
  /**
   * Command listener for softkeys.
   * Its options are 'EXIT', which exits the program, or 'SELECT', which selects 
   * the option currently highlighted in the List.
   */
  public void commandAction( Command command, Displayable displayable)
  {
    if ( command == exitCmd ) OHandset.getMIDlet().notifyDestroyed();
    else if ( command == selectCmd )
    {
      switch( scrollList.getSelectedIndex() )
      {
      case NEW_EXPENSE:
	ScreenNavigator.goForward( new ExpensePadInputScreen() );
	break;
      case LIST_ALL:
	ScreenNavigator.goForward( new ListRecordsScreen() );
	break;
      case DELETE_ALL:
	try
	{
	  ExpenseStore.getInstance().deleteAll();

	  // now display the confirmation screen
	  ScreenNavigator.
	    goForward( createNotificationScreen( "Deleted!",
						 "All expenses have been deleted" ) );
						      
	} 
	catch ( Exception e )
	{
	  displayEx( e, null );
	}
	break;
      case DOWNLOAD:
	  ScreenNavigator.goForward( new NetworkScreen( false ) );
	break;
      case UPLOAD:
	  ScreenNavigator.goForward( new NetworkScreen( true ) );
	break;
      case CONFIG:
	ScreenNavigator.goForward( new NetworkConfigScreen() );
      }
    }   
  }

  /**
   * Creates a screen to notify the user that an action has occured.
   * <p>
   * The screen contains a soft key labelled OK that will return the user
   * to the home screen.
   *
   * @param title The title for the screen
   * @param text The text for the screen
   * @return an <code>OTextScreen</code> value
   */
  private OTextScreen createNotificationScreen( String title, String text ) 
  {
    // now create a screen acknowledging the confirmation and containing a
    // soft key to exit the confirmation screen
    OTextScreen screen =
      new OTextScreen( title, OUILook.PLAIN_MEDIUM,
		       text, OUILook.PLAIN_MEDIUM );
    screen.setEditable( false );
    OSoftKey okSoftKey = new OSoftKey( "OK" );
    OCommandAction okAction = new OCommandAction()
      {
	public void performAction()
	{ // return to the home screen
	  ScreenNavigator.goHome();
	}
      };
    okSoftKey.setAction( okAction );
    screen.addSoftKey( okSoftKey, Graphics.RIGHT );
    return screen;
  } // createNotificationScreen

}
