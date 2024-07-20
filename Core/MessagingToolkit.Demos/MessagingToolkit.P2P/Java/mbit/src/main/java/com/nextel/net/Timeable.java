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

/**
 * Interface for objects that require monitor of timeout conditions by a
 * <code>RequestTimer</code> object.  A <code>Timeable</code> instance must
 * implement the following methods to control its <code>RequestTimer</code>
 * delegate.  The <code>OInputStream</code> associated with a request for
 * reading resulting data will call these methods toggling timing services
 * on and off as required.
 *
 * @see com.nextel.net.HttpRequest
 * @see com.nextel.net.RequestTimer
 */
public interface Timeable {


/**
 * Notifies the request being timed that a timeout condition has occured.  This
 * method should contain the code required by this type of request to break
 * out of its latent state.
 */
 public void timeout();

 /**
  * Activates the associated <code>RequestTimer</code> to begin timing to
  * monitor for timeout conditions.
  */
 public void startTimer();

 /**
  * Deactivates the associated <code>RequestTimer</code> from monitoring
  * for timeout conditions.  The <code>RequestTimer</code> is still alive
  * but not currently monitoring.
  */
 public void stopTimer();

 /**
  * Deactivates the associated <code>RequestTimer</code> from monitoring
  * and sets internal flags for the <code>Thread</code> to exit.
  */
 public void cancelTimer();
}