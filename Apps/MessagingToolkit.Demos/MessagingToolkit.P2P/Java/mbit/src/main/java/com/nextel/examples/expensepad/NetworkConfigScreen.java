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
import com.nextel.rms.*;
import com.nextel.util.Logger;

import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Graphics;

/**
 * Entry screen for server information.
 * Note that the information that is entered on this screen is just the 
 * server name/ip and its port. The protocol and the servlet are defined
 * in {@link com.nextel.examples.expensepad.ExpenseStore}.
 * @author Ryan Wollmuth
 */
public class NetworkConfigScreen extends OCompositeScreen implements CommandListener
{
  private Command backCmd = new Command( "BACK", Command.BACK, 0 );
  private Command saveCmd = new Command( "SAVE", Command.BACK, 0 );
  
  private OTextField uploadURI = 
    new OTextField( 18, OUILook.PLAIN_SMALL, OTextField.ANY );
  private OTextField downloadURI = 
    new OTextField( 18, OUILook.PLAIN_SMALL, OTextField.ANY );
  
  private OLabel uploadLabel = new OLabel( "Upload URI:", OUILook.PLAIN_SMALL );
  private OLabel downloadLabel = new OLabel( "D/L URI:", OUILook.PLAIN_SMALL );
  private OURIRecord storeRecord;
  
  /** Creates a <code>NetworkConfigScreen</code> instance. */
  public NetworkConfigScreen()
  {
    super( "Network Config", OUILook.PLAIN_SMALL, 1 );
    init();
  }
  
  // Initializes the screen
  private void init()
  {
    addCommand( saveCmd );
    addCommand( backCmd );
    setCommandListener( this );
    
    uploadURI.allow( ":." );
    add( uploadLabel, 0, 0, Graphics.HCENTER );
    add( uploadURI, 0, 1, Graphics.LEFT );
    
    downloadURI.allow( ":." );
    add( downloadLabel, 0, 2, Graphics.HCENTER );
    add( downloadURI, 0, 3, Graphics.LEFT );
    
        
    // Check to see if a record exists for the ExpenseStore...
    OAbstractRecord[] storeRecords = null;
    try
    {
      storeRecords = OURIStore.getInstance()
        .getAll( null, 
        new OURIRecordFilter( ExpenseStore.getInstance().getClass().getName() ) 
      );
    } catch ( Exception e ) { Logger.ex( e ); }
    
    if ( storeRecords != null && storeRecords.length > 0 )
    {
      storeRecord = (OURIRecord)storeRecords[ 0 ];
      try
      {
        // Parse out the protocol and the servlet...
        String destinationURI = storeRecord.destinationURI;
        
        int startIndex = ExpenseStore.PROTOCOL.length();
        int endIndex = destinationURI.indexOf( ExpenseStore.UPLOAD_SERVLET );
        String destinationServer = destinationURI.substring( startIndex, endIndex );

        uploadURI.setText( destinationServer );
       
        String sourceURI = storeRecord.sourceURI;

        startIndex = ExpenseStore.PROTOCOL.length();
        endIndex = sourceURI.indexOf( ExpenseStore.DOWNLOAD_SERVLET );
        destinationServer = sourceURI.substring( startIndex, endIndex );
          
        downloadURI.setText( destinationServer );
        
      } catch ( Exception e ) { e.printStackTrace(); Logger.ex( e ); }
    }
  }
  
  /**
   * Listener for soft key actions
   */
  public void commandAction( Command command, Displayable displayable)
  {
    
    if ( command == backCmd )
    {
      ScreenNavigator.goBack();
    }
    else if ( command == saveCmd )
    {
      // Creates or updates the OURIRecord and goes back to the main menu
      if ( storeRecord == null )
      {
        storeRecord = new OURIRecord();
        storeRecord.storeName = ExpenseStore.getInstance().getClass().getName();
        storeRecord.sourceURI = ExpenseStore.PROTOCOL + downloadURI.getText() + 
          ExpenseStore.DOWNLOAD_SERVLET;
        storeRecord.destinationURI = ExpenseStore.PROTOCOL + uploadURI.getText() +
          ExpenseStore.UPLOAD_SERVLET;
        
        try
        {
          OURIStore.getInstance().addRecord( storeRecord );
        } catch ( Exception e ) { Logger.ex( e ); }
      } else {
        storeRecord.sourceURI = ExpenseStore.PROTOCOL + downloadURI.getText() + 
          ExpenseStore.DOWNLOAD_SERVLET;
        storeRecord.destinationURI = ExpenseStore.PROTOCOL + uploadURI.getText() + 
          ExpenseStore.UPLOAD_SERVLET;
        
        try
        {
          OURIStore.getInstance().updateRecord( storeRecord );
        } catch ( Exception e ) { Logger.ex( e ); }
      }
      
      OHandset.beep();
      ScreenNavigator.goHome();
    }
  }
}
