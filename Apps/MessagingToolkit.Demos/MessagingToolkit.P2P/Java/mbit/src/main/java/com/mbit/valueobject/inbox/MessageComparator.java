package com.mbit.valueobject.inbox;

import javax.microedition.rms.RecordComparator;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 11, 2006 3:33:41 PM
 * @version : $Id:
 */
public class MessageComparator implements RecordComparator {


    public int compare(byte[] bytes1, byte[] bytes2) {
        int compvalue = EQUIVALENT;
        try {
            ByteArrayInputStream bin = new ByteArrayInputStream(bytes1);
            DataInputStream din = new DataInputStream(bin);
            long id1 = din.readLong();
            din.close();
            bin.close();

            bin = new ByteArrayInputStream(bytes2);
            din = new DataInputStream(bin);
            long id2 = din.readLong();

            if (id1 > id2) {
                compvalue = FOLLOWS;
            } else {
                compvalue = PRECEDES;
            }

            din.close();
            bin.close();
            return compvalue;
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return compvalue;
        }
    }
}


