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
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Displayable;
import com.nextel.util.Logger;
import com.nextel.examples.ui.*;

/**
 * This is the main input screen for the ExpensePad application.
 * It consists of a money field, a scrolllist, and a date field.
 * When the 'SAVE' soft key is pressed, the information from the fields is 
 * gathered and saved to the RMS using {@link ExpenseRecord} and 
 * {@link ExpenseStore}.
 * <P>
 * This screen also allows for the modification of records when an
 *  <code>ExpenseRecord is passed in the constructor.
 * @author Ryan Wollmuth
 */
public class ExpensePadInputScreen
  extends OCompositeScreen implements CommandListener
{
  private ExpensePadMainScreen mainScreen;
  private Command cancelCmd = new Command( "CANCEL", Command.CANCEL, 1 );
  private Command saveCmd = new Command( "SAVE", Command.OK, 1 );
  
  private OMoneyField expenseAmount = new OMoneyField( 4, OUILook.PLAIN_MEDIUM );
  private ODateField expenseDate = new ODateField( OUILook.PLAIN_MEDIUM );
  private OScrollList expenseType;
  private ExpenseRecord record;
  
  // Modify this if you wish to add other categories.
  private String[] categories = 
  { "Miscellaneous",
    "Airplane",
    "Breakfast",
    "Bus Fare",
    "Dinner",
    "Fuel",
    "Hotel",
    "Laundry",
    "Lunch",
    "Office Supplies",
    "Parking",
    "Postage",
    "Rental Car",
    "Snack",
    "Subway",
    "Taxi",
    "Tip",
    "Toll",
    "Train"
  };
  
  /**
   * Creates an <code>ExpensePadInputScreen</code> instance.
   */
  public ExpensePadInputScreen()
  {
    super( 1 );
    init();
  }
  
  /**
   * Creates an <code>ExpensePadInputScreen</code> instance.
   * @param record The record that you wish to update.
   */
  public ExpensePadInputScreen( ExpenseRecord record )
  {
    super( 1 );
    this.record = record;
    init();
  }
  
  // Initializes the screen. 
  private void init()
  {
    setCommandListener( this );
    addCommand( saveCmd );
    addCommand( cancelCmd );
    int index = 0;
    if ( record != null )
    {
      // If the record is not null, we set the fields to the data that is 
      // in the record.
      try
      {
        expenseDate.setDate( record.getMonth(), record.getDay(), record.getYear() );
        expenseAmount.setDollars( record.getDollars() );
        expenseAmount.setCents( record.getCents() );
      } catch ( Exception e ) {} // Should never get here as it saved as a valid date.
      for ( ; index < categories.length; index++ )
      {
        if ( categories[ index ].equals( record.getExpenseType() ) )
          break;
      }
    }
    
    add( expenseAmount, 0, 0, Graphics.HCENTER );
    int scrollListHeight = getBodyHeight() - expenseAmount.getHeight() 
      - expenseDate.getHeight() - 4 /* Padding */;
    expenseType = new OScrollList( getWidth(), scrollListHeight,
      OUILook.PLAIN_MEDIUM, OUILook.PLAIN_SMALL );
    expenseType.populate( categories );
    try
    {
      expenseType.setSelectedIndex( index );
    } catch ( Exception e ) {} // Should never get here as it will always be a valid index.
    add( expenseType, 0, 1, Graphics.HCENTER );  
    add( expenseDate, 0, 2, Graphics.HCENTER );
  }
  
  /**
   * Command listener for softkeys. If save is pressed, the record
   * is saved to the RMS.
   */
  public void commandAction( Command command, Displayable displayable)
  {
    if ( command == cancelCmd )
    {
      ScreenNavigator.goHome();
    } 
    else if ( command == saveCmd )
    {
      if ( record == null )
        record = new ExpenseRecord();
      int month = Integer.parseInt( expenseDate.getMonth() );
      int day = Integer.parseInt( expenseDate.getDay() );
      int year = Integer.parseInt( expenseDate.getYear() );
      
      record.setExpenseDate( month, day, year );
      
      String amount = expenseAmount.getDollars();
      int dollars = 0;
      if ( amount != null && amount.trim().length() > 0 ) 
      {
	dollars = Integer.parseInt( amount );
      }
      // else  nothing was entered for dollars

      amount = expenseAmount.getCents();
      int cents = 0;
      if ( amount != null && amount.trim().length() > 0 ) 
      {
	cents = Integer.parseInt( amount );
      }
      //else  nothing was entered for cents
      
      record.setExpenseAmount( dollars, cents );
      record.setExpenseType( (String)expenseType.getSelectedValue() );
      boolean updated = false;
      try
      {
        if ( record.getRecordId() != record.NO_RECORD_ID )
        {
          ExpenseStore.getInstance().updateRecord( record );
          updated = true;
        }
        else
          ExpenseStore.getInstance().addRecord( record );
        OHandset.beep();
      } catch ( Exception e ) { Logger.ex( e ); }
      ScreenNavigator.goForward( new ConfirmRecordScreen( record, updated ) );
    }
  }
}
