package com.mbit.browser;

import com.mbit.browser.MIDPFormTextField;

import javax.microedition.lcdui.ItemStateListener;
import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.Item;
import javax.microedition.lcdui.Display;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Mar 12, 2006 10:44:50 PM
 * @version : $Id:
 */

/**
 * This is some fancy footwork here. Perhaps too fancy.  What I
 * want to accomplish is that when the user edits the TextField,
 * that as soon as they hit "save", that we return to the browser
 * screen.  In the MIDP "form" model, after entering or modifying
 * text, they are sent back to the Form, which then requires them
 * to enter another command to go back to the browser.
 * <p/>
 * <p/>
 * <p/>
 * I would like to use the itemStateChanged() interface to go
 * right back to the browser screen, but for some reason that
 * won't work in the MIDP emulator. It says "SCREEN event when no
 * next?"  (whatever the hell that means).
 * <p/>
 * <p/>
 * <p/>
 * So, I am using the callSerially() interface, which does seem to
 * do the right thing, though it is pretty hard to understand why.
 */
public class StateChangeListener implements ItemStateListener, Runnable {
    Command cmd;
    MIDPFormTextField form;

    StateChangeListener(Command cmd, MIDPFormTextField form) {
        this.cmd = cmd;
        this.form = form;
    }

    public void run() {
        // Send the "done" command
        form.commandAction(cmd, form);
    }

    public void itemStateChanged(Item item) {
        Display d = Display.getDisplay(form.app);
        // form.commandAction(cmd, form);
        d.callSerially(this);
    }
}