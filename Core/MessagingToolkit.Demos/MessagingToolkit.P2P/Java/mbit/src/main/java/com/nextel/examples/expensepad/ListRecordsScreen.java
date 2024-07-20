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
import com.nextel.util.Logger;
import com.nextel.rms.OAbstractRecord;

import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Font;

/**
 * Screen used to display the records out.
 * This screen uses the {@link com.nextel.ui.OGrid} to display the information
 * in table format.
 * @author Ryan Wollmuth
 */
public class ListRecordsScreen extends OCompositeScreen
{
  private static final int LEFT_MARGIN = 2;
  private static final int RIGHT_MARGIN = 2;
  private OAbstractRecord[] records;
  private OGridInfo gridInfo;
  private OGrid recordGrid;
  private OSoftKey backKey = new OSoftKey( "BACK" );
  private OCommandAction backAction = new OCommandAction()
  {
    public void performAction()
    {
      ScreenNavigator.goHome();
    }
  };
  
  private OSoftKey menuKey = new OSoftKey( "SELECT" );
  private OCommandAction menuAction = new OCommandAction()
  {
    public void performAction()
    {
      ScreenNavigator.
	goForward( new ExpenseRecordMenuScreen( (ExpenseRecord)
						records[ recordGrid.
							 getSelectedIndex() ] )
      );
    }
  };
  
  /**
   * Creates a <code>ListRecordsScreen</code> instance.
   */
  public ListRecordsScreen()
  {
    super( ExpenseStore.getInstance().getNumRecords() + " Expenses",
      OUILook.PLAIN_SMALL, 1 );
    init();
  }
  
  // Initializes the screen.
  private void init()
  {
    backKey.setAction( backAction );
    addSoftKey( backKey, Graphics.LEFT );
    
    if ( ExpenseStore.getInstance().getNumRecords() > 0 )
    {
      menuKey.setAction( menuAction );
      addSoftKey( menuKey, Graphics.RIGHT );
    
      int columnWidths[] = { (getWidth()/3) - 
        LEFT_MARGIN, (getWidth()/3 - 10), getWidth()/3 + 5};
      int columnAlignments[] = {Graphics.LEFT, Graphics.LEFT, Graphics.RIGHT};
      int maxWidth  = getWidth() - LEFT_MARGIN - RIGHT_MARGIN;
      int maxHeight = 40;
      Font columnHeadingFont = OUILook.PLAIN_SMALL;
      Font selectedFont = OUILook.PLAIN_SMALL;
      Font unselectedFont = OUILook.PLAIN_SMALL;

      gridInfo = new OGridInfo( OGridInfo.GRID_BORDER_TYPE_NONE,
                                5, 3, null, columnAlignments, columnWidths, 
                                maxWidth, columnHeadingFont, selectedFont, 
                                unselectedFont );
      recordGrid = new OGrid( gridInfo );


      try
      {
        records = ExpenseStore.getInstance().getAll( 
	        new ExpenseRecordComparator(), null 
	      );
      } 
      catch ( Exception e )
      {
        Logger.ex( e );
        e.printStackTrace();
      }
      
      ExpenseRecord record;
      OGridObjectRow[] rows = new OGridObjectRow[ records.length ];

      int totalDollars = 0;
      int totalCents = 0;
      for ( int index = 0; index < records.length; index++ )
      {
        record = (ExpenseRecord) records[ index ];
        OGridObjectRow row = new OGridObjectRow( 3 );
        row.addCell( 0, record.getMonth() + "/" + record.getDay() );
        row.addCell( 1, 
          ( record.getExpenseType().length() < 4 ? record.getExpenseType() :
            record.getExpenseType().substring( 0, 4 ) )
        );

	// if cents is < 10 add a leading 0 
	String cents = Integer.toString( record.getCents() );
	if ( cents.length() == 1 ) cents = "0" + cents; 
	
        row.addCell( 2, record.getDollars() + "." + cents );
        rows[ index ] = row;
        
        totalDollars += record.getDollars();
        totalCents += record.getCents();
        if ( totalCents >= 100 )
        {
          totalDollars++;
          totalCents -= 100;
        }
      }
      recordGrid.populate( rows );
      add( recordGrid, 0, 0, Graphics.HCENTER );

      // if the total cents is < 10 add a leading 0
      String cents = Integer.toString( totalCents );
      if ( totalCents < 10 ) cents = "0" + cents;      
      OLabel total = new OLabel( "Total: $" + totalDollars + "." + cents, 
        OUILook.PLAIN_SMALL );
      add( total, 0, 1, Graphics.RIGHT );
    } else {
      OLabel label = new OLabel( "No records found.", OUILook.PLAIN_SMALL );
      add( label, 0, 0, Graphics.HCENTER );
    }                          
  }
  
}
