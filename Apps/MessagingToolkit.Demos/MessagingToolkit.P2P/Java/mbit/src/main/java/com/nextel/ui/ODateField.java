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
import com.nextel.util.*;
import com.nextel.exception.*;


/**
 * This class provides for the entry and display of  a date as three fields in
 * MM/DD/YYYY format.
 * <p><b>Example:</b><img src="doc-files/DateField.jpg">
 * <p>
 * Key presses in the month field are validated to prevent any value greater
 * than 12 from being entered.
 * <p>
 * The day field is validated against the value in
 * the month field. For example, if the month field
 * holds value 11 then the day field will not accept any value greater than 30.
 * <p>
 * Key presses in the month field are NOT validated against the current value in
 * the day field, and key presses in the day field when the month is 2 are NOT
 * validated against the year to prevent 29 from being entered during a year that
 * is not a leap year. Both of these validations can be formed after data is
 * entered by calling {@link #isValid()}.
 * 
 * @author Glen Cordrey
 */
public class ODateField extends OPanel implements OFocusListener
{
  private OMonthField monthField;
  private ODayField dayField;
  private OTextField yearField;
  private OLabel fwdSlash1;
  private OLabel fwdSlash2;
    
  /**
   * Creates a new <code>ODateField</code> instance.
   * @param title Title to display with the field.
   * @param titlePosition Position to place the title - either
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.TOP} or
   * {@link javax.microedition.lcdui.Graphics#TOP Graphics.LEFT}. Any other
   * value will result in Graphics.LEFT being used.
   * @param font Font to use for the field.
   */
  public ODateField( OLabel title, int titlePosition, Font font )
  {
    super( title, titlePosition );
    init(font);
  } // ODateField
        
    /**
     * Creates a new <code>ODateField</code> instance.
     * @param font Font to user for the field
     */
  public ODateField( Font font )
  {
    super();
    init(font);
  }
        
  /**
   * Initializes the GUI components, and adds them to the parent container.
   * @param font  The font for the fields.
   */
  protected void init( Font font )
  {
    if (font == null)  font = OUILook.PLAIN_MEDIUM;
        
    monthField = new OMonthField(font);
    dayField = new ODayField(font);
    yearField = new OTextField(4, font, OTextField.NUMERIC);
    fwdSlash1 = new OLabel("/", font);
    fwdSlash2 = new OLabel("/", font);
        
    add( monthField );
    add( fwdSlash1 );
    add( dayField );
    add( fwdSlash2 );
    add( yearField );
        
    setDate();
	
    dayField.addFocusListener( this );
  } // init
    
    /**
     * Sets the date to today's date.
     */
  public void setDate()
  {
    if ( Debugger.ON ) Logger.dev( "ODateField.setDate ENTERED" );

    MDate date = new MDate();

    //the month is incremented by 1 because the Java uses a 0-based month
    try {
      monthField.setText( Integer.toString( date.getMonth() ) );
      dayField.setText( Integer.toString( date.getDay() ) );
      yearField.setText( Integer.toString( date.getYear() ) );
    }
    catch(InvalidData ex)
    { // do Nothing.
    }
        
    if ( Debugger.ON ) Logger.dev( "ODateField.setDate EXITTING" );
  } // setDate
    
    /**
     * Sets the date to the given month, day, and year values.
     */
  public void setDate(int month, int day, int year)
    throws InvalidData
  {        
    if ( Debugger.ON ) Logger.dev( "ODateField.setDate(int, int, int) ENTERED" );
        
    // check the format
    if ( !ODateField.isValid( month, day, year ))
      throw new InvalidData("ODateField.setDate : invalid date " +
			    month + "/" + day + "/" + year );
	
    monthField.setText( Integer.toString( month ) );
    dayField.setText( Integer.toString( day ) );
    yearField.setText( Integer.toString( year ) );
        
    if ( Debugger.ON ) Logger.dev( "ODateField.setDate(int, int, int) EXITTING" );
  } // setDate
    
    
    
  /**
   * Gets the day.
   *
   * @return a <code>String</code> value
   */
  public String getDay()
  {
    return dayField.getText();
  } // getDay
    
  /**
   * Gets the year.
   *
   * @return a <code>String</code> value
   */
  public String getYear()
  {
    return yearField.getText();
  } // getYear
    
  /**
   * Gets the month.
   *
   * @return a <code>String</code> value
   */
  public String getMonth()
  {
    if ( Debugger.ON ) Logger.dev( "ODateField.getMonth CALLED, returning " +
				monthField.getText() );
    return monthField.getText();
        
  } // getMonth
    

    /**
     * Determines if the given month, day, and year combination is valid.
     *
     * @return true if the date is valid
     */
  public static boolean isValid(int month, int day, int year)
  {
        
    if ( !( month >= 1 && month <= 12 )
	 || !(day >=1 && day <= 31)
	 || !(year >= 1000 && year <= 9999)
	 || ( day == 31 &&
	      ( month == 4 /*april*/ || month == 6  /*june*/ ||
		month == 9 /*sept*/  || month == 11 /*nov*/ ) ) ||
	 ( month == 2 /*feb*/ && day > 29 ) )
    {
      if ( Debugger.ON ) Logger.dev( "ODateField.isValid RETURNING false" );
      return false;
    }
        
    if ( month == 2 && day == 29 )
    {   // if a year is divisible by 4 it is a leap year UNLESS it is also
      // divisible by 100 AND is not divisible by 400
      if ( year % 4 > 0
	   || ( ( year % 100 == 0 ) && ( year % 400 > 0 ) ) )
      {
	if ( Debugger.ON ) Logger.dev( "ODateField.isValid RETURNING false" );
	return false;
      }
    }
    if ( Debugger.ON ) Logger.dev( "ODateField.isValid RETURNING true" );
    return true;
  }    
    
  /**
   * Determines if the date in the date field is valid.
   *
   * @return true if the date is valid
   */
  public boolean isValid()
  {                
    if ( Debugger.ON ) Logger.dev( "ODateField.isValid ENTERED" );
	
    String month = monthField.getText();
    String day = dayField.getText();
    String year = yearField.getText();
        
    if ( month == null || month.trim().length() <= 0 ||
	 day == null || day.trim().length() <= 0 ||
	 year == null || year.trim().length() <= 0 )
      return false;

    return ODateField.isValid( Integer.parseInt(month),
			       Integer.parseInt(day), Integer.parseInt(year) );
                
  } // isValid
         
    /**
     * Signals that the component has lost focus.
     *
     * @param event Event describing the focus change.
     */
  public void focusLost(OFocusEvent event)
  {
    // do Nothing.
  }
    
  /**
   * Signals that the component has gained focus.
   *
   * If the month field loses focus, the day field is updated with 
   * the current month value which the day field uses to validate the entry of the day.
   *
   * @param event Event describing the focus change.
   */
  public void focusGained(OFocusEvent event)
  {
    // now set the month for the day field to use in validating input
    String monthText = monthField.getText();
    if ( monthText == null || monthText.trim().length() == 0 )
    {
      dayField.setMonth( 0 );
    }
    else
    {
      dayField.setMonth( Integer.parseInt(monthText) );
    }
  }
    
}// ODateField
