package messagingtoolkit.p2pclient;

import java.io.IOException;
import javax.microedition.io.*;
import javax.wireless.messaging.*;

/**
 * MMS sender
 *
 */
public class MmsSender extends Thread {

    private MmsMessage message;
    private String appID;

    public MmsSender(MmsMessage message, String appID) {
        this.message = message;
        this.appID = appID;
    }

    /**
     * Send the message. Called on a separate thread so we don't have
     * contention for the display
     */
    public void run() {
        String address = message.getDestination() + ":" + appID;

        MessageConnection mmsconn = null;

        try {
            // Open the message connection
            mmsconn = (MessageConnection) Connector.open(address);

            MultipartMessage mmmessage =
                    (MultipartMessage) mmsconn.newMessage(MessageConnection.TEXT_MESSAGE);
            mmmessage.setAddress(address);

            MessagePart[] parts = message.getParts();

            for (int i = 0; i < parts.length; i++) {
                mmmessage.addMessagePart(parts[i]);
            }
            mmmessage.setSubject(message.getSubject());
            mmsconn.send(mmmessage);
        } catch (Exception e) {
            e.printStackTrace();
        }

        if (mmsconn != null) {
            try {
                mmsconn.close();
            } catch (IOException ioe) {
                ioe.printStackTrace();
            }
        }
    }
}
