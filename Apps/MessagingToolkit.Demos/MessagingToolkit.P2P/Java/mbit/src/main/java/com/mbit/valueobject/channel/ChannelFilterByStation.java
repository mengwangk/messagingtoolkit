package com.mbit.valueobject.channel;

import javax.microedition.rms.RecordFilter;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 5, 2006 12:12:48 PM
 * @version : $Id:
 */
public class ChannelFilterByStation implements RecordFilter {

    private ByteArrayInputStream bin;
    private DataInputStream din;
    private String stationName;

    public ChannelFilterByStation(String name) {
        stationName = name;
    }

    public boolean matches(byte[] bytes) {
        try {
            bin = new ByteArrayInputStream(bytes);
            din = new DataInputStream(bin);
            din.readInt();
            din.readUTF();
            din.readUTF();
            din.readUTF();
            din.readUTF();
            String name = din.readUTF();
            din.close();
            bin.close();
            return (stationName.equals(name));
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return false;
        }
    }
}
