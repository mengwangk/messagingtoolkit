package com.mbit;

import com.mbit.base.BaseMidlet;
import com.mbit.screen.*;
import com.mbit.valueobject.rss.RssFeedStore;
import com.mbit.valueobject.rss.RssItemStore;
import com.mbit.valueobject.advertisement.AdStore;
import com.mbit.valueobject.station.StationStore;
import com.mbit.valueobject.channel.ChannelStore;
import com.mbit.valueobject.parameter.ParameterStore;
import com.mbit.valueobject.inbox.MessageStore;

import javax.microedition.midlet.MIDletStateChangeException;
import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Display;

import de.enough.polish.util.Locale;

//#ifdef polish.debugEnabled
import de.enough.polish.util.Debug;
//#endif

import java.util.Random;


/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 11, 2006 9:38:19 PM
 * @version : $Id:
 */
public class MainMidlet extends BaseMidlet {

    public Command backCmd = new Command(Locale.get("cmd.back"), Command.BACK, 1);
    public Command exitCmd = new Command(Locale.get("cmd.exit"), Command.EXIT, 2);
    public Command okCmd = new Command(Locale.get("cmd.ok"), Command.OK, 3);
    public Command cancelCmd = new Command(Locale.get("cmd.cancel"), Command.CANCEL, 4);
    public Command stopCmd = new Command(Locale.get("cmd.stop"), Command.STOP, 5);
    public Command saveCmd = new Command(Locale.get("cmd.save"), Command.ITEM, 6);

    public MainScreen mainScreen;
    public MySpaceScreen mySpaceScreen;
    public StationScreen stationScreen;
    public RecentScreen recentScreen;
    public HotScreen hotScreen;
    public CheckforUpdateScreen checkforUpdateScreen;
    public long sessionId;
    private boolean isOnline;


    /**
     * Constructor.
     */
    public MainMidlet() {
        mainScreen = new MainScreen(this);
        mySpaceScreen = new MySpaceScreen(this);
        stationScreen = new StationScreen(this);
        recentScreen = new RecentScreen(this);
        hotScreen = new HotScreen(this);
        checkforUpdateScreen = new CheckforUpdateScreen(this);
    }

    /**
     * Start up application
     *
     * @throws MIDletStateChangeException
     */
    protected void startApp() throws MIDletStateChangeException {
        Display display = Display.getDisplay(this);
        display.setCurrent(mainScreen);
        // Create a session key
        sessionId = createSessionId();
        isOnline = false;

      /*  ParameterStore parameterStore = ParameterStore.getInstance();
        StationStore stationStore = StationStore.getInstance();
        ChannelStore channelStore = ChannelStore.getInstance();
        RssFeedStore feedStore = RssFeedStore.getInstance();
        RssItemStore itemStore = RssItemStore.getInstance();
        AdStore adStore = AdStore.getInstance();
        MessageStore msgStore = MessageStore.getInstance();

        try {
            parameterStore.deleteAll();
            stationStore.deleteAll();
            channelStore.deleteAll();
            feedStore.deleteAll();
            itemStore.deleteAll();
            adStore.deleteAll();
            msgStore.deleteAll();
        } catch (Exception ex) {

        }*/

    }

    /**
     * Pause application
     */
    protected void pauseApp() {

    }

    /**
     * Destroy application
     *
     * @param b
     * @throws MIDletStateChangeException
     */
    protected void destroyApp(boolean b) throws MIDletStateChangeException {

    }

    /**
     * Command listener class.
     *
     * @param command
     * @param displayable
     */
    public void commandAction(Command command, Displayable displayable) {

    }

    /**
     * Exit from the system
     */
    public void quit() {
        notifyDestroyed();
    }

    public long createSessionId() {
        Random random = new Random();
        return (random.nextLong() + 100);
    }

    public long getSessionId() {
        return sessionId;
    }

    public boolean isOnline() {
        return isOnline;
    }

    public void setOnline(boolean online) {
        isOnline = online;
    }
}
