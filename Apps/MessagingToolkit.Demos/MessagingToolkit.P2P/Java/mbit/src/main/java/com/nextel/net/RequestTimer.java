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
 * A class that serves as a timer for <code>Timeable</code> objects.  A class
 * that requires timing services is attached to this class through the
 * <code>RequestTimer</code> constructor along with a timeout value in
 * seconds.  A <code>Timeable</code> object contains a timeout() method which
 * the <code>RequestTimer</code> calls if a timeout condition occurs.
 *
 * This class is used to detect latency conditions that may occur in <code>
 * HttpRequest</code> instances.
 *
 * Typically, a <code>Timeable</code> object will wrap I/O related calls with
 * calls to startTimer() and stopTimer().  It is important to note that a
 * <code>RequestTimer</code> should be initialized with timeout values under
 * those implemented by the KVM provider to be effective.  At the time of
 * development these timeout values implemented on the Motorola i85 were set
 * to 20 seconds for opening stream connections and 40 seconds for stream
 * read operations.
 *
 * @see com.nextel.net.Timeable
 * @see com.nextel.net.HttpRequest
 */
public class RequestTimer extends Thread {

  private long timeout;
  private Timeable timeable;
  private long startTime;
  private boolean active = false;
  private boolean timing = false;

  /**
   * Creates a <code>RequestTimer</code> for the provided <code>Timeable
   * </code> object with a timeout provided in seconds.
   *
   * @param t a <code>Timeable</code> instance for this <code>RequestTimer</code>
   * to monitor.
   *
   * @param timeout an <code>int</code> setting the timeout value in seconds
   */
  public RequestTimer(Timeable t, int timeout) {
    this.timeout = timeout * 1000;
    timeable = t;
  }

  /**
   * The <code>Thread#run()</code> method for this <code>RequestTimer</code>.
   * The <code>RequestTimer#start()</code> method must be called to initiate
   * this object as a <code>Thread</code> prior to calling the startTimer()
   * method.
   */
  public void run() {
    active = true;
    startTime = System.currentTimeMillis();

    while (active) {
      if (timing) {
        if ((System.currentTimeMillis() - getStartTime(0L)) > timeout) {
          timeable.timeout();
          timing = false;
        }
        else {
          try {
            sleep(1000L);
          }
          catch (InterruptedException ie) {
            active = false;
          }
        }
      }
      else {
        try {
          synchronized (this) {
           wait();
          }
        }
        catch (Exception e) {
          active = false;
        }
      }
    }
  }

  /**
   * A synchronized method for the <code>RequestTimer</code> and classes
   * that call this class to coordinate access to the value that is referenced
   * as the <code>RequestTimer</code> start time.
   *
   * @param t a <code>long</code> referenced to determine if the start time
   * attribute is to be accessed with a new value or the value last set.  Calling
   * this method with t=0 will return the current start time value.  Calling
   * this method with t=n where n > 0 will access the start time value set to n.
   *
   * @return a <code>long</code> representing the value of the start time
   * attribute.
   */
  public synchronized long getStartTime(long t) {
    if (t != 0L) {
      startTime = t;
    }
    return startTime;
  }

  /**
   * This method activates the <code>RequestTimer</code> to monitor for a
   * timeout condition.  The start time attribute is set to the current time.
   */
  public void startTimer() {
    if (!timing) {
      timing = true;
      getStartTime(System.currentTimeMillis());
      synchronized (this) {
        notifyAll();
      }
    }
  }

  /**
   * Deactivates the <code>RequestTimer</code> from monitoring for timeout
   * conditions.
   */
  public void stopTimer() {
    timing = false;
  }

  /**
   * This method should be called when the <code>RequestTimer</code> is no longer
   * needed.  Calling this method will set an internal flag which will result
   * the run() method to exit.
   */
  public void cancelTimer() {
    active = false;
    synchronized (this) {
      notifyAll();
    }
  }
}
