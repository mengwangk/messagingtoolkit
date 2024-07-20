package com.mbit;

import j2meunit.framework.TestCase;
import j2meunit.framework.TestMethod;
import j2meunit.framework.Test;
import j2meunit.framework.TestSuite;

import java.io.InputStream;
import java.io.DataInputStream;

import com.nextel.net.HttpRequest;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 28, 2006 7:14:18 PM
 * @version : $Id:
 */
public class TestHttpMidlet extends TestCase {

    public TestHttpMidlet() {
        super();    //To change body of overridden methods use File | Settings | File Templates.
    }

    public TestHttpMidlet(String string) {
        super(string);    //To change body of overridden methods use File | Settings | File Templates.
    }

    public TestHttpMidlet(String string, TestMethod testMethod) {
        super(string, testMethod);    //To change body of overridden methods use File | Settings | File Templates.
    }

    protected void setUp() throws Exception {
        super.setUp();    //To change body of overridden methods use File | Settings | File Templates.
    }

    protected void tearDown() throws Exception {
        super.tearDown();    //To change body of overridden methods use File | Settings | File Templates.
    }

    protected void testRetrieveUrl() {
        try {
            HttpRequest httpRequest = new HttpRequest("http://www.google.com.my", 20, 3);
            StringBuffer results = new StringBuffer();

            // create DataInputStream on top of the socket connection
            InputStream inputStream = httpRequest.get();
            DataInputStream dataInputStream = new DataInputStream(inputStream);

            // retrieve the contents of the requested page from Web server
            int inputChar;
            while ((inputChar = dataInputStream.read()) != -1) {
                results.append((char) inputChar);
            }
        } catch (Exception e) {
            e.printStackTrace();
            throw new RuntimeException(e.getMessage());
        }
    }

    public Test suite() {
        TestSuite suite = new TestSuite();
        suite.addTest(new TestHttpMidlet("Http Test", new TestMethod() {
            public void run(TestCase tc) {
                ((TestHttpMidlet) tc).testRetrieveUrl();
            }
        }));
        return suite;
    }

    public static void main(String[] args) {
        String[] runnerArgs = new String[]{"com.mbit.TestHttpMidlet"};
        j2meunit.textui.TestRunner.main(runnerArgs);
    }


}
