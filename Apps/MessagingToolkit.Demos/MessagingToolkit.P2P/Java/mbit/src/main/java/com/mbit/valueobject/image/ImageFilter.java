package com.mbit.valueobject.image;

import javax.microedition.rms.RecordFilter;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 14, 2006 10:14:29 AM
 * @version : $Id:
 */
public class ImageFilter implements RecordFilter {

    private ByteArrayInputStream bin;
    private DataInputStream din;
    private String name;

    public ImageFilter(String title) {
        this.name = title;
    }
    public boolean matches(byte[] bytes) {
        try {
            bin = new ByteArrayInputStream(bytes);
            din = new DataInputStream(bin);
            String n = din.readUTF();
            din.close();
            bin.close();
            return (name.equals(n));
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return false;
        }
    }
}