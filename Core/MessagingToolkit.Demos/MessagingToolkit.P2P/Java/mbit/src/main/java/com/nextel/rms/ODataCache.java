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

import javax.microedition.rms.InvalidRecordIDException;
import javax.microedition.rms.RecordComparator;
import javax.microedition.rms.RecordFilter;
/**
 * Interface for classes to implement a data cache for RMS activities
 *
 * @author Ryan Wollmuth
 */
public interface ODataCache
{
  /**
   * Adds a record into the cache.
   * @param record The record to insert into the cache
   * @param int Record ID of the record being stored
   */
  public void add( OAbstractRecord record );

  /**
   * Deletes a record from the cache.
   * @param record The record you wish to delete.
   * @throws InvalidRecordIDException If the record id is not found
   */
  public void delete( OAbstractRecord record  )
    throws InvalidRecordIDException;
  
  /**
   * Delete all the records from the cache
   */
  public void deleteAll();
  
  /**
   * Retrieve a record from the cache.
   * @param recordId Record ID of the record to retrieve.
   * @return An OAbstractRecord for the given id
   * @throws InvalidRecordIDException If the record id is not found.
   */
  public OAbstractRecord get( int recordId )
    throws InvalidRecordIDException;
  
  /** 
   * Retrieve all records from the cache.
   * @return An array of AbstractRecords
   */
  public OAbstractRecord[] getAll();
  
  /**
   * Retrieve all records from the cache, using the RecordComparator
   * to sort the records and the RecordFilter to filter records.
   * @param comparator A RecordComparator used to sort the records. May be null.
   * @param filter A RecordFilter used to filter the records. May be null.
   * @return An array of AbstractRecords, sorted by the provided comparator.
   */
  public OAbstractRecord[] getAll( RecordComparator comparator, RecordFilter filter );
  
  /**
   * Get the size of the cache (number of records).
   * @return Size of the cache
   */
  public int getNumRecords();
  
  /**
   * Returns whether the cache has been initialized or not
   * @return A boolean flag telling whether it is initialized or not.
   */
  public boolean isInitialized();
  
  /**
   * Sets whether the cache has been initialized or not.
   * This is set within <code>OAbstractStore</code>. It is initialized after
   * a <code>getAll()</code> has been called without a RecordFilter. It is 
   * uninitialized when the <code>deleteAll()</code> method is called.
   * @param initialized The boolean flag telling whether the cache is 
   * intialized or not.
   */
  public void setInitialized( boolean initialized );
  
  /**
   * Lifecycle method to load the records into the cache.
   * @param records An array of records to populate the cache with.
   */
  public void load( OAbstractRecord[] records );
  
  /**
   * Lifecycle method to unload the cache.
   * Used within the <code>OAbstractStore</code> in places where an
   * <code>OutOfMemoryException</code> could be thrown. In those cases,
   * the store will empty the cache. Suggested use would be to empty your cache 
   * and possibly set the initialzed flag to false. However, that is also done
   * in the OAbstractStore. It will reinitialize the next time 
   * <code>getAll()</code> is called.
   */
  public void unload();

} // ODataCache
