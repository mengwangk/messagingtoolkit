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

package com.nextel.io; // Generated package name

/**
 * Interface for any class that can express itself in an external format.
 * <p>
 * For example, if class <code>MyRecord</code> extends
 * {@link com.nextel.rms.OAbstractRecord}, and
 * <code>MyRecord</code> objects are to be sent to a server, some mechanism is
 * needed to express <code>MyRecord</code> objects for transmission. If
 * <code>MyRecord</code> implements this interface AND <code>MyRecord</code>s
 * are stored in a class that extends {@link com.nextel.rms.OSynchStore}, then
 * {@link com.nextel.rms.OSynchStore#upload} and
 * {@link com.nextel.rms.OSynchStore#download} can be used to import and export
 * the data store.
 */
public interface OExtern 
{
  /**
   * Writes the object in a form that can be transmitted external to the
   * handset. 
   * @return a <code>byte[]</code> value
   */
  public byte [] writeExternal();

    /**
   * Reads an instance of this class that is represented in an external form
   * (the same form used by {@link #writeExternal}.
   *
   * @param in The object in externalized form.
   */
public void readExternal( byte [] in );
} // OExtern
