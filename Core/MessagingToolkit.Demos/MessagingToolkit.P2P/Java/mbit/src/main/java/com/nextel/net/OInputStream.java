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

import java.io.InputStream;
import java.io.IOException;
import java.io.DataInput;
import java.io.DataInputStream;
import javax.microedition.io.Connection;

/**
 * This class serves as an <code>InputStream</code> that can be monitored for
 * timeout conditions.  Essentially, this class wraps a <code>DataInputStream
 * </code> instance and binds to a <code>Timeable</code> object.  Due to the
 * fact that latency can occur while reading from an <code>InputStream</code>,
 * operations on this class are preceded and followed by calls to the <code>
 * Timeable</code> object to start and stop its associated <code>ReqeustTimer
 * </code>.  This allows easy detection of timeout conditions to occur on the
 * underlying <code>InputStream</code>.
 * <p>
 * In the event of a Timeout occurrence the <code>RequestTimer</code> initiates
 * a call to the timeout method of this class.  A <code>Timeable</code> object
 * is responsible for handling actions necessary to incur a timeout through the
 * timeout method.  In this class, a timeout is essentially incurred through
 * closing the underlying <code>DataInputStream</code> and <code>Connection
 * </code>.  Subsequent calls on this <code>OInputStream</code> may result in
 * an <code>IOException</code>.  To differentiate an <code>IOException
 * </code> due to a timeout condition, an internal flag is set in the timeout
 * method.  In <code>OInputStream</code> methods that throw <code>IOException,
 * </code> detection of the set timeout flag results in throwing a <code>
 * TimeoutException</code>.  Because <code>TimeoutException</code> is a
 * subclass of <code>IOException</code> it will be necessary for code that
 * catches these exceptions to make the distinction between <code>
 * TimeoutException</code> and other instances of <code>IOException</code>
 * if necessary.
 *
 * @see com.nextel.net.Timeable
 * @see com.nextel.net.TimeoutException
 * @see java.io.DataInputStream
 * @see java.io.IOException
 */
public class OInputStream extends InputStream implements Timeable {
  private Connection conn;
  private DataInputStream dataInputStream;
  private RequestTimer timer;
  private int timeout;
  private boolean hasTimedOut;


  private OInputStream() {
  }

  /**
   * Constructs an instance where this class serves as a wrapper for the provided
   * <code>InputStream</code> and the provided <code>Timeable</code> is the
   * object that will use this <code>OInputStream</code> for accessing the
   * available data.
   *
   * @param is a <code>java.io.InputStream</code> that is the underlying
   * object for this wrapper class.
   *
   * @param request a <code>com.nextel.net.Timeable</code> object that will
   * use this class for <code>DataInputStream</code> operations.
   */
  public OInputStream(Connection conn, InputStream is, int timeout) {
    this.conn = conn;
    dataInputStream = new DataInputStream(is);
    hasTimedOut = false;
    this.timeout = timeout;
    System.out.println("OInputStream: timeout is " + timeout);
    if (timeout > 0) {
      timer = new RequestTimer(this, timeout);
      timer.start();
    }
  }

  /**
   * Wraps the <code>DataInputStream#available()</code> method with calls to
   * the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#available()
   */
  public int available() throws IOException {
    startTimer();
    try {
       return dataInputStream.available();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#close()</code> method with calls to
   * the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#close()
   *
   * @throws java.io.IOException if DataInputStream#close() encounters a problem
   */
  public void close() throws IOException {
    if (hasTimedOut) {
      dataInputStream.close();
    }
    else {
      startTimer();
      dataInputStream.close();
      stopTimer();
    }
    cancelTimer();
  }

  /**
   * Wraps the <code>DataInputStream#reset()</code> method with calls to
   * the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#reset()
   *
   * @throws java.io.IOException if DataInputStream#reset() encounters a problem
   */
  public void reset() throws IOException {
    startTimer();
    try {
      dataInputStream.reset();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#markSupported()</code> method with calls to
   * the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @return a <code>boolean</code> indicating if the underlying implementation
   * supports position marking.
   *
   * @see java.io.DataInputStream#markSupported()
   */
  public boolean markSupported() {
    startTimer();
    boolean b = dataInputStream.markSupported();
    stopTimer();
    return b;
  }

  /**
   * Wraps the <code>DataInputStream#mark()</code> method with calls to
   * the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#mark
   *
   */
  public void mark(int readLimit) {
    startTimer();
    dataInputStream.mark(readLimit);
    stopTimer();
  }

  /**
   * Wraps the <code>DataInputStream#read()</code> method with calls to
   * the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#read()
   *
   * @throws java.io.IOException if DataInputStream#read() encounters a problem
   */
  public int read() throws IOException {
    startTimer();
    try {
      return dataInputStream.read();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#read(byte [])</code> method with calls to
   * the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#read(byte [])
   *
   * @throws java.io.IOException if DataInputStream#read(byte []) encounters a
   * problem
   */
  public int read(byte[] buffer) throws IOException {
    startTimer();
    try {
      return dataInputStream.read(buffer);
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#read(byte [], int, int)</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#read(byte [], int, int)
   *
   * @throws java.io.IOException if DataInputStream#read(byte [], int, int)
   * encounters a problem
   */
  public int read(byte [] buffer, int offset, int length) throws IOException {
    startTimer();
    try {
      return dataInputStream.read(buffer, offset, length);
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readFully(byte [])</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readFully(byte [])
   *
   * @throws java.io.IOException if DataInputStream#readFully(byte [])
   * encounters a problem
   */
  public void readFully(byte [] buffer) throws IOException {
    startTimer();
    try {
      dataInputStream.readFully(buffer);
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readFully(byte [], int, int)</code> method
   * with calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readFully(byte [], int, int)
   *
   * @throws java.io.IOException if DataInputStream#readFully(byte [], int, int)
   * encounters a problem
   */
  public void readFully(byte [] buffer, int offset, int length) throws IOException {
    startTimer();
    try {
      dataInputStream.readFully(buffer, offset, length);
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#skipBytes(int)</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#skipBytes(int)
   *
   * @throws java.io.IOException if DataInputStream#skipBytes(int)
   * encounters a problem
   */
  public int skipBytes(int skipVal) throws IOException {
    startTimer();
    try {
      return dataInputStream.skipBytes(skipVal);
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readBoolean()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readBoolean()
   *
   * @throws java.io.IOException if DataInputStream#readBoolean()
   * encounters a problem
   */
  public boolean readBoolean() throws IOException {
    startTimer();
    try {
      return dataInputStream.readBoolean();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readByte()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readByte()
   *
   * @throws java.io.IOException if DataInputStream#readByte()
   * encounters a problem
   */
  public byte readByte() throws IOException {
    startTimer();
    try {
      return dataInputStream.readByte();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readUnsignedByte()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readUnsignedByte()
   *
   * @throws java.io.IOException if DataInputStream#readUnsignedByte()
   * encounters a problem
   */
  public int readUnsignedByte() throws IOException {
    startTimer();
    try {
      return dataInputStream.readUnsignedByte();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readShort()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readShort()
   *
   * @throws java.io.IOException if DataInputStream#readShort()
   * encounters a problem
   */
  public short readShort() throws IOException {
    startTimer();
    try {
      return dataInputStream.readShort();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readUnsignedShort()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readUnsignedShort()
   *
   * @throws java.io.IOException if DataInputStream#readUnsignedShort()
   * encounters a problem
   */
  public int readUnsignedShort() throws IOException {
    startTimer();
    try {
      return dataInputStream.readUnsignedShort();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }


  /**
   * Wraps the <code>DataInputStream#readChar()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readChar()
   *
   * @throws java.io.IOException if DataInputStream#readChar()
   * encounters a problem
   */
  public char readChar() throws IOException {
    startTimer();
    try {
      return dataInputStream.readChar();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readInt()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readInt()
   *
   * @throws java.io.IOException if DataInputStream#readInt()
   * encounters a problem
   */
  public int readInt() throws IOException {
    startTimer();
    try {
      return dataInputStream.readInt();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readLong()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readLong()
   *
   * @throws java.io.IOException if DataInputStream#readLong()
   * encounters a problem
   */
  public long readLong() throws IOException {
    startTimer();
    try {
      return dataInputStream.readLong();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readUTF()</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readUTF()
   *
   * @throws java.io.IOException if DataInputStream#readUTF()
   * encounters a problem
   */
  public String readUTF() throws IOException {
    startTimer();
    try {
      return dataInputStream.readUTF();
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#readUTF(DataInput)</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#readUTF(DataInput)
   *
   * @throws java.io.IOException if DataInputStream#readUTF(DataInput)
   * encounters a problem
   */
  public String readUTF(DataInput in) throws IOException {
    startTimer();
    try {
      return dataInputStream.readUTF(in);
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Wraps the <code>DataInputStream#skip(long)</code> method with
   * calls to the <code>Timeable</code> startTimer() and stopTimer().
   *
   * @see java.io.DataInputStream#skip(long)
   *
   * @throws java.io.IOException if DataInputStream#skip(long)
   * encounters a problem
   */
  public long skip(long skipVal) throws IOException {
    startTimer();
    try {
      return dataInputStream.skip(skipVal);
    }
    catch (IOException io) {
      if (hasTimedOut) {
        throw new TimeoutException(timeout + " sec. timeout has been reached");
      }
      else {
        throw io;
      }
    }
    finally {
      stopTimer();
    }
  }

  /**
   * Provides an interrupt to the initiated request.  This method is typically
   * invoked from the <code>RequestTimer</code> delegate.
   *
   * @see com.nextel.net.RequestTimer
   */
  public void timeout() {
    hasTimedOut = true;
    try {
      close();
      if (conn != null) {
        conn.close();
      }
    }
    catch (IOException i) {
      i.printStackTrace();
    }
  }

  /**
   * Issues a startTimer() call to the <code>RequestTimer</code>.  This method
   * is typically invoked from the <code>OInputStream</code> instance
   * associated with this request.
   *
   * @see com.nextel.net.RequestTimer
   */
  public void startTimer() {
    if (timer != null) {
      timer.startTimer();
    }
  }

  /**
   * Issues a stopTimer() call to the <code>RequestTimer</code>.  This method
   * is typically invoked from the <code>OInputStream</code> instance
   * associated with this request.
   *
   * @see com.nextel.net.RequestTimer
   */
  public void stopTimer() {
    if (timer != null) {
      timer.stopTimer();
    }
  }


  /**
   * Issues a cancelTimer() call to the <code>RequestTimer</code>.  This method
   * is typically invoked from the <code>OInputStream</code> instance
   * associated with this request.  This method is called after the request
   * has completed all operations that could result in a timeout.  Calling this
   * method results in cleaning up resources used by the <code>RequestTimer
   * </code>
   *
   * @see com.nextel.net.RequestTimer
   */
  public void cancelTimer() {
    if (timer != null) {
      timer.cancelTimer();
    }
  }

}
