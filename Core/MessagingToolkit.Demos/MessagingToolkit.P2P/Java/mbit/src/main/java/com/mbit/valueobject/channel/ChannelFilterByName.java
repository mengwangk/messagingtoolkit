package com.mbit.valueobject.channel;

import javax.microedition.rms.RecordFilter;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 4, 2006 10:26:38 AM
 * @version : $Id:
 */
public class ChannelFilterByName implements RecordFilter {

    private ByteArrayInputStream bin;
    private DataInputStream din;
    private String channelName;

    public ChannelFilterByName(String name) {
        channelName = name;
    }
    public boolean matches(byte[] bytes) {
        try {
            bin = new ByteArrayInputStream(bytes);
            din = new DataInputStream(bin);
            din.readInt();
            String name = din.readUTF();
            din.close();
            bin.close();
            return (channelName.equals(name));
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return false;
        }
    }
}