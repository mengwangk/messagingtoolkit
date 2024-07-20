package com.mbit.screen;

import com.mbit.MainMidlet;
import com.mbit.custom.ImageTicker;
import com.mbit.util.ImageUtils;
import com.mbit.common.Constants;
import com.mbit.messaging.sms.SMSSender;
import com.mbit.valueobject.inbox.MessageComparator;
import com.mbit.valueobject.inbox.MessageRecord;
import com.mbit.valueobject.inbox.MessageStore;
import com.nextel.rms.OAbstractRecord;
import de.enough.polish.ui.Screen;
import de.enough.polish.ui.ScreenStateListener;
import de.enough.polish.ui.TabbedForm;
import de.enough.polish.util.Locale;

import javax.microedition.lcdui.*;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 16, 2006 2:44:33 PM
 * @version : $Id:
 */
public class MySpaceScreen extends TabbedForm
        implements ScreenStateListener, CommandListener, ItemCommandListener {

    private int lastTabIndex;
    private MainMidlet midlet;
    private Display display;
    //private ChoiceGroup msgList;
    private Image msgImage;

    private Command viewCmd = new Command(Locale.get("cmd.view"), Command.ITEM, 10);
    private Command sendCmd = new Command(Locale.get("cmd.send"), Command.ITEM, 20);
    private Command deleteCmd = new Command(Locale.get("cmd.delete"), Command.ITEM, 30);

    private Alert msgBox;

    private MessageDetailsScreen messageDetailsScreen;

    private SendMessageScreen sendMessageScreen;

    /**
     * Constructor
     */
    public MySpaceScreen(MainMidlet midlet) {
        //#style tabbedForm
        super(Locale.get("myspace.title"),
                new String[]{
                        Locale.get("myspace.inbox"),
                        Locale.get("myspace.outbox"),
                        Locale.get("myspace.favorite"),
                        Locale.get("myspace.group"),
                        Locale.get("myspace.blog")
                }, null);

        this.midlet = midlet;
        setScreenStateListener(this);
        setCommandListener(this);

        addCommand(midlet.backCmd);
        addCommand(viewCmd);
        addCommand(deleteCmd);
        addCommand(sendCmd);

        display = Display.getDisplay(midlet);
        lastTabIndex = -1;
        /*
        msgList = new ChoiceGroup("", ChoiceGroup.EXCLUSIVE);
        msgList.setDefaultCommand(viewCmd);
        msgList.setItemCommandListener(this);
        */
        try {
            msgImage = Image.createImage(Constants.NEWS_IMAGE);
        } catch (Exception ex) {

        }

        try {
            msgBox = new Alert(Locale.get("myspace.inbox"), Locale.get("inbox.msgdeleted"),
                    Image.createImage(Constants.APPLICATION_ICON), AlertType.CONFIRMATION);
            msgBox.setTimeout(Alert.FOREVER);
        } catch (Exception ex) {

        }

        messageDetailsScreen = new MessageDetailsScreen(midlet);
        sendMessageScreen = new SendMessageScreen(midlet);
    }


    public void setup() {
        this.setActiveTab(0);

    }

    public void display() {
        // manually call screenStateChanged, so that commands are updated accordingly:
        screenStateChanged(this);
        Display.getDisplay(midlet).setCurrent(this);
    }

    public void screenStateChanged(Screen screen) {
        int tabIndex = this.getSelectedTab();
        if (tabIndex != this.lastTabIndex) {
            //#debug
            System.out.println("ScreenStateChanged: new tab=" + tabIndex);
            if (tabIndex == 0) {
                deleteAll(0);
                //msgList.deleteAll();

                // Inbox
                MessageStore msgStore = MessageStore.getInstance();
                try {
                    OAbstractRecord[] messages = msgStore.getAll(new MessageComparator(), null);
                    if (messages != null && messages.length > 0) {
                        Image image = null;
                        for (int i = 0; i < messages.length; i++) {
                            MessageRecord msg = (MessageRecord) messages[i];
                            if (msg.getImageData1() != null) {
                                image = ImageUtils.createThumbnail(ImageUtils.convertByteArrayToImage(msg.getImageData1()));
                            } else {
                                image = msgImage;
                            }
                            ImageTicker item = new ImageTicker(
                                    image,
                                    msg.getTitle().trim(),
                                    getWidth()
                            );
                            item.setDefaultCommand(viewCmd);
                            item.setItemCommandListener(this);
                            //msgList.append(msg.getTitle().trim(), image);
                            append(0, item);
                        }
                        //append(0, msgList);
                    }

                } catch (Exception ex) {
                    //#debug
                    System.out.println(ex.getMessage());
                }
            } else if (tabIndex == 1) {


            }
            this.lastTabIndex = tabIndex;
        }
    }

    public void commandAction(Command cmd, Displayable disp) {
        int tabIndex = this.getSelectedTab();
        if (cmd == midlet.backCmd) {
            display.setCurrent(midlet.mainScreen);
        } else if (cmd == deleteCmd) {
            if (tabIndex == 0) {
                // Inbox
                int selectedItem = getCurrentIndex();

                // Remove from screen
                delete(0, selectedItem);

                // Remove from storage
                MessageStore msgStore = MessageStore.getInstance();
                try {
                    OAbstractRecord[] messages = msgStore.getAll(new MessageComparator(), null);
                    msgStore.deleteRecord(messages[selectedItem]);
                    msgBox.setString(Locale.get("inbox.msgdeleted"));
                    display.setCurrent(msgBox, this);
                } catch (Exception ex) {
                    msgBox.setString(ex.getMessage());
                    display.setCurrent(msgBox, this);
                }
            }
        } else if (cmd == sendCmd) {
            int selectedItem = getCurrentIndex();
            if (tabIndex == 0) {

                MessageStore msgStore = MessageStore.getInstance();
                try {
                    OAbstractRecord[] messages = msgStore.getAll(new MessageComparator(), null);
                    sendMessageScreen.setup();
                    sendMessageScreen.display((MessageRecord) messages[selectedItem]);
                } catch (Exception ex) {
                    //#debug
                    System.out.println(ex.getMessage());
                }
            }

            /*
            SMSSender smsSender = new SMSSender(midlet);
            smsSender.setDestinationAddress("+60192292309");
            smsSender.setMessage("test message for wma 1.1");
            new Thread(smsSender).start();
            */

        }
    }

    public void commandAction(Command command, Item item) {
        int tabIndex = this.getSelectedTab();
        if (command == viewCmd) {
            int selectedItem = getCurrentIndex();
            // Display the news
            if (selectedItem >= 0) {
                if (tabIndex == 0) {
                    MessageStore msgStore = MessageStore.getInstance();
                    try {
                        OAbstractRecord[] messages = msgStore.getAll(new MessageComparator(), null);
                        messageDetailsScreen.setup();
                        messageDetailsScreen.display((MessageRecord) messages[selectedItem]);
                    } catch (Exception ex) {
                        //#debug
                        System.out.println(ex.getMessage());
                    }
                }

            }
        }
    }
}
