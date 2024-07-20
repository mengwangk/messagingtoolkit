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
import java.io.IOException;
import javax.microedition.rms.RecordComparator;
import javax.microedition.rms.RecordFilter;
import javax.microedition.rms.RecordEnumeration;
import javax.microedition.rms.RecordStore;
import javax.microedition.rms.RecordStoreException;
import javax.microedition.rms.RecordStoreFullException;
import javax.microedition.rms.RecordStoreNotOpenException;
import javax.microedition.rms.RecordStoreNotFoundException;
import javax.microedition.rms.InvalidRecordIDException;

/**
 * An abstraction of
 * {@link javax.microedition.rms.RecordStore javax.microeditions.rms.RecordStore}
 * that accesses the store
 * contents as {@link com.nextel.rms.OAbstractRecord}s rather than byte arrays.
 * <P>
 * Here's a code example for a store that contains instances of
 * <code>MyRecord</code>, which subclasses
 * {@link com.nextel.rms.OAbstractRecord}:
 * <p>
 * <pre>
 * <code>
 * public class MyStore implements OAbstractStore
 * {
 *   private static MyStore thisStore;
 * 
 *   private MyStore()
 *   {
 *    super( "My Store Name", // name of the store
 *           true // use default cache
 *          );
 *   }
 * 
 *   public static MyStore getInstance()
 *   {
 *     if ( thisStore == null ) thisStore = new MyStore();
 *     return thisStore;
 *   }
 *    
 *   protected OAbstractRecord createRecord()
 *   { // this store contains MyRecord instances
 *     return new MyRecord(); 
 *   } 
 * }
 * </code>
 * </pre>
 * <h2>Caching</h2>
 * Any class that extends this class can enable in-memory caching
 * of its data store via
 * one of the following mechanisms:
 * <ul>
 * <p><li>Call {@link #OAbstractStore( String, boolean )} with the second
 * argument (<code>enableCache</code>) equal to <code>true</code>. This will use
 * {@link com.nextel.rms.ODataCacheImpl} as the data cache</li>.
 *
 * <p><li>Call {@link #OAbstractStore( String, String )}, which will use the
 * data cache defined by the second argument.</li>
 *
 * <p><li>Call {@link #setCache}, which will use the
 * data cache defined by the argument.</li>
 * </ul>
 * <p>This class instantiates the data cache
 * via <pre><code>Class.forName(classname ).newInstance()</code></pre>
 * Therefore any MIDlet that uses a cache-enabled store must contain in its jad file
 * property <pre><code>iDEN-Install-Class-n</code></pre>
 * which is discussed in more
 * detail in the 
 * <i>Motorola Multi-Communication Device J2ME Developers Guide</i> available at
 * <a href="http://www.idendev.com">http://www.idendev.com</a>.
 * If the default cache is used the value would be
 * <pre><code>iDEN-Install-Class-1=com.nextel.rms.ODataCacheImpl</code></pre>
 * (If you use a custom cache class you would replace the 
 * <code>com.nextel.rms.ODataCacheImpl</code> with the fully-specified name of
 * your custom cache class.)
 * <p>
 * {@link #OAbstractStore( String, String )} and {@link #setCache} let you
 * use a custom cache that you have written instead of the default cache.
 * You might want to implement a custom cache
 * if, for example, you frequently access the records
 * in a specific order, such as sorted by some field in the record,
 * and for efficiency reasons want the cache to retain the sort order.
 * <p>
 * Any cache, whether the {@link com.nextel.rms.ODataCacheImpl default} or a
 * custom cache, is treated as a write-through cache, i.e. all add, delete, and
 * update operations are reflected in both the cache and the underlying data
 * store. Operations to the underlying data store are synchronous, i.e.
 * {@link #addRecord} and all other methods that modify the data store do not
 * return until the operation on the underlying data store is complete.
 * <p>
 * If you obfuscate when creating the MIDlet jar you must ensure
 * that the class file for the data cache is not renamed, i.e. that it
 * retains its original class name. How you do this depends upon the obfuscator
 * that you use.
 * <p> 
 * The data cache is empty until the first call to {@link #getAll}, which
 * retrieves all of the records from the RMS and populates the cache.
 * To improve perceived performance when the data store is first accessed as the
 * result of a user action you might consider pre-poulating the cache by
 * creating, when your MIDlets starts, a thread that calls {@link #getAll}.
 * @author Glen Cordrey
 */
abstract public class OAbstractStore
{
  // Name of the store
  private String storeName;

  // Instance of the persistent store
  private RecordStore myStore = null;

  // Data cache
  private ODataCache CACHE;

  // Indicates whether the cache is enabled or not 
  private boolean cacheEnabled = false;

  // Name of the cache class. Defaults to DEFAULT_CACHE_CLASS.
  private String cacheClassName = DEFAULT_CACHE_CLASS;

  // Number of times the store has been opened.
  private int storeOpened = 0;

  // The default cache class.
  private static final String DEFAULT_CACHE_CLASS =
    "com.nextel.rms.ODataCacheImpl";

  /**
   * Abstract method to create an empty record.
   * <P>
   * <b>Example: </b>
   * <pre>
   * <code>
   * public class MyStore extends OAbstractStore
   * {
   *   ...
   *   protected OAbstractRecord createRecord()
   *   {
   *     return new MyRecord();
   *   }
   * </code>
   * </pre>
   * where MyRecord is the name of your record.
   * @return An empty OAbstractRecord.
   */
  abstract protected OAbstractRecord createRecord();

  /**
   * Creates a new <code>OAbstractStore</code> instance, opens the store and
   * creates the default cache (ODataCacheImpl) if the enableCache flag is true.
   * @param name Name of the persistent store for the underlying RMS.
   * @param enableCache Flag to tell whether caching is enabled.
   */
  protected OAbstractStore ( String name, boolean enableCache )
  {
    storeName = name;
    // ensure that the store is open
    getStore();
    if ( enableCache )
      enableCache();
  } // constructor

  /**
   * Creates a new <code>OAbstractStore</code> instance, opens the store and
   * creates the cache with the given class cache name.
   * @param name Name of the persistent store for the underlying RMS.
   * @param cacheClassName Name of the caching implementation 
   */
  protected OAbstractStore ( String name, String cacheClassName )
  {
    storeName = name;
    // ensure that the store is open
    getStore();
    this.cacheClassName = cacheClassName;
    enableCache();
  } // constructor

  /**
   * Enables the data cache. 
   * <p>
   * By default, the cache class will be 
   * {@link com.nextel.rms.ODataCacheImpl}.
   * <br>However, you can set the name of the 
   * cache class by calling the {@link #setCache} method.
   * If a class name was passed in ( newly implemented cache ) and is not 
   * found by the data store, caching is disabled.
   * @return A boolean telling whether the cache was successfully enabled.
   */
  private boolean enableCache()
  {
    if ( Debugger.ON )
      Logger.dev( "OAbstractStore.enableCache started" );
    try
    {
      CACHE = (ODataCache)Class.forName( cacheClassName ).newInstance();
      cacheEnabled = true;
    }
    catch ( ClassNotFoundException ex )
    {} // Do nothing. The cache will not be set
    catch ( InstantiationException ie )
    {} // Do nothing. The cache will not be set
    catch ( IllegalAccessException iae )
    {}
    return cacheEnabled;
  }
  /**
   * Sets the name of the cache class and enables the cache.
   * <p>
   * If caching was enabled, the previous cache will be overridden by 
   * this implementation.
   * @param cacheClassName A properly formatted class name 
   *  (ex: com.nextel.rms.ODataCacheImpl)
   */
  public void setCache( String cacheClassName )
  {
    this.cacheClassName = cacheClassName;
    enableCache();
  }

  /**
   * Gets the persistent store.
   * <p>
   * This 
   * @return a <code>RecordStore</code> value
   */
  private RecordStore getStore ()
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.getStore ENTERED" );

    if ( myStore == null )
    {
      try
      {
          if ( Debugger.ON ) Logger.dev( "OAbstractStore.getStore opening Store" );
          myStore = RecordStore.openRecordStore( storeName, true );
        storeOpened++;
      }
      catch( RecordStoreException ex )
      {
          // TBD: Don't log the exception here, but instead throw a RuntimeError
          // (or custom unchecked exception) which will contain in it's message
          // the string to be displayed, and the caller will then log the
          // exception ( which results in a screen displaying the message).
          // or possibly just rethrow this exception?
          String message = "Unable to access " + storeName;
          Logger.ex( ex, message );
        // TBD: replace RuntimeException with a ChainedError
          throw new RuntimeException( message );
      }
    }
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.getStore EXITTING" );
    return myStore;

  } // getStore

  /**
   * Writes an  record into the persistent store.
   *
   * @param record The  record to write
   * @exception IOException if an error occurs
   * @exception RecordStoreFullException if an error occurs
   * @exception RecordStoreException if an error occurs
   */
  public synchronized void addRecord ( OAbstractRecord record )
    throws IOException, RecordStoreFullException, RecordStoreException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.addRecord ENTERED" );
    byte [] byteArray = record.writeObject();
    int id = getStore().addRecord( byteArray, 0, byteArray.length );
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.addRecord got recordID: " +
      id );

    record.setRecordId(id);

    if ( cacheEnabled && CACHE.isInitialized() )
    {
      CACHE.add( record );
    }
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.addRecord EXITTING" );

  } // addRecord

  /**
   * Returns the number of records in the record store.
   * Note that this returns the TOTAL number of records in the record store.
   * If you use a filter, the number of records returned could be substantially
   * less.
   * @return Size of the record store (-1 if not initialized).
   */
  public int getNumRecords ()
  {
    if ( cacheEnabled && CACHE.isInitialized() )
      return CACHE.getNumRecords();
    else
    {
      try
      {
        return getStore().getNumRecords();
      }
      catch( RecordStoreNotOpenException ex )
      { // this should never happen because our getStore method ensures the store
        // is open, but just in case ...
        Logger.ex( ex );
      }
    }
    return -1; // this should never be necessary 
  } // getNumRecords

  /**
   * Gets an enumeration of records from the RecordStore.
   * THIS METHOD DOES NOT USE DATA CACHING. It should only be used
   * when you want to utilize the keepUpdated flag (e.g. Always have
   * an updated RecordEnumeration ).
   * @param filter Used to filter records. May be null.
   * @param comparator Used to sort records. May be null.
   * @param keepUpdated When true, will keep the RecordEnumeration that is
   * passed back an accurate reflection of what is in the data store.
   * @returns A RecordEnumeration containing the byte[] records that match
   * your search criteria.
   * @throws RecordStoreNotOpenException
   */
  public RecordEnumeration enumerateRecords
    ( RecordFilter filter, RecordComparator comparator, boolean keepUpdated )
    throws RecordStoreNotOpenException
  {
    return getStore().enumerateRecords( filter, comparator, keepUpdated );
  }

  /**
   * Gets all of the records in the record store.
   * Returns null if no records are found in the store.
   * @return An array of records sorted by the comparator, or null if no records found.
   * @throws RecordStoreException If there is a problem with the store.
   * @throws IOException If any other error occurs.
   */
  public OAbstractRecord [] getAll()
    throws RecordStoreException, IOException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.getAll CALLED" );
    return getAll( null, null );
  } // getAll

  /**
   * Gets all existing records, sorting by the RecordComparator and filtering
   * by the RecordFilter.
   * Either the RecordComparator or the RecordFilter may be null. Having
   * both of them null would serve the same purpose as the <code>getAll()</code>
   * method.
   * Returns null if no records are found in the store.
   *
   * @param comparator Used to sort the records returned. May be null.
   * @param filter Used to filter the records returned. May be null.
   * @return array of records, or null if no records exist
   * @exception RecordStoreException if an error occurs
   * @exception IOException if an error occurs
   */
  public  OAbstractRecord[] getAll ( RecordComparator comparator, RecordFilter filter )
    throws RecordStoreException, IOException
  {
    if ( cacheEnabled && CACHE.isInitialized() )
      return CACHE.getAll( comparator, filter );
    else
    {
      if ( Debugger.ON ) Logger.dev( "OAbstractStore.getAll ENTERED" );
      OAbstractRecord [] records = null;
      int nbrOfRecords =  getNumRecords();

      if ( Debugger.ON ) Logger.dev( "OAbstractStore.getAll found " + nbrOfRecords +
                " records in persistent store" );

      if ( nbrOfRecords > 0 )
      {
        // get an enumeration of all of the records
        RecordEnumeration enum =
            getStore().enumerateRecords( filter, comparator, false );

        //
        // Let's get the number of records based on the enumeration.
        //
        nbrOfRecords = enum.numRecords();

        // First go through the enumeration and get all of the record ids
        int [] recordIds = new int[ nbrOfRecords   ];
        if ( Debugger.ON ) Logger.dev( "OAbstractStore.getAll(f,c) nbrOfRecords=" + nbrOfRecords);
        int idx = 0;

        while (enum.hasNextElement())
        {
            recordIds[ idx ] = enum.nextRecordId();
                if ( Debugger.ON ) Logger.dev( "      recordId[" + idx + "]=" + recordIds[idx]);
                idx++;
        }

        // now get each record from the enumeration, save its record id, and
        // place it in the return array and in the cache
        enum.reset();

        // first create the array of records to return
        records = new OAbstractRecord[ nbrOfRecords  ];
        idx = 0;
        while (enum.hasNextElement())
        {
            OAbstractRecord thisRecord = createRecord();
            byte []  rec = enum.nextRecord();
            thisRecord.readObject( rec );
            // save the record id in the record, and save the record
          if ( Debugger.ON ) Logger.dev( "      recordIds[" + idx + "]=" + recordIds[idx]);
            thisRecord.setRecordId( recordIds[ idx ] );
            records[ idx ] = thisRecord;
          if ( Debugger.ON ) Logger.dev( "      records[" + idx + "]=" + records[idx] + ", recordId=" + thisRecord.getRecordId());
          if ( Debugger.ON ) Logger.dev( "      records[" + idx + "].hashCode=" + thisRecord.hashCode());
          idx++;
        }

        if ( cacheEnabled && filter == null )
        {
          // Load the cache with the records, then set the initialized flag...
          CACHE.load( records );
          CACHE.setInitialized( true );
        }
      }
      if ( Debugger.ON ) Logger.dev( "OAbstractStore.getAll EXITTING, " +
                nbrOfRecords  + " records" +
                ", records=" + records );
      return records;
    } // End else loop
  } // getAll

  /**
   * Closes the persistent store. 
   * If caching is enabled, it deletes all of the records in the cache
   * and uninitializes it. This is called on <code>deleteAll()</code> 
   * @exception RecordStoreException If a problem with the data store occurs.
   */
  public void close ()
    throws RecordStoreException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.close ENTERED" );
    if ( myStore != null )
    {
      // Close until the RecordStoreNotOpenException is caught. The
      // store should only be opened once, but just to make sure.
      try
      {
        while ( true )
        {
            getStore().closeRecordStore();
          storeOpened--;
        }
      }
      catch ( RecordStoreNotOpenException rse )
      {
        myStore = null;
        if ( cacheEnabled )
        {
          CACHE.deleteAll();
          CACHE.setInitialized( false );
        }
      }
    }
  } // close

  /**
   * Deletes a record in the persistent store. 
   * If caching is enabled, it also removes it from the data cache.
   * @param record The record to delete
   * @throws InvalidRecordIDException If the record is not found in the store.
   * @throws RecordStoreException If any other problems with the store occur.
   */
  public synchronized void deleteRecord ( OAbstractRecord record )
    throws InvalidRecordIDException, RecordStoreException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.deleteRecord ENTERED for rec id "
                + record.getRecordId() );

    int id = record.getRecordId();
    getStore().deleteRecord( id );

    if ( cacheEnabled && CACHE.isInitialized() )
    {
      CACHE.delete( record );
    }
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.deleteRecord EXITTING" );

  } // deleteRecord

  /**
   * Deletes a record in the persistent store. 
   * If caching is enabled, it also removes it from the data cache.
   * @param recordId Identifies the record  to delete
   * @throws InvalidRecordIDException If the record is not found in the store.
   * @throws RecordStoreException If any other problems with the store occur.
   */
  public synchronized void deleteRecord (int id)
    throws InvalidRecordIDException, RecordStoreException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.deleteRecord ENTERED for rec id "
                + id );

    getStore().deleteRecord( id );

    if ( cacheEnabled && CACHE.isInitialized() )
    {
      try {
         CACHE.delete( CACHE.get(id));
      }
      catch (Exception e){}
    }
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.deleteRecord EXITTING" );

  } // deleteRecord

    /**
     * Retrieves a record.
     *   If caching is enabled, the record is retrieved from the cache. Otherwise,
     * the record is retrieved from the record store.
     *   If the record is not found, an InvalidRecordIDException is thrown.
     * @param recordId The id of the record to retrieve.
     * @return an <code>OAbstractRecord</code> matching the record id.
     * @throws InvalidRecordIDException If the record id is not found.
     * @throws RecordStoreException If any other problem with the data store occurs.
     * @throws IOException If any other error occurs.
     */
  public OAbstractRecord getRecord ( int recordId)
    throws InvalidRecordIDException, RecordStoreException, IOException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.getRecord ENTERED w/record id " +
                recordId );
    if ( cacheEnabled && CACHE.isInitialized() )
    {
      if ( Debugger.ON ) Logger.dev( "OAbstractStore.getRecord EXITTING" );
      return CACHE.get( recordId );
    }
    else
    {
      OAbstractRecord record = createRecord();
      record.readObject( getStore().getRecord( recordId ) );
      record.setRecordId( recordId );

      if ( Debugger.ON ) Logger.dev( "OAbstractStore.getRecord EXITTING" );
      return record;
    }
  } // getRecord

    /**
     * Updates a record in the persistent store.
     * If caching is enabled, the record is also updated in the data cache.
     * @param record Record containing updated value(s)
     * @throws InvalidRecordIDException If the record is not found in the data store.
     * @throws RecordStoreException If a problem occurs in the data store.
     * @exception IOException if an error occurs
     */
  public synchronized void updateRecord ( OAbstractRecord record )
    throws InvalidRecordIDException, RecordStoreException, IOException
  {
    if ( Debugger.ON ) Logger.dev( "OAbstractStore.updateRecord ENTERED w/record id "
                + record.getRecordId() );
    if ( cacheEnabled && CACHE.isInitialized() )
    {
      CACHE.add( record );
    }
    byte [] serializedRecord = record.writeObject();
    getStore().setRecord( record.getRecordId(), serializedRecord,
         0, serializedRecord.length );

    if ( Debugger.ON ) Logger.dev( "OAbstractStore.updateRecord EXITTING" );
  } // updateRecord

  /**
   * Deletes all records in the store.
   * If caching is enabled, the cache is emptied and is uninitialized.
   * @throws RecordStoreException if a record store-related exception occurred.
   * @exception RunTimeException if an unexpected error (i.e OutOfMemoryException)
   * occurs.
   */
  public synchronized void deleteAll()
    throws RecordStoreException
  {
    try
    {
      close();
      RecordStore.deleteRecordStore( storeName );
    }
    catch (RecordStoreNotFoundException e)
    {
     // Data is already deleted, don't bother throwing exception
    }
    catch ( RecordStoreException rse )
    {
      // Since this is what we say we are throwing, just throw this
      throw rse;
    }
    catch( Throwable ex )
    {
      String message = new String();
      if ( ex instanceof OutOfMemoryError )
      {
          message = "Memory is full : ";
        // Since memory is filled up, call the unload() method on the cache,
        // uninitialize the cache and suggest to the VM that it garbage collect.
        CACHE.unload();
        CACHE.setInitialized( false );
        System.gc();
      }
      message += "Unable to delete " + storeName;
      throw new RuntimeException( message );
    }
  } // deleteAll


}// OAbstractStore

