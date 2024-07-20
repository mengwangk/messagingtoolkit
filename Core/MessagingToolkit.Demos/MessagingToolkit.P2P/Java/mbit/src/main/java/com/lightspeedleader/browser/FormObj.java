package com.lightspeedleader.browser;

import java.util.Vector;

public class FormObj {

    public String method;
    public String action;
    public Vector oiv;
    public Vector hov;

    public FormObj(String s, String s1) {
        oiv = new Vector(1);
        hov = new Vector(1);
        method = s;
        action = s1;
    }

    public void addObjectIndex(int i) {
        oiv.addElement(new Integer(i));
    }

    public void addHiddenObject(HiddenObj hiddenobj) {
        hov.addElement(hiddenobj);
    }

    public void reset() {
        for (int i = 0; i < oiv.size(); i++) {
            int j = ((Integer) oiv.elementAt(i)).intValue();
            HotSpot hotspot = (HotSpot) HTMLStream.hsvector.elementAt(j);
            if (hotspot.type == 1) {
                TextObj textobj = (TextObj) hotspot.obj;
                textobj.value = "";
                textobj.paint(MapCanvas.VG);
            }
            if (hotspot.type == 4) {
                ChoiceObj choiceobj = (ChoiceObj) hotspot.obj;
                choiceobj.flag = false;
                choiceobj.paint(MapCanvas.VG);
            }
        }

    }

    public void go() {
        String s = "";
        for (int i = 0; i < hov.size(); i++) {
            HiddenObj hiddenobj = (HiddenObj) hov.elementAt(i);
            if (s.length() != 0) {
                s = s + "&";
            }
            s = s + hiddenobj.name + "=" + hiddenobj.value;
        }

        for (int j = 0; j < oiv.size(); j++) {
            int k = ((Integer) oiv.elementAt(j)).intValue();
            HotSpot hotspot = (HotSpot) HTMLStream.hsvector.elementAt(k);
            if (hotspot.type == 1) {
                if (s.length() != 0) {
                    s = s + "&";
                }
                TextObj textobj = (TextObj) hotspot.obj;
                s = s + textobj.name + "=" + textobj.value;
            }
            if (hotspot.type == 2) {
                if (s.length() != 0) {
                    s = s + "&";
                }
                SelectObj selectobj = (SelectObj) hotspot.obj;
                OptionObj optionobj = (OptionObj) selectobj.ov.elementAt(selectobj.index);
                String s2 = selectobj.value;
                if (optionobj.name.length() > 0) {
                    s2 = optionobj.name;
                }
                s = s + selectobj.name + "=" + s2;
            }
            if (hotspot.type != 4) {
                continue;
            }
            ChoiceObj choiceobj = (ChoiceObj) hotspot.obj;
            if (!choiceobj.flag) {
                continue;
            }
            if (s.length() != 0) {
                s = s + "&";
            }
            String s1 = choiceobj.value;
            if (s1.length() == 0) {
                s1 = "on";
            }
            s = s + choiceobj.name + "=" + s1;
        }

        s = Tools.RepString(s, " ", "+");
        HTMLStream htmlstream;
        if (method.equals("get")) {
            htmlstream = new HTMLStream(action + "?" + s);
        } else {
            htmlstream = new HTMLStream(action, s);
        }
        htmlstream = null;
        JCellBrowser.pageurl = action + "?" + s;
        MapCanvas _tmp = JCellBrowser.mc;
        MapCanvas.BaseY = 0;
        JCellBrowser.mc.repaint();
    }
}
