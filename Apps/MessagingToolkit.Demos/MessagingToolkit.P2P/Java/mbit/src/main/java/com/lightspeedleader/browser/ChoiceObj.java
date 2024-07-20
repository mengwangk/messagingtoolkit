package com.lightspeedleader.browser;

public class ChoiceObj {

    public int type;
    public String name;
    public String value;
    public boolean flag;
    int x;
    int y;

    public ChoiceObj(int i, String s, String s1, boolean flag1, int j, int k) {
        type = i;
        name = s;
        value = s1;
        flag = flag1;
        x = j;
        y = k;
    }

    public void paint(VirtualGraphics virtualgraphics) {
        if (type == 1) {
            if (flag) {
                virtualgraphics.drawImage(MapCanvas.RCImage, x, y + 1, 20);
            } else {
                virtualgraphics.drawImage(MapCanvas.RAImage, x, y + 1, 20);
            }
        } else if (flag) {
            virtualgraphics.drawImage(MapCanvas.CCImage, x, y + 1, 20);
        } else {
            virtualgraphics.drawImage(MapCanvas.CBImage, x, y + 1, 20);
        }
    }
}
