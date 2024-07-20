package com.lightspeedleader.browser;

public class HotSpot {

    public int bx;
    public int by;
    public int ex;
    public int ey;
    public int height;
    public int type;
    public Object obj;

    public HotSpot() {
    }

    public void setBxBy(int i, int j) {
        bx = i;
        by = j;
    }

    public void setExEy(int i, int j) {
        ex = i;
        ey = j;
    }

    public void setObj(int i, Object obj1, int j) {
        type = i;
        obj = obj1;
        height = j;
    }
}
