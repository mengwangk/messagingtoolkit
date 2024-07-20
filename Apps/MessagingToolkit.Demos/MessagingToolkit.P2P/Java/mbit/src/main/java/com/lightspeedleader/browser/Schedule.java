package com.lightspeedleader.browser;

import javax.microedition.lcdui.Image;

public class Schedule extends Thread {

    public static int speed = 150;

    Schedule() {
    }

    public void sleep(int i) {
        try {
            Thread.sleep(i);
        }
        catch (Exception exception) {
        }
    }

    public void run() {
        do {
            int k;
            do {
                do {
                    do {
                        do {
                            sleep(speed);
                            MapCanvas _tmp = JCellBrowser.mc;
                        } while (MapCanvas.mode == 0);
                        MapCanvas _tmp1 = JCellBrowser.mc;
                    } while (MapCanvas.still);
                    MapCanvas _tmp2 = JCellBrowser.mc;
                } while (MapCanvas.LoadFlag);
                int i = VirtualGraphics.GFV.size();
                if (i > 0) {
                    label0:
                    for (int j = 0; j < i; j++) {
                        GifFrame gifframe = (GifFrame) VirtualGraphics.GFV.elementAt(j);
                        int i1 = gifframe.x;
                        int j1 = gifframe.y;
                        int l1 = VirtualGraphics.VGCV.size();
                        int j2 = 0;
                        do {
                            if (j2 >= l1) {
                                continue label0;
                            }
                            VGCommand vgcommand = (VGCommand) VirtualGraphics.VGCV.elementAt(j2);
                            if (vgcommand.type == 2 && vgcommand.x1 == i1 && vgcommand.y1 == j1) {
                                gifframe.next();
                                vgcommand.img = gifframe.getImage();
                                continue label0;
                            }
                            j2++;
                        } while (true);
                    }

                    JCellBrowser.mc.repaint();
                    JCellBrowser.mc.serviceRepaints();
                }
                k = VirtualGraphics.VFV.size();
            } while (k <= 0);
            int l = 0;
            while (l < k) {
                VideoFrame videoframe = (VideoFrame) VirtualGraphics.VFV.elementAt(l);
                int k1 = videoframe.x;
                int i2 = videoframe.y;
                int k2 = VirtualGraphics.VGCV.size();
                int l2 = 0;
                do {
                    if (l2 >= k2) {
                        break;
                    }
                    VGCommand vgcommand1 = (VGCommand) VirtualGraphics.VGCV.elementAt(l2);
                    if (vgcommand1.type == 2 && vgcommand1.x1 == k1 && vgcommand1.y1 == i2) {
                        Image image = videoframe.getImage();
                        if (image != null) {
                            vgcommand1.img = image;
                            JCellBrowser.mc.repaint();
                            JCellBrowser.mc.serviceRepaints();
                        } else {
                            videoframe.close();
                        }
                        break;
                    }
                    l2++;
                } while (true);
                l++;
            }
        } while (true);
    }

}
