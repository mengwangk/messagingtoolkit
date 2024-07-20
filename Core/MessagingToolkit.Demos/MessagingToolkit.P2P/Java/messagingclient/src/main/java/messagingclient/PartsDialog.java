package messagingclient;

import javax.microedition.lcdui.*;
import javax.wireless.messaging.MessagePart;
import java.io.InputStream;


public class PartsDialog implements CommandListener {
    private static final Command CMD_BACK = new Command ("Back", Command.BACK, 1);
    private static final Command CMD_NEXT = new Command ("Next", Command.OK, 1);
    private static final Command CMD_OK = new Command ("OK", Command.OK, 1);
    private static final Command CMD_CANCEL = new Command ("Cancel", Command.CANCEL, 1);

    /**
     * current display.
     */
    private MMSSend mmsSend;
    private List typeList;
    public int counter = 0;

    /**
     * Creates a new instance of PartsDialog
     */
    public PartsDialog (MMSSend mmsSend) {
        this.mmsSend = mmsSend;

        String[] stringArray = {"Text", "Image"};

        typeList = new List ("Add Part: Type", Choice.EXCLUSIVE, stringArray, null);
        typeList.addCommand (CMD_BACK);
        typeList.addCommand (CMD_NEXT);
        typeList.setCommandListener (this);
    }

    public void show () {
        mmsSend.getDisplay ().setCurrent (typeList);
    }

    /**
     * Respond to commands, including exit
     *
     * @param c user interface command requested
     * @param s screen object initiating the request
     */
    public void commandAction (Command c, Displayable s) {
        try {
            if (c == CMD_BACK) {
                mmsSend.show ();
            }
            else if (c == CMD_NEXT) {
                if (typeList.getSelectedIndex () == 0) {
                    mmsSend.getDisplay ().setCurrent (new TextDialog ());
                }
                else {
                    mmsSend.getDisplay ().setCurrent (new ImageDialog ());
                }
            }
        }
        catch (Exception ex) {
            ex.printStackTrace ();
        }
    }

    private class TextDialog extends Form implements CommandListener {
        private TextField text;
        private String mimeType = "text/plain";

        public TextDialog () {
            super ("Add Text");

            text = new TextField ("Text: ", null, 256, TextField.ANY);
            append (text);
            append ("MIME-Type: " + mimeType);

            addCommand (CMD_OK);
            addCommand (CMD_CANCEL);
            setCommandListener (this);
        }

        public void commandAction (Command c, Displayable s) {
            try {
                if (c == CMD_OK) {
                    String encoding = "UTF-8";
                    byte[] contents = text.getString ().getBytes (encoding);
                    mmsSend.getMessage ()
                        .addPart (
                            new MessagePart (
                                contents, 0, contents.length, mimeType,
                                "id" + counter, "contentLocation", encoding));
                    counter++;
                    mmsSend.show ();
                }
                else if (c == CMD_CANCEL) {
                    mmsSend.show ();
                }
            }
            catch (Exception ex) {
                ex.printStackTrace ();
            }
        }
    }

    private class ImageDialog extends Form implements CommandListener {
        private ChoiceGroup cg;
        private String mimeType = "image/png";
        private String[] resouces = {"/Duke1.png", "/Duke2.png"};
        private String[] imagesNames = {"Duke 1", "Duke 2"};

        public ImageDialog () {
            super ("Add Image");

            cg = new ChoiceGroup ("Select Image", Choice.EXCLUSIVE, imagesNames, null);
            append (cg);

            append ("MIME-Type: " + mimeType);

            addCommand (CMD_OK);
            addCommand (CMD_CANCEL);
            setCommandListener (this);
        }

        public void commandAction (Command c, Displayable s) {
            try {
                if (c == CMD_OK) {
                    int index = cg.getSelectedIndex ();
                    String resouce = resouces[index];
                    InputStream is = getClass ().getResourceAsStream (resouce);
                    byte[] contents = new byte[is.available ()];
                    is.read (contents);

                    String contentLocation = imagesNames[index];
                    mmsSend.getMessage ()
                        .addPart (
                            new MessagePart (
                                contents, 0, contents.length, mimeType,
                                "id" + counter, contentLocation, null));
                    counter++;
                    mmsSend.show ();
                }
                else if (c == CMD_CANCEL) {
                    mmsSend.show ();
                }
            }
            catch (Exception ex) {
                ex.printStackTrace ();
            }
        }
    }
}
