package com.mbit.screen;

import com.mbit.browser.BrowserScreen;
import com.mbit.MainMidlet;
import com.mbit.valueobject.rss.RssItemRecord;

import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.Display;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 11, 2006 11:59:06 PM
 * @version : $Id:
 */
public class NewsDetailsScreen extends BrowserScreen implements CommandListener {

    private MainMidlet midlet;
    private NewsScreen newsScreen;
    private SaveNewsScreen saveNewsScreen;
    private RssItemRecord rssItem;

    /**
     * Constructor
     */
    public NewsDetailsScreen(MainMidlet midlet, NewsScreen newsScreen) {
        super(midlet);

        this.midlet = midlet;
        this.newsScreen = newsScreen;

        setCommandListener(this);
        addCommand(midlet.backCmd);
        addCommand(midlet.saveCmd);
        //setFullScreenMode(true);

        saveNewsScreen = new SaveNewsScreen(midlet, this);
    }

    public void setup() {

    }

    public void display(RssItemRecord rssItem) {

        this.rssItem = rssItem;

        //#if polish.midp2
        setTitle(rssItem.getTitle());
        //#endif

        setText(rssItem.getDescription());
        Display.getDisplay(midlet).setCurrent(this);
    }

    /**
     * @param command
     * @param displayable
     */
    public void commandAction(Command command, Displayable displayable) {
        if (command == midlet.backCmd || command == midlet.okCmd) {
            Display.getDisplay(midlet).setCurrent(newsScreen);
        } else if (command == midlet.saveCmd) {
            saveNewsScreen.setup(rssItem);
            saveNewsScreen.display();
        } else {
            super.commandAction(command, displayable);
        }
    }
}


