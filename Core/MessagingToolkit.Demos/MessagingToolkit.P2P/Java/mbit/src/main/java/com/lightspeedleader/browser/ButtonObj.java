package com.lightspeedleader.browser;

public class ButtonObj {

    public String type;
    public String value;
    public int formid;
    int C0;
    int x;
    int y;
    int C1;

    public ButtonObj(String s, String s1, int i, int j, int k) {
        type = s;
        value = Tools.cutString(s1, 72);
        formid = i;
        x = j;
        y = k;
        C0 = MapCanvas.strWidth(value);
        C1 = (17 - MapCanvas.fontHeight) / 2;
    }

    public int getWidth() {
        return C0 + 16;
    }

    public void paint(VirtualGraphics virtualgraphics) {
        int i = virtualgraphics.getColor();
        virtualgraphics.setColor(0x808080);
        virtualgraphics.fillRoundRect(x + 3, y + 1, C0 + 8, 16, 4, 4);
        virtualgraphics.setColor(0);
        virtualgraphics.drawRoundRect(x + 3, y + 1, C0 + 8, 16, 4, 4);
        virtualgraphics.drawString(value, x + 7, y + C1, 20);
        virtualgraphics.setColor(i);
    }
}
