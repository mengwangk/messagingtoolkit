package com.mbit.browser;

import com.mbit.browser.FormWidget;

import java.util.Vector;

/**
 */
public class PicoForm {

    /**
     * Keep a linked list of allocated objects,
     * to ease the job of the GC.
     */
    public static final int GET = 0;
    public static final int POST = 1;
    public int method = GET;
    public String action;

    public Vector widgets;

    public PicoForm() {
        widgets = new Vector();
    }

    public void addWidget(FormWidget widget) {
        widgets.addElement(widget);
    }

    public int getMethod() {
        return method;
    }

    public String getAction() {
        return action;
    }

    public int size() {
        return widgets.size();
    }

    public FormWidget elementAt(int i) {
        return (FormWidget) widgets.elementAt(i);
    }

}
