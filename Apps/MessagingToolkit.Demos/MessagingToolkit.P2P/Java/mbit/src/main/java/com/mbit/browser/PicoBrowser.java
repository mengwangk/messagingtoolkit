package com.mbit.browser;

import com.mbit.base.BaseMidlet;
import com.mbit.browser.BrowserScreen;

import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.Display;
import javax.microedition.lcdui.Displayable;
import javax.microedition.midlet.MIDletStateChangeException;

public class PicoBrowser extends BaseMidlet {
    // display manager
    Display display = null;

    BrowserScreen browser;

    /**
     * Construct a browser MIDlet
     */
    public PicoBrowser() {
        display = Display.getDisplay(this);
    }

    /**
     * Returns the BrowserScreen associated with this application.
     * This is the object which implements the core of the browser.
     */
    public BrowserScreen browserCanvas() {
        return browser;
    }

    /**
     * Start the MIDlet
     */
    public void startApp() throws MIDletStateChangeException {
        browser = new BrowserScreen(this);
        /*browser.setText("testing");
        browser.setTitle("just a test");*/
        //browser.gotoHomepage();
        display.setCurrent(browser);
    }

    public void pauseApp() {
    }

    public void destroyApp(boolean unconditional) {
        notifyDestroyed();
    }

    public void commandAction(Command command, Displayable displayable) {
        //To change body of implemented methods use File | Settings | File Templates.
    }

}

