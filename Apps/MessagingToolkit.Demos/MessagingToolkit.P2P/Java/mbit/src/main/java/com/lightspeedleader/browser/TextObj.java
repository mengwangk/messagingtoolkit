package com.lightspeedleader.browser;

public class TextObj {

    public String name;
    public String value;
    public boolean password;
    int x;
    int y;
    int C1;

    public TextObj(String s, String s1, boolean flag, int i, int j) {
        name = s;
        value = s1;
        password = flag;
        x = i;
        y = j;
        C1 = (16 - MapCanvas.fontHeight) / 2;
    }

    public void setValue(String s) {
        value = s;
    }

    public void paint(VirtualGraphics virtualgraphics) {
        virtualgraphics.drawImage(MapCanvas.TextImage, x, y, 20);
        if (password) {
            String s = "";
            for (int i = 0; i < value.length(); i++) {
                s = s + "*";
            }

            virtualgraphics.drawString(Tools.cutString(s, 66), x + 7, y + 3 + C1, 20);
        } else {
            virtualgraphics.drawString(Tools.cutString(value, 66), x + 7, y + 3 + C1, 20);
        }
    }
}
