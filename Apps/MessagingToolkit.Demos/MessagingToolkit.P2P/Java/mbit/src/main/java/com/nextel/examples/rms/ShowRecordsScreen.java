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
import javax.microedition.lcdui.List;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Graphics;

import com.nextel.ui.*;
import com.nextel.util.Logger;
import com.nextel.examples.ui.ScreenNavigator;
import com.nextel.rms.OAbstractRecord;


/**
 * The main screen for deleting and editing Salary records.
 * <p/>
 * This screen uses a {@link com.nextel.ui.OScrollList} to display the records
 * and {@link com.nextel.ui.OPushButton}s to do actions on the records. You will
 * see that there are numerous {@link com.nextel.ui.OCommandAction}s written. Each
 * one is triggered when a <code>OPushButton</code> or <code>OSoftKey</code>
 * is pressed.
 *
 * @author Ryan Wollmuth
 */
public class ShowRecordsScreen extends OCompositeScreen {

    private OAbstractRecord[] records;
    String[] data;
    private OScrollList scrollList;
    private OPushButton editButton = new OPushButton("Edit", OUILook.PLAIN_SMALL,
            "EDIT");
    private OPushButton deleteButton = new OPushButton("Delete",
            OUILook.PLAIN_SMALL, "DELETE");
    private OPushButton purgeButton = new OPushButton("Purge",
            OUILook.PLAIN_SMALL, "PURGE");
    private OSoftKey backSoftKey = new OSoftKey("BACK");
    private SalaryStore store;

    // Back command action when the 'BACK' soft key is pressed.
    private OCommandAction backCmdAction = new OCommandAction() {
        public void performAction() {
            ScreenNavigator.goHome();
        }
    };

    // Displays the edit screen for the current record.
    private OCommandAction editCmdAction = new OCommandAction() {
        public void performAction() {
            int recordId = records[scrollList.getSelectedIndex()].getRecordId();
            ScreenNavigator.goForward(new SalaryRecordScreen(recordId));
        }
    };

    // Deletes the record currently selected.
    private OCommandAction deleteCmdAction = new OCommandAction() {
        public void performAction() {
            try {
                store.deleteRecord(records[scrollList.getSelectedIndex()]);
            }
            catch (Exception e) {
                Logger.ex(e);
            }
            ScreenNavigator.goHome();
        }
    };

    // Deletes all of the records in the SalaryStore.
    private OCommandAction deleteAllCmdAction = new OCommandAction() {
        public void performAction() {
            try {
                store.deleteAll();
            }
            catch (Exception e) {
                Logger.ex(e);
            }
            ScreenNavigator.goHome();
        }
    };

    /**
     * Creates a <code>ShowRecordsScreen</code> instance.
     */
    public ShowRecordsScreen() {
        super("Show Salaries", OUILook.PLAIN_MEDIUM, 3);
        init();
    }

    /**
     * Initializes the screen.
     * If there are no records found, "No records found" will be displayed
     * on the screen. Otherwise, a scrolllist with 3 buttons (Edit, Delete, Purge)
     * will be shown.
     */
    public void init() {
        backSoftKey.setAction(backCmdAction);
        addSoftKey(backSoftKey, Graphics.LEFT);

        try {
            if (store == null) store = SalaryStore.getInstance();

            // populate the cache
            records = store.getAll();

            if (records != null && records.length != 0) {
                // place in an array the names that are in the salary records so we can
                // use them when displaying the list of records
                data = new String[ records.length ];
                SalaryRecord record;
                for (int i = 0; i < records.length; i++) {
                    record = (SalaryRecord) records[i];
                    data[i] = record.getName();
                }
            }
        } catch (Exception e) {
            Logger.ex(e);
        }

        if (records == null || (records != null && records.length == 0)) {
            OLabel label = new OLabel("No records found.", OUILook.PLAIN_MEDIUM);
            add(label, 1, 0, Graphics.HCENTER);
        } else {
            // create a scroll list with the names that are in the salary records
            scrollList = new OScrollList(getWidth(), getBodyHeight() / 2,
                    OUILook.PLAIN_MEDIUM, OUILook.TEXT_FONT);
            scrollList.populate(data);

            int row = 0;
            add(scrollList, 1 /* center column */, row, Graphics.HCENTER);

            // add to the screen buttons for manipulating the records

            editButton.setAction(editCmdAction);
            add(editButton, 0 /* leftmost column */, ++row, Graphics.RIGHT);

            deleteButton.setAction(deleteCmdAction);
            add(deleteButton, 2 /* rightmost column */, row, Graphics.RIGHT);

            purgeButton.setAction(deleteAllCmdAction);
            add(purgeButton, 1 /* center column */, ++row, Graphics.LEFT);
        }
    }
}
