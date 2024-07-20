package messagingclient;

import javax.microedition.io.Connector;
import javax.wireless.messaging.MessageConnection;
import javax.wireless.messaging.MessagePart;
import javax.wireless.messaging.MultipartMessage;
import java.io.IOException;


public class SenderThread extends Thread {
    private MMSMessage message;
    private String appID;

    public SenderThread (MMSMessage message, String appID) {
        this.message = message;
        this.appID = appID;
    }

    /**
     * Send the message. Called on a separate thread so we don't have
     * contention for the display
     */
    public void run () {
        String address = message.getDestination () + ":" + appID;
        MessageConnection mmsconn = null;

        try {
            /** Open the message connection. */
            mmsconn = (MessageConnection) Connector.open (address);

            MultipartMessage mmmessage =
                (MultipartMessage) mmsconn.newMessage (MessageConnection.TEXT_MESSAGE);
            mmmessage.setAddress (address);

            MessagePart[] parts = message.getParts ();

            for (int i = 0; i < parts.length; i++) {            	
                mmmessage.addMessagePart (parts[i]);
            }

            mmmessage.setSubject (message.getSubject ());

            mmsconn.send (mmmessage);
        }
        catch (Exception e) {
            e.printStackTrace ();
        }

        if (mmsconn != null) {
            try {
                mmsconn.close ();
            }
            catch (IOException ioe) {
                ioe.printStackTrace ();
            }
        }
    }
}
