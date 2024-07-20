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
import javax.microedition.lcdui.Font;
import com.nextel.exception.InvalidData;

/**
 * A field for display and entry of a numeric day.
 *
 * @author Glen Cordrey
 */

public class ODayField extends OTextField
{
  // The month this day is in.  This value may be set by some coordinating
  // object, such as a panel containing both month and day fields, to cause the
  // day value to be validated upon entry.  for example, if the month is set to
  // 4, and the first digit entered in the day field is 3, then a successive
  // entry of 1 (making the day 31) will be prohibited
  private int month = 0;
        
  /**
   * Creates a new <code>ODayField</code> instance.
   * @param font The font to use for the field.
   */
  public ODayField( Font font )
  {
    super(2, font, NUMERIC );
  }
    
    
  /**
   * Sets the month in which the day occurs.  If set to a value other than 0
   * the user will be prevented from entering a day which is invalid for that
   * month.
   *
   * @param month The month in which the day occurs
   */
  public void  setMonth( int month )
  {
    if ( Debugger.ON ) Logger.dev( "ODayField.setMonth CALLED w/month=" + month );
    this.month = month;
  } // setMonth
    
    /**
     * Processes the press of a key on the key pad.
     * <p>
     * If the key pressed is not a number key the key is simply passed to the
     * class's parent class for processing.  If the key is a number key and
     * appending it to any existing digit in the field would result in an invalid
     * day then the a beep is generated to signal the user that the value is
     * invalid.  If the value is valid the number is appended to the text
     * displayed in the field.
     *
     * @param keyCode Code of the key pressed
     */
  public void keyPressed( int keyCode )
  {
    if ( Debugger.ON ) Logger.dev( "ODayField.keyPressed ENTERED w/keyCode=" +
				keyCode );
        
    if ( keyCode > 0 )
    {
      char keyChar = ( char ) keyCode;
      if ( Character.isDigit( keyChar ) )
      {
	// check whether the day is valid
	StringBuffer dayBuff = new StringBuffer();
	dayBuff.append( getText() ).append( keyChar);
                
	String dayString = new String( dayBuff );
	int day = Integer.parseInt( dayString );
	if ( day == 0
	     ||
	     day > 31
	     ||
	     ( day == 31
	       &&
	       ( month == 4 /*april*/ || month == 6  /*june*/ ||
		 month == 9 /*sept*/  || month == 11 /*nov*/ ) )
	     ||
	     ( month == 2 /*feb*/ && day > 29 ) )
	{ // this day is not in the month, so don't accept it
	  OHandset.beep();
	}
	else
	{
	  Character newChar = new Character( keyChar );
	  try { append( newChar.toString() ); }
	  catch ( InvalidData ex ) 
	  { OHandset.beep(); }
	}
	if ( Debugger.ON ) Logger.dev( "ODayField.keyPressed.1 EXITTING" );
	return;
      }
    }
        
    // it's not a key that we handle
    super.keyPressed( keyCode );
    if ( Debugger.ON ) Logger.dev( "ODayField.keyPressed.2 EXITTING" );
  } // keyPressed
    
}// ODayField
