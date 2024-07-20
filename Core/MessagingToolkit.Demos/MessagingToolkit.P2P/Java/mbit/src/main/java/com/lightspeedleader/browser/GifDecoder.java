package com.lightspeedleader.browser;

import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;
import java.io.InputStream;

public class GifDecoder {

    private int E0;
    private int E1[];
    private int E2;
    private int E3;
    private int E4;
    private int E5;
    private int E6;
    private boolean E7;
    private int E8[];
    private int E9;
    private int EA;
    private int EB;
    private int EC;
    private int ED;
    private boolean EE;
    private boolean EF;
    private int F0[];
    private int F1;
    private boolean F2;
    private int F3;
    private long F4;
    private int F5;
    private static final int F6[] = {
            8, 8, 4, 2
    };
    private static final int F8[] = {
            0, 4, 2, 1
    };
    int F9;
    int poolsize;
    int FA;
    byte C2[];
    int FB;
    int FC;
    int FD;

    public GifDecoder(byte abyte0[]) {
        E0 = -1;
        E1 = new int[280];
        E2 = -1;
        E3 = 0;
        E4 = 0;
        E5 = 0;
        E6 = 0;
        E7 = false;
        E8 = null;
        E9 = 0;
        EA = 0;
        EB = 0;
        EC = 0;
        ED = 0;
        EE = false;
        EF = false;
        F0 = null;
        F1 = 0;
        F5 = 0;
        F9 = 0;
        C2 = abyte0;
        poolsize = C2.length;
        FA = 0;
    }

    public GifDecoder(InputStream inputstream) {
        E0 = -1;
        E1 = new int[280];
        E2 = -1;
        E3 = 0;
        E4 = 0;
        E5 = 0;
        E6 = 0;
        E7 = false;
        E8 = null;
        E9 = 0;
        EA = 0;
        EB = 0;
        EC = 0;
        ED = 0;
        EE = false;
        EF = false;
        F0 = null;
        F1 = 0;
        F5 = 0;
        F9 = 0;
        byte abyte0[][] = new byte[32][1];
        int i = 0;
        int j = 0;
        do {
            abyte0[i] = new byte[1024];
            try {
                j = inputstream.read(abyte0[i]);
            }
            catch (Exception exception) {
                j = 0;
            }
            poolsize += j;
            if (j != 1024) {
                break;
            }
            i++;
        } while (true);
        C2 = new byte[poolsize];
        FA = 0;
        int k = 0;
        boolean flag = false;
        for (int i1 = 0; i1 <= i; i1++) {
            int l;
            if (i1 == i) {
                l = j;
            } else {
                l = 1024;
            }
            System.arraycopy(abyte0[i1], 0, C2, k, l);
            k += l;
        }

    }

    public boolean moreFrames() {
        return poolsize - FA >= 16;
    }

    public void nextFrame() {
        F9++;
    }

    public Image decodeImage() {
        return decodeImage(F9);
    }

    public Image decodeImage(int i) {
        Object obj = null;
        if (i <= E0) {
            return null;
        }
        if (E0 < 0) {
            if (!E3()) {
                return null;
            }
            if (!E4()) {
                return null;
            }
        }
        do {
            if (!E9(1)) {
                return null;
            }
            int j = E1[0];
            if (j == 59) {
                return null;
            }
            if (j == 33) {
                if (!E7()) {
                    return null;
                }
            } else if (j == 44) {
                if (!E5()) {
                    return null;
                }
                Image image = E0();
                E0++;
                if (E0 < i) {
                    image = null;
                } else {
                    return image;
                }
            }
        } while (true);
    }

    public void clear() {
        C2 = null;
        E1 = null;
        E8 = null;
        F0 = null;
    }

    private Image E0() {
        int i = EB;
        int j = EC;
        boolean flag = false;
        int j1 = 0;
        int k1 = 0;
        int ai[] = new int[4096];
        int ai1[] = new int[4096];
        int ai2[] = new int[8192];
        if (!E9(1)) {
            return null;
        }
        int k = E1[0];
        Image image = Image.createImage(i, j);
        Graphics g = image.getGraphics();
        int ai3[] = E8;
        if (EE) {
            ai3 = F0;
        }
        if (E2 >= 0) {
            ai3[E2] = VirtualGraphics.bgcolor;
        }
        int l2 = 1 << k;
        int j3 = l2 + 1;
        int k2 = k + 1;
        int l3 = l2 + 2;
        int k3 = -1;
        int j4 = -1;
        for (int l1 = 0; l1 < l2; l1++) {
            ai1[l1] = l1;
        }

        int j2 = 0;
        E2();
        j1 = 0;
        label0:
        for (int i2 = 0; i2 < j; i2++) {
            int i1 = 0;
            do {
                if (i1 >= i) {
                    break;
                }
                if (j2 == 0) {
                    int i4 = E1(k2);
                    if (i4 < 0) {
                        return image;
                    }
                    if (i4 > l3 || i4 == j3) {
                        return image;
                    }
                    if (i4 == l2) {
                        k2 = k + 1;
                        l3 = l2 + 2;
                        k3 = -1;
                        continue;
                    }
                    if (k3 == -1) {
                        ai2[j2++] = ai1[i4];
                        k3 = i4;
                        j4 = i4;
                        continue;
                    }
                    int i3 = i4;
                    if (i4 == l3) {
                        ai2[j2++] = j4;
                        i4 = k3;
                    }
                    for (; i4 > l2; i4 = ai[i4]) {
                        ai2[j2++] = ai1[i4];
                    }

                    j4 = ai1[i4];
                    if (l3 >= 4096) {
                        return image;
                    }
                    ai2[j2++] = j4;
                    ai[l3] = k3;
                    ai1[l3] = j4;
                    if (++l3 >= 1 << k2 && l3 < 4096) {
                        k2++;
                    }
                    k3 = i3;
                }
                int l = ai2[--j2];
                if (l < 0) {
                    return image;
                }
                if (i1 == 0) {
                    FC = 0;
                    FB = ai3[l];
                    FD = 0;
                } else if (FB != ai3[l]) {
                    g.setColor(FB);
                    g.drawLine(FD, j1, FD + FC, j1);
                    FC = 0;
                    FB = ai3[l];
                    FD = i1;
                    if (i1 == i - 1) {
                        g.setColor(ai3[l]);
                        g.drawLine(i1, j1, i1, j1);
                    }
                } else {
                    FC++;
                    if (i1 == i - 1) {
                        g.setColor(FB);
                        g.drawLine(FD, j1, FD + FC, j1);
                    }
                }
                i1++;
            } while (true);
            if (EF) {
                j1 += F6[k1];
                do {
                    if (j1 < j) {
                        continue label0;
                    }
                    if (++k1 > 3) {
                        return image;
                    }
                    j1 = F8[k1];
                } while (true);
            }
            j1++;
        }

        return image;
    }

    private int E1(int i) {
        while (F5 < i) {
            if (F2) {
                return -1;
            }
            if (F1 == 0) {
                F1 = E8();
                F3 = 0;
                if (F1 <= 0) {
                    F2 = true;
                    break;
                }
            }
            F4 += E1[F3] << F5;
            F3++;
            F5 += 8;
            F1--;
        }
        int j = (int) F4 & (1 << i) - 1;
        F4 >>= i;
        F5 -= i;
        return j;
    }

    private void E2() {
        F5 = 0;
        F1 = 0;
        F4 = 0L;
        F2 = false;
        F3 = -1;
    }

    private boolean E3() {
        if (!E9(6)) {
            return false;
        }
        return E1[0] == 71 && E1[1] == 73 && E1[2] == 70 && E1[3] == 56 && (E1[4] == 55 || E1[4] == 57) && E1[5] == 97;
    }

    private boolean E4() {
        if (!E9(7)) {
            return false;
        }
        E3 = EA(E1[0], E1[1]);
        E4 = EA(E1[2], E1[3]);
        int i = E1[4];
        E5 = E1[5];
        int j = E1[6];
        E6 = 2 << (i & 7);
        E7 = EB(i, 128);
        E8 = null;
        return !E7 || E6(E6, true);
    }

    private boolean E5() {
        if (!E9(9)) {
            return false;
        }
        E9 = EA(E1[0], E1[1]);
        EA = EA(E1[2], E1[3]);
        EB = EA(E1[4], E1[5]);
        EC = EA(E1[6], E1[7]);
        int i = E1[8];
        EE = EB(i, 128);
        ED = 2 << (i & 7);
        EF = EB(i, 64);
        F0 = null;
        return !EE || E6(ED, false);
    }

    private boolean E6(int i, boolean flag) {
        int ai[] = new int[i];
        for (int j = 0; j < i; j++) {
            if (!E9(3)) {
                return false;
            }
            ai[j] = E1[0] << 16 | E1[1] << 8 | E1[2] | 0xff000000;
        }

        if (flag) {
            E8 = ai;
        } else {
            F0 = ai;
        }
        return true;
    }

    private boolean E7() {
        if (!E9(1)) {
            return false;
        }
        int i = E1[0];
        int j = -1;
        switch (i) {
            case 249:
                j = E8();
                if (j < 0) {
                    return true;
                }
                if ((E1[0] & 1) != 0) {
                    E2 = E1[3];
                } else {
                    E2 = -1;
                }
                break;
        }
        do {
            j = E8();
        } while (j > 0);
        return true;
    }

    private int E8() {
        if (!E9(1)) {
            return -1;
        }
        int i = E1[0];
        if (i != 0 && !E9(i)) {
            return -1;
        } else {
            return i;
        }
    }

    private boolean E9(int i) {
        if (FA + i >= poolsize) {
            return false;
        }
        for (int j = 0; j < i; j++) {
            int k = C2[FA + j];
            if (k < 0) {
                k += 256;
            }
            E1[j] = k;
        }

        FA += i;
        return true;
    }

    private static final int EA(int i, int j) {
        return j << 8 | i;
    }

    private static final boolean EB(int i, int j) {
        return (i & j) == j;
    }

}
