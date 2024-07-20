package com.lightspeedleader.browser;

import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;

public class KbdTextBox extends Canvas {

    String text;
    int CanvasWidth;
    int CanvasHeight;
    Font font;
    boolean password;
    Image _fld0129;
    Graphics _fld012A;
    int _fld012F;
    String pp[][][];
    String up[][][] = {
            {
                    {
                            "1", "2", "3"
                    }, {
                    "4", "5", "6"
            }, {
                    "7", "8", "9"
            }, {
                    "<-", "0", "."
            }
            }, {
            {
                    "A", "B", "C"
            }, {
            "D", "E", "F"
    }, {
            "G", "H", "I"
    }, {
            "<-", "J", "K"
    }
    }, {
            {
                    "L", "M", "N"
            }, {
            "O", "P", "Q"
    }, {
            "R", "S", "T"
    }, {
            "<-", "U", "V"
    }
    }, {
            {
                    "W", "X", "Y"
            }, {
            "Z", ",", ";"
    }, {
            ":", "?", "&"
    }, {
            "<-", "!", " "
    }
    }, {
            {
                    "\"", "'", "`"
            }, {
            "~", "{", "}"
    }, {
            "^", "[", "]"
    }, {
            "<-", "(", ")"
    }
    }
    };
    String lp[][][] = {
            {
                    {
                            "1", "2", "3"
                    }, {
                    "4", "5", "6"
            }, {
                    "7", "8", "9"
            }, {
                    "<-", "0", "."
            }
            }, {
            {
                    "a", "b", "c"
            }, {
            "d", "e", "f"
    }, {
            "g", "h", "i"
    }, {
            "<-", "j", "k"
    }
    }, {
            {
                    "l", "n", "m"
            }, {
            "o", "p", "q"
    }, {
            "r", "s", "t"
    }, {
            "<-", "u", "v"
    }
    }, {
            {
                    "w", "x", "y"
            }, {
            "z", ",", ";"
    }, {
            ":", "?", "&"
    }, {
            "<-", "!", " "
    }
    }, {
            {
                    "+", "-", "*"
            }, {
            "/", "\\", "%"
    }, {
            "=", "#", "$"
    }, {
            "<-", "@", "_"
    }
    }
    };

    KbdTextBox() {
        font = null;
        _fld0129 = null;
        _fld012A = null;
        _fld012F = 0;
        init("");
    }

    KbdTextBox(String s) {
        font = null;
        _fld0129 = null;
        _fld012A = null;
        _fld012F = 0;
        init(s);
    }

    void init(String s) {
        text = s;
        CanvasWidth = MapCanvas.CanvasWidth;
        CanvasHeight = MapCanvas.CanvasHeight;
        password = false;
        pp = up;
        _fld0129 = Image.createImage(CanvasWidth, CanvasHeight);
        _fld012A = _fld0129.getGraphics();
    }

    public String getString() {
        return text;
    }

    public void setString(String s) {
        text = s;
    }

    public void setPassword(boolean flag) {
        password = flag;
    }

    int strWidth(String s) {
        return font.stringWidth(s);
    }

    int strHeight() {
        return font.getHeight();
    }

    public void paint(Graphics g) {
        if (font == null) {
            font = g.getFont();
        }
        int i = strHeight();
        byte byte0 = 64;
        int j = 4 * i + 5;
        int k = (CanvasWidth - byte0) / 2;
        int l = ((CanvasHeight - j) + i) / 2 + 1;
        if (l <= i + 2) {
            l = i + 2;
        }
        _fld012A.setColor(0xffffff);
        _fld012A.fillRect(0, 0, CanvasWidth, CanvasHeight);
        _fld012A.setColor(0);
        _fld012A.drawRect(0, 0, CanvasWidth - 1, i + 2);
        int i1 = CanvasWidth - 4;
        String s;
        if (password) {
            s = "";
            for (int j1 = 0; j1 < text.length(); j1++) {
                s = s + "*";
            }

        } else {
            s = text;
        }
        if (strWidth(s) <= i1) {
            _fld012A.drawString(s, 2, 2, 20);
        } else {
            for (; strWidth(s) > i1; s = s.substring(1, s.length())) {
            }
            _fld012A.drawString(s, 2, 2, 20);
        }
        if (_fld012F != 5) {
            _fld012A.setColor(0xc0c0c0);
            _fld012A.fillRect(k, l, byte0, j);
            _fld012A.setColor(0);
            _fld012A.drawRect(k, l, byte0, j);
            for (int k1 = 1; k1 < 3; k1++) {
                _fld012A.drawLine(k + 21 * k1, l, k + 21 * k1, l + j);
            }

            for (int l1 = 1; l1 < 4; l1++) {
                _fld012A.drawLine(k, l + (i + 1) * l1, k + byte0, l + (i + 1) * l1);
            }

            for (int i2 = 0; i2 < 4; i2++) {
                for (int j2 = 0; j2 < 3; j2++) {
                    if (i2 == 3 && j2 == 0) {
                        for (int k2 = 0; k2 < 6; k2++) {
                            int l2 = k + 21 * j2 + k2 + 7;
                            int i3 = l + (i + 1) * i2 + i / 2 + 1;
                            _fld012A.drawLine(l2, i3 - k2, l2, i3 + k2);
                        }

                    } else {
                        _fld012A.drawString(pp[_fld012F][i2][j2], k + 11 + 21 * j2, l + (i + 1) * i2 + 2, 17);
                    }
                }

            }

        } else {
            _fld012A.drawString("MIDP-KBD 1.0", CanvasWidth / 2, i * 2 + 6, 17);
            _fld012A.drawString("Open Source Software", CanvasWidth / 2, i * 4, 17);
        }
        g.setClip(0, 0, CanvasWidth, CanvasHeight);
        g.drawImage(_fld0129, 0, 0, 20);
    }

    void _mth0129(String s) {
        text += s;
    }

    void _mth012A() {
        int i = text.length();
        if (i > 0) {
            text = text.substring(0, i - 1);
        }
    }

    public void keyProc(int i) {
        int j = getGameAction(i);
        if (j == 5) {
            if (_fld012F < 4) {
                _fld012F++;
            } else {
                _fld012F = 0;
            }
        }
        if (j == 2) {
            if (_fld012F > 0) {
                _fld012F--;
            } else {
                _fld012F = 4;
            }
        }
        if (j == 1) {
            if (pp == up) {
                return;
            }
            pp = up;
        }
        if (j == 6) {
            if (pp == lp) {
                return;
            }
            pp = lp;
        }
        if (j == 8) {
            _fld012F = 5;
        }
        if (_fld012F != 5) {
            if (i == 49) {
                _mth0129(pp[_fld012F][0][0]);
            }
            if (i == 50) {
                _mth0129(pp[_fld012F][0][1]);
            }
            if (i == 51) {
                _mth0129(pp[_fld012F][0][2]);
            }
            if (i == 52) {
                _mth0129(pp[_fld012F][1][0]);
            }
            if (i == 53) {
                _mth0129(pp[_fld012F][1][1]);
            }
            if (i == 54) {
                _mth0129(pp[_fld012F][1][2]);
            }
            if (i == 55) {
                _mth0129(pp[_fld012F][2][0]);
            }
            if (i == 56) {
                _mth0129(pp[_fld012F][2][1]);
            }
            if (i == 57) {
                _mth0129(pp[_fld012F][2][2]);
            }
            if (i == 42) {
                _mth012A();
            }
            if (i == 48) {
                _mth0129(pp[_fld012F][3][1]);
            }
            if (i == 35) {
                _mth0129(pp[_fld012F][3][2]);
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
}
