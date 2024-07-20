package com.lightspeedleader.browser;

import javax.microedition.rms.RecordStore;
import java.util.Vector;

public class CachePool {

    Vector C3;
    public int limit;
    public int total;

    public CachePool(int i) {
        C3 = new Vector(1);
        limit = i;
        reset();
        try {
            RecordStore recordstore = RecordStore.openRecordStore("JCellBrowser.cache", false);
            if (recordstore != null) {
                int j = recordstore.getNumRecords();
                for (int k = 1; k <= j; k += 2) {
                    String s = new String(recordstore.getRecord(k));
                    byte abyte0[] = recordstore.getRecord(k + 1);
                    C3.addElement(new CacheObj(s, abyte0));
                }

                recordstore.closeRecordStore();
            }
        }
        catch (Exception exception) {
        }
    }

    public void reset() {
        C3.removeAllElements();
        total = 0;
    }

    public void setCache(String s, byte abyte0[]) {
        if (abyte0.length >= limit) {
            return;
        }
        if (getCache(s) != null) {
            return;
        }
        do {
            if (total + abyte0.length < limit) {
                break;
            }
            CacheObj cacheobj = (CacheObj) C3.elementAt(0);
            total -= cacheobj.buffer.length;
            C3.removeElementAt(0);
            if (C3.size() == 0) {
                total = 0;
            }
        } while (true);
        total += abyte0.length;
        C3.addElement(new CacheObj(s, abyte0));
    }

    public byte[] getCache(String s) {
        int i = C3.size();
        for (int j = 0; j < i; j++) {
            CacheObj cacheobj = (CacheObj) C3.elementAt(j);
            if (s.equals(cacheobj.name)) {
                return cacheobj.buffer;
            }
        }

        return null;
    }

    public void saveCache() {
        try {
            RecordStore.deleteRecordStore("JCellBrowser.cache");
        }
        catch (Exception exception) {
        }
        int i = C3.size();
        if (i == 0) {
            return;
        }
        try {
            RecordStore recordstore = RecordStore.openRecordStore("JCellBrowser.cache", true);
            for (int j = 0; j < i; j++) {
                CacheObj cacheobj = (CacheObj) C3.elementAt(j);
                recordstore.addRecord(cacheobj.name.getBytes(), 0, cacheobj.name.length());
                recordstore.addRecord(cacheobj.buffer, 0, cacheobj.buffer.length);
            }

            recordstore.closeRecordStore();
        }
        catch (Exception exception1) {
        }
    }
}
