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
import javax.microedition.lcdui.Font;
import com.nextel.util.Debugger;
import com.nextel.util.Logger;
import com.nextel.exception.InvalidData;


/** This class displays two fields, one for US dollars and one for cents.
 * <p><b>Example:</b><img src="doc-files/MoneyField.jpg">
 * <p>
 * The dollars field is preceded by a $ sign, and the fields are separated by a
 * decimal point.
 *
 * @author Glen Cordrey
 */

public class OMoneyField extends OPanel
{
  private OTextField dollarsField;
  private OTextField centsField;
  private OLabel dollarSym;
  private OLabel dotSym;
    
  /**
   * Creates a new <code>OMoneyField</code> instance.
   * @param dollarDigits Maximum number of digits to allow in the
   * dollars field.
   * @param title Title to display with the field
   * @param titlePosition  Position of the title
   * @param titlePosition Position to place the title - either
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.TOP} or
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.LEFT}. Any other
   * value will result in Graphics.LEFT being used.
   * @param font Font for the field
   */    
  public OMoneyField( int dollarDigits, OLabel title,
		      int titlePosition, Font font )
  {
    super( title, titlePosition );
    init(dollarDigits, font);        
  } // OMoneyField
    
    /**
     * Creates a new <code>OMoneyField</code> instance.
     * @param dollarDigits Maximum number of digits to allow in the dollars
     * field.
     * @param font Font for the field.
     */    
  public OMoneyField( int dollarDigits, Font font )
  {
    super();
    init(dollarDigits, font);
  } // OMoneyField

    /**
     * Initializes the object.
     * @param dollarDigits Maximum number of digits to allow in the dollars
     * field.
     * @param font Font for the field.
     */    
  protected void init(int dollarDigits, Font font)
  {
    if (font == null) font = OUILook.PLAIN_MEDIUM;
        
    dollarsField = new OTextField( dollarDigits, font, OTextField.NUMERIC );
    centsField = new OTextField( 2, font, OTextField.NUMERIC );
        
    dotSym = new OLabel( ".", font );
    dollarSym = new OLabel( "$", font );
        
    add( dollarSym );
    add( dollarsField );
    add( dotSym );
    // this kluge forces the cents field to be aligned with the dollars field,
    // which it would otherwise not because of an assumption in OPanel that
    // all components in a panel have the same height, which is not the case
    // here because the dollars field has a multi-line label while the cents
    // field has a single-line label
    add( centsField );
        
  } // init
    
    /**
     * Gets the number of dollars in the field.
     * @return Number of dollars
     */    
  public String getDollars()
  {
    return dollarsField.getText();
  } // getDollars
    
  /**
   * Sets the text in the dollars portion of the field.
   *
   * @param value The number of dollars
   * @exception InvalidData if <code>value</code> is less than 0 or
   * is too large for the number of
   * digits allowed in the dollars field
   */
  public void setDollars( int value )
    throws InvalidData
  { 
    if ( Debugger.ON ) Logger.dev( "OMoneyField.setDollars ENTERED" );
    if ( value < 0 ) throw new InvalidData( "OMoneyField.setDollars: '" +
					    value + "' < 0" );
    dollarsField.setText( Integer.toString( value ) );
    if ( Debugger.ON ) Logger.dev( "OMoneyField.setDollars EXITTING" );
  } // setDollars

    /**
     * Gets the number of cents in the field
     * @return The number of cents in the field
     */    
  public String getCents()
  {
    return centsField.getText();
  } // getCents

    /**
     * Sets the text in the cents portion of the field.
     *
     * @param value The number of cents
     * @exception InvalidData if <code>value</code> is less than 0 or greater
     * than 99
     */
  public void setCents( int value )
      throws InvalidData
  { 
    if ( Debugger.ON ) Logger.dev( "OMoneyField.setCents ENTERED" );
    if ( value < 0 || value > 99 )
      throw new InvalidData( "OMoneyField.setCents: '" + value +
			     "' < 0 or > 99" );
    String text = Integer.toString( value );
    if ( text.length() == 1 ) text = "0" + text; // add leading 0
    centsField.setText( text );
    if ( Debugger.ON ) Logger.dev( "OMoneyField.setCents EXITTING" );
  } // setCents

}// OMoneyField
