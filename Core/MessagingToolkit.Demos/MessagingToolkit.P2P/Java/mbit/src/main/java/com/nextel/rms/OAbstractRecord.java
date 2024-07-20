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
package com.nextel.rms;

import com.nextel.util.Debugger;
import com.nextel.util.Logger;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;

/**
 *  An abstract implementation of a record in RMS.
 * <P>
 *  The key portions of this code are the {@link #writeStream writeStream()}
 * and the {@link #populate populate()} methods. These are the
 * abstract methods that the developer will define. Information about these 2
 * methods can be found in the javadoc below.
 * <P>
 *  This class also overrides the <code>hashCode()</code> method, using the
 * record id of the record as its hashcode. This method is used by the default
 * cache (<code>DataCacheImpl</code>) and overrides the <code>hashCode()</code>
 * in java.lang.Object. If you wish to use another value,
 * override it in your record class.
 * @author Glen Cordrey
 */

abstract public class OAbstractRecord
{
  /** The default value of recordId. */
  public final static int NO_RECORD_ID = -1;

  /** The record id of the current record */
  private int recordId = NO_RECORD_ID;

  // Streams used to convert record to a byte[]
  private ByteArrayOutputStream byteStream = new ByteArrayOutputStream();
  private DataOutputStream dataStream = new DataOutputStream( byteStream );
  /**
   * Creates a new <code>OAbstractRecord</code> instance.
   * 
   */
  protected OAbstractRecord ()
  { }

  /**
   * Creates a new <code>OAbstractRecord</code> instance.
   *
   * @param byteArray Bytes which express an OAbstractRecord which was
   * previously serialized via {@link #writeObject writeObject}
   * @exception IOException if an error occurs
   */
  protected OAbstractRecord ( byte [] byteArray )
    throws IOException
  {
    readObject( byteArray );
  } // OAbstractRecord


  /**
   * Gets the value of recordId.
   * @return Value of recordId.
   */
  public int getRecordId()
  {return this.recordId;}

  /**
   * Sets the value of recordId.
   * @param value  Value to assign to recordId.
   */
  void setRecordId(int  value)
  {this.recordId = value;}


  /**
   * Writes the record object to a byte array, which can then be stored
   * in the file system.
   *
   * @return a <code>byte[]</code> value
   * @exception IOException if an error occurs
   */
  public byte [] writeObject ()
    throws IOException
  {
    byteStream.reset();
    writeStream( dataStream);
    return byteStream.toByteArray();
  } // writeObject

  /**
   * Abstract method that writes the current record to a dataStream.
   * <br>
   * Called from the {@link #writeObject writeObject} method.
   * <br>
   * User will implement this by doing write() calls on the dataStream.
   * <p>
   * <b>Example:</b>
   * <pre>
   * <code>
   * String field1, field2;
   * int field3;
   * ...
   * protected void writeStream( DataOutputStream dataStream )
   * {
   *   dataStream.writeUTF( field1 );
   *   dataStream.writeUTF( field2 );
   *   dataStream.writeInt( field3 );
   * }
   * </code>
   * </pre>
   * <br>
   * where field<i>n</i> are the fields in your record.
   * @param dataStream The data stream to write to
   * @throws IOException If there is a problem with creating the streams.
   */
  abstract protected void writeStream( DataOutputStream dataStream )
    throws IOException;

  /**
   * Reads in a record from the given byte array.
   * <br>
   * Calls the abstract method {@link #populate populate()} to populate the record.
   * @param byteArray The byte array that was read in.
   * @throws IOException If there is a problem with creating the streams
   */
  public  void readObject ( byte [] byteArray )
    throws IOException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractRecord.readObject ENTERED" );

    ByteArrayInputStream byteInputStream = null;
    DataInputStream dataInputStream = null;

    try
    {
      byteInputStream = new ByteArrayInputStream( byteArray );
      dataInputStream = new DataInputStream( byteInputStream );
      populate( dataInputStream );
    }
    finally
    {
      try
      {
        byteInputStream.close();
          dataInputStream.close();
      } catch ( Exception e ) {
          if ( Debugger.ON ) Logger.dev( "OAbstractRecord.readObject EXCEPTION line 168" );
          e.printStackTrace();}
    }

    if ( Debugger.ON ) Logger.dev( "OAbstractRecord.readObject EXIT" );
  } // readObject

  /**
   * Abstract record that reads in the record from a data stream.
   * <br>
   * Called from the {@link #readObject readObject} method.
   * <p>
   * <b>Example:</b>
   * <pre>
   * <code>
   * String field1, field2;
   * int field3;
   * ...
   * void populate( DataInputStream inputStream )
   * {
   *  field1 = inputStream.readUTF();
   *  field2 = inputStream.readUTF();
   *  field3 = inputStream.readInt();
   * }
   * </code>
   * </pre>
   * where field<i>n</i> are the fields in your record.
   * @param inputStream Stream to read data from.
   * @throws IOException If there is a problem with the DataInputStream.
   */
  abstract protected void populate ( DataInputStream inputStream )
    throws IOException;


  /** Method used to return the hash code. This is used by the
   * default cache to determine a hash value.
   * @return An integer value representing the id of
   * this record.
   */
  public int hashCode() {
      return recordId;
  }

}// OAbstractRecord


