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
package com.nextel.examples.rms;

import javax.microedition.midlet.MIDlet;
import javax.microedition.midlet.MIDletStateChangeException;

import com.nextel.ui.OHandset;
import com.nextel.examples.ui.ScreenNavigator;

/**
 * Salary keeper is an example MIDlet using the UI and RMS toolkits.
 * <p/>
 * This MIDlet uses the RMS toolkit to store information about people's salary.
 * It lets the user create, edit, and delete records that contain a name and a
 * salary.
 * <p/>
 * The user is presented with an initial screen that contains two possible
 * actions:
 * <ul>
 * <p><li>Create a new record</li>
 * <p><li>View all saved records</li>
 * </ul>
 * If the user selects the action to create a new record a screen is displayed
 * that lets him enter a name and a salary. If he selects the action to view the
 * saved records a screen is displayed containing a scroll list of all saved
 * records and 3 pushbuttons that allow the user to edit the selected record,
 * delete the selected record, or purge (delete) all records.
 *
 * @author Ryan Wollmuth
 */
public class SalaryKeeper extends MIDlet {
    private SKMainScreen mainScreen;

    /**
     * Called when the application is started or resumed from a suspension
     */
    protected void startApp() throws MIDletStateChangeException {
        if (mainScreen != null) { // resuming after an interruption, so restore the last screen
            OHandset.getDisplay().setCurrent(ScreenNavigator.getCurrentScreen());
        } else  // first time in this method
        {
            OHandset.setMIDlet(this);
            mainScreen = new SKMainScreen();
            ScreenNavigator.goForward(mainScreen);
        }
    }

    /**
     * Called when the app is suspended.
     */
    protected void pauseApp() {
    }

    /** Called when the app is closed */
    protected void destroyApp(boolean param) throws MIDletStateChangeException {}
}
