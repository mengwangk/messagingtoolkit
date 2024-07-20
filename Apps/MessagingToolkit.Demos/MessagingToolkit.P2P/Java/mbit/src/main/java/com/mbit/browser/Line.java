package com.mbit.browser;

import com.mbit.browser.HtmlItem;

import java.util.Vector;

/**
 * A line containing a list of items.
 * <p/>
 * A Line has a baseline y position, as well as left and right margins. The line
 * is rendered from it's bottom y position.
 * <p/>
 * The line has a height above the baseline.
 */
public class Line {

    /**
     * Keep a linked list of allocated objects,
     * to ease the job of the GC.
     */
    private static Line poolHead;
    private Line next;

    public Vector items;
    public int ypos = 0;
    public int height = 0;

    private void initializeState() {
        height = 0;
        ypos = 0;
        if (items == null) {
            items = new Vector();
        } else {
            items.removeAllElements();
        }
    }

    public Line() {
    }

    public static Line newLine(int y) {
        Line line = get();
        line.initializeState();
        line.ypos = y;
        return line;
    }

    public static synchronized Line get() {
        Line item;
        if (poolHead != null) {
            item = poolHead;
            poolHead = item.next;
        } else {
            item = new Line();
        }
        return item;
    }

    public static synchronized void put(Line line) {
        line.next = poolHead;
        poolHead = line;
    }


    public void addItem(HtmlItem item) {
        items.addElement(item);
    }

    public int size() {
        return items.size();
    }

    public Object elementAt(int i) {
        return items.elementAt(i);
    }


}
