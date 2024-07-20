package com.mbit.messaging.sms;

import com.mbit.MainMidlet;
import com.mbit.common.Constants;

import javax.microedition.io.Connector;
import javax.microedition.lcdui.Alert;
import javax.microedition.lcdui.Image;
import javax.microedition.lcdui.AlertType;
import javax.microedition.lcdui.Display;
import javax.wireless.messaging.TextMessage;
import javax.wireless.messaging.MessageConnection;
import java.io.IOException;

import de.enough.polish.util.Locale;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 18, 2006 6:20:14 PM
 * @version : $Id:
 */
public class SMSSender implements Runnable {

    /**
     * SMS prefix
     */
    public static final String PREFIX = "sms://";
    /**
     * The port on which we send SMS messages
     */
    private String smsPort;

    /**
     * The URL to send the message to
     */
    private String destinationAddress;

    /**
     * The message
     */
    private String message;

    private Alert msgBox;

    /**
     * Main midlet
     */
    private MainMidlet midlet;

    /**
     * Initialize the MIDlet with the current display object and
     * graphical components.
     */
    public SMSSender(MainMidlet midlet) {
        this.midlet = midlet;
        this.smsPort = midlet.getAppProperty("SMS-Port");
        if (smsPort == null || "".equals(smsPort)) {
            smsPort = "50000";
        }
        this.destinationAddress = null;

        try {
            msgBox = new Alert(Locale.get("sms.msgtitle"), "",
                    Image.createImage(Constants.APPLICATION_ICON), AlertType.CONFIRMATION);
            msgBox.setTimeout(Alert.FOREVER);
        } catch (Exception ex) {

        }
    }

    public String getSmsPort() {
        return smsPort;
    }

    public void setSmsPort(String smsPort) {
        this.smsPort = smsPort;
    }

    public String getDestinationAddress() {
        return destinationAddress;
    }

    public void setDestinationAddress(String destinationAddress) {
        this.destinationAddress = destinationAddress;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    /**
     * Send the message. Called on a separate thread so we don't have
     * contention for the display
     */
    public void run() {
        if (!destinationAddress.startsWith(PREFIX)) {
            destinationAddress = PREFIX + destinationAddress;
        }
        String address = destinationAddress + ":" + smsPort;

        MessageConnection smsConn = null;
        try {
            /** Open the message connection. */
            smsConn = (MessageConnection) Connector.open(address);

            TextMessage txtmessage = (TextMessage) smsConn.newMessage(
                    MessageConnection.TEXT_MESSAGE);
            txtmessage.setAddress(address);
            txtmessage.setPayloadText(message);
            smsConn.send(txtmessage);

            msgBox.setString(Locale.get("sms.msgsent"));
            Display.getDisplay(midlet).setCurrent(msgBox);
        } catch (Throwable t) {
            msgBox.setString(t.getMessage());
            Display.getDisplay(midlet).setCurrent(msgBox);
            //#debug
            System.out.println("Send caught: ");
            t.printStackTrace();
        } finally {
            if (smsConn != null) {
                try {
                    smsConn.close();
                } catch (IOException ioe) {
                    System.out.println("Closing connection caught: ");
                    ioe.printStackTrace();
                }
            }
        }
    }

    /**
     * Check the phone number for validity
     * Valid phone numbers contain only the digits 0 thru 9, and may contain
     * a leading '+'.
     */
    public static boolean isValidPhoneNumber(String number) {
        char[] chars = number.toCharArray();
        if (chars.length == 0) {
            return false;
        }
        int startPos = 0;
        // initial '+' is OK
        if (chars[0] == '+') {
            startPos = 1;
        }
        for (int i = startPos; i < chars.length; ++i) {
            if (!Character.isDigit(chars[i])) {
                return false;
            }
        }
        return true;
    }
}