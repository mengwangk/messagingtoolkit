package messagingclient;

import javax.microedition.io.Connector;
import javax.microedition.io.PushRegistry;
import javax.microedition.lcdui.*;
import javax.microedition.midlet.MIDlet;
import javax.wireless.messaging.*;
import java.io.IOException;


/**
 * An example MIDlet displays text from an MMS MessageConnection
 */
public class MMSReceive extends MIDlet implements CommandListener, Runnable, MessageListener {
    /**
     * user interface command for indicating Exit request.
     */
    private static final Command CMD_EXIT = new Command ("Exit", Command.EXIT, 2);

    /**
     * user interface text box for the contents of the fetched URL.
     */
    private Form content;

    /**
     * current display.
     */
    private Display display;

    /**
     * instance of a thread for asynchronous networking and user interface.
     */
    private Thread thread;

    /**
     * Flag to signal end of processing.
     */
    private boolean done;

    /**
     * The applicationID on which we listen for MMS messages
     */
    private String appID;

    /**
     * MMS message connection for inbound text messages.
     */
    private MessageConnection mmsconn;

    /**
     * The screen to display when we return from being paused
     */
    private Displayable resumeScreen;

    /**
     * Initialize the MIDlet with the current display object and
     * graphical components.
     */
    public MMSReceive () {
        appID = getAppProperty ("MMS-ApplicationID");

        display = Display.getDisplay (this);

        content = new Form ("MMS Receive");
        content.addCommand (CMD_EXIT);
        content.setCommandListener (this);
        content.append ("Receiving...");

        Alert sendingMessageAlert = new Alert ("MMS", null, null, AlertType.INFO);
        sendingMessageAlert.setTimeout (5000);
        sendingMessageAlert.setCommandListener (this);

        resumeScreen = content;
    }

    /**
     * Start creates the thread to do the MessageConnection receive
     * text.
     * It should return immediately to keep the dispatcher
     * from hanging.
     */
    public void startApp () {
        /** MMS connection to be read. */
        String mmsConnection = "mms://:" + appID;

        /** Open the message connection. */
        if (mmsconn == null) {
            try {
                mmsconn = (MessageConnection) Connector.open (mmsConnection);
                mmsconn.setMessageListener (this);
            }
            catch (IOException ioe) {
                ioe.printStackTrace ();
            }
        }

        /** Initialize the text if we were started manually. */
        String[] connections = PushRegistry.listConnections (true);

        if ((connections == null) || (connections.length == 0)) {
            content.deleteAll ();
            content.append ("Waiting for MMS on applicationID " + appID + "...");
        }

        done = false;

        /* Check whether there is already a message waiting */
        if ((thread == null) && !done) {
            thread = new Thread (this);
            thread.start ();
        }

        display.setCurrent (resumeScreen);
    }

    /**
     * Notification that a message arrived.
     *
     * @param conn the connection with messages available
     */
    public void notifyIncomingMessage (MessageConnection conn) {

        if ((thread == null) && !done) {
            thread = new Thread (this);
            thread.start ();
        }
    }

    /**
     * Message reading thread.
     */
    public void run () {
        /** Check for mms connection. */
        try {
            Message msg = mmsconn.receive ();

            if (msg != null) {
                String senderAddress = msg.getAddress ();
                content.deleteAll ();

                String titleStr = senderAddress.substring (6);
                int colonPos = titleStr.indexOf (":");

                if (colonPos != -1) {
                    titleStr = titleStr.substring (0, colonPos);
                }

                content.setTitle ("From: " + titleStr);

                if (msg instanceof MultipartMessage) {
                    MultipartMessage mpm = (MultipartMessage) msg;
                    StringBuffer buff = new StringBuffer ("Subject: ");
                    buff.append ((mpm.getSubject ()));
                    buff.append ("\nDate: ");
                    if (mpm.getTimestamp () != null) {
                        buff.append (mpm.getTimestamp ().toString ());
                        buff.append ("\nContent:");
                    }

                    StringItem messageItem = new StringItem ("Message", buff.toString ());
                    messageItem.setLayout (Item.LAYOUT_NEWLINE_AFTER);
                    content.append (messageItem);

                    MessagePart[] parts = mpm.getMessageParts ();

                    if (parts != null) {
                        for (int i = 0; i < parts.length; i++) {
                            buff = new StringBuffer ();

                            MessagePart mp = parts[i];
                            buff.append ("Content-Type: ");

                            String mimeType = mp.getMIMEType ();
                            buff.append (mimeType);

                            String contentLocation = mp.getContentLocation ();
                            buff.append ("\nContent:\n");

                            byte[] ba = mp.getContent ();

                            try {
                                Image image = Image.createImage (ba, 0, ba.length);
                                content.append (buff.toString ());

                                ImageItem imageItem =
                                    new ImageItem (
                                        contentLocation, image,
                                        Item.LAYOUT_NEWLINE_AFTER, contentLocation);
                                content.append (imageItem);
                            }
                            catch (IllegalArgumentException iae) {
                                buff.append (new String (ba));

                                StringItem stringItem = new StringItem ("Part", buff.toString ());
                                stringItem.setLayout (Item.LAYOUT_NEWLINE_AFTER);
                                content.append (stringItem);
                            }
                        }
                    }
                }

                display.setCurrent (content);
            }
        }
        catch (IOException e) {
            e.printStackTrace ();
        }
    }

    /**
     * Pause signals the thread to stop by clearing the thread field.
     * If stopped before done with the iterations it will
     * be restarted from scratch later.
     */
    public void pauseApp () {
        done = true;
        thread = null;
        resumeScreen = display.getCurrent ();
    }

    /**
     * Destroy must cleanup everything.  The thread is signaled
     * to stop and no result is produced.
     *
     * @param unconditional true if a forced shutdown was requested
     */
    public void destroyApp (boolean unconditional) {
        done = true;
        thread = null;

        if (mmsconn != null) {
            try {
                mmsconn.close ();
            }
            catch (IOException e) {
                // Ignore any errors on shutdown
            }
        }
    }

    /**
     * Respond to commands, including exit
     *
     * @param c user interface command requested
     * @param s screen object initiating the request
     */
    public void commandAction (Command c, Displayable s) {
        try {
            if ((c == CMD_EXIT) || (c == Alert.DISMISS_COMMAND)) {
                destroyApp (false);
                notifyDestroyed ();
            }
        }
        catch (Exception ex) {
            ex.printStackTrace ();
        }
    }
}
