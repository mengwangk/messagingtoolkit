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

package com.nextel.util; // Generated package name
import java.util.Calendar;

/**
 * Convenience class to simplify access to values from
 * <code>java.util.Calendar</code>
 * <p>
 * Whereas <code>Calendar</code> uses a 0-base month (i.e., January is month 0),
 * this class uses a 1-based month.
 *
 */

public class MDate 
{  
  /** Requests the month when calling {@link #toString} */
  public final static int MONTH = 1;

  /** Requests the month when calling {@link #toString} */
  public final static int DAY = 2;

  /** Requests the a 2-digit year when calling {@link #toString} */
  public final static int SHORT_YEAR = 4;

  /** Requests the a 4-digit year when calling {@link #toString} */
  public final static int LONG_YEAR = 8;

  private final static char SPACE = ' ';
  private final static char DELIMITER = '/';
  private int month;
  private int day;
  private int year;


  /**
   * Creates a new <code>MDate</code> instance with the current date.
   *
   */
  public MDate() 
  { 
    if ( Debugger.ON ) Logger.dev( "MDate.MDate ENTERED" );
    Calendar calendar = Calendar.getInstance();

    //the month is incremented by 1 because Java uses a 0-based month
    month = calendar.get( Calendar.MONTH ) + 1;
    day = calendar.get( Calendar.DAY_OF_MONTH );
    year = calendar.get( Calendar.YEAR );

    if ( Debugger.ON ) Logger.dev( "MDate.MDate EXITTING" );
  } // MDate

  /**
   * Creates a new <code>MDate</code> instance with the supplied values.
   *
   * @param month Month for the date.
   * @param day Day for the date.
   * @param year Year for the date.
   */
  public MDate ( int month, int day, int year)
  {
    this.month = month;
    this.day = day;
    this.year = year;
  } // constructor

  /**
   * Gets the value of year.
   * @return Value of year.
   */
  public int getYear() 
  {return this.year;}
   
  /**
   * Sets the value of year.
   * @param value  Value to assign to year.
   */
  public void setYear(int  value) 
  {this.year = value;}
   
  /**
   * Gets the value of day.
   * @return Value of day.
   */
  public int getDay() 
  {return this.day;}
   
  /**
   * Sets the value of day.
   * @param value  Value to assign to day.
   */
  public void setDay(int  value) 
  {this.day = value;}
   
  /**
   * Gets the value of month.
   * @return Value of month.
   */
  public int getMonth() 
  {return this.month;}
   
  /**
   * Sets the value of month.
   * @param value  Value to assign to month.
   */
  public void setMonth(int  value) 
  {this.month = value;}

  /**
   * Increases the month by 1. Incrementing month 12 gives month 1.
   *
   */
  public void incrementMonth() 
  { 
    if ( Debugger.ON ) Logger.dev( "MDate.incrementMonth ENTERED" );
    month++;
    if ( month > 12 ) 
    {
      month = 1;
      year++;
    }
    
    if ( Debugger.ON ) Logger.dev( "MDate.incrementMonth EXITTING" );
  } // incrementMonth
  
  // fields is an OR of MONTH et al
  /**
   * Returns a string representing the date.
   *
   * @param fields The fields to include in the string, a combination of
   * {@link #MONTH}, {@link #DAY}, and {@link #SHORT_YEAR} or {@link #LONG_YEAR}
   * @return a <code>String</code> value. Fields will be separated by '/'
   */
  public String toString( int fields ) 
  { 
    if ( Debugger.ON ) Logger.dev( "MDate.toString ENTERED" );
    StringBuffer sBuff = new StringBuffer();
    if ( ( fields & MONTH ) > 0 )
    {
      String monthText = Integer.toString( month );
      if ( monthText.length() == 1 ) 
      {
	sBuff.append( SPACE );
      }
      sBuff.append( monthText );
    }
    if ( ( fields & DAY ) > 0 )
    {
      if ( sBuff.length() > 0 ) 
      {
	sBuff.append( DELIMITER );
      }
      String dayText = Integer.toString( day );
      if ( dayText.length() == 1 ) 
      {
	sBuff.append( SPACE );
      }
      sBuff.append( dayText );
    }
    if ( ( fields & SHORT_YEAR ) > 0 ||
	 ( fields & LONG_YEAR ) > 0 )
    {
      if ( sBuff.length() > 0 ) 
      {
	sBuff.append( DELIMITER );
      }
      String yearText = Integer.toString( year );
      int offset = ( fields &  SHORT_YEAR ) == 0 ? 0 : 2; 
      sBuff.append( yearText.substring( offset ) );
    }
    String returnValue = sBuff.toString();
    if ( Debugger.ON ) Logger.dev( "MDate.toString RETURNING " + returnValue );
    return returnValue;
  } // toString

  /**
   * Demonstrates and tests the method.
   *
   * @param args No command line arguments are used.
   */
  public static void main (String[] args) 
  {
    MDate date = new MDate( 11, 4, 2000 );
    int fields = MONTH + DAY + LONG_YEAR;
    System.out.println( "for 11/4/2000 w month, day, and long year, got \"" +
			date.toString( fields ) + "\"" );

    date.incrementMonth();
    fields  = MONTH + SHORT_YEAR;
    System.out.println( "for 12/4/2001 w month & int year, got \"" +
			date.toString( fields ) + "\"" );
       
    date.incrementMonth();
    fields = MONTH + LONG_YEAR;
    System.out.println( "for 1/4/2002 w month and long year, got \"" +
			date.toString( fields ) + "\"" );
    
  } // main
   
}// MDate
