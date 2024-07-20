package com.lightspeedleader.browser;

import java.util.Vector;

public class SelectObj {

    public String name;
    public String value;
    public int index;
    public Vector ov;
    int x;
    int y;
    int C1;

    SelectObj(String s, int i, int j) {
        ov = new Vector(1);
        name = s;
        x = i;
        y = j;
        value = "";
        index = 0;
        C1 = (16 - MapCanvas.fontHeight) / 2;
    }

    public void addOption(OptionObj optionobj) {
        ov.addElement(optionobj);
        if (ov.size() == 1) {
            value = optionobj.value;
            index = 0;
        } else if (optionobj.flag) {
            value = optionobj.value;
            index = ov.size() - 1;
        }
    }

    public void setOption(int i) {
        index = i;
        OptionObj optionobj = (OptionObj) ov.elementAt(i);
        value = optionobj.value;
    }

    public void paint(VirtualGraphics virtualgraphics) {
        virtualgraphics.drawImage(MapCanvas.SelectImage, x, y, 20);
        virtualgraphics.drawString(Tools.cutString(value, 55), x + 7, y + 3 + C1, 20);
    }
}
