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
import javax.microedition.lcdui.Graphics;

/**
 * This is the record menu screen. Users can select whether they wish
 * to edit or delete a record.
 *
 * @author Ryan Wollmuth
 */
public class ExpenseRecordMenuScreen extends OCompositeScreen
{
  private OSoftKey backSoftKey = new OSoftKey( "BACK" );
  private OPushButton editButton = new OPushButton( "EDIT", OUILook.PLAIN_MEDIUM, 
    "OK" );
  private OPushButton deleteButton = new OPushButton( "DELETE", OUILook.PLAIN_MEDIUM, 
    "OK" );
  private OButtonGroup butGroup = new OButtonGroup();
  private OCommandAction editCmd = new OCommandAction()
  {
    public void performAction()
    {
      ScreenNavigator.goForward( new ExpensePadInputScreen( record ) );
    }
  };
  
  private OCommandAction deleteCmd = new OCommandAction()
  {
    public void performAction()
    {
      try
      {
        ExpenseStore.getInstance().deleteRecord( record );
        OHandset.beep();
	ScreenNavigator.goHome();
      }
      catch ( Exception e )
      {
        displayEx( e, null );
      }
    }
  };
  
  private OCommandAction backCmd = new OCommandAction()
  {
    public void performAction()
    {
      ScreenNavigator.goForward( new ListRecordsScreen() );
    }
  };
      
  private ExpenseRecord record;
  
  /**
   * Creates a <code>ExpenseRecordMenuScreen</code> instance.
   * @param The record that will be acted upon.
   */
  public ExpenseRecordMenuScreen( ExpenseRecord record )
  {
    super( "Record Options", OUILook.PLAIN_MEDIUM, 1 );
    this.record = record; 
    init();
  }
  
  // Initializes the screen.
  private void init()
  {
    editButton.setAction( editCmd );
    backSoftKey.setAction( backCmd );
    deleteButton.setAction( deleteCmd );
    
    addSoftKey( backSoftKey, Graphics.LEFT );
    butGroup.add( editButton );
    butGroup.add( deleteButton );
    
    OLabel recordText = new OLabel( record.toString(), OUILook.PLAIN_SMALL );
    add( recordText, 0, 0, Graphics.HCENTER );
    add( butGroup, 0, 1, Graphics.HCENTER );
  }
 
}
