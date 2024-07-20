package com.lightspeedleader.browser;

import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;
import java.io.InputStream;

public class JpegDecoder {

    int _fld0116;
    int _fld0117;
    int _fld0118;
    int _fld0119;
    int _fld011A;
    int qt[][];
    CellComponent _fld011B[];
    HuffmanTable _fld011C[];
    HuffmanTable _fld011D[];
    int _fld011E;
    int _fld011F;
    int _fld0120;
    int _fld0121;
    byte _fld0122[][];
    int ri;
    int _fld0123[] = {
            0, 1, 8, 16, 9, 2, 3, 10, 17, 24,
            32, 25, 18, 11, 4, 5, 12, 19, 26, 33,
            40, 48, 41, 34, 27, 20, 13, 6, 7, 14,
            21, 28, 35, 42, 49, 56, 57, 50, 43, 36,
            29, 22, 15, 23, 30, 37, 44, 51, 58, 59,
            52, 45, 38, 31, 39, 46, 53, 60, 61, 54,
            47, 55, 62, 63
    };
    Image _fld0124;
    boolean _fld0125;
    int _fld0126;
    public int poolsize;
    int FA;
    byte C2[];
    int _fld0127[];

    public JpegDecoder(byte abyte0[]) {
        qt = new int[4][64];
        _fld011B = new CellComponent[6];
        _fld011C = new HuffmanTable[4];
        _fld011D = new HuffmanTable[4];
        _fld0122 = new byte[5][];
        _fld0125 = false;
        _fld0126 = 0;
        _fld0127 = new int[64];
        C2 = abyte0;
        poolsize = C2.length;
        FA = 0;
    }

    public JpegDecoder(InputStream inputstream) {
        qt = new int[4][64];
        _fld011B = new CellComponent[6];
        _fld011C = new HuffmanTable[4];
        _fld011D = new HuffmanTable[4];
        _fld0122 = new byte[5][];
        _fld0125 = false;
        _fld0126 = 0;
        _fld0127 = new int[64];
        byte abyte0[][] = new byte[64][1];
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

    public void setColorImage() {
        _fld0125 = false;
    }

    public void setGrayScaleImage() {
        _fld0125 = true;
    }

    public Image decodeImage() {
        _fld0124 = null;
        _mth0116();
        return _fld0124;
    }

    void _mth0116() {
        for (int i = 0; i < 6; i++) {
            _fld011B[i] = new CellComponent();
        }

        for (int j = 0; j < 4; j++) {
            _fld011C[j] = new HuffmanTable();
            _fld011D[j] = new HuffmanTable();
        }

        if (_mth0123() != 65496) {
            return;
        }
        do {
            int k = _mth0122();
            int l = _mth0122();
            if (k != 255) {
                return;
            }
            for (; l == 255; l = _mth0122()) {
            }
            switch (l) {
                case 193:
                case 194:
                case 195:
                case 197:
                case 198:
                case 199:
                case 200:
                case 201:
                case 202:
                case 203:
                case 204:
                case 205:
                case 206:
                case 207:
                    return;

                case 219:
                    if (!_mth0117()) {
                        return;
                    }
                    break;

                case 192:
                    if (!_mth0118()) {
                        return;
                    }
                    break;

                case 221:
                    if (!_mth0119()) {
                        return;
                    }
                    break;

                case 196:
                    if (!_mth011A()) {
                        return;
                    }
                    break;

                case 218:
                    if (!_mth011B()) {
                        return;
                    }
                    break;

                case 217:
                    _mth0120();
                    return;

                default:
                    int i1 = _mth0123() - 2;
                    _mth0121(i1);
                    break;

                case 1: // '\001'
                case 208:
                case 209:
                case 210:
                case 211:
                case 212:
                case 213:
                case 214:
                case 215:
                    break;
            }
        } while (true);
    }

    boolean _mth0117() {
        int l;
        for (l = _mth0123() - 2; l > 0; l -= 64) {
            int i = _mth0122();
            l--;
            int k = i & 0xf;
            int j = i >> 4;
            if (j != 0 || k >= 4) {
                return false;
            }
            for (int i1 = 0; i1 < 64; i1++) {
                qt[k][i1] = _mth0122();
            }

        }

        return l == 0;
    }

    boolean _mth0118() {
        int l = _mth0123() - 2;
        if (_mth0122() != 8) {
            return false;
        }
        _fld0117 = _mth0123();
        _fld0116 = _mth0123();
        if (_fld0117 == 0 || _fld0116 == 0) {
            return false;
        }
        _fld011A = _mth0122();
        for (int i1 = 0; i1 < _fld011A; i1++) {
            int i = _mth0122();
            if (i > 5) {
                return false;
            }
            int j = _mth0122();
            int k = _mth0122();
            _fld011B[i].h = j >> 4;
            _fld011B[i].v = j & 0xf;
            _fld011B[i].DB = k;
            if (_fld011B[i].h == 0 || _fld011B[i].v == 0) {
                return false;
            }
            if (_fld011E < _fld011B[i].h) {
                _fld011E = _fld011B[i].h;
            }
            if (_fld011F < _fld011B[i].v) {
                _fld011F = _fld011B[i].v;
            }
        }

        _fld0120 = ((_fld0116 + _fld011E * 8) - 1) / (_fld011E * 8);
        _fld0121 = ((_fld0117 + _fld011F * 8) - 1) / (_fld011F * 8);
        _fld0118 = _fld0120 * _fld011E * 8;
        _fld0119 = _fld0121 * _fld011F * 8;
        for (int j1 = 0; j1 < _fld011A; j1++) {
            try {
                _fld0122[j1] = new byte[_fld0118 * _fld0119];
            }
            catch (Exception exception) {
                return false;
            }
        }

        return true;
    }

    boolean _mth0119() {
        if (_mth0123() != 4) {
            return false;
        } else {
            ri = _mth0123();
            return true;
        }
    }

    boolean _mth011A() {
        int ai[] = new int[16];
        for (int i1 = _mth0123() - 2; i1 > 0;) {
            int i = _mth0122();
            i1--;
            int k = i & 0xf;
            int l = i >> 4 & 1;
            if (k >= 2) {
                return false;
            }
            HuffmanTable huffmantable;
            if (l == 0) {
                huffmantable = _fld011C[k];
            } else {
                huffmantable = _fld011D[k];
            }
            for (int j1 = 0; j1 < 16; j1++) {
                ai[j1] = _mth0122();
            }

            i1 -= 16;
            int k1 = 0;
            while (k1 < 16) {
                for (int l1 = 0; l1 < ai[k1]; l1++) {
                    int j = _mth0122();
                    i1--;
                    huffmantable.addNode(k1 + 1, j);
                }

                k1++;
            }
        }

        return true;
    }

    boolean _mth011B() {
        int ai[] = new int[4];
        int ai1[] = new int[4];
        int ai2[] = new int[4];
        int ai3[] = new int[4];
        int j = _mth0123() - 2;
        int k = _mth0122();
        if (k < 1 || k > 4) {
            return false;
        }
        int i = k;
        for (int i1 = 0; i1 < i; i1++) {
            int l = _mth0122();
            if (l > 5) {
                return false;
            }
            ai[i1] = l;
            l = _mth0122();
            ai1[i1] = l >> 4;
            ai2[i1] = l & 0xf;
            if (ai1[i1] > 4 || ai2[i1] > 4) {
                return false;
            }
        }

        _mth0121(3);
        int ai4[] = new int[4];
        int ai5[] = new int[100];
        int ai6[] = new int[64];
        int ai7[] = new int[140];
        int j1 = 0;
        int k1 = 0;
        int l1 = 0;
        boolean flag = false;
        boolean flag1 = false;
        DecodeParam decodeParam = new DecodeParam();
        boolean flag2 = false;
        for (int k2 = 0; k2 < 4; k2++) {
            ai3[k2] = 0;
            ai4[k2] = 0;
        }

        decodeParam.DC = ai7;
        boolean flag4 = false;
        label0:
        do {
            int l2 = 0;
            label1:
            do {
                if (l2 >= i) {
                    continue label0;
                }
                HuffmanTable huffmantable1 = _fld011C[ai1[l2]];
                HuffmanTable huffmantable = _fld011D[ai2[l2]];
                int i4 = ai[l2];
                int k4 = 0;
                do {
                    if (k4 >= _fld011B[i4].h * _fld011B[i4].v) {
                        break;
                    }
                    int j2 = 0;
                    label2:
                    do {
                        int i2;
                        int ai9[];
                        label3:
                        {
                            int j5;
                            label4:
                            {
                                int l6;
                                label5:
                                {
                                    if (j2 >= 64) {
                                        break label2;
                                    }
                                    if (j1 <= 30 && !flag) {
                                        for (int l4 = 0; l4 < j1; l4++) {
                                            ai7[l4] = ai7[l1 + l4];
                                        }

                                        l1 = 0;
                                        int i5;
                                        if (poolsize - FA > 100) {
                                            i5 = 100;
                                        } else {
                                            i5 = poolsize - FA;
                                        }
                                        for (int l5 = 0; l5 < i5; l5++) {
                                            ai7[j1 + l5] = _mth0122();
                                        }

                                        j1 += i5;
                                        if (FA == poolsize) {
                                            flag = true;
                                        }
                                    }
                                    decodeParam.DD = l1;
                                    decodeParam.DE = k1;
                                    decodeParam.DF = j1;
                                    boolean flag3;
                                    if (j2 == 0) {
                                        flag3 = huffmantable1.deCode(decodeParam);
                                        i2 = HuffmanTable.deCode_code;
                                    } else {
                                        flag3 = huffmantable.deCode(decodeParam);
                                        i2 = HuffmanTable.deCode_code;
                                    }
                                    k1 = decodeParam.DE;
                                    l1 = decodeParam.DD;
                                    j1 = decodeParam.DF;
                                    k1 %= 8;
                                    if (j1 <= 0) {
                                        return false;
                                    }
                                    if (!flag3) {
                                        break label1;
                                    }
                                    ai9 = new int[4];
                                    if (j2 != 0) {
                                        break label3;
                                    }
                                    if (i2 == 0) {
                                        j5 = 0;
                                        break label4;
                                    }
                                    int j7 = i2;
                                    int j8 = 0;
                                    l6 = k1;
                                    if (k1 == 0) {
                                        if (ai7[l1] == 255) {
                                            while (ai7[l1 + 1] == 255) {
                                                l1++;
                                                j1--;
                                            }
                                            if (ai7[l1 + 1] != 0) {
                                                break label1;
                                            }
                                        } else if (ai7[l1] == 0 && ai7[l1 - 1] == 255) {
                                            l1++;
                                            j1--;
                                            if (ai7[l1] == 255) {
                                                while (ai7[l1 + 1] == 255) {
                                                    l1++;
                                                    j1--;
                                                }
                                                if (ai7[l1 + 1] != 0) {
                                                    break label1;
                                                }
                                            }
                                        }
                                    }
                                    label6:
                                    do {
                                        do {
                                            label7:
                                            do {
                                                do {
                                                    do {
                                                        if (j7 <= 0 || j8 >= 4) {
                                                            break label5;
                                                        }
                                                        int l7 = 8 - k1;
                                                        ai9[j8] = ai7[l1];
                                                        if (j7 >= l7) {
                                                            l1++;
                                                            k1 = 0;
                                                            j1--;
                                                        } else {
                                                            k1 += j7;
                                                        }
                                                        j7 -= l7;
                                                        j8++;
                                                    } while (k1 != 0);
                                                    if (ai7[l1] != 255) {
                                                        continue label7;
                                                    }
                                                    while (ai7[l1 + 1] == 255) {
                                                        l1++;
                                                        j1--;
                                                    }
                                                } while (ai7[l1 + 1] == 0);
                                                break label6;
                                            } while (ai7[l1] != 0 || ai7[l1 - 1] != 255);
                                            l1++;
                                            j1--;
                                        } while (ai7[l1] != 255);
                                        while (ai7[l1 + 1] == 255) {
                                            l1++;
                                            j1--;
                                        }
                                    } while (ai7[l1 + 1] == 0);
                                    break label1;
                                }
                                j5 = ((ai9[0] & 0xff) << 24) + ((ai9[1] & 0xff) << 16) + ((ai9[2] & 0xff) << 8) + (ai9[3] & 0xff);
                                j5 <<= l6;
                                if (j5 > 0) {
                                    j5 = 0 - (~j5 >>> 32 - i2);
                                } else {
                                    j5 >>>= 32 - i2;
                                }
                            }
                            ai4[l2] += j5;
                            ai5[0] = ai4[l2];
                            j2++;
                            continue;
                        }
                        int k6;
                        int i7;
                        label8:
                        {
                            if (i2 == 0) {
                                while (j2 < 64) {
                                    ai5[j2] = 0;
                                    j2++;
                                }
                                continue;
                            }
                            if (i2 == 240) {
                                int l8 = 0;
                                while (l8 < 16) {
                                    ai5[j2] = 0;
                                    j2++;
                                    l8++;
                                }
                                continue;
                            }
                            int i6 = i2 >> 4;
                            k6 = i2 & 0xf;
                            for (int i9 = 0; i9 < i6; i9++) {
                                ai5[j2] = 0;
                                j2++;
                            }

                            if (k6 == 0) {
                                continue;
                            }
                            int k7 = k6;
                            int k8 = 0;
                            i7 = k1;
                            if (k1 == 0) {
                                if (ai7[l1] == 255) {
                                    while (ai7[l1 + 1] == 255) {
                                        l1++;
                                        j1--;
                                    }
                                    if (ai7[l1 + 1] != 0) {
                                        break label1;
                                    }
                                } else if (ai7[l1] == 0 && ai7[l1 - 1] == 255) {
                                    l1++;
                                    j1--;
                                    if (ai7[l1] == 255) {
                                        while (ai7[l1 + 1] == 255) {
                                            l1++;
                                            j1--;
                                        }
                                        if (ai7[l1 + 1] != 0) {
                                            break label1;
                                        }
                                    }
                                }
                            }
                            label9:
                            do {
                                do {
                                    label10:
                                    do {
                                        do {
                                            do {
                                                if (k7 <= 0 || k8 >= 4) {
                                                    break label8;
                                                }
                                                int i8 = 8 - k1;
                                                ai9[k8] = ai7[l1];
                                                if (k7 >= i8) {
                                                    l1++;
                                                    k1 = 0;
                                                    j1--;
                                                } else {
                                                    k1 += k7;
                                                }
                                                k7 -= i8;
                                                k8++;
                                            } while (k1 != 0);
                                            if (ai7[l1] != 255) {
                                                continue label10;
                                            }
                                            while (ai7[l1 + 1] == 255) {
                                                l1++;
                                                j1--;
                                            }
                                        } while (ai7[l1 + 1] == 0);
                                        break label9;
                                    } while (ai7[l1] != 0 || ai7[l1 - 1] != 255);
                                    l1++;
                                    j1--;
                                } while (ai7[l1] != 255);
                                while (ai7[l1 + 1] == 255) {
                                    l1++;
                                    j1--;
                                }
                            } while (ai7[l1 + 1] == 0);
                            break label1;
                        }
                        int k5 = ((ai9[0] & 0xff) << 24) + ((ai9[1] & 0xff) << 16) + ((ai9[2] & 0xff) << 8) + (ai9[3] & 0xff);
                        k5 <<= i7;
                        if (k5 > 0) {
                            k5 = 0 - (~k5 >>> 32 - k6);
                        } else {
                            k5 >>>= 32 - k6;
                        }
                        ai5[j2] = k5;
                        j2++;
                    } while (true);
                    if (j2 > 64) {
                        break label1;
                    }
                    int ai8[] = qt[_fld011B[i4].DB];
                    for (int j6 = 0; j6 < 64; j6++) {
                        ai5[j6] *= ai8[j6];
                        ai6[_fld0123[j6]] = ai5[j6];
                    }

                    _mth011E(ai6);
                    _mth011F(i4, ai3[l2], _fld0127, _fld0122[l2]);
                    ai3[l2]++;
                    k4++;
                } while (true);
                l2++;
            } while (true);
            do {
                int i3 = ai7[l1];
                int j3 = ai7[l1 + 1];
                if (i3 == 255 && j3 != 255 && j3 != 0) {
                    if (j3 < 208 || j3 > 215) {
                        _mth0121(-j1);
                        return true;
                    }
                    ai4[0] = ai4[1] = ai4[2] = ai4[3] = 0;
                    l1 += 2;
                    k1 = 0;
                    j1 -= 2;
                    break;
                }
                if (j1 <= 30 && !flag) {
                    for (int k3 = 0; k3 < j1; k3++) {
                        ai7[k3] = ai7[l1 + k3];
                    }

                    l1 = 0;
                    int l3;
                    if (poolsize - FA > 100) {
                        l3 = 100;
                    } else {
                        l3 = poolsize - FA;
                    }
                    for (int j4 = 0; j4 < l3; j4++) {
                        ai7[j1 + j4] = _mth0122();
                    }

                    j1 += l3;
                    if (FA == poolsize) {
                        flag = true;
                    }
                }
                l1++;
                if (--j1 <= 0) {
                    return true;
                }
                i3 = ai7[l1];
                j3 = ai7[l1 + 1];
            } while (true);
            if (flag4) {
                return true;
            }
        } while (true);
    }

    int _mth011C(int i, int j) {
        return i + (1 << j - 1) >> j;
    }

    int _mth011D(int i) {
        if (i > 255) {
            i = 255;
        } else if (i < 0) {
            i = 0;
        }
        return i;
    }

    void _mth011E(int ai[]) {
        int ai1[] = new int[64];
        int k6 = 0;
        int l6 = 0;
        for (int i7 = 8; i7 > 0; i7--) {
            if ((ai[k6 + 8] | ai[k6 + 16] | ai[k6 + 24] | ai[k6 + 32] | ai[k6 + 40] | ai[k6 + 48] | ai[k6 + 56]) == 0) {
                int k7 = ai[k6 + 0] << 2;
                ai1[l6 + 0] = k7;
                ai1[l6 + 8] = k7;
                ai1[l6 + 16] = k7;
                ai1[l6 + 24] = k7;
                ai1[l6 + 32] = k7;
                ai1[l6 + 40] = k7;
                ai1[l6 + 48] = k7;
                ai1[l6 + 56] = k7;
                k6++;
                l6++;
            } else {
                int k4 = ai[k6 + 16];
                int i5 = ai[k6 + 48];
                int i4 = (k4 + i5) * 4433;
                int i1 = i4 + i5 * -15137;
                int k1 = i4 + k4 * 6270;
                k4 = ai[k6 + 0];
                i5 = ai[k6 + 32];
                int i = k4 + i5 << 13;
                int k = k4 - i5 << 13;
                int i2 = i + k1;
                int k3 = i - k1;
                int k2 = k + i1;
                int i3 = k - i1;
                i = ai[k6 + 56];
                k = ai[k6 + 40];
                i1 = ai[k6 + 24];
                k1 = ai[k6 + 8];
                i4 = i + k1;
                k4 = k + i1;
                i5 = i + i1;
                int k5 = k + k1;
                int i6 = (i5 + k5) * 9633;
                i *= 2446;
                k *= 16819;
                i1 *= 25172;
                k1 *= 12299;
                i4 *= -7373;
                k4 *= -20995;
                i5 *= -16069;
                k5 *= -3196;
                i5 += i6;
                k5 += i6;
                i += i4 + i5;
                k += k4 + k5;
                i1 += k4 + i5;
                k1 += i4 + k5;
                ai1[l6 + 0] = _mth011C(i2 + k1, 11);
                ai1[l6 + 56] = _mth011C(i2 - k1, 11);
                ai1[l6 + 8] = _mth011C(k2 + i1, 11);
                ai1[l6 + 48] = _mth011C(k2 - i1, 11);
                ai1[l6 + 16] = _mth011C(i3 + k, 11);
                ai1[l6 + 40] = _mth011C(i3 - k, 11);
                ai1[l6 + 24] = _mth011C(k3 + i, 11);
                ai1[l6 + 32] = _mth011C(k3 - i, 11);
                k6++;
                l6++;
            }
        }

        l6 = 0;
        boolean flag = false;
        boolean flag1 = false;
        for (int i8 = 0; i8 < 8; i8++) {
            int l7 = i8 * 8;
            int l4 = ai1[l6 + 2];
            int j5 = ai1[l6 + 6];
            int j4 = (l4 + j5) * 4433;
            int j1 = j4 + j5 * -15137;
            int l1 = j4 + l4 * 6270;
            int j = ai1[l6 + 0] + ai1[l6 + 4] << 13;
            int l = ai1[l6 + 0] - ai1[l6 + 4] << 13;
            int j2 = j + l1;
            int l3 = j - l1;
            int l2 = l + j1;
            int j3 = l - j1;
            j = ai1[l6 + 7];
            l = ai1[l6 + 5];
            j1 = ai1[l6 + 3];
            l1 = ai1[l6 + 1];
            j4 = j + l1;
            l4 = l + j1;
            j5 = j + j1;
            int l5 = l + l1;
            int j6 = (j5 + l5) * 9633;
            j *= 2446;
            l *= 16819;
            j1 *= 25172;
            l1 *= 12299;
            j4 *= -7373;
            l4 *= -20995;
            j5 *= -16069;
            l5 *= -3196;
            j5 += j6;
            l5 += j6;
            j += j4 + j5;
            l += l4 + l5;
            j1 += l4 + j5;
            l1 += j4 + l5;
            int j7 = _mth011C(j2 + l1, 18) + 128;
            _fld0127[l7 + 0] = _mth011D(j7);
            j7 = _mth011C(j2 - l1, 18) + 128;
            _fld0127[l7 + 7] = _mth011D(j7);
            j7 = _mth011C(l2 + j1, 18) + 128;
            _fld0127[l7 + 1] = _mth011D(j7);
            j7 = _mth011C(l2 - j1, 18) + 128;
            _fld0127[l7 + 6] = _mth011D(j7);
            j7 = _mth011C(j3 + l, 18) + 128;
            _fld0127[l7 + 2] = _mth011D(j7);
            j7 = _mth011C(j3 - l, 18) + 128;
            _fld0127[l7 + 5] = _mth011D(j7);
            j7 = _mth011C(l3 + j, 18) + 128;
            _fld0127[l7 + 3] = _mth011D(j7);
            j7 = _mth011C(l3 - j, 18) + 128;
            _fld0127[l7 + 4] = _mth011D(j7);
            l6 += 8;
        }

    }

    void _mth011F(int i, int j, int ai[], byte abyte0[]) {
        int i2 = j / (_fld011B[i].h * _fld011B[i].v);
        j %= _fld011B[i].h * _fld011B[i].v;
        int i1 = i2 % _fld0120;
        int j1 = i2 / _fld0120;
        int k1 = j % _fld011B[i].h;
        int l1 = j / _fld011B[i].h;
        if (i1 >= _fld0120 || j1 >= _fld0121) {
            return;
        }
        i1 *= _fld011E * 8;
        j1 *= _fld011F * 8;
        int j2 = _fld011E / _fld011B[i].h;
        int k2 = _fld011F / _fld011B[i].v;
        k1 *= j2 * 8;
        l1 *= k2 * 8;
        int k = i1 + k1;
        int l = j1 + l1;
        int l3 = _fld0118 * l + k;
        boolean flag = false;
        boolean flag1 = false;
        for (int j3 = 0; j3 < k2; j3++) {
            for (int k3 = 0; k3 < j2; k3++) {
                int i4 = 0;
                for (int l2 = 0; l2 < 8; l2++) {
                    int j4 = l3 + (j3 + l2 * k2) * _fld0118 + k3;
                    for (int i3 = 0; i3 < 8; i3++) {
                        abyte0[j4] = (byte) ai[i4];
                        i4++;
                        j4 += j2;
                    }

                }

            }

        }

    }

    void _mth0120() {
        int j2 = 0;
        _fld0124 = Image.createImage(_fld0116, _fld0117);
        Graphics g = _fld0124.getGraphics();
        if (_fld011A == 3 && !_fld0125) {
            for (int k2 = 0; k2 < _fld0119; k2++) {
                if (k2 >= _fld0117) {
                    j2 += _fld0118;
                } else {
                    for (int i3 = 0; i3 < _fld0118; i3++) {
                        if (i3 >= _fld0116) {
                            j2++;
                            continue;
                        }
                        int i = _fld0122[0][j2] & 0xff;
                        int k = _fld0122[1][j2] & 0xff;
                        k -= 128;
                        int l = _fld0122[2][j2] & 0xff;
                        l -= 128;
                        int i1 = i + (1402 * l) / 1000;
                        if (i1 > 255) {
                            i1 = 255;
                        } else if (i1 < 0) {
                            i1 = 0;
                        }
                        int j1 = i - (34414 * k + 0x116f6 * l) / 0x186a0;
                        if (j1 > 255) {
                            j1 = 255;
                        } else if (j1 < 0) {
                            j1 = 0;
                        }
                        int k1 = i + (1772 * k) / 1000;
                        if (k1 > 255) {
                            k1 = 255;
                        } else if (k1 < 0) {
                            k1 = 0;
                        }
                        int l1 = (i1 << 16) + (j1 << 8) + k1;
                        g.setColor(l1);
                        g.drawLine(i3, k2, i3, k2);
                        j2++;
                    }

                }
            }

        } else {
            for (int l2 = 0; l2 < _fld0119; l2++) {
                if (l2 >= _fld0117) {
                    j2 += _fld0118;
                    continue;
                }
                for (int j3 = 0; j3 < _fld0118; j3++) {
                    if (j3 >= _fld0116) {
                        j2++;
                    } else {
                        int j = _fld0122[0][j2] & 0xff;
                        int i2 = (j << 16) + (j << 8) + j;
                        g.setColor(i2);
                        g.drawLine(j3, l2, j3, l2);
                        j2++;
                    }
                }

            }

        }
    }

    void _mth0121(int i) {
        if (FA + i >= poolsize) {
            FA = poolsize;
        } else if (FA + i < 0) {
            FA = 0;
        } else {
            FA += i;
        }
    }

    int _mth0122() {
        if (FA == poolsize) {
            return 0;
        }
        int i = C2[FA];
        if (i < 0) {
            i += 256;
        }
        if (FA + 1 <= poolsize) {
            FA++;
        }
        return i;
    }

    int _mth0123() {
        return _mth0122() << 8 ^ _mth0122();
    }

    void clear() {
    }
}
