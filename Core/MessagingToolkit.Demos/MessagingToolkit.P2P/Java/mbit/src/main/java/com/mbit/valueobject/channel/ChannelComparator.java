package com.mbit.valueobject.channel;

import javax.microedition.rms.RecordComparator;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 4, 2006 10:26:44 AM
 * @version : $Id:
 */
public class ChannelComparator implements RecordComparator {


    public int compare(byte[] bytes1, byte[] bytes2) {
        int compvalue = EQUIVALENT;
        try {
            ByteArrayInputStream bin = new ByteArrayInputStream(bytes1);
            DataInputStream din = new DataInputStream(bin);
            int id1 = din.readInt();
            din.close();
            bin.close();

            bin = new ByteArrayInputStream(bytes2);
            din = new DataInputStream(bin);
            int id2 = din.readInt();

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


