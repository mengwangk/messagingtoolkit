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

import com.nextel.rms.OAbstractRecord;
import com.nextel.util.*;
import com.nextel.io.OExtern;
import com.nextel.net.HttpRequest;
import com.nextel.exception.NotFoundException;

import java.io.IOException;
import java.io.DataOutputStream;
import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import javax.microedition.rms.RecordStore;
import javax.microedition.rms.RecordStoreException;
import javax.microedition.rms.RecordStoreFullException;
import javax.microedition.rms.RecordStoreNotOpenException;
import javax.microedition.rms.RecordStoreNotFoundException;
import javax.microedition.rms.RecordFilter;
import java.io.DataInputStream;
import java.util.Hashtable;


/**
 * An abstract base class for record
 * stores that can be synchronized with external sources.
 * @author Glen Cordrey
 */
abstract public class OSynchStore
  extends OAbstractStore implements OExtern
{
  // Timeout length in seconds?...
  private static final int TIMEOUT = 0;
  // Number of retries...
  private static final int RETRIES = 3;
  
  /**
   * Creates a new <code>OSynchStore</code> instance, opens the store and
   * creates the cache, if the enableCache flag is true...
   */
  protected OSynchStore ( String name, boolean enableCache )
  {
    super( name, enableCache );
  } // constructor

  /**
   * Creates a new <code>OSynchStore</code> instance, opens the store and
   * creates the cache with the given class cache name.
   */
  protected OSynchStore ( String name, String cacheClassName )
  {
    super( name, cacheClassName );
  } // constructor
  
  /**
   * Sends the store's records to the external destination.
   *
   */
  public void upload()
    throws NotFoundException
  {
    InputStream stream = null;
    ByteArrayOutputStream byteOutput = new ByteArrayOutputStream();
    DataOutputStream outputStream = new DataOutputStream( byteOutput );
    
    try
    {
      OURIRecordFilter uriFilter = new OURIRecordFilter( this.getClass().getName() );
      OAbstractRecord[] uriRecords = OURIStore.getInstance().getAll( null, uriFilter );
      if ( uriRecords == null || uriRecords.length > 1 || uriRecords.length == 0 )
      {
        throw new NotFoundException( "No OURIRecord found." ); 
      } else {
        OURIRecord uriRecord = (OURIRecord)uriRecords[ 0 ];
        String url = uriRecord.destinationURI;
        byte[] recordBytes = writeExternal();
        
        if ( recordBytes != null )
        {
          // Make the HTTP connection and send it on its way!
          Hashtable headers = new Hashtable();
          headers.put( "User-Agent", "Profile/MIDP-1.0 Configuration/CLDC-1.0" );
          headers.put( "Content-Type", "application/x-www-form-urlencoded" );
          HttpRequest httpRequest = new HttpRequest( url, TIMEOUT, RETRIES );
          httpRequest.post( headers, recordBytes );
          httpRequest.cleanup();
        }
      }
    }
    catch ( Exception e ) 
    {
      e.printStackTrace();
    }
    finally 
    {
      try
      {
        stream.close();
        byteOutput.close();
        outputStream.close();
      } catch ( Exception io ) {}
    }
  } // upload  

  /**
   * Populates the data store from the external source.
   *
   */
  public void download()
    throws NotFoundException
  {
    // Deleting all records before downloading...
    InputStream inputStream = null;
    try
    {
    
      // Find the record that contains the sourceURI.
      OURIRecordFilter filter = new OURIRecordFilter( this.getClass().getName() );
      OAbstractRecord[] uriRecords = OURIStore.getInstance().getAll( null, filter );
      if ( uriRecords == null || uriRecords.length > 1 )
      {
        throw new NotFoundException( "No OURIRecord found." ); 
      } else {
        OURIRecord uriRecord = (OURIRecord)uriRecords[ 0 ];
        String url = uriRecord.sourceURI;
        HttpRequest httpRequest = new HttpRequest( url, TIMEOUT, RETRIES );
      
        // An assumption is made that any parameters are set on the GET line
        // of the URL. 
        inputStream = httpRequest.get();
        OSynchRecord[] records = parse( inputStream );
        
        // Now add all of these into the record store.
        for ( int i = 0; i < records.length; i++ )
        {
          OSynchRecord record = records[ i ];
          super.addRecord( record );
        }
        httpRequest.cleanup();
      }
    } catch ( Exception e ) { e.printStackTrace(); }
    finally 
    {
      try
      {
        inputStream.close();
      } catch ( IOException io ) {}
    }
  } // download
  
  /**
   * Abstract method that will be able to parse out an InputStream
   * into OSynchRecords by breaking the stream into record
   * byte[] and calling <code>readExternal()</code> on that record.
   * @param stream The input stream contains the external data.
   * @return An array of OSynchRecords representing the records 
   * in the byte array.
   * @throws IOException if there are problems reading the input stream.
   */ 
  abstract protected OSynchRecord[] parse( InputStream stream )
   throws IOException;
  
}// OSynchStore

