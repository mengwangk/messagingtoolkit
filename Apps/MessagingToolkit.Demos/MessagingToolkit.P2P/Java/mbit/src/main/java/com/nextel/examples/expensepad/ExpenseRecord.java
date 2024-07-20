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

import com.nextel.rms.OSynchRecord;
import com.nextel.util.StringUtils;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.util.Date;
import java.util.Calendar;
import java.util.Vector;

/**
 * An instance of an ExpensePad record. 
 * This class extends OSynchRecord. This makes it very easy to convert this record 
 * to a byte stream and send either to the underlying RMS or over the network to
 * a waiting resource.
 * @author Ryan Wollmuth
 */
public class ExpenseRecord extends OSynchRecord
{
  // This is the makeup of the expense record. 
  // It contains a dollars and cents field for the amount of the expense,
  // an expense type, the date the expense happened, and a uploaded flag.
  // The uploaded flag is set to true when a record has been sent to the remote 
  // server OR when it has been downloaded from a remote server.
  private int expenseAmountDollars;
  private int expenseAmountCents;
  private String expenseType;
  private Calendar expenseDate;
  private boolean uploaded;
  
  /**
   * Creates an <code>ExpenseRecord</code> instance.
   */
  public ExpenseRecord()
  {
    super();
  }
  
  /**
   * Creates an <code>ExpenseRecord</code> instance.
   * @param array The byte array that will populate this record.
   */
  public ExpenseRecord( byte[] array )
    throws IOException
  {
    super( array );
  }
  
  /** Getter for dollar amount
   * @return The amount of dollars for this expense.
   */
  public int getDollars()
  {
    return expenseAmountDollars;
  }
  
  /**
   * Getter for cent amount
   * @return The amount of cents for this expense.
   */
  public int getCents()
  {
    return expenseAmountCents;
  }
  
  /** 
   * Getter for the month.
   * @return The month (in int form) when the expense occured.
   */
  public int getMonth()
  {
    return expenseDate.get( Calendar.MONTH ) + 1;
  }
  
  /**
   * Getter for the day.
   * @return The day of the month (in int form) when the expense occured.
   */ 
  public int getDay()
  {
    return expenseDate.get( Calendar.DAY_OF_MONTH );
  }
  
  /**
   * Getter for the year.
   * @return The year (in int form) when the expense occured.
   */
  public int getYear()
  {
    return expenseDate.get( Calendar.YEAR );
  }
  
  /**
   * Getter for the raw date.
   * @return The raw date represented as a long.
   */
  public long getRawDate()
  {
    return expenseDate.getTime().getTime();
  }
  
  /**
   * Getter for the expense type.
   * @return The expense type for this expense.
   */
  public String getExpenseType()
  {
    return expenseType;
  }
  
  /** 
   * Setter for the expense amount ( dollars and cents ).
   * @param dollars The amount of dollars for this expense.
   * @param cents The amount of cents for this expense.
   */
  public void setExpenseAmount( int dollars, int cents )
  {
    this.expenseAmountDollars = dollars;
    this.expenseAmountCents = cents;
  }
  
  /**
   * Setter for the expense date ( month, day, year ).
   * @param month The month the expense took place.
   * @param day The day the expense took place.
   * @param year The year the expense took place.
   */
  public void setExpenseDate( int month, int day, int year )
  {
    expenseDate = Calendar.getInstance();
    expenseDate.set( Calendar.MONTH, month - 1 );
    expenseDate.set( Calendar.DAY_OF_MONTH, day );
    expenseDate.set( Calendar.YEAR, year );
  }
  
  /**
   * Setter for the expense type.
   * @param expenseType The type of the expense.
   */
  public void setExpenseType( String expenseType )
  {
    this.expenseType = expenseType;
  }
  
  /**
   * Setter for the uploaded flag
   * @param uploaded Whether the record has been uploaded.
   */
  public void setUploaded( boolean uploaded )
  {
    this.uploaded = uploaded;
  }
  
  /**
   * Getter for the uploaded flag.
   * @return A boolean flag representing whether the record has been uploaded.
   */
  public boolean getUploaded() { return uploaded; }
  
  /**
   * Simple representation of this record as a String.
   * @return A String that contains information about this record.
   */
  public String toString()
  {
    // if cents is a single digit add a leading 0
    String cents = null;
    if ( expenseAmountCents < 10 ) cents = "0" + expenseAmountCents;
    else  cents = Integer.toString( expenseAmountCents );
    
    return expenseType + "\n" + ( expenseDate.get( Calendar.MONTH ) + 1 ) + "/" + 
      expenseDate.get( Calendar.DAY_OF_MONTH ) + " $" + expenseAmountDollars + 
      "." + cents;
  }
  
  /** 
   * Write the contents of an expense to the dataStream.
   * @param dataStream The stream to write the record to
   */
  public void writeStream( DataOutputStream dataStream )
    throws IOException
  {
    dataStream.writeInt( expenseAmountDollars );
    dataStream.writeInt( expenseAmountCents );
    dataStream.writeUTF( expenseType );
    dataStream.writeLong( expenseDate.getTime().getTime() );
    dataStream.writeBoolean( uploaded );
  }  
  
  /**
   * Populates the current record from the inputStream.
   * @param inputStream The stream to read the record from.
   */
  protected void populate(DataInputStream inputStream)
    throws IOException
  {
    try
    {
      expenseAmountDollars = inputStream.readInt();
      expenseAmountCents = inputStream.readInt();
      expenseType = inputStream.readUTF();
      expenseDate = Calendar.getInstance();
      expenseDate.setTime( new Date( inputStream.readLong() ) );
      uploaded = inputStream.readBoolean();
      
    } catch ( Exception e )
    {
      e.printStackTrace();
      throw new IOException( e.getMessage() );
    }
  }
  
  /**
   * Reads an instance of this class that is represented in an external form
   * (the same form used by {@link #writeExternal}.
   * Since the format we chose was XML, we parse out the XML strings and 
   * populate this record with it.
   *
   * @param in The object in externalized form.
   */
  public void readExternal( byte[] in )
  {
    String recordString = new String( in );
    
    Vector v = StringUtils.tokenize( recordString, "\n" );
    for ( int i = 0; i < v.size(); i++ )
    {
      String line = (String) v.elementAt( i );
      if ( ! line.equals( "<record>" ) &&
           ! line.equals( "</record>" ) &&
           ! line.equals( "\n" ) )
      {
        String attr = getAttribute( line );
        if ( attr.equals( "expenseDate" ) )
        {
          expenseDate = Calendar.getInstance();
          expenseDate.setTime( 
            new Date( Long.parseLong( getValue( line ) ) ) );
        }
        else if ( attr.equals( "expenseAmount" ) )
        {
          String expenseAmount = getValue( line );
          int periodPos = expenseAmount.indexOf( '.' );
          expenseAmountDollars = 
            Integer.parseInt( expenseAmount.substring( 0, periodPos ) 
          );
          expenseAmountCents = 
            Integer.parseInt( 
            expenseAmount.substring( periodPos + 1, expenseAmount.length() ) 
          );
        } 
        else if ( attr.equals( "expenseType" ) )
        {
          expenseType = getValue( line );
        }
      }
    } 
    uploaded = true;
  }
  
  /**
   * Used to get the attribute between the tags.
   * @param s The string to parse the attribute out of.
   * @return The attribute found.
   */
  private String getAttribute( String s )
  {
    int closeTagIndex = s.indexOf( ">" ); 
    return s.substring( 2, closeTagIndex );
  }
  
  /**
   * Used to get the value from between XML tags
   * @param s The string to parse the value out of.
   * @return The value found.
   */
  private String getValue( String s )
  {
    int closeTagIndex = s.indexOf( ">" );
    String noOpenTag = s.substring( closeTagIndex + 1 );
    int openTagIndex = noOpenTag.indexOf( "</" );
    return noOpenTag.substring( 0, openTagIndex );
  }
  
  /**
   * Writes the object in a form that can be transmitted external to the
   * handset.
   * We are writing our record out in a XML format.
   * @return a <code>byte[]</code> value
   */
  public byte[] writeExternal()
  { 
    StringBuffer recordBuffer = new StringBuffer();
    recordBuffer.append( "&record=" );
    recordBuffer.append( "<record>\n" );
    recordBuffer.append( " <expenseAmount>" + expenseAmountDollars + "." + 
      expenseAmountCents + "</expenseAmount>\n" );
    recordBuffer.append( " <expenseType>" + expenseType + "</expenseType>\n" );
    recordBuffer.append( " <expenseDate>" + getRawDate() + "</expenseDate>\n" );
    recordBuffer.append( "</record>\n" );
    return recordBuffer.toString().getBytes();
  }
  
}
