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

import com.nextel.io.OExtern;
import java.io.IOException;
/**
 * This interface defines any record that can be synchronized 
 * with (i.e., loaded from or to) a data source external to the handset.
 * <code>OSynchRecord</code>s should be stored in a
 * {@link com.nextel.rms.OSynchStore}.
 * @author Glen Cordrey
 */
abstract public class OSynchRecord extends OAbstractRecord
					    implements OExtern
{
  /**
   * Creates an <code>OSynchRecord</code> instance.
   */
  public OSynchRecord()
  {
    super();
  }
  
  /**
   * Creates an <code>OSynchRecord</code> instance.
   * @param array The byte array to populate the record.
   */
  public OSynchRecord( byte[] array )
    throws IOException
  {
    super( array );
  }
  
  /**
   * Writes the record in a form that can be transmitted external to the
   * handset. This may simply be the byte array that you get when you retrieve
   * the record from the store, but you could also add other formatting, such
   * as XML, to the record when expressing it for external use.
   *
   * @return a <code>byte[]</code> value
   */
  abstract public byte [] writeExternal();
  
  /**
   * Reads the record from an external form. As with {@link #writeExternal},
   * this could simply be the byte array that the record assumes when stored in
   * it's {@link com.nextel.rms.OSynchStore}, but could also contain additional
   * formatting, such as XML, that the sender and receiver of the record agree
   * upon. 
   *
   * @param in The record in externalized form.
   */
  abstract public void readExternal( byte [] in );
} // OSynchRecord

