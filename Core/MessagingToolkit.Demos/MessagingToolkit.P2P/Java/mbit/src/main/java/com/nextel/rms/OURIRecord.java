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

import java.io.DataOutputStream;
import java.io.DataInputStream;
import java.io.IOException;
import com.nextel.util.Logger;
/**
 * A record in OURIStore.
 * <p>
 * Each record defines the source and destination URIs for an OSynchStore.
 * This record is used by the {@link com.nextel.rms.OSynchStore#upload upload()}
 * and {@link com.nextel.rms.OSynchStore#download download()} methods of the 
 * OSynchStore.
 *
 * @author Glen Cordrey
 */
public class OURIRecord extends OAbstractRecord
{
  /** the name of the store that these URIs are for */
  public String storeName;

  /** the URI for getting data to download to storeName */
  public String sourceURI;

  /** the URI to which data from storeName is uploaded */
  public String destinationURI;
  
  /**
   * Creates an <code>OURIRecord</code> instance.
   */
  public OURIRecord()
  {
    super();
  }
  
  /**
   * Creates an <code>OURIRecord</code> instance.
   * @param array The record to read in.
   */
  public OURIRecord( byte[] array )
    throws IOException
  {
    super( array );
  }
  
  /** 
   * Writes the URIRecord to the dataStream
   * @param dataStream The data stream to write the URIRecord to.
   * @throws IOException If a problem occurs with the stream.
   */
  protected void writeStream( DataOutputStream dataStream )
    throws IOException
  { 
    dataStream.writeUTF( storeName );
    dataStream.writeUTF( sourceURI );
    dataStream.writeUTF( destinationURI );
  } // writeStream

  /**
   * Populates the URIRecord with the data from the inputStream.
   * @param inputStream data stream to read the record info from.
   * @throws IOException If a problem occurs with the stream.
   */
  protected void populate( DataInputStream inputStream )
    throws IOException
  {
    storeName = inputStream.readUTF();
    sourceURI = inputStream.readUTF();
    destinationURI = inputStream.readUTF();
  } // populate
  
} // OURIRecord

