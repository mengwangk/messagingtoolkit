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

import java.util.Hashtable;
import java.util.Enumeration;
import javax.microedition.rms.InvalidRecordIDException;
import javax.microedition.rms.RecordComparator;
import javax.microedition.rms.RecordFilter;
import java.util.Vector;

import com.nextel.util.Debugger;
import com.nextel.util.Logger;

/**
 * Implementation of a data cache.
 * <p>
 * This cache is maintained as a Hashtable where the key is the record id
 * (obtained via {@link com.nextel.rms.OAbstractRecord#getRecordId}) and the
 * value is the {@link com.nextel.rms.OAbstractRecord}.
 *
 * @author  Ryan Wollmuth
 */
public class ODataCacheImpl implements ODataCache
{
    // The cache itself.
    private Hashtable CACHE = new Hashtable( 20 );
    
    // Whether the cache is initialized or not
    private boolean initialized;
    
    // Key used to add and drop records
    private Integer key;

    private RecordComparator comparator;
    
    
    /**
     * Creates a new <code>ODataCacheImpl</code> instance.
     */
    public ODataCacheImpl() { 
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl() called" );
    }

    /**
     * Get the size of the cache (number of records)
     * @return Size of the cache
     */
    public int getNumRecords()
    {
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.getNumRecords() started" );
      if ( initialized )
      {
        if ( Debugger.ON )
          Logger.dev( "ODataCacheImpl.getNumRecords() exiting with size of " + 
            CACHE.size() );
        return CACHE.size();
      }
      // Should never get here. In OAbstractStore, there is a check to see
      // if the cache is initialized.
      return -1; // Cache has not been initialized.
    }
    
    /**
     * Returns whether the cache has been initialized or not
     * @return A boolean flag telling whether it is initialized or not.
     */
    public boolean isInitialized()
    {
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.isInitialized() called" );
      return initialized;
    }
    
    public void setInitialized( boolean initialized )
    {
      this.initialized = initialized;
    }
    
    /**
     * Delete all the records from the cache
     */
    public void deleteAll()
    {
      if ( initialized )
        CACHE.clear();
    }
     
    /**
     * Sorting routine used by getAll( comparator ) to sort using a record
     * comparator.
     * @param records An array of OAbstractRecords
     * @return A sorted array of OAbstractRecords
     */
    private OAbstractRecord[] quickSort( OAbstractRecord[] records )
    {
      int start = 0;
      int end = records.length - 1;
      return quickSort( records, start, end );
    }
 
    /**
     * The quicksort method
     */
    private OAbstractRecord[] quickSort( OAbstractRecord[] records , int start, int end )
    {
      int piv;
      if (end > start)
      {
        piv = partition( records, start, end);
        quickSort( records, start, piv - 1);
        quickSort( records, piv + 1, end);
      }
      return records;
    }
    
    /** 
     * The partition method
     */
    private int partition( OAbstractRecord[] records, int start, int end )
    {
        int left, right;
        OAbstractRecord pivot = records[ end ];

        left = start - 1;
        right = end;
        for ( ;; )
        {
            while ( compare( pivot, records[ ++left ] ) == comparator.FOLLOWS )
            {
                if (left == end) break;
            }
            while ( compare( pivot, records[ --right ] ) == comparator.PRECEDES )
            {
                if (right == start) break;
            }
            if (left >= right) break;
            records = swap( records, left, right);
        }
        records = swap( records, left, end);
        return left;
    }

    /**
     * Swap method used by partition to swap 2 records quickly.
     */
    private OAbstractRecord[] swap( OAbstractRecord[] records, int pos1, int pos2 )
    {
      try
      {
        OAbstractRecord temp = records[ pos1 ];
        records[ pos1 ] = records[ pos2 ];
        records[ pos2 ] = temp;
      }
      catch ( Exception e ) 
      {
        e.printStackTrace();
      }
      return records;
    }
    
    /**
     * Does a RecordComparator.compare on 2 AbstractRecords
     * @param record1 The first record to compare
     * @param record2 The second record to compare
     * @return An int that is returned from RecordComparator.compare()
     */
    private int compare( OAbstractRecord record1, OAbstractRecord record2 )
    {
      try
      {
        byte[] byteArray1 = record1.writeObject();
        byte[] byteArray2 = record2.writeObject();
      
        return comparator.compare( byteArray1, byteArray2 );
      }
      catch ( Exception e ) 
      {
        e.printStackTrace();
      }
      return -1;
    }
    
    /**
     * Retrieve all records from the cache, using the RecordComparator
     * to sort the records. 
     * @param comparator A RecordComparator used to sort the records.
     * @return An array of AbstractRecords, sorted by the provided comparator.
     *  null is returned if no records are found in the cache.
     */
    public OAbstractRecord[] getAll( RecordComparator comparator, RecordFilter filter )
    {
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.getAll( comparator, filter ) started" );

      this.comparator = comparator;
      OAbstractRecord[] records = getAll();
      
      // If records = null, there are no records in the cache.
      // Get out.
      if ( records == null ) 
        return null;
      
      if ( initialized && filter != null )
      {
        // Filter the records out that don't belong
        Vector recordVector = new Vector();
        OAbstractRecord record = null;
        for ( int i = 0; i < records.length; i++ )
        {  
          record = records[ i ];
          byte[] byteArray = null;
          try
          {
            byteArray = record.writeObject();
          } catch ( Exception e ) { Logger.ex( e ); }
          
          if ( filter.matches( byteArray ) )
          {
            recordVector.addElement( record );
          }
        }
        
        records = new OAbstractRecord[ recordVector.size() ];
        for ( int i = 0; i < recordVector.size(); i++ )
          records[ i ] = (OAbstractRecord) recordVector.elementAt( i );
      }
      if ( initialized && comparator != null )
      {
        try
        {
          records = quickSort( records );
        }
        catch ( Exception e ) 
        {
          e.printStackTrace();
        }
        return records;
      }
      return records;
    }
    
    /**
     * Retrieve all records from the cache.
     * @return An array of AbstractRecords
     */
    public OAbstractRecord[] getAll()
    {
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.getAll() started" );
      if ( initialized )
      {
        Enumeration keys = CACHE.keys();
        OAbstractRecord record;
        
        OAbstractRecord[] records = new OAbstractRecord[ CACHE.size() ];
        int counter = 0;
        while ( keys.hasMoreElements() )
        {
          key = (Integer)keys.nextElement();
          record = (OAbstractRecord)CACHE.get( key );
          records[ counter++ ] = record;
        }
        if ( Debugger.ON )
          Logger.dev( "ODataCacheImpl.getAll() exiting with " +
            records.length + " records." );
        return records;
      } 
      // Should never get here as there is a check to see if the cache
      // is initialized in OAbstractStore
      return null;
    }
    
    /**
     * Adds a record into the cache.
     * @param OAbstractRecord The record to insert into the cache
     * @param int Record ID of the record being stored
     */
    public void add(OAbstractRecord record)
    {
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.add() started" );
      
      key = new Integer( record.getRecordId() );
      CACHE.put( key, record );
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.add() exiting" );
    }
    
    /**
     * Deletes a record from the cache.
     * @param record The record you wish to delete
     * @throws InvalidRecordIDException If the record id is not found.
     */
    public void delete( OAbstractRecord record)
      throws InvalidRecordIDException
    {
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.delete() started" );
      key = new Integer( record.getRecordId() );
      if ( CACHE.remove( key ) == null )
        throw new InvalidRecordIDException( "Record ID " + 
          record.getRecordId() + " not found in cache." );
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.delete() exiting" );
    }
    
    /**
     * Retrieve a record from the cache.
     * @param int Record ID of the record to retrieve.
     * @return An OAbstractRecord for the given id (null if cache is not initialized)
     * @throws InvalidRecordIDException If the record id is not found.
     */
    public OAbstractRecord get(int recordId) throws InvalidRecordIDException
    {
      OAbstractRecord record;
      if ( Debugger.ON )
        Logger.dev( "ODataCacheImpl.get() started" );
      if ( initialized )
      {
        key = new Integer( recordId );
        
        record = (OAbstractRecord) CACHE.get( key );
        if ( record == null )
          throw new InvalidRecordIDException( "Record ID " + recordId + 
            " not found in the cache." );
        else
        {
          if ( Debugger.ON )
            Logger.dev( "ODataCacheImpl.get() exiting normally" );
          return record;
        }
      }
      // Should never get here as the OAbstractStore checks to see if the
      // cache is enabled. But just in case...
      return null;
    }    
    
    /**
     * Lifecycle method to load the records into the cache.
     * @param records An array of records to populate the cache with.
     */
    public void load(OAbstractRecord[] records)
    {
      if ( Debugger.ON ) Logger.dev( "load( records ) started" );
      for ( int i = 0; i < records.length; i++ )
      {
        OAbstractRecord record = records[ i ];
        add( record );
      }
      if ( Debugger.ON ) Logger.dev( "load( records ) exiting normally." );
    }
    
    /**
     * Lifecycle method to unload the cache.
     * Used within the <code>OAbstractStore</code> in places where an
     * <code>OutOfMemoryException</code> could be thrown. In those cases,
     * the store will empty the cache. It will reinitialize the next time
     * <code>getAll()</code> is called.
     */
    public void unload()
    {
      // Internally, we are going to call deleteAll() and set the initialized
      // flag to false.
      deleteAll();
      initialized = false;
    }
    
}
