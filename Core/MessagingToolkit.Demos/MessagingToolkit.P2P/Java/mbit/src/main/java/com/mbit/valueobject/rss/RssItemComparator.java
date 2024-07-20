package com.mbit.valueobject.rss;

import javax.microedition.rms.RecordComparator;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 7, 2006 8:04:39 PM
 * @version : $Id:
 */
public class RssItemComparator implements RecordComparator {

    /**
     * Constructor for the class.
     */
    public RssItemComparator() {
    }

    /**
     * Returns <code>PRECEDES</code> if <code>bytes1</code> is parsed
     * AFTER <code>bytes2</code>. Latest items first.
     *
     * @return the comparison value
     */
    public int compare(byte[] bytes1, byte[] bytes2) {
        int compValue = EQUIVALENT;
        try {
            ByteArrayInputStream bin = new ByteArrayInputStream(bytes1);
            DataInputStream din = new DataInputStream(bin);
            din.readInt();
            long rec1time = din.readLong();
            din.close();
            bin.close();

            bin = new ByteArrayInputStream(bytes2);
            din = new DataInputStream(bin);
            din.readInt();
            long rec2time = din.readLong();

            // Is the parse time of bytes1 before rec2s
            if (rec1time < rec2time) {
                compValue = FOLLOWS;
            } else if (rec1time > rec2time) {
                compValue = PRECEDES;
            }

            din.close();
            bin.close();
            return compValue;
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return compValue;
        }
    }
}
