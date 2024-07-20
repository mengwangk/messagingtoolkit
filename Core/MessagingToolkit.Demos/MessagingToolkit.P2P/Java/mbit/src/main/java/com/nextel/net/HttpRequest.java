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

import com.nextel.util.Debugger;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.rms.RecordEnumeration;
import javax.microedition.rms.RecordStore;
import javax.microedition.rms.RecordStoreException;
import java.io.*;
import java.util.Enumeration;
import java.util.Hashtable;

/**
 * A class that represents HTTP requests.  Standard HTTP GET and POST
 * requests are supported with retry and timeout options.  This class implements
 * the <code>Timeable</code> interface for working with the <code>RequestTimer
 * </code> class to implement timeout conditions.  This class implements the
 * <code>RequestListener</code> interface to serve as the default listener
 * to be notified when the request is complete.
 * <p/>
 * It should be noted that the timeout option should only be used for timeout
 * values less than the defaults implemented in the KVM.  If the timeout value
 * exceeds the KVM default, the KVM timeout will be invoked prior to any
 * timeout that can be implemented in the provided code.
 */
public class HttpRequest extends Request implements Timeable, RequestListener {
    private OutputStream outputStream;
    private RequestTimer timer;
    private RequestListener requestListener = this;
    private int timeout;
    private int recordId;
    private int thisTry;


    /**
     * Creates an <code>HttpRequest</code> for the specified location.
     *
     * @param url a <code>String</code> representing the URL location for this
     *            request.  The URL string should contain any necessary port designations.
     */
    public HttpRequest(String url) {
        this(url, 0, 0);
    }

    /**
     * Creates an <code>HttpRequest</code> for the specified location and
     * timeout/retry options.
     *
     * @param url     a <code>String</code> representing the URL location for this
     *                request.  The URL string should contain any necessary port designations.
     * @param timeout an <code>int</code> representing the maximum number of
     *                seconds allowed for this request to complete before the request is
     *                terminated.  This value if specified should be lower than the default
     *                KVM timeout.
     * @param retries an <code>int</code> representing the maximum number of
     *                times the request should be attempted.
     */
    public HttpRequest(String url, int timeout, int retries) {
        super(url, retries);
        this.timeout = timeout;
    }

    /**
     * Cleans up resources used by this request.  Implementations using <code>
     * HttpRequest</code> should call this method when the request's
     * <code>Connection</code> and <code>InputStream</code> are no longer needed.
     */
    public void cleanup() {
        try {
            if (getInputStream() != null) {
                getInputStream().close();
            }
            if (outputStream != null) {
                outputStream.close();
            }
            if (getConnection() != null) {
                getConnection().close();
            }
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Mutator method for the record Id of this request when it is stored
     * in the RMS store.
     */
    public void setRecordId(int id) {
        recordId = id;
    }

    /**
     * Accessor method for the record Id of this request.
     */
    public int getRecordId() {
        return recordId;
    }

    /**
     * Accessor method for the timeout attribute.
     */
    public int getTimeout() {
        return timeout;
    }

    /**
     * Mutator method for the timeout attribute.
     */
    public void setTimeout(int val) {
        timeout = val;
    }

    /**
     * Implementation of sending an HTTP request using the GET method.  This
     * method should be used when the complete request is contained within the
     * URL location specified in the <code>HttpRequest</code> constructor.
     *
     * @return a <code>java.io.InputStream</code> which can be used for reading
     *         result data from the issued request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream get() throws IOException {
        return (get(null));
    }

    /**
     * Provides an interrupt to the initiated request.  This method is typically
     * invoked from the <code>RequestTimer</code> delegate.
     *
     * @see com.nextel.net.RequestTimer
     */
    public void timeout() {
        try {
            if (getInputStream() != null) {
                getInputStream().close();
            }

            if (getConnection() != null) {
                getConnection().close();
            }
            if (thisTry == getRetries() - 1) {
                TimeoutException t =
                        new TimeoutException(getTimeout() + " sec. timeout reached");
                getRequestListener().exception(t, this);
            }
        }
        catch (IOException i) {
            i.printStackTrace();
        }
    }


    /**
     * Implementation of sending an HTTP request using the GET method.  The
     * request is constructed by appending the contents of the supplied <code>
     * Hashtable</code> to the URL location specified in the constructor call.
     * <p/>
     * The <code>Hashtable</code> elements are separated from the URL location
     * by a '?' character.  Each name/value pair appears as an assignent
     * (i.e. name=value) with '&' separating each pair.
     *
     * @param parameters a <code>Hashtable</code> of elements that make up
     *                   the query string portion of a URL location.  It should be observed that
     *                   the contents of the parameters when appended to the URL location should
     *                   not exceed the 256 character limit imposed by the HTTP GET method.
     * @return a <code>java.io.InputStream</code> that may be used for reading
     *         result data from the issued request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream get(Hashtable parameters) throws IOException {
        if (getURL() == null) {
            getRequestListener().exception(new Exception("URL cannot be null"), this);
        } else {
            StringBuffer queryStr = new StringBuffer(getURL());

            if (parameters != null) {
                queryStr.append("?");
                queryStr.append(urlencode(parameters));
            }

            thisTry = 0;
            while (thisTry <= getRetries()) {
                try {
                    makeConnection(queryStr.toString());
                    timer = getRequestTimer();
                    startTimer();
                    setInputStream(makeInputStream());
                    stopTimer();
                    try {
                        getRequestListener().completed(this);
                    }
                    catch (Exception e) {
                    }
                    break;
                }
                catch (Exception e) {
                    if (thisTry == getRetries() - 1) {
                        getRequestListener().exception(e, this);
                    } else {
                        continue;
                    }
                }
                finally {
                    cancelTimer();
                    thisTry++;
                }
            }
        }
        return getInputStream();
    }


    private void makeConnection(String queryString) throws IOException {
        if (!Debugger.UNIT_TEST) {
            setConnection((HttpConnection) Connector.open(queryString, Connector.READ_WRITE, true));
        } else {
            setConnection(new HttpRequestDriver.MyHttpConnection());
        }
    }

    private InputStream makeInputStream() throws IOException {
        if (!Debugger.UNIT_TEST) {
            InputStream i = getHttpConnection().openInputStream();
            if (getTimeout() > 0) {
                return new OInputStream(getConnection(), i, getTimeout());
            } else {
                return i;
            }
        } else {
            String temp = "this is a bytestream of input data";
            return new ByteArrayInputStream(temp.getBytes());
        }
    }

    private OutputStream makeOutputStream() throws IOException {
        if (!Debugger.UNIT_TEST)
            return getHttpConnection().openOutputStream();
        else {
            return new ByteArrayOutputStream();
        }
    }

    /**
     * An implementation of the HTTP POST method.  This method should be used
     * when the complete HTTP request is contained in the URL location provided
     * in the <code>HttpRequest</code> constructor.
     *
     * @return a <code>java.io.InputStream</code> for reading result data from
     *         the issued request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream post() throws IOException {
        String nullStr = null;
        return post(nullStr);
    }

    /**
     * An implementation of the HTTP POST method.  The URL location provided in
     * the constructor is used to initiate the request.  Additional data contained
     * in the <code>String</code> parameter is then sent to the URL resource.
     *
     * @param data a <code>String</code> containing data that is sent through
     *             the HTTP request in addition to that which is contained in the URL location.
     * @return a <code>java.io.InputStream</code> which can be used for reading
     *         result data from the issued request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream post(String data) throws IOException {
        if (data != null) {
            return post(data.getBytes());
        } else {
            byte [] nullArray = null;
            return post(nullArray);
        }
    }

    /**
     * An implementation of the HTTP POST method.  This method should be used to
     * send additional request data in conjunction with the URL location provided
     * in the <code>HttpRequest</code> constructor.
     *
     * @param data a <code>byte</code> array containing request data.  A typical
     *             use of this <code>byte</code> array would be for it to contain a query
     *             string of name/value pairs.
     * @return a java.io.InputStream which can be used for reading result data
     *         from the issued request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream post(byte[] data) throws IOException {
        return post(null, data);
    }

    /**
     * An implementation of the HTTP POST method that supports the handling of
     * request properties.  The request properties are used to setup the
     * connection prior to sending data to the URL request location provided in
     * the <code>HttpAsyncRequest</code> constructor.
     *
     * @param parameters a <code>Hashtable</code> containing name/value pairs
     *                   for setting up HTTP request properties.  Examples of such properties are
     *                   <p/>
     *                   <ul>
     *                   <li>Content-Length</li>
     *                   <li>User-Agent</li>
     *                   <li>Accept-Language</li>
     *                   <li>Accept</li>
     *                   <li>Connection</li>
     *                   </ul>
     *                   <p/>
     * @return a java.io.InputStream which can be used for reading result data
     *         from the issues request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream post(Hashtable parameters) throws IOException {
        String nullStr = null;
        return post(parameters, nullStr);
    }

    /**
     * An implementation of the HTTP POST method that supports a <code>
     * Hashtable</code> that contains request parameters and additional request
     * data from the URL location provided in the <code>HttpRequest</code>
     * constructor.
     *
     * @param parameters a <code>Hashtable</code> containing request parameters
     *                   for configuring the HTTP request.
     * @param data       a <code>String</code> of additional data to be sent to the URL
     *                   resource.  A typical use of the <code>String</code> is for it to contain
     *                   name/value pairs that comprise an HTTP query string.
     * @return a java.io.InputStream which can be used for reading result data
     *         from the issued request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream post(Hashtable parameters, String data) throws IOException {
        if (data != null) {
            return post(parameters, data.getBytes());
        } else {
            byte [] nullArray = null;
            return post(parameters, nullArray);
        }
    }

    /**
     * An implementation of the HTTP POST method which supports a <code>
     * Hashtable</code> that contains request parameters and additional request
     * data from the URL location provided in the <code>HttpRequest</code>
     * constructor.
     *
     * @param parameters a <code>Hashtable</code> containing request parameters
     *                   for configuring the HTTP request.
     * @param data       a <code>byte</code> array for additional data to be sent as
     *                   part of the HTTP POST request.  A typical use of the <code>byte</code>
     *                   array is for it to contain name/value pairs that comprise an HTTP query
     *                   string.
     * @return a java.io.InputStream which can be used to read result data
     *         from the issued request.
     * @throws java.io.IOException if a problem occurs during the initiated
     *                             request.
     */
    public InputStream post(Hashtable parameters, byte[] data) throws IOException {
        outputStream = null;
        thisTry = 0;

        while (thisTry < getRetries()) {
            try {
                makeConnection(getURL());
                sendPostParameters(parameters);

                if (data != null) {
                    sendPostByteStream(data);
                }
                timer = getRequestTimer();
                startTimer();
                setInputStream(makeInputStream());
                stopTimer();
                try {
                    getRequestListener().completed(this);
                }
                catch (Exception e) {
                }
                break;
            }
            catch (IOException ioe) {
                if (thisTry == getRetries() - 1) {
                    getRequestListener().exception(ioe, this);
                } else {
                    continue;
                }
            }
            finally {
                thisTry++;
                cancelTimer();
                try {
                    if (outputStream != null) outputStream.close();
                }
                catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }
        return getInputStream();
    }

    /**
     * Provides a mechanism for sending the contents of an RMS data store to a
     * URL resource.  The resource that receives the request should be programmed
     * to handle the data format of the RMS store.  The URL string provided in
     * the <code>HttpRequest</code> constructor should contain the complete
     * resource to receive the contents of the RMS data store.
     * <p/>
     * The data store is sent to the resource iteratively by record.  Each request
     * is comprised of the URL to create the <code>HTTPConnection</code> and the
     * record byte[] which is sent over the connection's <code>OutputStream</code>
     * using the HTTP POST method.  Request parameters are used to indicate the
     * current record being sent using the parameter "RECORD_NUM" and the total
     * number of records the resource should expect using the parameter
     * "TOTAL_RECORDS".
     *
     * @param rmsName a <code>String</code> that contains the name of the RMS
     *                store which contains the records that are to be sent in the request.
     * @throws javax.microedition.rms.RecordStoreException
     *                             if a problem occurs
     *                             retrieving the data from the record store.
     * @throws java.io.IOException if a problem occurs while initiating the
     *                             HTTP request.
     */
    public void put(String rmsName) throws RecordStoreException, IOException {
        RecordStore rs = null;
        RecordEnumeration re = null;

        if (rmsName == null) {
            getRequestListener().exception(
                    new RecordStoreException(
                            "record store name cannot be null"),
                    this
            );
        } else {
            thisTry = 0;
            while (thisTry < getRetries()) {
                int count = 0;
                try {
                    rs = RecordStore.openRecordStore(rmsName, false);
                    if (rs != null) {
                        re = rs.enumerateRecords(null, null, true);
                        int size = rs.getNumRecords();

                        Hashtable h = new Hashtable();
                        h.put("TOTAL_RECORDS", Integer.toString(size));

                        while (re.hasNextElement()) {
                            System.out.println("HttpRequest: sending record " + (count + 1));
                            h.put("RECORD_NUM", Integer.toString(++count));
                            makeConnection(getURL());
                            sendPostParameters(h);
                            sendPostByteStream(re.nextRecord());
                            cleanup();
                            System.out.println("HttpRequest: done cleaning up record " + count);
                        }
                        try {
                            getRequestListener().completed(this);
                        }
                        catch (Exception e) {
                        }
                        System.out.println("HttpRequest: put() completed");
                        break;
                    }
                }
                catch (Exception e) {
                    System.out.println("HttpRequest: put() exception " + e.getMessage() + " on record " + count);
                    if (thisTry == getRetries() - 1) {
                        getRequestListener().exception(e, this);
                    } else {
                        continue;
                    }
                }
                finally {
                    thisTry++;
                    if (getConnection() != null) getConnection().close();
                    if (rs != null) rs.closeRecordStore();
                }
            }
        }
    }

    /**
     * Issues a startTimer() call to the <code>RequestTimer</code>.  This method
     * is typically invoked from the <code>OInputStream</code> instance
     * associated with this request.
     *
     * @see com.nextel.net.RequestTimer
     * @see com.nextel.net.OInputStream
     */
    public void startTimer() {
        if (getTimeout() > 0) {
            timer = getRequestTimer();
            timer.startTimer();
        }
    }

    /**
     * Issues a stopTimer() call to the <code>RequestTimer</code>.  This method
     * is typically invoked from the <code>OInputStream</code> instance
     * associated with this request.
     *
     * @see com.nextel.net.RequestTimer
     * @see com.nextel.net.OInputStream
     */
    public void stopTimer() {
        if (getTimeout() > 0) {
            timer = getRequestTimer();
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
     * @see com.nextel.net.OInputStream
     */
    public void cancelTimer() {
        if (getTimeout() > 0) {
            if (timer != null) {
                timer.cancelTimer();
            }
        }
    }

    /**
     * Mutator method for the <code>RequestListener</code> associated with this
     * <code>HttpRequest</code>.
     *
     * @see HttpRequest#getRequestListener()
     * @see com.nextel.net.RequestListener
     */
    public void setRequestListener(RequestListener listener) {
        requestListener = listener;
    }

    /**
     * Accessor method for the <code>RequestListener</code> associated with this
     * <code>HttpRequest</code>.  By default the <code>HttpRequest</code> serves
     * as a <code>RequestListener</code>.  When the request has completed the
     * associated <code>RequestListener</code> completed() method is called.
     * Since the <code>HttpRequest</code> is synchronous it is not as critical
     * to serve as a <code>RequestListener</code>.  The calling code will receive
     * an <code>InputStream</code> as the result of issuing a post or get method
     * which will serve as indication that the request has completed.
     * If a <code>RequestListener</code> is associated with this <code>
     * HttpRequest</code> (as is the case when this class is a delegate to the
     * <code>HttpAsyncRequest</code>) then the associated completed() method
     * for the <code>RequestListener</code> is called.
     *
     * @see com.nextel.net.RequestListener
     */
    public RequestListener getRequestListener() {
        return requestListener;
    }

    /**
     * Required by the <code>RequestListener</code> interface.  This method is
     * typically called when the request has completed.  A <code>RequestListener
     * </code> instance associated with this <code>HttpRequest</code> will have
     * its completed() method called.  This method for the <code>HttpRequest
     * </code> by default does nothing.
     *
     * @param i the <code>Request</code> that has just successfully completed. In
     *          all cases this should be "this".
     * @see RequestListener
     */
    public void completed(Request r) {
    }

    /**
     * Required by the <code>RequestListener</code> interface.  This method is
     * called if an exception occurs while initiating the HTTP request and if
     * the number of retries have been reached.  This allows the exception
     * to be provided to the calling code through the <code>RequestListener</code>
     *
     * @see com.nextel.net.RequestListener
     */
    public void exception(Exception e, Request r) {
    }


    private void sendPostParameters(Hashtable parameters) throws IOException {
        getHttpConnection().setRequestMethod(HttpConnection.POST);
        if (parameters != null) {
            Enumeration enum = parameters.keys();
            while (enum.hasMoreElements()) {
                String key = (String) enum.nextElement();
                getHttpConnection().setRequestProperty(key,
                        (String) parameters.get(key));
            }
        }
    }

    private void sendPostByteStream(byte [] data) throws IOException {
        getHttpConnection().setRequestProperty("Content-Length", "" + data.length);
        try {
            timer = getRequestTimer();
            startTimer();
            outputStream = makeOutputStream();
            stopTimer();
            startTimer();
            outputStream.write(data);
            stopTimer();
            outputStream.flush();
        }
        finally {
            cancelTimer();
            if (outputStream != null) {
                outputStream.close();
            }
        }
    }

    private String urlencode(Hashtable h) {
        String temp = null;
        if (h != null) {
            temp = h.toString();
            temp = temp.replace('{', ' ').replace('}', ' ').trim();
            temp = replaceCommas(temp);
            temp = temp.replace(' ', '+');
        }
        return temp;
    }

    private String replaceCommas(String commaStr) {
        String processed = "";
        String finalStr = "";
        boolean lastTime = false;
        int start = 0;
        int index = commaStr.indexOf(",");

        while (index > -1) {
            processed = commaStr.substring(start, index);
            start = index + 1;
            commaStr = commaStr.substring(start).trim();
            processed = processed.replace(',', '&').trim();
            index = commaStr.indexOf(",");
            finalStr = finalStr.concat(processed);
        }
        finalStr = finalStr.concat("&");
        finalStr = finalStr.concat(commaStr);
        return finalStr;
    }

    private RequestTimer getRequestTimer() {
        if (timer == null) {
            timer = new RequestTimer(this, getTimeout());
            timer.start();
        }
    return timer;
  }
}
