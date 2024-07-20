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

import java.util.Vector;
import javax.microedition.rms.RecordStoreFullException;
import javax.microedition.rms.RecordStoreException;
import java.io.IOException;

/**
 * Class used to store vectors of {@link com.nextel.rms.OAbstractRecord}s.
 * <br>
 * This class is to be used in conjunction with a predefined 
 * store. For deletion of a record store, deletion of individual records,
 * and retrieval of individual records refer to the 
 * {@link OAbstractStore} object. This class handles creation of new
 * records, updating of records and retrieval of all records from the 
 * record store using the <code>Vector</code> object.
 * @author  Ryan Wollmuth
 */
public class OPersistentVector
{
  // The local copy of the store
  private OAbstractStore store;
  
  /**
   * Creates a new <code>OVectorStore</code> instance.
   */
  public OPersistentVector(OAbstractStore store)
  {
      this.store = store;
  }
  
  /** 
   * Saves all records in a Vector into the record store.
   * If the record does not have a record id, we will assume that
   * it is a newly added record.
   * @param v A vector of AbstractRecords
   * @throws IOException
   * @throws RecordStoreFullException
   * @throws RecordStoreException
   * @exception ClassCastException If any element of <code>v</code> is not
   * an OAbstractRecord.
   */
  public synchronized void store( Vector v )
    throws IOException, RecordStoreFullException, RecordStoreException
  { 
    
    // Parse through the Vector and save all the records
    OAbstractRecord record;
    for ( int i = 0; i < v.size(); i++ )
    {
      record = (OAbstractRecord) v.elementAt( i );
      if ( record.getRecordId() != record.NO_RECORD_ID )
        store.updateRecord( record );
      else
        store.addRecord( record );
    }
  }
  
  /** 
   * Get all the records in the datastore and pass back in a Vector.
   * @return A vector containing all of the records in the datastore.
   * @throws IOException
   * @throws RecordStoreException
   */
  public Vector retrieve()
    throws IOException, RecordStoreException
  {
    OAbstractRecord[] records = store.getAll();
    Vector v = new Vector();
    OAbstractRecord record;
    if ((records != null) && (records.length > 0)) {
        for ( int i = 0; i < records.length; i++ )
        {
            record = records[ i ];
            v.addElement( record );
        }
    }
    return v;
  }
}
