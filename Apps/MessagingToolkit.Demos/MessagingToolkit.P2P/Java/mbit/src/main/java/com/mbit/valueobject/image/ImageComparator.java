package com.mbit.valueobject.image;

import javax.microedition.rms.RecordComparator;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 14, 2006 10:14:36 AM
 * @version : $Id:
 */
public class ImageComparator implements RecordComparator {


    public int compare(byte[] bytes1, byte[] bytes2) {
        int compvalue = EQUIVALENT;
        try {
            ByteArrayInputStream bin = new ByteArrayInputStream(bytes1);
            DataInputStream din = new DataInputStream(bin);
            String id1 = din.readUTF();
            din.close();
            bin.close();

            bin = new ByteArrayInputStream(bytes2);
            din = new DataInputStream(bin);
            String id2 = din.readUTF();

            if (id1.compareTo(id2) > 0) {
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

