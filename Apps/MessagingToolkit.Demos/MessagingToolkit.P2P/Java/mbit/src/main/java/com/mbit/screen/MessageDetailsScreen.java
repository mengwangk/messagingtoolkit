package com.mbit.screen;

import com.mbit.MainMidlet;
import com.mbit.browser.BrowserScreen;
import com.mbit.browser.ImageCache;
import com.mbit.valueobject.inbox.MessageRecord;

import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Display;
import javax.microedition.lcdui.Displayable;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 12, 2006 6:16:36 PM
 * @version : $Id:
 */
public class MessageDetailsScreen extends BrowserScreen implements CommandListener {

    private MainMidlet midlet;

    private MessageRecord message;

    /**
     * Constructor
     */
    public MessageDetailsScreen(MainMidlet midlet) {
        super(midlet);

        this.midlet = midlet;

        setCommandListener(this);
        addCommand(midlet.backCmd);
    }

    public void setup() {

    }

    public void display(MessageRecord message) {

        this.message = message;

        if (message.getImageData1() != null) {
            addCacheImage(new ImageCache(message.getImage1(), message.getImageData1()));
        }
        if (message.getImageData2() != null) {
            addCacheImage(new ImageCache(message.getImage2(), message.getImageData2()));
        }
        if (message.getImageData3() != null) {
            addCacheImage(new ImageCache(message.getImage3(), message.getImageData3()));
        }

        //#if polish.midp2
        setTitle(message.getTitle().trim());
        //#endif

        setText(message.getBody());

        Display.getDisplay(midlet).setCurrent(this);
    }

    /**
     * @param command
     * @param displayable
     */
    public void commandAction(Command command, Displayable displayable) {
        if (command == midlet.backCmd || command == midlet.okCmd) {
            Display.getDisplay(midlet).setCurrent(midlet.mySpaceScreen);
        } else {
            super.commandAction(command, displayable);
        }
    }
}



