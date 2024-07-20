package com.lightspeedleader.browser;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Image;
import java.io.*;
import java.util.Vector;

public class HTMLStream {

    HttpConnection conn;
    InputStream is;
    ByteArrayInputStream FE;
    InputStreamReader FF;
    int total;
    int _fld0100;
    byte buffer[];
    String _fld0101;
    String _fld0102;
    String _fld0103;
    public static int vx = 0;
    public static int vy = 0;
    public static int slen;
    Vector sv;
    int CanvasWidth;
    int CanvasHeight;
    Font font;
    int fontHeight;
    int _fld0104;
    int _fld0105;
    int _fld0106;
    int _fld0107;
    public static int lcolor = 255;
    Vector _fld0108;
    HotSpot hs;
    boolean _fld0109;
    public static int hsindex = 0;
    public static Vector hsvector = new Vector(1);
    public static int formindex = -1;
    public static Vector formvector = new Vector(1);
    boolean _fld010A;
    boolean _fld010B;
    boolean _fld010C;
    String _fld010D;
    boolean _fld010E;
    boolean _fld010F;
    boolean _fld0110;
    boolean _fld0111;
    int _fld0112;
    int _fld0113[];
    int _fld0114;
    public static GifFrame GF;
    public static VideoFrame VF;
    public static String ErrorLine;

    public HTMLStream(String s) {
        _fld0104 = 3;
        _fld0105 = 3;
        _fld0106 = 0xffffff;
        _fld0107 = 0;
        _fld0108 = new Vector(1);
        _fld0109 = false;
        _fld010A = false;
        _fld010B = false;
        _fld010C = false;
        _fld010D = "";
        _fld010E = false;
        _fld010F = false;
        _fld0110 = false;
        _fld0111 = false;
        _fld0112 = 0;
        _fld0113 = new int[8];
        _fld0114 = 16;
        HTMLStreamX(s, "");
    }

    HTMLStream(String s, String s1) {
        _fld0104 = 3;
        _fld0105 = 3;
        _fld0106 = 0xffffff;
        _fld0107 = 0;
        _fld0108 = new Vector(1);
        _fld0109 = false;
        _fld010A = false;
        _fld010B = false;
        _fld010C = false;
        _fld010D = "";
        _fld010E = false;
        _fld010F = false;
        _fld0110 = false;
        _fld0111 = false;
        _fld0112 = 0;
        _fld0113 = new int[8];
        _fld0114 = 16;
        HTMLStreamX(s, s1);
    }

    public void HTMLStreamX(String s, String s1) {
        Runtime.getRuntime().gc();
        MapCanvas _tmp = JCellBrowser.mc;
        MapCanvas.LoadFlag = true;
        JCellBrowser.mc.repaint();
        JCellBrowser.mc.serviceRepaints();
        if (!s.startsWith("http://")) {
            s = "http://" + s;
        }
        _fld0101 = s;
        _fld0102 = Tools.GetURLRoot(_fld0101);
        _fld0103 = Tools.GetURLBase(_fld0101);
        total = 0;
        _fld0100 = 0;
        buffer = null;
        int i = 999;
        try {
            if ((buffer = JCellBrowser.cachepool.getCache(s)) == null) {
                conn = (HttpConnection) Connector.open(s);
                if (s1.length() > 0) {
                    conn.setRequestMethod("POST");
                    conn.setRequestProperty("User-Agent", "Profile/MIDP-1.0 Configuration/CLDC-1.0");
                    conn.setRequestProperty("Content-Language", "en-US");
                    conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
                    conn.setRequestProperty("Content-Length", "" + s1.length());
                    conn.setRequestProperty("Cache-Control", "no-store");
                    conn.setRequestProperty("Pragma", "no-cache");
                    conn.setRequestProperty("Expires", "Tue, 1 Jul 1997 00:00:00 GMT");
                    OutputStream outputstream = conn.openOutputStream();
                    outputstream.write(s1.getBytes());
                    outputstream.flush();
                    outputstream.close();
                }
                if (((i = conn.getResponseCode()) == 301 || (i = conn.getResponseCode()) == 302) && s1.length() == 0) {
                    s = conn.getHeaderField("Location");
                    if (s != null) {
                        JCellBrowser.pageurl = s;
                    }
                    _fld0101 = s;
                    _fld0102 = Tools.GetURLRoot(_fld0101);
                    _fld0103 = Tools.GetURLBase(_fld0101);
                    conn.close();
                    conn = (HttpConnection) Connector.open(s);
                }
                if ((i = conn.getResponseCode()) != 200) {
                    throw new Exception();
                }
                total = (int) conn.getLength();
                if (total > 0) {
                    buffer = new byte[total];
                    DataInputStream datainputstream = conn.openDataInputStream();
                    datainputstream.readFully(buffer);
                    datainputstream.close();
                    if (s1.length() == 0 && s.indexOf('?') == -1) {
                        JCellBrowser.cachepool.setCache(s, buffer);
                    }
                } else {
                    is = conn.openInputStream();
                }
            } else {
                total = buffer.length;
            }
        }
        catch (Exception exception) {
            String s2 = "<center>JCellBrowser Error<br>" + i + "</center><hr size=2>Document <u>" + _fld0101 + "</u> is not found.";
            if (i == 999) {
                s2 = s2 + "<hr>" + exception.toString();
            }
            buffer = s2.getBytes();
            total = buffer.length;
        }
        if (total > 0) {
            FE = new ByteArrayInputStream(buffer, 0, total);
            try {
                FF = new InputStreamReader(FE, JCellBrowser.encoding);
            }
            catch (Exception exception1) {
                FF = new InputStreamReader(FE);
            }
        } else {
            try {
                FF = new InputStreamReader(is, JCellBrowser.encoding);
            }
            catch (Exception exception2) {
                FF = new InputStreamReader(is);
            }
        }
        sv = new Vector(32);
        _mth0100();
        buffer = null;
        try {
            if (FF != null) {
                FF.close();
                FF = null;
            }
            if (FE != null) {
                FE.close();
                FE = null;
            }
            if (is != null) {
                is.close();
                is = null;
            }
            if (conn != null) {
                conn.close();
                conn = null;
            }
        }
        catch (Exception exception3) {
        }
        CanvasWidth = MapCanvas.CanvasWidth;
        CanvasHeight = MapCanvas.CanvasHeight;
        font = MapCanvas.font;
        fontHeight = 0;
        MapCanvas.VG.reset();
        render(MapCanvas.VG);
        sv.removeAllElements();
        sv = null;
        MapCanvas _tmp1 = JCellBrowser.mc;
        MapCanvas.LoadFlag = false;
    }

    String FE() {
        StringBuffer stringbuffer = new StringBuffer();
        int i = -1;
        do {
            try {
                if ((i = FF.read()) == -1) {
                    break;
                }
                char c = (char) i;
                if (c == '\n') {
                    break;
                }
                if (c != '\r') {
                    stringbuffer.append(c);
                }
                continue;
            }
            catch (Exception exception) {
            }
            break;
        } while (true);
        if (stringbuffer.length() == 0) {
            if (i == -1) {
                return null;
            } else {
                return "";
            }
        } else {
            return stringbuffer.toString();
        }
    }

    String FF(String s) {
        if (s.startsWith("http://")) {
            return s;
        }
        if (s.startsWith("#")) {
            return JCellBrowser.pageurl;
        }
        if (s.startsWith("/") || s.startsWith("\\")) {
            s = s.substring(1);
            return _fld0102 + s;
        } else {
            return _fld0103 + s;
        }
    }

    void _mth0100() {
        String s = "";
        do {
            String s1;
            if ((s1 = FE()) == null) {
                break;
            }
            s1 = s1.trim();
            if (s1.length() != 0) {
                if (s.length() > 0) {
                    s = s + " ";
                }
                do {
                    int i;
                    if ((i = s1.indexOf("<")) != -1) {
                        s = s + s1.substring(0, i);
                        if (s.length() > 0) {
                            sv.addElement(s);
                        }
                        s = "<";
                        s1 = s1.substring(i + 1);
                    }
                    if ((i = s1.indexOf(">")) != -1) {
                        s = s + s1.substring(0, i + 1);
                        if (s.length() > 0) {
                            sv.addElement(s);
                        }
                        s = "";
                        s1 = s1.substring(i + 1);
                    }
                } while (s1.indexOf("<") != -1 || s1.indexOf(">") != -1);
                s = s + s1;
            }
        } while (true);
        if (s.length() > 0) {
            sv.addElement(s);
        }
    }

    Vector _mth0101(String s) {
        Vector vector = new Vector(1);
        String s1 = "";
        String s2;
        for (int i = 1; (s2 = Tools.GetTokenZ(s, i, '"', true)) != null; i++) {
            if (i % 2 == 0) {
                s2 = Tools.RepString(s2, " ", "_");
            }
            s1 = s1 + s2;
        }

        s1 = Tools.RepString(s1, "= ", "=");
        s1 = Tools.RepString(s1, "<", "");
        s1 = Tools.RepString(s1, "/>", ">");
        s1 = Tools.RepString(s1, ">", "");
        String s3;
        for (int j = 1; (s3 = Tools.GetTokenZ(s1, j, ' ', true)) != null; j++) {
            vector.addElement(s3);
        }

        return vector;
    }

    public int strWidth(String s) {
        return font.stringWidth(s);
    }

    int strHeight() {
        if (fontHeight == 0) {
            fontHeight = font.getHeight() - 2;
        }
        return fontHeight;
    }

    void _mth0102() {
        _fld0108.removeAllElements();
    }

    void _mth0103(VirtualGraphics virtualgraphics, String s, int i, int j) {
        if (_fld0109) {
            _fld0109 = false;
            if (s.startsWith(" ")) {
                hs.bx += slen;
            }
        }
        if (!_fld010C) {
            virtualgraphics.drawString(s, i, j, 20);
            if (_fld010A) {
                virtualgraphics.drawString(s, i + 1, j, 20);
            }
            if (_fld010F || _fld0111) {
                int k = j + strHeight();
                int l = i;
                if (_fld0110 && l != 0) {
                    virtualgraphics.drawLine(l + slen, k, l + strWidth(s), k);
                } else {
                    virtualgraphics.drawLine(l, k, l + strWidth(s), k);
                }
                _fld0110 = false;
            }
        } else {
            boolean flag = false;
            boolean flag1 = false;
            if (_fld010A) {
                flag = true;
            }
            if (_fld010F || _fld0111) {
                flag1 = true;
            }
            CenterText centertext = new CenterText(s, flag, flag1);
            _fld0108.addElement(centertext);
            if (_fld010E) {
                String s1 = "";
                for (int i1 = 0; i1 < _fld0108.size(); i1++) {
                    CenterText centertext1 = (CenterText) _fld0108.elementAt(i1);
                    s1 = s1 + centertext1.text;
                }

                int j1 = (CanvasWidth - strWidth(s1)) / 2;
                for (int k1 = 0; k1 < _fld0108.size(); k1++) {
                    CenterText centertext2 = (CenterText) _fld0108.elementAt(k1);
                    virtualgraphics.drawString(centertext2.text, j1, j, 20);
                    if (centertext2.bold) {
                        virtualgraphics.drawString(centertext2.text, j1 + 1, j, 20);
                    }
                    if (centertext2.underline) {
                        if (k1 == 0 && centertext2.text.startsWith(" ")) {
                            virtualgraphics.drawLine(j1 + slen, j + strHeight(), j1 + strWidth(centertext2.text), j + strHeight());
                        } else {
                            virtualgraphics.drawLine(j1, j + strHeight(), j1 + strWidth(centertext2.text), j + strHeight());
                        }
                    }
                    j1 += strWidth(centertext2.text);
                }

                _mth0102();
            }
        }
        _fld010E = false;
    }

    void _mth0104() {
        if (vx + _fld0104 < CanvasWidth) {
            vx += _fld0104;
        } else {
            vx = _fld0112 * _fld0114 + _fld0104;
            vy += strHeight();
        }
    }

    void render(VirtualGraphics virtualgraphics) {
        vx = 0;
        vy = 0;
        slen = strWidth(" ");
        _fld010A = false;
        _fld010B = false;
        _fld010C = false;
        _fld010D = "";
        _fld010E = false;
        _fld010F = false;
        _fld0110 = false;
        _fld0111 = false;
        _fld0112 = 0;
        _fld0109 = false;
        hsindex = 0;
        hsvector.removeAllElements();
        formindex = -1;
        FormObj formobj = null;
        formvector.removeAllElements();
        _fld0106 = 0xffffff;
        _fld0107 = 0;
        lcolor = 255;
        SelectObj selectobj = null;
        OptionObj optionobj = null;
        boolean flag = false;
        String s3 = "";
        TextObj textobj = null;
        boolean flag1 = false;
        String s4 = "";
        virtualgraphics.setBgColor(_fld0106);
        virtualgraphics.setColor(_fld0107);
        Tools.initColorStack();
        Tools.pushColorStack(_fld0107);
        ErrorLine = null;
        label0:
        for (int k = 0; k < sv.size(); k++) {
            String s;
            if (ErrorLine == null) {
                s = (String) sv.elementAt(k);
                sv.setElementAt(null, k);
            } else {
                s = ErrorLine;
                ErrorLine = null;
            }
            s = s.trim();
            if (s.length() == 0) {
                continue;
            }
            String s1 = s.toLowerCase();
            if (s1.endsWith("-->")) {
                _fld010B = false;
                continue;
            }
            if (s1.equals("</head>")) {
                _fld010B = false;
                continue;
            }
            if (s1.equals("</title>")) {
                _fld010B = false;
                continue;
            }
            if (_fld010B) {
                continue;
            }
            if (s1.startsWith("<!--")) {
                _fld010B = true;
                continue;
            }
            if (s1.equals("<head>")) {
                _fld010B = true;
                continue;
            }
            if (s1.equals("<title>")) {
                _fld010B = true;
                continue;
            }
            if (s1.startsWith("<")) {
                if (s1.startsWith("<a")) {
                    if (_fld010C) {
                        _fld010C = false;
                        if (vx != _fld0112 * _fld0114) {
                            _fld010E = true;
                            _mth0103(virtualgraphics, "", vx, vy);
                            vx = _fld0112 * _fld0114;
                            vy += strHeight();
                        }
                    }
                    hs = new HotSpot();
                    hs.setBxBy(vx, vy);
                    _mth0104();
                    _fld0109 = true;
                    Vector vector = _mth0101(s);
                    boolean flag3 = false;
                    for (int l2 = 0; l2 < vector.size(); l2++) {
                        String s18 = (String) vector.elementAt(l2);
                        if (s18.toLowerCase().startsWith("href=")) {
                            flag3 = true;
                            hs.setObj(0, FF(s18.substring(5)), strHeight());
                        }
                    }

                    if (flag3) {
                        virtualgraphics.setColor(lcolor);
                        _fld0111 = true;
                        _fld0110 = true;
                    } else {
                        hs = null;
                        _fld0109 = false;
                    }
                    continue;
                }
                if (s1.equals("</a>")) {
                    _fld0109 = false;
                    _mth0104();
                    if (hs != null) {
                        hs.setExEy(vx, vy);
                        hsvector.addElement(hs);
                    }
                    virtualgraphics.setColor(_fld0107);
                    _fld0111 = false;
                    continue;
                }
                if (s1.equals("<b>")) {
                    _fld010A = true;
                    continue;
                }
                if (s1.equals("</b>")) {
                    _fld010A = false;
                    continue;
                }
                if (s1.startsWith("<body")) {
                    Vector vector1 = _mth0101(s);
                    int i2 = 0;
                    do {
                        if (i2 >= vector1.size()) {
                            continue label0;
                        }
                        String s15 = (String) vector1.elementAt(i2);
                        if (s15.toLowerCase().startsWith("bgcolor=")) {
                            _fld0106 = Tools.GetColor(s15.substring(8));
                            virtualgraphics.setBgColor(_fld0106);
                        }
                        if (s15.toLowerCase().startsWith("text=")) {
                            _fld0107 = Tools.GetColor(s15.substring(5));
                            Tools.initColorStack();
                            Tools.pushColorStack(_fld0107);
                            virtualgraphics.setColor(_fld0107);
                        }
                        if (s15.toLowerCase().startsWith("link=")) {
                            lcolor = Tools.GetColor(s15.substring(5));
                        }
                        if (s15.toLowerCase().startsWith("background=")) {
                            String s19 = FF(s15.substring(11));
                            Image image2 = Tools.LoadHtmlImage(s19);
                            virtualgraphics.setBgImage(image2);
                        }
                        i2++;
                    } while (true);
                }
                if (s1.startsWith("<br")) {
                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                    }
                    vx = _fld0112 * _fld0114;
                    vy += strHeight();
                    continue;
                }
                if (s1.equals("<center>")) {
                    if (vx != _fld0112 * _fld0114) {
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    _fld010C = true;
                    _fld010D = "";
                    _mth0102();
                    continue;
                }
                if (s1.equals("</center>")) {
                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    _fld010C = false;
                    continue;
                }
                if (s1.startsWith("<form")) {
                    String s5 = "get";
                    String s12 = "";
                    Vector vector8 = _mth0101(s);
                    for (int l4 = 0; l4 < vector8.size(); l4++) {
                        String s25 = (String) vector8.elementAt(l4);
                        if (s25.toLowerCase().startsWith("method=")) {
                            s5 = s25.substring(7).toLowerCase();
                        }
                        if (s25.toLowerCase().startsWith("action=")) {
                            s12 = s25.substring(7);
                        }
                    }

                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    formobj = new FormObj(s5, FF(s12));
                    formindex++;
                    continue;
                }
                if (s1.equals("</form>")) {
                    if (formobj != null) {
                        formvector.addElement(formobj);
                    }
                    formobj = null;
                    continue;
                }
                if (s1.startsWith("<font")) {
                    Tools.pushColorStack(_fld0107);
                    Vector vector2 = _mth0101(s);
                    int j2 = 0;
                    do {
                        if (j2 >= vector2.size()) {
                            continue label0;
                        }
                        String s16 = (String) vector2.elementAt(j2);
                        if (s16.toLowerCase().startsWith("color=")) {
                            _fld0107 = Tools.GetColor(s16.substring(6));
                            virtualgraphics.setColor(_fld0107);
                        }
                        j2++;
                    } while (true);
                }
                if (s1.equals("</font>")) {
                    _fld0107 = Tools.popColorStack();
                    virtualgraphics.setColor(_fld0107);
                    continue;
                }
                if (s1.startsWith("<hr")) {
                    int l = 1;
                    Vector vector3 = _mth0101(s);
                    for (int i3 = 0; i3 < vector3.size(); i3++) {
                        String s20 = (String) vector3.elementAt(i3);
                        if (!s20.toLowerCase().startsWith("size=")) {
                            continue;
                        }
                        l = Tools.toInt(s20.substring(5));
                        if (l == 0) {
                            l = 1;
                        }
                    }

                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    vy += _fld0105;
                    virtualgraphics.fillRect(0, vy, CanvasWidth, l);
                    vy += l + _fld0105;
                    continue;
                }
                if (s1.startsWith("<h")) {
                    if (!s1.equals("<h1>") && !s1.equals("<h2>") && !s1.equals("<h3>") && !s1.equals("<h4>") && !s1.equals("<h5>") && !s1.equals("<h6>"))
                    {
                        continue;
                    }
                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    _fld010A = true;
                    continue;
                }
                if (s1.startsWith("</h")) {
                    if (!s1.equals("</h1>") && !s1.equals("</h2>") && !s1.equals("</h3>") && !s1.equals("</h4>") && !s1.equals("</h5>") && !s1.equals("</h6>"))
                    {
                        continue;
                    }
                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    _fld010A = false;
                    continue;
                }
                if (s1.startsWith("<img")) {
                    String s6 = "";
                    Vector vector4 = _mth0101(s);
                    for (int j3 = 0; j3 < vector4.size(); j3++) {
                        String s21 = (String) vector4.elementAt(j3);
                        if (s21.toLowerCase().startsWith("src=")) {
                            s6 = FF(s21.substring(4));
                        }
                    }

                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    GF = new GifFrame();
                    Image image = Tools.LoadHtmlImage(s6);
                    if (image == null) {
                        image = MapCanvas.NoneImage;
                    }
                    vy += _fld0105;
                    if (_fld010C) {
                        virtualgraphics.drawImage(image, CanvasWidth / 2, vy, 17);
                        GF.setXY(CanvasWidth / 2, vy);
                    } else {
                        virtualgraphics.drawImage(image, 0, vy, 20);
                        GF.setXY(0, vy);
                    }
                    if (GF.size() > 1) {
                        VirtualGraphics.GFV.addElement(GF);
                    }
                    GF = null;
                    vy += image.getHeight() + _fld0105;
                    image = null;
                    continue;
                }
                if (s1.startsWith("<mvideo")) {
                    String s7 = "";
                    Vector vector5 = _mth0101(s);
                    for (int k3 = 0; k3 < vector5.size(); k3++) {
                        String s22 = (String) vector5.elementAt(k3);
                        if (s22.toLowerCase().startsWith("src=")) {
                            s7 = FF(s22.substring(4));
                        }
                    }

                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    VF = new VideoFrame();
                    Image image1 = Tools.LoadVideoImage(s7);
                    if (image1 == null) {
                        continue;
                    }
                    vy += _fld0105;
                    if (_fld010C) {
                        virtualgraphics.drawImage(image1, CanvasWidth / 2, vy, 17);
                        VF.setXY(CanvasWidth / 2, vy);
                    } else {
                        virtualgraphics.drawImage(image1, 0, vy, 20);
                        VF.setXY(0, vy);
                    }
                    VirtualGraphics.VFV.addElement(VF);
                    VF = null;
                    vy += image1.getHeight() + _fld0105;
                    image1 = null;
                    continue;
                }
                if (s1.startsWith("<input")) {
                    String s8 = "";
                    String s13 = "";
                    String s17 = "";
                    boolean flag5 = false;
                    Vector vector10 = _mth0101(s);
                    for (int l5 = 0; l5 < vector10.size(); l5++) {
                        String s28 = (String) vector10.elementAt(l5);
                        if (s28.toLowerCase().startsWith("type=")) {
                            s8 = s28.substring(5).toLowerCase();
                        }
                        if (s28.toLowerCase().startsWith("name=")) {
                            s13 = s28.substring(5);
                        }
                        if (s28.toLowerCase().startsWith("value=")) {
                            s17 = Tools.RepString(s28.substring(6), "_", " ");
                        }
                        if (s28.toLowerCase().equals("checked")) {
                            flag5 = true;
                        }
                    }

                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    if (s8.length() == 0) {
                        s8 = "text";
                    }
                    if (s8.equals("hidden") && formobj != null) {
                        formobj.addHiddenObject(new HiddenObj(s13, s17));
                    }
                    if (s8.equals("text") || s8.equals("password")) {
                        boolean flag6 = false;
                        if (s8.equals("password")) {
                            flag6 = true;
                        }
                        vy += _fld0105;
                        hs = new HotSpot();
                        hs.setBxBy(vx, vy);
                        hs.setExEy(vx + 80, vy);
                        TextObj textobj1 = new TextObj(s13, s17, flag6, vx, vy);
                        textobj1.paint(virtualgraphics);
                        hs.setObj(1, textobj1, 22);
                        hsvector.addElement(hs);
                        vy += 22 + _fld0105;
                        if (formobj != null) {
                            formobj.addObjectIndex(hsvector.size() - 1);
                        }
                    }
                    if (s8.equals("submit") || s8.equals("reset")) {
                        vy += _fld0105;
                        ButtonObj buttonobj = new ButtonObj(s8, s17, formindex, vx, vy);
                        buttonobj.paint(virtualgraphics);
                        hs = new HotSpot();
                        hs.setBxBy(vx, vy);
                        hs.setExEy(vx + buttonobj.getWidth(), vy);
                        hs.setObj(3, buttonobj, 17);
                        hsvector.addElement(hs);
                        vy += 16 + _fld0105;
                    }
                    if (!s8.equals("radio") && !s8.equals("checkbox")) {
                        continue;
                    }
                    byte byte0 = 1;
                    if (s8.equals("checkbox")) {
                        byte0 = 2;
                    }
                    ChoiceObj choiceobj = new ChoiceObj(byte0, s13, s17, flag5, vx, vy);
                    choiceobj.paint(virtualgraphics);
                    hs = new HotSpot();
                    hs.setBxBy(vx, vy);
                    vx += 17;
                    hs.setExEy(vx, vy);
                    hs.setObj(4, choiceobj, 13);
                    hsvector.addElement(hs);
                    if (formobj != null) {
                        formobj.addObjectIndex(hsvector.size() - 1);
                    }
                    continue;
                }
                if (s1.startsWith("<select")) {
                    String s9 = "";
                    Vector vector6 = _mth0101(s);
                    for (int l3 = 0; l3 < vector6.size(); l3++) {
                        String s23 = (String) vector6.elementAt(l3);
                        if (s23.toLowerCase().startsWith("name=")) {
                            s9 = s23.substring(5);
                        }
                    }

                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    vy += _fld0105;
                    selectobj = new SelectObj(s9, vx, vy);
                    s3 = "";
                    continue;
                }
                if (s1.startsWith("<option")) {
                    String s10 = "";
                    boolean flag4 = false;
                    Vector vector9 = _mth0101(s);
                    for (int i5 = 0; i5 < vector9.size(); i5++) {
                        String s26 = (String) vector9.elementAt(i5);
                        if (s26.toLowerCase().startsWith("value=")) {
                            s10 = Tools.RepString(s26.substring(6), "_", " ");
                        }
                        if (s26.toLowerCase().startsWith("selected")) {
                            flag4 = true;
                        }
                    }

                    flag = true;
                    if (s3.length() > 0 && optionobj != null) {
                        optionobj.setValue(s3);
                        if (selectobj != null) {
                            selectobj.addOption(optionobj);
                        }
                        s3 = "";
                    }
                    optionobj = new OptionObj(s10, flag4);
                    continue;
                }
                if (s1.equals("</select>")) {
                    flag = false;
                    hs = new HotSpot();
                    hs.setBxBy(vx, vy);
                    hs.setExEy(vx + 80, vy);
                    hs.setObj(2, selectobj, 22);
                    hsvector.addElement(hs);
                    if (s3.length() > 0 && optionobj != null) {
                        optionobj.setValue(s3);
                        if (selectobj != null) {
                            selectobj.addOption(optionobj);
                            selectobj.paint(virtualgraphics);
                        }
                        s3 = "";
                    }
                    vy += 22 + _fld0105;
                    if (formobj != null) {
                        formobj.addObjectIndex(hsvector.size() - 1);
                    }
                    continue;
                }
                if (s1.startsWith("<textarea")) {
                    String s11 = "";
                    Vector vector7 = _mth0101(s);
                    for (int i4 = 0; i4 < vector7.size(); i4++) {
                        String s24 = (String) vector7.elementAt(i4);
                        if (s24.toLowerCase().startsWith("name=")) {
                            s11 = s24.substring(5);
                        }
                    }

                    if (vx != _fld0112 * _fld0114) {
                        _fld010E = true;
                        _mth0103(virtualgraphics, "", vx, vy);
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    vy += _fld0105;
                    textobj = new TextObj(s11, "", false, vx, vy);
                    flag1 = true;
                    s4 = "";
                    continue;
                }
                if (s1.equals("</textarea>")) {
                    flag1 = false;
                    textobj.setValue(s4);
                    textobj.paint(virtualgraphics);
                    s4 = "";
                    hs = new HotSpot();
                    hs.setBxBy(vx, vy);
                    hs.setExEy(vx + 80, vy);
                    hs.setObj(1, textobj, 22);
                    hsvector.addElement(hs);
                    vy += 22 + _fld0105;
                    if (formobj != null) {
                        formobj.addObjectIndex(hsvector.size() - 1);
                    }
                    continue;
                }
                if (s1.equals("<u>")) {
                    _fld010F = true;
                    _fld0110 = true;
                    continue;
                }
                if (s1.equals("</u>")) {
                    _fld010F = false;
                    _fld0110 = false;
                    continue;
                }
                if (s1.equals("<li>") && _fld0112 > 0) {
                    if (vx != _fld0112 * _fld0114) {
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                    }
                    if (_fld0113[_fld0112] >= 0) {
                        _fld0113[_fld0112]++;
                        virtualgraphics.drawString("" + _fld0113[_fld0112] + ". ", _fld0112 * _fld0114, vy, 24);
                        continue;
                    }
                    if (_fld0112 == 1) {
                        virtualgraphics.fillRoundRect(_fld0112 * _fld0114 - 10, vy + 5, 5, 5, 5, 5);
                    } else {
                        virtualgraphics.fillRect(_fld0112 * _fld0114 - 10, vy + 6, 4, 4);
                    }
                    continue;
                }
                if (s1.equals("<ul>") || s1.equals("<dir>")) {
                    int i1 = _fld0112;
                    if (_fld0112 < 7) {
                        _fld0112++;
                        _fld0113[_fld0112] = -1;
                    }
                    if (vx != i1 * _fld0114) {
                        if (_fld010C) {
                            _fld010E = true;
                            _mth0103(virtualgraphics, "", vx, vy);
                        }
                        vy += strHeight();
                    }
                    vx = _fld0112 * _fld0114;
                    _fld010C = false;
                    continue;
                }
                if (s1.equals("</ul>") || s1.equals("</dir>")) {
                    if (_fld0112 > 0) {
                        _fld0112--;
                    }
                    vx = _fld0112 * _fld0114;
                    vy += strHeight();
                    continue;
                }
                if (s1.equals("<ol>")) {
                    int j1 = _fld0112;
                    if (_fld0112 < 7) {
                        _fld0112++;
                        _fld0113[_fld0112] = 0;
                    }
                    if (vx != j1 * _fld0114) {
                        if (_fld010C) {
                            _fld010E = true;
                            _mth0103(virtualgraphics, "", vx, vy);
                        }
                        vy += strHeight();
                    }
                    vx = _fld0112 * _fld0114;
                    _fld010C = false;
                    continue;
                }
                if (!s1.equals("</ol>")) {
                    continue;
                }
                if (_fld0112 > 0) {
                    _fld0112--;
                }
                vx = _fld0112 * _fld0114;
                vy += strHeight();
                continue;
            }
            if (s.indexOf('&') >= 0) {
                boolean flag2 = false;
                do {
                    int k1;
                    if ((k1 = s.indexOf("&#")) == -1) {
                        break;
                    }
                    int k2 = k1;
                    int j4 = s.length();
                    int j5 = k1;
                    do {
                        if (j5 >= j4) {
                            break;
                        }
                        if (s.charAt(j5) == ';') {
                            k2 = j5;
                            break;
                        }
                        j5++;
                    } while (true);
                    if (k2 == k1) {
                        break;
                    }
                    s = s.substring(0, k1) + "*" + s.substring(k2 + 1);
                } while (true);
                s = Tools.RepString(s, "&copy;", "(c)");
                s = Tools.RepString(s, "&amp;", "&");
                s = Tools.RepString(s, "&lt;", "<");
                s = Tools.RepString(s, "&gt;", ">");
                s = Tools.RepString(s, "&nbsp;", " ");
            }
            if (flag) {
                if (s3.length() > 0) {
                    s3 = s3 + " ";
                }
                s3 = s3 + s;
                continue;
            }
            if (flag1) {
                if (s4.length() > 0) {
                    s4 = s4 + " ";
                }
                s4 = s4 + s;
                continue;
            }
            String s2;
            for (int j = 1; (s2 = Tools.GetTokenZ(s, j, ' ', true)) != null; j++) {
                int i = strWidth(s2);
                if (vx + slen + i < CanvasWidth) {
                    if (vx != _fld0112 * _fld0114) {
                        s2 = " " + s2;
                        i = slen + i;
                    }
                    _mth0103(virtualgraphics, s2, vx, vy);
                    vx += i;
                    continue;
                }
                int l1 = s2.length();
                do {
                    if (l1 <= 0) {
                        break;
                    }
                    l1--;
                    String s14 = s2.substring(0, l1);
                    if (vx != _fld0112 * _fld0114) {
                        s14 = " " + s14;
                    }
                    int k4 = strWidth(s14);
                    if (vx + k4 >= CanvasWidth) {
                        continue;
                    }
                    _fld010E = true;
                    _mth0103(virtualgraphics, s14, vx, vy);
                    s2 = s2.substring(l1);
                    vx = _fld0112 * _fld0114;
                    label1:
                    do {
                        if ((i = strWidth(s2)) <= CanvasWidth) {
                            break;
                        }
                        int k5 = s2.length();
                        String s27;
                        int i6;
                        do {
                            if (k5 <= 0) {
                                continue label1;
                            }
                            k5--;
                            s27 = s2.substring(0, k5);
                            i6 = strWidth(s27);
                        } while (vx + i6 > CanvasWidth);
                        _fld010E = true;
                        vx = _fld0112 * _fld0114;
                        vy += strHeight();
                        _mth0103(virtualgraphics, s27, vx, vy);
                        s2 = s2.substring(k5);
                    } while (true);
                    break;
                } while (true);
                if (_fld010C) {
                    _fld010E = true;
                    _mth0103(virtualgraphics, "", vx, vy);
                }
                vx = _fld0112 * _fld0114;
                vy += strHeight();
                _mth0103(virtualgraphics, s2, vx, vy);
                vx += i;
            }

        }

        vy += strHeight();
    }

}
