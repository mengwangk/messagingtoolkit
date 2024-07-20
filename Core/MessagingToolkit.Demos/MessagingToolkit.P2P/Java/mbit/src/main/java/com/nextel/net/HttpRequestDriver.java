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

import java.util.*;
import javax.microedition.midlet.*;
import javax.microedition.lcdui.*;
import javax.microedition.io.*;
import java.io.*;


/**
 * Test driver for {@link HttpRequest}
 */
public class HttpRequestDriver extends MIDlet implements CommandListener {
    public static int TIMEOUT = 20;
    public static int RETRY = 2;
    public static final Command okCommand = new Command("OK", Command.OK, 1);

    public HttpRequestDriver() {
    }

    public void startApp() {
        Form form = new Form("HttpRequestDriver");
        System.out.println("\nTest Started");
        form.setCommandListener(this);
        form.addCommand(okCommand);

        Display display = Display.getDisplay(this);
        display.setCurrent(form);
        HttpRequest request = new HttpRequest("some url", TIMEOUT, RETRY);

        System.out.println("Test 1 - Starting");
        if (request.getTimeout() == TIMEOUT)
            System.out.println("Test 1 - successful");
        else
            System.out.println("Test 1 - failed");

        System.out.println("Test 2 - Starting");
        request.setTimeout(TIMEOUT + 5);
        if (request.getTimeout() == TIMEOUT + 5)
            System.out.println("Test 2 - successful");
        else
            System.out.println("Test 2 - failed");

        System.out.println("Test 3 - Starting");
        try {
            InputStream is = request.get();
            System.out.println("Test 3 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 3 - Exception: " + ex);
            ex.printStackTrace();
        }

        System.out.println("Test 4 - Starting");
        Hashtable ht = new Hashtable();
        ht.put("key1", "value1");
        ht.put("key2", "value2");
        try {
            InputStream is = request.get(ht);
            System.out.println("Test 4 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 4 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 5 - Starting");
        try {
            InputStream is = request.post();
            System.out.println("Test 5 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 5 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 6 - Starting");
        try {
            InputStream is = request.post("some data");
            System.out.println("Test 6 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 6 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 7 - Starting");
        try {
            InputStream is = request.post(new String("some data").getBytes());
            System.out.println("Test 7 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 7 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 8 - Starting");
        try {
            InputStream is = request.post(new String("some data").getBytes());
            System.out.println("Test 8 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 8 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 9 - Starting");
        try {
            InputStream is = request.post(ht);
            System.out.println("Test 9 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 9 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 10 - Starting");
        try {
            InputStream is = request.post(ht, "some data");
            System.out.println("Test 10 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 10 - Exception: " + ex);
            ex.printStackTrace();
        }

        System.out.println("Test 11 - Starting");
        try {
            String nullStr = null;
            InputStream is = request.post(ht, nullStr);
            System.out.println("Test 11 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 11 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 12 - Starting");
        try {
            Hashtable h = null;
            String nullStr = null;
            InputStream is = request.post(h, nullStr);
            System.out.println("Test 12 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 12 - Exception: " + ex);
            ex.printStackTrace();
        }

        System.out.println("Test 13 - Starting");
        try {
            InputStream is = request.post(null, "some data");
            System.out.println("Test 13 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 13 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 14 - Starting");
        try {
            InputStream is = request.post(ht, new String("some data").getBytes());
            System.out.println("Test 14 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 14 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 15 - Starting");
        try {
            byte [] b = null;
            InputStream is = request.post(ht, b);
            System.out.println("Test 15 - Success. InputStream = " + is);
        }
        catch (Exception ex) {
            System.out.println("Test 15 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 16 - Starting");
        try {
            request.put("HTTPSTORE");
            System.out.println("Test 16 - Success.");
        }
        catch (Exception ex) {
            System.out.println("Test 16 - Exception: " + ex);
            ex.printStackTrace();
        }
        System.out.println("Test 17 - Starting");
        try {
            request.put(null);
            System.out.println("Test 17 - Success.");
        }
        catch (Exception ex) {
            System.out.println("Test 17 - Exception: " + ex);
            ex.printStackTrace();
        }
    }

    public void showAlert(String s, Form form) {
        Alert alert = new Alert("HttpRequestDriver", s, null, AlertType.INFO);
        alert.setTimeout(Alert.FOREVER);
        Display display = Display.getDisplay(this);
        display.setCurrent(alert, form);
    }

    public void commandAction(Command c, Displayable d) {
        notifyDestroyed();
    }

    public void pauseApp() {
    }

    public void destroyApp(boolean unconditional) {
    }

    /**
     * Connection class for the test driver.
     */
    public static class MyHttpConnection implements HttpConnection {
        public OutputStream openOutputStream() throws IOException {
            return null;
        }

        public InputStream openInputStream() throws IOException {
            return null;
        }

        public DataInputStream openDataInputStream() throws IOException {
            return null;
        }

        public DataOutputStream openDataOutputStream() throws IOException {
            return null;
        }

        public void close() throws IOException {
        }

        public String getEncoding() {
            return "";
        }

        public long getLength() {
            return 0L;
        }

        public String getType() {
            return "";
        }

        public long getDate() {
            return 0L;
        }

        public long getExpiration() {
            return 0L;
        }

        public String getFile() {
            return "";
        }

        public String getHeaderField(int i) {
            return "";
        }

        public String getHeaderField(String i) {
            return "";
        }

        public long getHeaderFieldDate(String i, long j) {
            return 0L;
        }

        public int getHeaderFieldInt(String i, int j) {
            return 0;
        }

        public String getHeaderFieldKey(int i) {
            return "";
        }

        public String getHost() {
            return "";
        }

        public long getLastModified() {
            return 0L;
        }

        public int getPort() {
            return 0;
        }

        public String getProtocol() {
            return "";
        }

        public String getQuery() {
            return "";
        }

        public String getRef() {
            return "";
        }

        public String getRequestMethod() {
            return "";
        }

        public String getRequestProperty(String i) {
            return "";
        }

        public int getResponseCode() {
            return 0;
        }

        public String getResponseMessage() {
            return "";
        }

        public String getURL() {
            return "";
        }

        public void setRequestMethod(String i) {
        }

        public void setRequestProperty(String i, String j) {
        }
    }
}
