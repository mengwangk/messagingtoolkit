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

/**
 * A field containing a numeric month.
 * <p>
 * Only valid months are allowed; any attempt to enter an invalid month results
 * in a beep.
 * @author Glen Cordrey
 */

public class OMonthField extends OTextField
{
    
  /**
   * Creates a new instance.
   * @param font The font to use for the field
   */
  public OMonthField(Font font)
  {
    super(2, font, NUMERIC );
  }
    
  /**
   * Processes the press of a key while this field has focus.
   * <p>
   * If the key pressed is a number it is appended to any number already in the
   * field and the resulting value is validated.  If the press would result in
   * an invalid month (e.g., the field already contains a 1 and 4 is pressed) a
   * beep is sounded. 
   * <p>
   * If the key is not a number it is passed to the parent class for processing
   * @param keyCode Code of the pressed key
   */
  public void keyPressed( int keyCode )
  {
    if ( Debugger.ON ) Logger.dev( "OMonthField.keyPressed ENTERED w/keyCode= " +
				keyCode );
    if ( keyCode > 0 )
    {
      char keyChar = ( char ) keyCode;
      if ( Character.isDigit( keyChar ) )
      {
	// check whether the month is valid
	StringBuffer monthBuff = new StringBuffer();
	  monthBuff.append( getText() ).append( keyChar );
	String newMonth = new String( monthBuff );
	int monthValue = Integer.parseInt( newMonth );
	if ( monthValue > 0 && monthValue <= 12 )
	{ // it's a valid month
                    
	  super.keyPressed( keyCode );//appendText( keyChar );
	}
	else // invalid month
	{
	  OHandset.beep();
	}
	if ( Debugger.ON ) Logger.dev( "OMonthField.keyPressed EXIT1 " );
	return;
      }
    }
        
    // it's not a key that we handle
    super.keyPressed( keyCode );
    if ( Debugger.ON ) Logger.dev( "OMonthField..keyPRessed EXIT2 " );
  } // keyPressed
    
}// OMonthField
