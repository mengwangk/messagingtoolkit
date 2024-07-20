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

package com.nextel.net;

import javax.microedition.io.Connection;
import java.io.IOException;
import java.io.InputStream;

/**
 * An interface for objects that require status notification of <code>
 * Request</code> objects.  The purpose of the <code>RequestListener</code> is
 * to implement the actions necessary for a successful (<code>
 * RequestListener#completed</code>) or unsuccessful (<code>
 * RequestListener#exception</code>) notification from a specific <code>
 * Request</code>.  The <code>RequestListener</code> is bound to the <code>
 * Request</code> through calling the request's setRequestListener() method.
 * It would be typical for the implementing <code>MIDlet</code> class to also
 * implement the <code>RequestListener</code> however any delegated class
 * could be sufficient.  Since many of the <code>Request</code> classes
 * implement this interface it is common for RequestListeners to chain the
 * completed and exception methods.  An example of this can be seen in the
 * code for the <code>HttpAsyncRequest</code> and the <code>HttpRequest</code>
 * delegate that is uses.  In this scenario, the underlying <code>HttpRequest
 * </code> calls the completed or exception methods on <code>HttpAsyncRequest
 * </code> which chains the call to the <code>RequestListener</code> that it
 * is bound to.
 */
public interface RequestListener {

/**
 * This method should contain code specific to actions that should be taken
 * when a request has completed.  The request object associated with a
 * <code>RequestListener</code> will call this method once the request has
 * reached a satisfactory level of completion for that specific request type.
 *
 * @param r the <code>Request</code> object that has successfully completed
 * its request transmission.  This is usually the <code>Request</code> object
 * bound to this <code>RequestListener</code>.
 */
  public void completed(Request r);

  /**
   * This method should contain code specific to actions that should be taken
   * when a <code>Request</code> object is indicating that the request cannot
   * be carried out due to thrown exceptions.  The <code>Request</code>
   * associated with this <code>RequestListener</code> will call this method
   * when such a condition has occurred.
   *
   * @param e the <code>Exception</code> condition that has occurred within
   * the <code>Request</code> object that is preventing the request from
   * completing.
   *
   * @param r the <code>Request</code> associated with this exception.
   */
  public void exception(Exception e, Request r);
}
