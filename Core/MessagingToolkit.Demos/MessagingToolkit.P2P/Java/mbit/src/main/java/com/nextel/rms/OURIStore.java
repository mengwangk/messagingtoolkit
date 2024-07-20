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
package com.nextel.rms; // Generated package name
import com.nextel.io.OExtern;

/**
 * A store of {@link com.nextel.rms.OURIRecord}s.
 * <p>
 * This store contains records that define the source and destination URIs for
 * any {@link com.nextel.rms.OSynchStore}s.
 *
 * @author Glen Cordrey
 */
public class OURIStore extends OAbstractStore 
{
  private static OURIStore store;
  
  // Creates an OURIStore instance.
  private OURIStore()
  {
    super( "URIStore", false );
  } // constructor
  
  /**
   * Gets an instance of the OURIStore. 
   * @return An instance of a OURIStore.
   */
  public static OURIStore getInstance()
  {
    if ( store == null )
      store = new OURIStore();
    return store;
  }
  
  /**
   * Returns an empty OURIRecord.
   * @return An empty OURIRecord
   */
  protected OAbstractRecord createRecord() 
  { 
    return new OURIRecord();
  } // createRecord


} // OURIStore
