package com.mbit.screen;

import com.mbit.MainMidlet;
import com.mbit.messaging.sms.SMSSender;
import com.mbit.util.TextUtils;
import com.mbit.common.Constants;
import com.mbit.valueobject.rss.RssItemRecord;
import com.mbit.valueobject.inbox.MessageRecord;

import javax.microedition.lcdui.*;

import de.enough.polish.util.Locale;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 12, 2006 6:29:32 PM
 * @version : $Id:
 */
public class SendMessageScreen extends Form implements CommandListener {

    private MainMidlet midlet;
    private TextField phoneNumber;
    private MessageRecord message;
    private Alert msgBox;
    private StringItem msgContent;

    /**
     * Constructor
     */
    public SendMessageScreen(MainMidlet midlet) {
        //#style displayScreen
        super(Locale.get("inbox.sendmsg"));
        this.midlet = midlet;

        phoneNumber = new TextField(Locale.get("inbox.phonenumber"), "", 50, TextField.PHONENUMBER);
        append(phoneNumber);
        setCommandListener(this);
        addCommand(midlet.okCmd);
        addCommand(midlet.cancelCmd);

        msgContent = new StringItem(Locale.get("inbox.msgcontent"), "");
        append(msgContent);

        try {
            msgBox = new Alert(Locale.get("inbox.msgtitle"), "",
                    Image.createImage(Constants.APPLICATION_ICON), AlertType.CONFIRMATION);
            msgBox.setTimeout(Alert.FOREVER);
        } catch (Exception ex) {

        }
    }

    public void setup() {

    }

    public void display(MessageRecord message) {
        this.message = message;
        msgContent.setText(TextUtils.stripHTMLTags(message.getBody()));
        Display.getDisplay(midlet).setCurrent(this);
    }

    public void commandAction(Command command, Displayable displayable) {
        Display display = Display.getDisplay(midlet);
        if (command == midlet.okCmd) {
            String msisdn = phoneNumber.getString();
            if ("".equals(msisdn) || msisdn == null) {
                msgBox.setString(Locale.get("inbox.msgphonenumberempty"));
                display.setCurrent(msgBox, this);
            } else {
                String msg = msgContent.getText();
                SMSSender smsSender = new SMSSender(midlet);
                smsSender.setDestinationAddress(msisdn);
                smsSender.setMessage(msg);
                new Thread(smsSender).start();
            }
            display.setCurrent(midlet.mySpaceScreen);
        } else if (command == midlet.cancelCmd) {
            display.setCurrent(midlet.mySpaceScreen);
        }

    }

}
