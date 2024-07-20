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

import javax.microedition.rms.RecordStoreException;
import javax.microedition.rms.RecordStoreFullException;
import java.io.IOException;

/**
 * Class used to store arrays of {@link com.nextel.rms.OAbstractRecord}s.
 * <br>
 * This class is to be used in conjunction with a predefined 
 * store. For deletion of a record store, deletion of individual records,
 * and retrieval of individual records refer to the 
 * {@link OAbstractStore} object. This class handles creation of new
 * records, updating of records and retrieval of all records from the 
 * record store.
 * @author  Ryan Wollmuth
 */
public class OPersistentArray 
{
  // Used to talk to the RMS
  private OAbstractStore store;
  
  /**
   * Creates a new <code>OArrayStore</code> instance with the 
   * given <code>OAbstractStore</code>.
   * @param store A reference to an already opened data store.
   */
  public OPersistentArray(OAbstractStore store) 
  {
    this.store = store;
  }
  
  /**
   * Retrieves all records from the underlying RMS and passes them back
   * in a <code>OAbstractRecord[]</code>.
   * @return An array of records (null if no records found)
   * @throws RecordStoreException
   * @throws IOException 
   */
  public OAbstractRecord[] retrieve()
    throws RecordStoreException, IOException
  {
    return store.getAll();
  }
  
  /**
   * Saves records in the array down to the underlying RMS.
   * Detects whether it is a new record or needs to be updated.
   * @param records An array of records to store.
   * @throws IOException
   * @throws RecordStoreFullException
   * @throws RecordStoreException
   */
  public synchronized void store( OAbstractRecord[] records )
    throws IOException, RecordStoreFullException, RecordStoreException
  {
    OAbstractRecord record;
    for ( int i = 0; i < records.length; i++ )
    {
      record = records[ i ];
      if ( record.getRecordId() == record.NO_RECORD_ID )
        store.addRecord( record );
      else 
        store.updateRecord( record );
    }
  }
}
