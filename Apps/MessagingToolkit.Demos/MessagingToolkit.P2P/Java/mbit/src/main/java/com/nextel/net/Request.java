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
import javax.microedition.io.DatagramConnection;
import javax.microedition.io.HttpConnection;
import java.io.InputStream;

/**
 * An abstract class for common request related classes.
 */
public abstract class Request {

  private String url;
  private int retries;
  private int port;
  private InputStream inputStream;
  private Connection conn;
  private byte [] byteData;

  /**
   * Default no arg constructor.
   */
  public Request() {
  }

  /**
   * Constructor for requests based on the provided location and number of
   * retries
   *
   * @param url a <code>String</code> containing the location of the resource.
   * The location string should contain any necessary port designations
   * required for connecting to the resource.
   *
   * @param retries an <code>int</code> specifying the number of attempts
   * that should be made in successfully completing the request.  Problems
   * encountered in making the request will not be reported until the number of
   * retries has been exhausted.
   */
  public Request (String url, int retries) {
    this.url = url;
    this.retries = retries;
  }

  /**
   * Creates a request based only upon the location of the resource.  Any
   * necessary port designations should be included with the location.  This
   * constructor will result in creating a request with 0 retries.
   *
   * @param url a <code>String</code> specifying the request location that will
   * be issued.
   */
  public Request (String url) {
    this (url, 0);
  }

  /**
   * Accessor method for the URL attribute.
   */
  public String getURL() {
    return url;
  }

  /**
   * Accessor method for the retries attribute.
   */
  public int getRetries() {
    return retries;
  }

  /**
   * Mutator method for the URL attribute.
   */
  public void setURL(String url) {
    this.url = url;
  }

  /**
   * Mutator method for the retries attribute.
   */
  public void setRetries(int retries) {
    this.retries = retries;
  }

  /**
   * Accessor method for request data that is stored in a <code>byte</code>
   * array.  This data is typically the load of a datagram for UDP or query
   * string data for HTTP requests.
   */
  public byte[] getByteData() {
    return byteData;
  }

  /**
   * Mutator method for request data that is stored in a <code>byte</code>
   * array.  This data is typically the load of a datagram for UDP or query
   * string data for HTTP requests.
   */
  public void setByteData(byte[] data) {
    byteData = data;
  }

  /**
   * Accessor method for the <code>InputStream</code> that is available as the
   * result of requests that provide reading data results.
   */
  public InputStream getInputStream() {
    return inputStream;
  }

  /**
   * Mutator method for <code>InputStream</code> instances related to requests.
   */
  public void setInputStream(InputStream is) {
    inputStream = is;
  }

  /**
   * Accessor method for <code>Connection</code> resources used in issuing
   * requests.
   */
  public Connection getConnection() {
    return conn;
  }

  /**
   * Accessor method for <code>DatagramConnection</code> resources used in
   * UDP type requests.
   */
  public DatagramConnection getDatagramConnection() {
    return ((DatagramConnection)getConnection());
  }

  /**
   * Accessor method for <code>HttpConnection</code> resources used in
   * HTTP type requests.
   */
  public HttpConnection getHttpConnection() {
    return (HttpConnection)getConnection();
  }

  /**
   * A mutator method for <code>Connection</code> resources used in issuing
   * requests.
   */

  public void setConnection(Connection conn) {
    this.conn = conn;
  }
}