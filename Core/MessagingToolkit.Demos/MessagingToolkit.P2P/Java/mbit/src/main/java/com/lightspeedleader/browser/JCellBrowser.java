package com.lightspeedleader.browser;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.lcdui.*;
import javax.microedition.midlet.MIDlet;

public class JCellBrowser extends MIDlet
        implements CommandListener {

    public static Display display;
    public static MapCanvas mc;
    public static String encoding = "UTF-8";
    public static String pageurl = "http://lightspeedleader.com/JCellBrowser/index.html";
    public static String helpurl = "http://lightspeedleader.com/JCellBrowser/help.htm";
    Command C4;
    Command C5;
    Command C6;
    List C7;
    Command C8;
    Form C9;
    Command CA;
    Command CB;
    Command CC;
    Command CD;
    Command CE;
    TextField tf;
    List CF;
    int D0;
    Object D1;
    Command D2;
    Command D3;
    Command D4;
    KbdTextBox D5;
    TextBox tb;
    List D6;
    String D8;
    String D9;
    public static CachePool cachepool;
    int DA;
    Bookmarks _fld0128;

    public JCellBrowser() {
    }

    public void startApp() {
        if (mc != null) {
            MapCanvas _tmp = mc;
            MapCanvas.still = false;
            display.setCurrent(mc);
            return;
        }
        String s = getAppProperty("JCellBrowser-Encoding");
        if (s != null) {
            s = s.trim();
            if (s.length() > 0) {
                encoding = s.trim();
            }
        }
        D8 = getAppProperty("JCellBrowser-Home");
        if (D8 != null) {
            D8 = D8.trim();
            if (D8.length() > 0) {
                pageurl = D8;
            } else {
                D8 = pageurl;
            }
        } else {
            D8 = pageurl;
        }
        D9 = D8;
        try {
            DA = Integer.parseInt(getAppProperty("JCellBrowser-Cache"));
        }
        catch (Exception exception) {
            DA = 16384;
        }
        cachepool = new CachePool(DA);
        _fld0128 = new Bookmarks();
        C4 = new Command("START", 1, 1);
        C5 = new Command("GO", 1, 1);
        D2 = new Command("SET", 1, 1);
        C6 = new Command("MENU", 1, 2);
        C8 = new Command("SET", 1, 1);
        D3 = new Command("EDIT", 1, 2);
        D4 = new Command("OK", 1, 1);
        CA = new Command("GO", 1, 1);
        CB = new Command("DEL", 1, 2);
        CC = new Command("ADD", 1, 1);
        CD = new Command("EDIT", 1, 2);
        CE = new Command("OK", 1, 1);
        C7 = new List("MENU", 3, new String[]{
                "Back", "Home", "Online Help", "Enter URL", "Bookmarks", "Add Bookmark", "Save Cache", "Reset Cache", "Return", "Exit JCellBrowser"
        }, null);
        C7.addCommand(C8);
        C7.setCommandListener(this);
        mc = new MapCanvas();
        mc.addCommand(C4);
        mc.addCommand(C6);
        mc.setCommandListener(this);
        mc.init();
        try {
            HttpConnection httpconnection = (HttpConnection) Connector.open(pageurl);
            httpconnection.close();
        }
        catch (Exception exception1) {
        }
        display = Display.getDisplay(this);
        display.setCurrent(mc);
    }

    public void commandAction(Command command, Displayable displayable) {
        if (command == CA) {
            int i = CF.getSelectedIndex();
            CF = null;
            Bookmarks _tmp = _fld0128;
            String s2 = Tools.GetToken((String) Bookmarks.BM.elementAt(i), 2);
            Tools.pushHistoryStack(pageurl);
            pageurl = s2;
            MapCanvas _tmp1 = mc;
            MapCanvas.delayload = true;
            MapCanvas _tmp2 = mc;
            MapCanvas.BaseY = 0;
            MapCanvas _tmp3 = mc;
            MapCanvas.still = false;
            display.setCurrent(mc);
            return;
        }
        if (command == CB) {
            int j = CF.getSelectedIndex();
            CF = null;
            Bookmarks _tmp4 = _fld0128;
            Bookmarks.BM.removeElementAt(j);
            _fld0128.save();
            MapCanvas _tmp5 = mc;
            MapCanvas.still = false;
            display.setCurrent(mc);
            return;
        }
        if (command == CD) {
            D5 = new KbdTextBox(tf.getString());
            D5.addCommand(CE);
            D5.setCommandListener(this);
            MapCanvas _tmp6 = mc;
            MapCanvas.still = true;
            display.setCurrent(D5);
            return;
        }
        if (command == CE) {
            tf.setString(D5.getString());
            D5 = null;
            MapCanvas _tmp7 = mc;
            MapCanvas.still = true;
            display.setCurrent(C9);
            return;
        }
        if (command == CC) {
            String s = tf.getString();
            if (s == null || s.trim().length() == 0) {
                s = "bookmark";
            }
            Bookmarks _tmp8 = _fld0128;
            Bookmarks.BM.addElement(s + "@" + pageurl);
            _fld0128.save();
            tf = null;
            C9 = null;
            MapCanvas _tmp9 = mc;
            MapCanvas.still = false;
            display.setCurrent(mc);
            return;
        }
        if (command == D3) {
            D5 = new KbdTextBox(tb.getString());
            if ((tb.getConstraints() & 0x10000) != 0) {
                D5.setPassword(true);
            }
            D5.addCommand(D4);
            D5.setCommandListener(this);
            MapCanvas _tmp10 = mc;
            MapCanvas.still = true;
            display.setCurrent(D5);
            return;
        }
        if (command == D4) {
            tb.setString(D5.getString());
            D5 = null;
            MapCanvas _tmp11 = mc;
            MapCanvas.still = true;
            display.setCurrent(tb);
            return;
        }
        if (command == C6) {
            MapCanvas _tmp12 = mc;
            MapCanvas.still = true;
            display.setCurrent(C7);
            return;
        }
        if (command == C8) {
            int k = C7.getSelectedIndex();
            if (k == 0) {
                MapCanvas _tmp13 = mc;
                if (MapCanvas.mode != 0) {
                    String s3 = Tools.popHistoryStack();
                    pageurl = s3;
                    MapCanvas _tmp14 = mc;
                    MapCanvas.delayload = true;
                    MapCanvas _tmp15 = mc;
                    MapCanvas.BaseY = 0;
                    MapCanvas _tmp16 = mc;
                    MapCanvas.still = false;
                    display.setCurrent(mc);
                }
            }
            if (k == 1) {
                Tools.pushHistoryStack(pageurl);
                pageurl = D8;
                MapCanvas _tmp17 = mc;
                MapCanvas.delayload = true;
                MapCanvas _tmp18 = mc;
                MapCanvas.BaseY = 0;
                MapCanvas _tmp19 = mc;
                MapCanvas.still = false;
                display.setCurrent(mc);
            }
            if (k == 2) {
                Tools.pushHistoryStack(pageurl);
                pageurl = helpurl;
                MapCanvas _tmp20 = mc;
                MapCanvas.delayload = true;
                MapCanvas _tmp21 = mc;
                MapCanvas.BaseY = 0;
                MapCanvas _tmp22 = mc;
                MapCanvas.still = false;
                display.setCurrent(mc);
            }
            if (k == 3) {
                D0 = 0;
                tb = new TextBox("", "", 128, 0);
                tb.setString(D9);
                tb.addCommand(D3);
                tb.addCommand(D2);
                tb.setCommandListener(this);
                MapCanvas _tmp23 = mc;
                MapCanvas.still = true;
                display.setCurrent(tb);
            }
            if (k == 4) {
                Bookmarks _tmp24 = _fld0128;
                int l = Bookmarks.BM.size();
                if (l == 0) {
                    return;
                }
                String as[] = new String[l];
                for (int j1 = 0; j1 < l; j1++) {
                    as[j1] = Tools.GetToken((String) Bookmarks.BM.elementAt(j1), 1);
                }

                CF = new List("Bookmarks", 3, as, null);
                CF.addCommand(CA);
                CF.addCommand(CB);
                CF.setCommandListener(this);
                MapCanvas _tmp25 = mc;
                MapCanvas.still = true;
                display.setCurrent(CF);
            }
            if (k == 5) {
                tf = new TextField("Description:", "", 128, 0);
                C9 = new Form("Add Bookmark");
                C9.append(tf);
                C9.addCommand(CC);
                C9.addCommand(CD);
                C9.setCommandListener(this);
                MapCanvas _tmp26 = mc;
                MapCanvas.still = true;
                display.setCurrent(C9);
            }
            if (k == 6) {
                cachepool.saveCache();
            }
            if (k == 7) {
                cachepool.reset();
            }
            if (k == 8) {
                MapCanvas _tmp27 = mc;
                MapCanvas.still = false;
                display.setCurrent(mc);
            }
            if (k == 9) {
                notifyDestroyed();
            }
            return;
        }
        if (command == C4) {
            mc.removeCommand(C4);
            mc.addCommand(C5);
            mc.addCommand(C6);
            MapCanvas _tmp28 = mc;
            MapCanvas.LogoImage = null;
            Tools.initHistoryStack();
            Tools.pushHistoryStack(pageurl);
            HTMLStream htmlstream = new HTMLStream(pageurl);
            htmlstream = null;
            MapCanvas _tmp29 = mc;
            MapCanvas.mode = 1;
            MapCanvas _tmp30 = mc;
            MapCanvas.BaseY = 0;
            mc.repaint();
            return;
        }
        if (command == D2) {
            switch (D0) {
                default:
                    break;

                case 0: // '\0'
                    MapCanvas _tmp31 = mc;
                    if (MapCanvas.mode == 0) {
                        pageurl = tb.getString();
                        MapCanvas _tmp32 = mc;
                        MapCanvas.still = false;
                        display.setCurrent(mc);
                    } else {
                        String s1 = tb.getString();
                        Tools.pushHistoryStack(pageurl);
                        pageurl = s1;
                        MapCanvas _tmp33 = mc;
                        MapCanvas.delayload = true;
                        MapCanvas _tmp34 = mc;
                        MapCanvas.BaseY = 0;
                        MapCanvas _tmp35 = mc;
                        MapCanvas.still = false;
                        display.setCurrent(mc);
                    }
                    D9 = pageurl;
                    tb = null;
                    break;

                case 1: // '\001'
                    TextObj textobj = (TextObj) D1;
                    textobj.value = tb.getString();
                    textobj.paint(MapCanvas.VG);
                    D1 = null;
                    tb = null;
                    MapCanvas _tmp36 = mc;
                    MapCanvas.still = false;
                    display.setCurrent(mc);
                    break;

                case 2: // '\002'
                    SelectObj selectobj = (SelectObj) D1;
                    selectobj.setOption(D6.getSelectedIndex());
                    selectobj.paint(MapCanvas.VG);
                    D1 = null;
                    D6 = null;
                    MapCanvas _tmp37 = mc;
                    MapCanvas.still = false;
                    display.setCurrent(mc);
                    break;
            }
        }
        if (command == C5) {
            if (HTMLStream.hsvector.size() > 0) {
                HotSpot hotspot = (HotSpot) HTMLStream.hsvector.elementAt(HTMLStream.hsindex);
                D0 = hotspot.type;
                switch (D0) {
                    default:
                        break;

                    case 0: // '\0'
                        String s4 = (String) hotspot.obj;
                        Tools.pushHistoryStack(pageurl);
                        pageurl = s4;
                        HTMLStream htmlstream1 = new HTMLStream(pageurl);
                        htmlstream1 = null;
                        MapCanvas _tmp38 = mc;
                        MapCanvas.BaseY = 0;
                        mc.repaint();
                        break;

                    case 1: // '\001'
                        TextObj textobj1 = (TextObj) hotspot.obj;
                        D1 = textobj1;
                        if (textobj1.password) {
                            tb = new TextBox("", "", 128, 0x10000);
                        } else {
                            tb = new TextBox("", "", 128, 0);
                        }
                        tb.setString(textobj1.value);
                        tb.addCommand(D3);
                        tb.addCommand(D2);
                        tb.setCommandListener(this);
                        MapCanvas _tmp39 = mc;
                        MapCanvas.still = true;
                        display.setCurrent(tb);
                        break;

                    case 2: // '\002'
                        SelectObj selectobj1 = (SelectObj) hotspot.obj;
                        D1 = selectobj1;
                        D6 = new List("Select", 3);
                        for (int i1 = 0; i1 < selectobj1.ov.size(); i1++) {
                            OptionObj optionobj = (OptionObj) selectobj1.ov.elementAt(i1);
                            D6.append(optionobj.value, null);
                        }

                        D6.setSelectedIndex(selectobj1.index, true);
                        D6.addCommand(D2);
                        D6.setCommandListener(this);
                        MapCanvas _tmp40 = mc;
                        MapCanvas.still = true;
                        display.setCurrent(D6);
                        break;

                    case 3: // '\003'
                        ButtonObj buttonobj = (ButtonObj) hotspot.obj;
                        if (buttonobj.formid < 0 || buttonobj.formid >= HTMLStream.formvector.size()) {
                            break;
                        }
                        FormObj formobj = (FormObj) HTMLStream.formvector.elementAt(buttonobj.formid);
                        if (buttonobj.type.equals("reset")) {
                            formobj.reset();
                        } else {
                            Tools.pushHistoryStack(pageurl);
                            formobj.go();
                        }
                        mc.repaint();
                        break;

                    case 4: // '\004'
                        ChoiceObj choiceobj = (ChoiceObj) hotspot.obj;
                        if (choiceobj.flag) {
                            choiceobj.flag = false;
                        } else {
                            choiceobj.flag = true;
                        }
                        choiceobj.paint(MapCanvas.VG);
                        mc.repaint();
                        break;
                }
            }
            return;
        } else {
            return;
        }
    }

    public void pauseApp() {
        MapCanvas _tmp = mc;
        MapCanvas.still = true;
    }

    public void destroyApp(boolean flag) {
    }

}
