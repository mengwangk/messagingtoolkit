package com.lightspeedleader.browser;

import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;
import java.util.Vector;

public class VirtualGraphics {

    int _fld0132;
    public static int bgcolor;
    int _fld0135;
    int _fld0136;
    Image _fld0137;
    Image _fld0138;
    Graphics BG;
    public static Vector VGCV = new Vector(1);
    public static Vector GFV = new Vector(1);
    public static Vector VFV = new Vector(1);

    public VirtualGraphics(int i, int j) {
        _fld0135 = i;
        _fld0136 = j;
        try {
            _fld0138 = Image.createImage(i, j);
            BG = _fld0138.getGraphics();
        }
        catch (Exception exception) {
        }
        reset();
    }

    public void reset() {
        _fld0132 = 0;
        bgcolor = 0xffffff;
        _fld0137 = null;
        VGCV.removeAllElements();
        GFV.removeAllElements();
        for (int i = 0; i < VFV.size(); i++) {
            VideoFrame videoframe = (VideoFrame) VFV.elementAt(i);
            videoframe.close();
        }

        VFV.removeAllElements();
    }

    public void setBgColor(int i) {
        bgcolor = i;
    }

    public void setBgImage(Image image) {
        _fld0137 = image;
    }

    public int getColor() {
        return _fld0132;
    }

    public void setColor(int i) {
        _fld0132 = i;
    }

    public void drawString(String s, int i, int j, int k) {
        if (s == null || s.length() == 0) {
            return;
        } else {
            VGCommand vgcommand = new VGCommand();
            vgcommand.type = 0;
            vgcommand._fld0130 = j;
            vgcommand._fld0131 = j + MapCanvas.fontHeight;
            vgcommand._fld0132 = _fld0132;
            vgcommand._fld0133 = k;
            vgcommand.x1 = i;
            vgcommand.y1 = j;
            vgcommand._fld0134 = s;
            VGCV.addElement(vgcommand);
            return;
        }
    }

    public void drawLine(int i, int j, int k, int l) {
        VGCommand vgcommand = new VGCommand();
        vgcommand.type = 1;
        vgcommand._fld0130 = j;
        vgcommand._fld0131 = l;
        vgcommand._fld0132 = _fld0132;
        vgcommand.x1 = i;
        vgcommand.y1 = j;
        vgcommand.x2 = k;
        vgcommand.y2 = l;
        VGCV.addElement(vgcommand);
    }

    public void drawImage(Image image, int i, int j, int k) {
        VGCommand vgcommand = new VGCommand();
        vgcommand.type = 2;
        vgcommand._fld0130 = j;
        vgcommand._fld0131 = j + image.getHeight();
        vgcommand._fld0133 = k;
        vgcommand._fld0132 = 0;
        vgcommand.x1 = i;
        vgcommand.y1 = j;
        vgcommand.img = image;
        VGCV.addElement(vgcommand);
    }

    public void drawRect(int i, int j, int k, int l) {
        VGCommand vgcommand = new VGCommand();
        vgcommand.type = 3;
        vgcommand._fld0130 = j;
        vgcommand._fld0131 = j + l;
        vgcommand._fld0132 = _fld0132;
        vgcommand.x1 = i;
        vgcommand.y1 = j;
        vgcommand.x2 = k;
        vgcommand.y2 = l;
        VGCV.addElement(vgcommand);
    }

    public void drawRoundRect(int i, int j, int k, int l, int i1, int j1) {
        VGCommand vgcommand = new VGCommand();
        vgcommand.type = 4;
        vgcommand._fld0130 = j;
        vgcommand._fld0131 = j + l;
        vgcommand._fld0132 = _fld0132;
        vgcommand.x1 = i;
        vgcommand.y1 = j;
        vgcommand.x2 = k;
        vgcommand.y2 = l;
        vgcommand.x3 = i1;
        vgcommand.y3 = j1;
        VGCV.addElement(vgcommand);
    }

    public void fillRect(int i, int j, int k, int l) {
        VGCommand vgcommand = new VGCommand();
        vgcommand.type = 5;
        vgcommand._fld0130 = j;
        vgcommand._fld0131 = j + l;
        vgcommand._fld0132 = _fld0132;
        vgcommand.x1 = i;
        vgcommand.y1 = j;
        vgcommand.x2 = k;
        vgcommand.y2 = l;
        VGCV.addElement(vgcommand);
    }

    public void fillRoundRect(int i, int j, int k, int l, int i1, int j1) {
        VGCommand vgcommand = new VGCommand();
        vgcommand.type = 6;
        vgcommand._fld0130 = j;
        vgcommand._fld0131 = j + l;
        vgcommand._fld0132 = _fld0132;
        vgcommand.x1 = i;
        vgcommand.y1 = j;
        vgcommand.x2 = k;
        vgcommand.y2 = l;
        vgcommand.x3 = i1;
        vgcommand.y3 = j1;
        VGCV.addElement(vgcommand);
    }

    public void render(Graphics g, int i) {
        int j = i + _fld0136;
        if (_fld0137 == null) {
            BG.setColor(bgcolor);
            BG.fillRect(0, 0, _fld0135, _fld0136);
        } else {
            int k = _fld0137.getWidth();
            int i1 = _fld0137.getHeight();
            int k1 = i % i1;
            for (int l1 = 0; l1 < _fld0135; l1 += k) {
                for (int i2 = -k1; i2 < _fld0136; i2 += i1) {
                    BG.drawImage(_fld0137, l1, i2, 20);
                }

            }

        }
        int l = VGCV.size();
        for (int j1 = 0; j1 < l; j1++) {
            VGCommand vgcommand = (VGCommand) VGCV.elementAt(j1);
            if (vgcommand._fld0131 >= i && vgcommand._fld0130 <= j) {
                BG.setColor(vgcommand._fld0132);
                switch (vgcommand.type) {
                    case 0: // '\0'
                        BG.drawString(vgcommand._fld0134, vgcommand.x1, vgcommand.y1 - i, vgcommand._fld0133);
                        break;

                    case 1: // '\001'
                        BG.drawLine(vgcommand.x1, vgcommand.y1 - i, vgcommand.x2, vgcommand.y2 - i);
                        break;

                    case 2: // '\002'
                        BG.drawImage(vgcommand.img, vgcommand.x1, vgcommand.y1 - i, vgcommand._fld0133);
                        break;

                    case 3: // '\003'
                        BG.drawRect(vgcommand.x1, vgcommand.y1 - i, vgcommand.x2, vgcommand.y2);
                        break;

                    case 4: // '\004'
                        BG.drawRoundRect(vgcommand.x1, vgcommand.y1 - i, vgcommand.x2, vgcommand.y2, vgcommand.x3, vgcommand.y3);
                        break;

                    case 5: // '\005'
                        BG.fillRect(vgcommand.x1, vgcommand.y1 - i, vgcommand.x2, vgcommand.y2);
                        break;

                    case 6: // '\006'
                        BG.fillRoundRect(vgcommand.x1, vgcommand.y1 - i, vgcommand.x2, vgcommand.y2, vgcommand.x3, vgcommand.y3);
                        break;
                }
            }
        }

        g.drawImage(_fld0138, 0, 0, 20);
    }

}
