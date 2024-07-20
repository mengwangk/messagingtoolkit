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

import com.nextel.util.Logger;
import javax.microedition.rms.RecordFilter;

/**
 * Record filter used to find the URIRecord that matches a specified store name.
 * @author Ryan Wollmuth
 */
public class OURIRecordFilter implements RecordFilter
{
  private String storeName;
  
  /**
   * Creates an <code>OURIRecordFilter</code> instance.
   * @param storeName The search term for the filter.
   */
  public OURIRecordFilter( String storeName )
  {
    this.storeName = storeName;
  }
  
  /**
   * Returns true if the storeName passed in at the constructor matches
   * the storeName in this record. Otherwise, returns false.
   * @param values The byte array that represents the record.
   * @return A boolean indicating whether the data matched the store name or not.
   */
  public boolean matches(byte[] values)
  {
    OURIRecord record = null;
    try
    {
      record = new OURIRecord( values );

    } 
    catch ( Exception e ) 
    {
      Logger.ex( e );
      return false;
    }
    if ( record.storeName.equals( storeName ) )
      return true;
    else
      return false;
  }
}
