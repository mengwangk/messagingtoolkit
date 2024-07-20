package com.mbit.valueobject.rss;

import javax.microedition.rms.RecordFilter;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 7, 2006 8:04:19 PM
 * @version : $Id:
 */
public class RssFeedFilterByUrl implements RecordFilter {

    private ByteArrayInputStream bin;
    private DataInputStream din;
    private String url;

    public RssFeedFilterByUrl(String url) {
        this.url = url;
    }

    public boolean matches(byte[] bytes) {
        try {
            bin = new ByteArrayInputStream(bytes);
            din = new DataInputStream(bin);
            String u = din.readUTF();
            din.close();
            bin.close();
            return (url.equals(u));
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return false;
        }
    }
}

