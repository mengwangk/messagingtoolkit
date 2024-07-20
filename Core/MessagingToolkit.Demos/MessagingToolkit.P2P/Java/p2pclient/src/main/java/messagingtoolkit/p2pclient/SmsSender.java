package messagingtoolkit.p2pclient;

import javax.microedition.io.Connector;
import com.sun.lwuit.*;
import javax.wireless.messaging.TextMessage;
import javax.wireless.messaging.MessageConnection;
import java.io.IOException;


/**
 * Sms sender
 *
 */
public class SmsSender implements Runnable {

    /**
     * SMS prefix
     */
    public static final String PREFIX = "sms://";
    
    /**
     * SMS port
     */
    public static final String SMS_PORT = "50000";
    
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

  
    /**
     * Main midlet
     */
    private StartApp midlet;

    /**
     * Initialize the MIDlet with the current display object and
     * graphical components.
     */
    public SmsSender(StartApp midlet) {
        this.midlet = midlet;
        this.smsPort = midlet.getAppProperty("SMS-Port");
        if (smsPort == null || "".equals(smsPort)) {
            smsPort = SMS_PORT;
        }
        this.destinationAddress = null;
       
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
            // Open the message connection
            smsConn = (MessageConnection) Connector.open(address);

            TextMessage txtmessage = (TextMessage) smsConn.newMessage(MessageConnection.TEXT_MESSAGE);
            txtmessage.setAddress(address);
            txtmessage.setPayloadText(message);
            smsConn.send(txtmessage);

            // Show dialog
            Dialog.show("Message sent", "Message is sent successfully", "OK", "Cancel");         
           
        } catch (Throwable t) {             	
        	System.out.println ("Send caught: ");
            t.printStackTrace ();  
        } finally {
            if (smsConn != null) {
                try {
                    smsConn.close();
                } catch (IOException ioe) {
                	System.out.println ("Closing connection caught: ");
                    ioe.printStackTrace ();
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
