package com.lightspeedleader.browser;

import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;

public class MapCanvas extends Canvas {

    public static int mode = 0;
    public static boolean still = false;
    public static int CanvasWidth;
    public static int CanvasHeight;
    public static VirtualGraphics VG;
    public static Image LogoImage;
    public static Image NoneImage;
    public static Image TextImage;
    public static Image SelectImage;
    public static Image RAImage;
    public static Image RCImage;
    public static Image CBImage;
    public static Image CCImage;
    public static boolean LoadFlag = false;
    public static Font font = null;
    public static int fontHeight = 0;
    public static String title1 = "Version 1.0";
    public static String title2 = "Open Source Software";
    public static int BaseY = 0;
    public static boolean delayload = false;

    public MapCanvas() {
    }

    public void init() {
        CanvasWidth = getWidth();
        CanvasHeight = getHeight();
        VG = new VirtualGraphics(CanvasWidth, CanvasHeight);
        try {
            Image image = Image.createImage("/JCellBrowser.png");
            LogoImage = Image.createImage(142, 31);
            Graphics g = LogoImage.getGraphics();
            g.drawImage(image, 0, 0, 20);
            NoneImage = Image.createImage(32, 32);
            g = NoneImage.getGraphics();
            g.drawImage(image, -80, 0, 20);
            SelectImage = Image.createImage(80, 22);
            g = SelectImage.getGraphics();
            g.drawImage(image, -112, 0, 20);
            TextImage = Image.createImage(80, 22);
            g = TextImage.getGraphics();
            g.drawImage(image, -192, 0, 20);
            RAImage = Image.createImage(17, 13);
            g = RAImage.getGraphics();
            g.drawImage(image, -272, 0, 20);
            RCImage = Image.createImage(17, 13);
            g = RCImage.getGraphics();
            g.drawImage(image, -272, -13, 20);
            CBImage = Image.createImage(17, 13);
            g = CBImage.getGraphics();
            g.drawImage(image, -289, 0, 20);
            CCImage = Image.createImage(17, 13);
            g = CCImage.getGraphics();
            g.drawImage(image, -289, -13, 20);
            g = null;
            image = null;
        }
        catch (Exception exception) {
        }
        (new Schedule()).start();
    }

    public static int strWidth(String s) {
        return font.stringWidth(s);
    }

    public static int strHeight() {
        if (fontHeight == 0) {
            fontHeight = font.getHeight() - 2;
        }
        return fontHeight;
    }

    public void showStringC(Graphics g, String s, int i, int j) {
        g.drawString(s, i, j, 17);
    }

    public void showBoldStringC(Graphics g, String s, int i, int j) {
        g.drawString(s, i, j, 17);
        g.drawString(s, i + 1, j, 17);
    }

    public void showStringL(Graphics g, String s, int i, int j) {
        g.drawString(s, i, j, 20);
    }

    public void showBoldStringL(Graphics g, String s, int i, int j) {
        g.drawString(s, i, j, 20);
        g.drawString(s, i + 1, j, 20);
    }

    public void paint(Graphics g) {
        if (font == null) {
            font = g.getFont();
        }
        if (delayload) {
            delayload = false;
            HTMLStream htmlstream = new HTMLStream(JCellBrowser.pageurl);
            htmlstream = null;
            repaint();
            serviceRepaints();
            return;
        }
        if (LoadFlag) {
            String s = " loading ";
            int j = strWidth(s);
            int i1 = (CanvasWidth - j) / 2;
            int k1 = (CanvasHeight - font.getHeight()) / 2;
            g.setColor(0x808080);
            g.fillRect(i1, k1 - 2, j, font.getHeight() + 4);
            g.setColor(0);
            g.drawRect(i1, k1 - 2, j, font.getHeight() + 4);
            showBoldStringC(g, s, CanvasWidth / 2, k1);
            return;
        }
        if (mode == 0) {
            g.setColor(0xffffff);
            g.fillRect(0, 0, CanvasWidth, CanvasHeight);
            g.setColor(0xcc0066);
            int i = CanvasWidth / 2;
            g.drawImage(LogoImage, i, 10, 17);
            int k = 40;
            showBoldStringC(g, title1, i, k);
            k += strHeight();
            showBoldStringC(g, title2, i, k);
            return;
        }
        g.setClip(0, 0, CanvasWidth, CanvasHeight);
        VG.render(g, BaseY);
        if (HTMLStream.hsvector.size() > 0) {
            HotSpot hotspot = (HotSpot) HTMLStream.hsvector.elementAt(HTMLStream.hsindex);
            g.setColor(HTMLStream.lcolor);
            int l = hotspot.bx;
            int j1 = (hotspot.by + 1) - BaseY;
            int l1 = hotspot.ex - 1;
            int i2 = (hotspot.ey + 1) - BaseY;
            int j2 = hotspot.height;
            g.drawLine(l, j1, l, j1 + j2);
            g.drawLine(l, j1, l + 1, j1);
            g.drawLine(l, j1 + j2, l + 1, j1 + j2);
            g.drawLine(l1, i2, l1, i2 + j2);
            g.drawLine(l1, i2, l1 - 1, i2);
            g.drawLine(l1, i2 + j2, l1 - 1, i2 + j2);
        }
    }

    public void keyProc(int i) {
        if (mode == 0) {
            return;
        }
        i = getGameAction(i);
        if (i == 1) {
            if (BaseY >= fontHeight) {
                BaseY -= fontHeight;
            } else {
                BaseY = 0;
            }
        }
        if (i == 6) {
            if (BaseY + CanvasHeight + fontHeight <= HTMLStream.vy) {
                BaseY += fontHeight;
            } else if (HTMLStream.vy > CanvasHeight) {
                BaseY = HTMLStream.vy - CanvasHeight;
            }
        }
        if (HTMLStream.hsvector.size() > 0) {
            if (i == 2 && HTMLStream.hsindex > 0) {
                HTMLStream.hsindex--;
            }
            if (i == 5 && HTMLStream.hsindex + 1 < HTMLStream.hsvector.size()) {
                HTMLStream.hsindex++;
            }
            if (i == 2 || i == 5) {
                HotSpot hotspot = (HotSpot) HTMLStream.hsvector.elementAt(HTMLStream.hsindex);
                if (hotspot.by < BaseY || hotspot.by > BaseY + CanvasHeight) {
                    BaseY = hotspot.by;
                }
                if (BaseY + CanvasHeight > HTMLStream.vy) {
                    BaseY = HTMLStream.vy - CanvasHeight;
                    if (BaseY < 0) {
                        BaseY = 0;
                    }
                }
            }
        }
        repaint();
    }

    public void keyPressed(int i) {
        keyProc(i);
    }

    public void keyRepeated(int i) {
        keyProc(i);
    }

    public void pointerPressed(int i, int j) {
        int k = HTMLStream.hsvector.size();
        for (int l = 0; l < k; l++) {
            HotSpot hotspot = (HotSpot) HTMLStream.hsvector.elementAt(l);
            int i1 = hotspot.bx;
            int j1 = hotspot.by - BaseY;
            int k1 = hotspot.ex;
            int l1 = (hotspot.ey + hotspot.height) - BaseY;
            if (i >= i1 && i <= k1 && j >= j1 && j <= l1) {
                HTMLStream.hsindex = l;
                repaint();
                return;
            }
        }

        if (j < CanvasHeight / 2) {
            if (BaseY >= fontHeight) {
                BaseY -= fontHeight;
            } else {
                BaseY = 0;
            }
        } else if (BaseY + CanvasHeight + fontHeight <= HTMLStream.vy) {
            BaseY += fontHeight;
        } else if (HTMLStream.vy > CanvasHeight) {
            BaseY = HTMLStream.vy - CanvasHeight;
        }
        repaint();
    }

}
