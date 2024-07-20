package com.lightspeedleader.browser;

import javax.microedition.lcdui.Image;
import java.util.Vector;

public class GifFrame {

    public int x;
    public int y;
    Vector fv;
    int index;

    public GifFrame() {
        fv = new Vector(1);
        index = 0;
    }

    public void setXY(int i, int j) {
        x = i;
        y = j;
    }

    public void addImage(Image image) {
        fv.addElement(image);
    }

    public int size() {
        return fv.size();
    }

    public Image getImage() {
        if (size() == 0) {
            return null;
        } else {
            return (Image) fv.elementAt(index);
        }
    }

    public void next() {
        if (index + 1 < size()) {
            index++;
        } else {
            index = 0;
        }
    }
}
