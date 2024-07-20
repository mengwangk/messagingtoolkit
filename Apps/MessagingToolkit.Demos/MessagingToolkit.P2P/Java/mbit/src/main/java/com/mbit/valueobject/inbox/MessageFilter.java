package com.mbit.valueobject.inbox;

import javax.microedition.rms.RecordFilter;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 11, 2006 11:41:30 AM
 * @version : $Id:
 */
public class MessageFilter implements RecordFilter {

    private ByteArrayInputStream bin;
    private DataInputStream din;
    private String title;

    public MessageFilter(String title) {
        this.title = title;
    }
    public boolean matches(byte[] bytes) {
        try {
            bin = new ByteArrayInputStream(bytes);
            din = new DataInputStream(bin);
            din.readLong();
            String t = din.readUTF();
            din.close();
            bin.close();
            return (title.equals(t));
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return false;
        }
    }
}
