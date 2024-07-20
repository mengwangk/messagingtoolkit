package com.mbit.screen;

import com.mbit.MainMidlet;
import com.mbit.browser.ImageCache;
import com.mbit.common.Constants;
import com.mbit.valueobject.inbox.MessageRecord;
import com.mbit.valueobject.inbox.MessageStore;
import com.mbit.valueobject.inbox.MessageType;
import com.mbit.valueobject.rss.RssItemRecord;
import de.enough.polish.util.Locale;

import javax.microedition.lcdui.*;
import java.util.Enumeration;
import java.util.Hashtable;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 11, 2006 6:22:35 PM
 * @version : $Id:
 */
public class SaveNewsScreen extends Form implements CommandListener {

    private MainMidlet midlet;
    private NewsDetailsScreen newsDetailsScreen;
    private TextField tag;
    private RssItemRecord rssItem;
    private Alert msgBox;

    /**
     * Constructor
     */
    public SaveNewsScreen(MainMidlet midlet, NewsDetailsScreen newsDetailsScreen) {
        //#style displayScreen
        super(Locale.get("save.item"));

        this.midlet = midlet;
        this.newsDetailsScreen = newsDetailsScreen;

        try {
            msgBox = new Alert(Locale.get("save.msgtitle"), Locale.get("save.msgsaved"),
                    Image.createImage(Constants.APPLICATION_ICON), AlertType.CONFIRMATION);
            msgBox.setTimeout(Alert.FOREVER);
        } catch (Exception ex) {

        }
        tag = new TextField(Locale.get("tag.news"), "", 200, TextField.ANY);
        append(tag);

        setCommandListener(this);
        addCommand(midlet.okCmd);
        addCommand(midlet.cancelCmd);

    }

    public void setup(RssItemRecord item) {
        this.rssItem = item;
        tag.setString(item.getTitle());

    }

    public void display() {
        Display.getDisplay(midlet).setCurrent(this);
    }


    public void commandAction(Command command, Displayable displayable) {
        Display display = Display.getDisplay(midlet);
        if (command == midlet.okCmd) {
            try {
                String msgTitle = tag.getString();
                if ("".equals(msgTitle) || msgTitle == null) {
                    msgTitle = rssItem.getTitle();
                }
                MessageRecord msg = new MessageRecord();
                msg.setTitle(msgTitle);
                msg.setBody(rssItem.getDescription());
                msg.setMsgTime(System.currentTimeMillis());
                msg.setMsgType(MessageType.RSS_NEWS);

                Hashtable imageTable = newsDetailsScreen.getImageTable();
                Enumeration e = imageTable.elements();
                int i = 1;
                while (e.hasMoreElements()) {
                    ImageCache r = (ImageCache) e.nextElement();
                    if (i == 1) {
                        msg.setImage1(r.getUrl());
                        msg.setImageLength1(r.getImageData().length);
                        msg.setImageData1(r.getImageData());
                    }
                    if (i == 2) {
                        msg.setImage2(r.getUrl());
                        msg.setImageLength2(r.getImageData().length);
                        msg.setImageData2(r.getImageData());
                    }
                    if (i == 3) {
                        msg.setImage3(r.getUrl());
                        msg.setImageLength3(r.getImageData().length);
                        msg.setImageData3(r.getImageData());
                    }
                    i++;
                }
                MessageStore messageStore = MessageStore.getInstance();
                try {
                    messageStore.addRecord(msg);
                    msgBox.setString(Locale.get("save.msgsaved"));
                } catch (Exception ex){
                    msgBox.setString(ex.getMessage());
                }
                display.setCurrent(msgBox, newsDetailsScreen);

            } catch (Exception ex) {
                msgBox.setString(ex.getMessage());
                display.setCurrent(msgBox, newsDetailsScreen);
            }
            //display.setCurrent(newsDetailsScreen);
        } else if (command == midlet.cancelCmd) {
            display.setCurrent(newsDetailsScreen);
        }
    }

}
