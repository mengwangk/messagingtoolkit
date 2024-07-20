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

import com.nextel.rms.OSynchStore;
import com.nextel.rms.OSynchRecord;
import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OURIRecord;
import com.nextel.rms.OURIStore;
import com.nextel.rms.OURIRecordFilter;
import com.nextel.net.OInputStream;
import com.nextel.util.StringUtils;
import com.nextel.util.Logger;

import java.util.Vector;
import java.io.IOException;
import java.io.EOFException;
import java.io.InputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import javax.microedition.rms.RecordStoreException;

/**
 * Class used to create, update, delete and search for ExpenseRecords.
 * <P>
 * This class extends OSynchStore, giving it the ability to upload or download
 * an entire "store" from the mobile device to a networked application. In our
 * case, we are talking to a servlet.
 * <P>
 * Most of the functionality for storing records to the underlying RMS
 * are found in the {@link com.nextel.rms.OAbstractStore} class.
 * @author Ryan Wollmuth
 */

public class ExpenseStore extends OSynchStore
{
  private static ExpenseStore store;
  
  // Servlet paths... 
  
  /** 
   * The protocol when an upload/download is called
   * <P>
   * Default is http. This is here for a user to add another protocol (https?).
   */
  public static final String PROTOCOL = "http://";
  
  /** The servlet to hit when upload is called. */
  public static final String UPLOAD_SERVLET = "/ExpenseServlet";
  
  /** The servlet to hit when download is called. */
  public static final String DOWNLOAD_SERVLET = "/ExpenseServlet?sync=true";
  
  /** 
   * Creates a <code>ExpenseStore</code> instance.
   */
  private ExpenseStore() {
    super( "expenses", true );
    
    try
    {
      OURIRecordFilter filter = new OURIRecordFilter( this.getClass().getName() );
      OAbstractRecord[] records = OURIStore.getInstance().getAll( null, filter );
    } catch ( Exception e ) { e.printStackTrace(); }
  }
  
  /**
   * Return an instance of this store.
   */
  public static ExpenseStore getInstance()
  {
    if ( store == null )
      store = new ExpenseStore();
    return store;
  }

  /**
   * Abstract method to create an empty record.
   * <P>
   * <b>Example: </b>
   * <pre>
   * <code>
   * public class MyStore extends OAbstractStore
   * {
   *  ...
   *  protected OAbstractRecord createRecord()
   *  {
   *    return new MyRecord();
   *  }
   * </code>
   * </pre>
   * where MyRecord is the name of your record.
   * @return An empty OAbstractRecord.
   */
  protected OAbstractRecord createRecord()
  {
    return new ExpenseRecord();
  }
  
  /**
   * Reads an instance of this class that is represented in an external form
   * (the same form used by {@link #writeExternal}.
   *
   * @param in The object in externalized form.
   */
  public void readExternal(byte[] in)
  {
  }
  
  /**
   * Abstract method that will be able to parse out a byte[]
   * into OAbstractRecords by breaking the array into record
   * byte[] and calling <code>readExternal()</code> on that record.
   * @param bytes The byte array that was read from the input stream.
   * @return An array of OAbstractRecords representing the records
   * in the byte array.
   * @throws IOException if there are problems reading the byte array.
   */
  protected OSynchRecord[] parse( InputStream stream )
    throws IOException
  {
    String xmlBuffer = new String();
    DataInputStream dataStream = new DataInputStream( stream );
    xmlBuffer = dataStream.readUTF();
    dataStream.close();
    
    OSynchRecord[] recordArray = new OSynchRecord[ 0 ];
    
    // Now we have the XML buffer. Break it up into records.
    if ( xmlBuffer != null )
    {
      Vector v = StringUtils.tokenize( xmlBuffer, "</record>" );
      recordArray = new OSynchRecord[ v.size() ];
      for ( int i = 0; i < v.size(); i++ )
      {
        String recordString = (String) v.elementAt( i );
        ExpenseRecord record = (ExpenseRecord)createRecord();
        record.readExternal( recordString.getBytes() );
        recordArray[ i ] = record;
      }
    }
    return recordArray;
  }
  
  /**
   * Writes the object in a form that can be transmitted external to the
   * handset.
   * @return a <code>byte[]</code> value
   */
  public byte[] writeExternal()
  {     
    StringBuffer recordBuf = new StringBuffer();
    try
    {
      OAbstractRecord[] records = super.getAll( null, new ExpenseRecordFilter() );

      if ( records != null )
      {
        for ( int i = 0; i < records.length; i++ )
        {
          OSynchRecord record = (OSynchRecord)records[ i ];
          recordBuf.append( new String( record.writeExternal() ) );
        }
      }
    } 
    catch ( Exception e )
    {
      Logger.ex( e );
    }    
    return recordBuf.toString().getBytes();
  }
  
}
