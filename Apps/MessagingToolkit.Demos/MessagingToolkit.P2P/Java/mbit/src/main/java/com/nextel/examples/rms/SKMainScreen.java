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

import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Displayable;

import javax.microedition.lcdui.List;

import com.nextel.ui.OHandset;
import com.nextel.examples.ui.ScreenNavigator;

/**
 * This is the main screen for the salary keeper program. It has 2 options:
 * Create a new record and show all the previous records created.
 *
 * @author Ryan Wollmuth
 */
public class SKMainScreen extends List implements CommandListener {

    private Command exitCmd = new Command("EXIT", Command.EXIT, 1);
    private Command nextCmd = new Command("SELECT", Command.OK, 1);

    private ShowRecordsScreen showRecords;
    private SalaryRecordScreen salaryRecord;

    public static final int NEW_RECORD = 0;
    public static final int SHOW_RECORDS = 1;

    /**
     * Creates a <code>SKMainScreen</code> instance.
     */
    public SKMainScreen() {
        super("Salary Keeper", IMPLICIT);
        init();
    }

    // Initializes the screen.
    private void init() {
        addCommand(nextCmd);
        addCommand(exitCmd);

        setCommandListener(this);

        insert(NEW_RECORD, "New record", null);
        insert(SHOW_RECORDS, "Saved records", null);

    }

    /**
     * Command listener for softkeys.
     * <p/>
     * There are 2 softkeys on this screen: 'SELECT' and 'EXIT'.
     * When 'SELECT' is pressed, it takes the current selected index
     * and figures out which screen to go to.
     */
    public void commandAction(Command command, Displayable displayable) {
        if (command == nextCmd) { // they pressed SELECT
            switch (getSelectedIndex()) {
                case NEW_RECORD:
                    ScreenNavigator.goForward(new SalaryRecordScreen());
                    break;
                case SHOW_RECORDS:
                    ScreenNavigator.goForward(new ShowRecordsScreen());
                    showRecords = null;
                    break;
                default:
            }
        } else if (command == exitCmd) { // they pressed EXIT
            OHandset.getMIDlet().notifyDestroyed();
        }
    }
}
