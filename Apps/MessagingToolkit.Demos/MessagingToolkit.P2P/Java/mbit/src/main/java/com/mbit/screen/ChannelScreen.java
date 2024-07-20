package com.mbit.screen;

import com.mbit.MainMidlet;
import com.mbit.update.NewsUpdater;
import com.mbit.util.ImageUtils;
import com.mbit.valueobject.channel.ChannelComparator;
import com.mbit.valueobject.channel.ChannelFilterByStation;
import com.mbit.valueobject.channel.ChannelRecord;
import com.mbit.valueobject.channel.ChannelStore;
import com.mbit.valueobject.station.StationRecord;
import com.nextel.rms.OAbstractRecord;
import de.enough.polish.util.Locale;

import javax.microedition.lcdui.*;


/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 11, 2006 11:58:32 PM
 * @version : $Id:
 */
public class ChannelScreen extends List implements CommandListener {

    private MainMidlet midlet;
    private StationRecord station;
    private OAbstractRecord[] channels;
    public NewsScreen newsScreen;

    /**
     * Constructor
     */
    public ChannelScreen(MainMidlet midlet) {
        //#style displayScreen
        super(Locale.get("channel.title"), List.IMPLICIT);

        this.midlet = midlet;

        setCommandListener(this);
        addCommand(midlet.backCmd);

        newsScreen = new NewsScreen(midlet, this);

    }

    public void setup(StationRecord station) {
        this.station = station;

        // Display the channel screen
        display();
    }

    public void display() {
        deleteAll();

        // Display the channels
        ChannelStore channelStore = ChannelStore.getInstance();
        try {
            channels = channelStore.getAll(new ChannelComparator(), new ChannelFilterByStation(station.getName()));
            if (channels != null && channels.length > 0) {
                for (int i = 0; i < channels.length; i++) {
                    ChannelRecord channel = (ChannelRecord) channels[i];
                    Image image = null;
                    if (channel.getImageData() != null) {
                        image = ImageUtils.convertByteArrayToImage(channel.getImageData());
                    }
                    //#style channelList
                    append(channel.getName(), image);
                }
            }
        } catch (Exception ex) {

        }
        Display.getDisplay(midlet).setCurrent(this);
    }


    public void commandAction(Command cmd, Displayable disp) {
        if (cmd == midlet.backCmd) {
            Display.getDisplay(midlet).setCurrent(midlet.stationScreen);
        } else if (cmd == List.SELECT_COMMAND) {
            try {
                int selectedItem = getSelectedIndex();
                ChannelRecord channel = (ChannelRecord) channels[selectedItem];
                NewsUpdater newsUpdater = new NewsUpdater(midlet, this, channel);
                newsUpdater.start();
            } catch (Exception ex) {
                //#debug
                System.out.println(ex.getMessage());

            }
        }
    }
}