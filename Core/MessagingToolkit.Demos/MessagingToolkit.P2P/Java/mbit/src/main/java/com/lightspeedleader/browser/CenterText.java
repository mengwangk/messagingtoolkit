package com.lightspeedleader.browser;

public class CenterText {

    public String text;
    public boolean bold;
    public boolean underline;

    public CenterText(String s, boolean flag, boolean flag1) {
        text = "";
        bold = false;
        underline = false;
        text = s;
        bold = flag;
        underline = flag1;
    }
}
