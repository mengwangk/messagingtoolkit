package messagingclient;

import javax.microedition.lcdui.*;
import javax.microedition.midlet.MIDlet;


/**
 * An example MIDlet to send text via an MMS MessageConnection
 */
public class MMSSend extends MIDlet implements CommandListener {
    /**
     * user interface command for indicating Exit request.
     */
    private static Command CMD_EXIT = new Command ("Exit", Command.EXIT, 2);

    /**
     * user interface command for sending the message
     */
    private static Command CMD_SEND = new Command ("Send", Command.ITEM, 1);

    /**
     * user interface command for adding message's part
     */
    private static Command CMD_ADD_PART = new Command ("Add Part", Command.ITEM, 1);

    /**
     * current display.
     */
    private Display display;

    /**
     * Area where the user enters the id of the message
     */
    private TextField idField;

    /**
     * Area where the user enters the subject of the message
     */
    private TextField subjectField;

    /**
     * Area where the user enters the phone number to send the message to
     */
    private TextField destinationField;

    /**
     * Area where the user enters the phone number to send the message to
     */
    private StringItem partsLabel;

    /**
     * Error message displayed when an invalid phone number is entered
     */
    private Alert errorMessageAlert;

    /**
     * Alert that is displayed when a message is being sent
     */
    private Alert sendingMessageAlert;

    /**
     * The last visible screen when we paused
     */
    private Displayable resumeScreen;
    private MMSMessage message;
    private PartsDialog partsDialog;

    /**
     * Initialize the MIDlet with the current display object and
     * graphical components.
     */
    public MMSSend () {
        String appID = getAppProperty ("MMS-ApplicationID");

        display = Display.getDisplay (this);

        Form mainForm = new Form ("New MMS");

        idField = new TextField ("Message ID:", appID, 256, TextField.ANY);
        mainForm.append (idField);

        subjectField = new TextField ("Subject:", null, 256, TextField.ANY);
        mainForm.append (subjectField);

        destinationField = new TextField ("Destination Address: ", "mms://", 256, TextField.ANY);
        mainForm.append (destinationField);

        partsLabel = new StringItem ("Parts:", "0");
        mainForm.append (partsLabel);

        mainForm.addCommand (CMD_EXIT);
        mainForm.addCommand (CMD_SEND);
        mainForm.addCommand (CMD_ADD_PART);
        mainForm.setCommandListener (this);

        errorMessageAlert = new Alert ("MMS", null, null, AlertType.ERROR);
        errorMessageAlert.setTimeout (5000);

        sendingMessageAlert = new Alert ("MMS", null, null, AlertType.INFO);
        sendingMessageAlert.setTimeout (5000);
        sendingMessageAlert.setCommandListener (this);

        resumeScreen = mainForm;

        message = new MMSMessage ();
    }

    /**
     * startApp should return immediately to keep the dispatcher
     * from hanging.
     */
    public void startApp () {
        display.setCurrent (resumeScreen);
    }

    /**
     * Remember what screen is showing
     */
    public void pauseApp () {
        resumeScreen = display.getCurrent ();
    }

    /**
     * Destroy must cleanup everything.
     *
     * @param unconditional true if a forced shutdown was requested
     */
    public void destroyApp (boolean unconditional) {
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
            else if (c == CMD_ADD_PART) {
                if (partsDialog == null) {
                    partsDialog = new PartsDialog (this);
                }

                partsDialog.show ();
            }
            else if (c == CMD_SEND) {
                promptAndSend ();
            }
        }
        catch (Exception ex) {
            ex.printStackTrace ();
        }
    }

    void show () {
        partsLabel.setText (Integer.toString (partsDialog.counter));
        display.setCurrent (resumeScreen);
    }

    Display getDisplay () {
        return display;
    }

    MMSMessage getMessage () {
        return message;
    }

    /**
     * Prompt for and send the message
     */
    private void promptAndSend () {
        try {
            String id = idField.getString ();
            String address = destinationField.getString ();
            message.setSubject (subjectField.getString ());
            message.setDestination (address);

            String statusMessage = "Sending message to " + address + "...";
            sendingMessageAlert.setString (statusMessage);

            new SenderThread (message, id).start ();
        }
        catch (IllegalArgumentException iae) {
            errorMessageAlert.setString (iae.getMessage ());
            display.setCurrent (errorMessageAlert);
        }
    }
}
