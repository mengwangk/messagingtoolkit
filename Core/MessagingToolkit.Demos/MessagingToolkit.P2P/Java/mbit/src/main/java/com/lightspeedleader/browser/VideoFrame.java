package com.lightspeedleader.browser;

import java.io.DataInputStream;
import javax.microedition.io.HttpConnection;
import javax.microedition.lcdui.Image;

public class VideoFrame {

    public int x;
    public int y;
    public HttpConnection conn;
    public DataInputStream dis;

    public VideoFrame() {
    }

    public void setXY(int i, int j) {
        x = i;
        y = j;
    }

    public void addStream(HttpConnection httpconnection, DataInputStream datainputstream) {
        conn = httpconnection;
        dis = datainputstream;
    }

    public Image getImage() {
        try {
            Image image;
            int i = dis.readUnsignedByte();
            int j = dis.readInt();
            byte abyte0[] = new byte[j];
            dis.readFully(abyte0);
            image = Image.createImage(abyte0, 0, j);
            return image;
        } catch (Exception exception) {
            return null;
        }
    }

    public void close() {
        try {
            dis.close();
            dis = null;
            conn.close();
            conn = null;
        }
        catch (Exception exception) {
        }
    }
}
