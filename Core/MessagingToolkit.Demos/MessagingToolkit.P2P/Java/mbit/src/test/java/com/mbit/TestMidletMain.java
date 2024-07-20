package com.mbit;

import j2meunit.midletui.TestRunner;

import javax.microedition.midlet.MIDletStateChangeException;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 28, 2006 9:04:04 PM
 * @version : $Id:
 */
public class TestMidletMain extends TestRunner {

    protected void startApp() throws MIDletStateChangeException {
        start(new String[] { "com.mbit.TestHttpMidlet" });
    }      

}
