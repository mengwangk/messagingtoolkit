package com.lightspeedleader.browser;

import javax.microedition.rms.RecordStore;
import java.util.Vector;

public class Bookmarks {

    public static Vector BM = new Vector(1);

    public Bookmarks() {
        reset();
        try {
            RecordStore recordstore = RecordStore.openRecordStore("JCellBrowser.BM", false);
            if (recordstore != null) {
                int i = recordstore.getNumRecords();
                for (int j = 1; j <= i; j++) {
                    String s = new String(recordstore.getRecord(j));
                    BM.addElement(s);
                }

                recordstore.closeRecordStore();
            }
        }
        catch (Exception exception) {
        }
    }

    public void reset() {
        BM.removeAllElements();
    }

    public void save() {
        try {
            RecordStore.deleteRecordStore("JCellBrowser.BM");
        }
        catch (Exception exception) {
        }
        int i = BM.size();
        if (i == 0) {
            return;
        }
        try {
            RecordStore recordstore = RecordStore.openRecordStore("JCellBrowser.BM", true);
            for (int j = 0; j < i; j++) {
                String s = (String) BM.elementAt(j);
                recordstore.addRecord(s.getBytes(), 0, s.length());
            }

            recordstore.closeRecordStore();
        }
        catch (Exception exception1) {
        }
    }

}
