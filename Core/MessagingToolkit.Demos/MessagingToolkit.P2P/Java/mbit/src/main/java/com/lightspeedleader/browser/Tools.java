package com.lightspeedleader.browser;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.lcdui.Image;
import java.io.DataInputStream;

public class Tools {

    public static int GetTokenQuickPos = -1;
    public static int ColorStackLevel;
    public static int ColorStack[];
    public static int ColorStackIndex = -1;
    public static int HistoryLevel;
    public static String HistoryStack[];
    public static int HistoryIndex = -1;

    public Tools() {
    }

    public static int toInt(String s) {
        int i = 0;
        try {
            i = Integer.parseInt(s);
        }
        catch (Exception exception) {
        }
        return i;
    }

    public static String GetTokenZ(String s, int i, char c, boolean flag) {
        boolean flag1 = false;
        int k = -1;
        int l = 1;
        if (flag && i > 1) {
            k = GetTokenQuickPos;
            l = i - 1;
        }
        do {
            if (flag) {
                GetTokenQuickPos = k;
            }
            int j = s.indexOf(c, k + 1);
            if (i == l) {
                if (j != -1) {
                    return s.substring(k + 1, j);
                } else {
                    return s.substring(k + 1, s.length());
                }
            }
            if (j == -1) {
                return null;
            }
            l++;
            k = j;
        } while (true);
    }

    public static String GetTokenY(String s, int i, char c) {
        return GetTokenZ(s, i, c, false);
    }

    public static String GetTokenX(String s, int i, char c) {
        String s1 = GetTokenY(s, i, c);
        if (s1 == null) {
            s1 = "";
        }
        return s1;
    }

    public static String GetToken(String s, int i) {
        return GetTokenX(s, i, '@');
    }

    public static String RepStringX(String s, String s1, String s2, boolean flag) {
        String s7 = "";
        if (s == null || s1 == null || s2 == null) {
            return s;
        }
        int j;
        if (flag) {
            s1 = s1.toLowerCase();
            int i;
            while ((i = s.toLowerCase().indexOf(s1)) != -1) {
                String s3 = s.substring(0, i);
                String s5 = s.substring(i + s1.length());
                s7 = s7 + s3 + s2;
                s = s5;
            }
        } else {
            while ((j = s.indexOf(s1)) != -1) {
                String s4 = s.substring(0, j);
                String s6 = s.substring(j + s1.length());
                s7 = s7 + s4 + s2;
                s = s6;
            }
        }
        return s7 + s;
    }

    public static String RepString(String s, String s1, String s2) {
        return RepStringX(s, s1, s2, false);
    }

    public static int GetColor(String s) {
        try {
            if (s == null) {
                return 0xffffff;
            }
            s = s.toLowerCase();
            if (s.equals("black")) {
                return 0;
            }
            if (s.equals("blue")) {
                return 4095;
            }
            if (s.equals("cyan")) {
                return 65535;
            }
            if (s.equals("gray")) {
                return 0x808080;
            }
            if (s.equals("green")) {
                return 65280;
            }
            if (s.equals("magenta")) {
                return 0xff00ff;
            }
            if (s.equals("red")) {
                return 0xff0000;
            }
            if (s.equals("white")) {
                return 0xffffff;
            }
            if (s.equals("yellow")) {
                return 0xffff00;
            }
            if (s.startsWith("#") && s.length() > 1) {
                s = s.substring(1);
            }
            if (s.startsWith("0x") && s.length() > 2) {
                s = s.substring(2);
            }
            return Integer.parseInt(s, 16);
        } catch (Exception exception) {
            return 0xffffff;
        }

    }

    public static void initColorStack() {
        ColorStackIndex = -1;
    }

    public static void pushColorStack(int i) {
        if (ColorStackIndex + 1 < ColorStackLevel) {
            ColorStackIndex++;
            ColorStack[ColorStackIndex] = i;
        }
    }

    public static int popColorStack() {
        int i = ColorStack[ColorStackIndex];
        if (ColorStackIndex > 0) {
            ColorStackIndex--;
        }
        return i;
    }

    public static void initHistoryStack() {
        HistoryIndex = -1;
    }

    public static void pushHistoryStack(String s) {
        if (s.indexOf('?') != -1) {
            return;
        }
        if (HistoryIndex >= 0 && s.equals(HistoryStack[HistoryIndex])) {
            return;
        }
        if (HistoryIndex + 1 < HistoryLevel) {
            HistoryIndex++;
            HistoryStack[HistoryIndex] = s;
        }
    }

    public static String popHistoryStack() {
        String s = HistoryStack[HistoryIndex];
        if (HistoryIndex > 0) {
            HistoryIndex--;
        }
        return s;
    }

    public static String GetURLBase(String s) {
        String s1 = s;
        if (s1 == null) {
            return "";
        }
        if (!s1.startsWith("http://")) {
            s1 = "http://" + s1;
        }
        int i = s1.length();
        int j;
        for (j = i - 1; j >= 0 && s1.charAt(j) != '/' && s1.charAt(j) != '\\'; j--) {
        }
        s1 = s1.substring(0, j + 1);
        if (s1.equals("http://")) {
            s1 = s + "/";
        }
        return s1;
    }

    public static String GetURLRoot(String s) {
        if (s == null) {
            return "";
        }
        if (s.startsWith("http://")) {
            s = s.substring(7);
        }
        int i = s.length();
        if (i == 0) {
            return "";
        }
        int j;
        for (j = 0; j < i && s.charAt(j) != '/' && s.charAt(j) != '\\'; j++) {
        }
        if (j < i) {
            s = s.substring(0, j + 1);
        } else {
            s = s + "/";
        }
        return "http://" + s;
    }

    public static Image CreateGifImage(byte abyte0[]) {
        try {
            Image image;
            GifDecoder gifdecoder = new GifDecoder(abyte0);
            image = null;
            for (; gifdecoder.moreFrames(); gifdecoder.nextFrame()) {
                try {
                    image = gifdecoder.decodeImage();
                    if (HTMLStream.GF != null && image != null) {
                        HTMLStream.GF.addImage(image);
                    }
                    continue;
                }
                catch (Exception exception1) {
                }
                break;
            }

            gifdecoder.clear();
            gifdecoder = null;
            if (HTMLStream.GF != null && HTMLStream.GF.size() > 1) {
                image = HTMLStream.GF.getImage();
            }
            return image;
        } catch (Exception ex) {

            return null;
        }
    }

    public static Image CreateJpegImage(byte abyte0[]) {
        JpegDecoder jpegdecoder = new JpegDecoder(abyte0);
        Image image = jpegdecoder.decodeImage();
        jpegdecoder.clear();
        jpegdecoder = null;
        return image;
    }

    public static Image LoadHtmlImage(String s) {
        String s1;
        s1 = s.toLowerCase();
        Object obj = null;
        Runtime.getRuntime().gc();
        if (!s1.endsWith(".png") && !s1.endsWith(".gif") && !s1.endsWith(".jpg") && s1.indexOf('?') == -1) {
            return null;
        }
        try {
            Image image;
            int i;
            byte abyte0[];
            if ((abyte0 = JCellBrowser.cachepool.getCache(s)) == null) {
                HttpConnection httpconnection = (HttpConnection) Connector.open(s);
                if (httpconnection.getResponseCode() != 200) {
                    httpconnection.close();
                    httpconnection = null;
                    throw new Exception();
                }
                i = (int) httpconnection.getLength();
                abyte0 = new byte[i];
                DataInputStream datainputstream = httpconnection.openDataInputStream();
                datainputstream.readFully(abyte0);
                datainputstream.close();
                datainputstream = null;
                httpconnection.close();
                httpconnection = null;
                if (s.indexOf('?') == -1) {
                    JCellBrowser.cachepool.setCache(s, abyte0);
                }
            } else {
                i = abyte0.length;
            }
            image = null;
            if (s1.endsWith(".png") || s1.indexOf('?') != -1) {
                image = Image.createImage(abyte0, 0, i);
            }
            if (s1.endsWith(".gif")) {
                image = CreateGifImage(abyte0);
            }
            if (s1.endsWith(".jpg")) {
                image = CreateJpegImage(abyte0);
            }
            abyte0 = null;
            return image;
        } catch (Exception exception) {
            return null;
        }

    }

    public static Image LoadVideoImage(String s) {
        try {
            HttpConnection httpconnection;
            DataInputStream datainputstream;
            httpconnection = (HttpConnection) Connector.open(s);
            if (httpconnection.getResponseCode() != 200) {
                httpconnection.close();
                httpconnection = null;
                throw new Exception();
            }
            datainputstream = httpconnection.openDataInputStream();
            int i = datainputstream.readUnsignedByte();
            int j = datainputstream.readUnsignedByte();
            if (j == 1) {
                return null;
            }
            datainputstream.close();
            httpconnection.close();
            httpconnection = null;
            Image image;
            int k = datainputstream.readInt();
            int l = datainputstream.readInt();
            int i1 = datainputstream.readUnsignedByte();
            int j1 = datainputstream.readInt();
            byte abyte0[] = new byte[j1];
            datainputstream.readFully(abyte0);
            image = Image.createImage(abyte0, 0, j1);
            HTMLStream.VF.addStream(httpconnection, datainputstream);
            return image;
        } catch (Exception ex) {

        }
        return null;
    }

    public static String cutString(String s, int i) {
        int j = s.length();
        do {
            if (MapCanvas.strWidth(s) <= i) {
                break;
            }
            if (--j == 0) {
                s = "";
                break;
            }
            s = s.substring(0, j);
        } while (true);
        return s;
    }

    static {
        ColorStackLevel = 16;
        ColorStack = new int[ColorStackLevel];
        HistoryLevel = 16;
        HistoryStack = new String[HistoryLevel];
    }
}
