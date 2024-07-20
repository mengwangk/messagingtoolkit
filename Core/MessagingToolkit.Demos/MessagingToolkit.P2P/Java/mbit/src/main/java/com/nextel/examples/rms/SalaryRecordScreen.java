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
import javax.microedition.lcdui.Graphics;
import java.io.IOException;

import com.nextel.ui.*;
import com.nextel.util.Logger;
import com.nextel.examples.ui.ScreenNavigator;

/**
 * This is the input screen for the Salary Keeper app.
 * <p/>
 * It contains 2 {@link com.nextel.ui.OTextField}s, for the name and the salary.
 *
 * @author Ryan Wollmuth
 */
public class SalaryRecordScreen extends OCompositeScreen
        implements CommandListener {
    private Command saveCmd = new Command("SAVE", Command.OK, 1);
    private Command backCmd = new Command("BACK", Command.BACK, 1);

    private OLabel label2 = new OLabel("Salary: $", OUILook.PLAIN_SMALL);
    private OTextField nameField =
            new OTextField(8, OUILook.PLAIN_SMALL,
                    OTextField.UPPERCASE | OTextField.LOWERCASE);
    private OTextField numberFld =
            new OTextField(7, OUILook.PLAIN_SMALL, OTextField.NUMERIC);
    private OLabel label1 = new OLabel("Name: ", OUILook.PLAIN_SMALL);
    private OPanel panel = new OPanel();
    private OPanel panel2 = new OPanel();

    private int recordID;
    private SalaryRecord record;

    /**
     * Creates a <code>SalaryRecordScreen</code> instance.
     */
    public SalaryRecordScreen() {
        super("Salary Record", OUILook.PLAIN_MEDIUM, 1);
        init();
    }

    /**
     * Creates a <code>SalaryRecordScreen</code> instance.
     *
     * @param recordID The recordID of the record you wish to edit
     */
    public SalaryRecordScreen(int recordID) {
        super("Salary Record", OUILook.PLAIN_MEDIUM, 1);
        this.recordID = recordID;
        init();
    }

    // Initializes the screen.
    // If recordID is not null, the fields are populated with the contents of the
    // record that matches the recordID.
    private void init() {
        nameField.allowSpaces(true);

        int x = 0;
        int y = 0;
        addCommand(saveCmd);
        addCommand(backCmd);

        setCommandListener(this);

        if (recordID != 0) {
            try {
                record = (SalaryRecord) SalaryStore.getInstance().getRecord(recordID);
                nameField.setText(record.getName());
                numberFld.setText(record.getSalary());
            }
            catch (Exception e) {
                Logger.ex(e);
            }
        }
        panel.add(label1);
        panel.add(nameField);
        add(panel, x, y++, Graphics.LEFT);

        panel2.add(label2);
        panel2.add(numberFld);

        add(panel2, x, y++, Graphics.LEFT);
    }

    /**
     * Command listener for soft keys.
     * <p/>
     * The 2 soft keys are 'BACK', which returns you to the main menu, and
     * 'SAVE', which saves the new record or updates the current one.
     */
    public void commandAction(Command command, Displayable displayable) {
        if (command == backCmd)
            ScreenNavigator.goBack();
        else if (command == saveCmd) {
            try {
                SalaryStore salaryStore = SalaryStore.getInstance();

                if (record == null)
                    record = new SalaryRecord();
                record.setName(nameField.getText());
                record.setSalary(numberFld.getText());

                if (recordID == 0)
                    salaryStore.addRecord(record);
                else {
                    salaryStore.updateRecord(record);
                }
                OHandset.beep();
                ScreenNavigator.goHome();
            }
            catch (Exception e) {
                Logger.ex(e);
            }
        }
    }
}
