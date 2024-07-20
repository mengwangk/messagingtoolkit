package com.mbit.screen;

import com.mbit.MainMidlet;
import com.mbit.util.ImageUtils;
import com.mbit.common.Constants;
import com.mbit.update.NewsConfigUpdater;
import com.mbit.valueobject.parameter.ParameterStore;
import com.mbit.valueobject.station.StationComparator;
import com.mbit.valueobject.station.StationRecord;
import com.mbit.valueobject.station.StationStore;
import com.nextel.rms.OAbstractRecord;
import de.enough.polish.util.Locale;

import javax.microedition.lcdui.*;


/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 11, 2006 9:27:10 PM
 * @version : $Id:
 */
public class StationScreen extends List implements CommandListener {


    private MainMidlet midlet;
    private NewsConfigUpdater newsConfigUpdater;
    public ChannelScreen channelScreen;
    public Command updateCmd = new Command(Locale.get("cmd.update"), Command.ITEM, 30);
    public Command addStationCmd = new Command(Locale.get("cmd.addstation"), Command.ITEM, 20);
    private Alert updateAlert;

    /**
     * Constructor
     */
    public StationScreen(MainMidlet midlet) {
        //#style stationScreen
        super(Locale.get("station.title"), List.IMPLICIT);

        this.midlet = midlet;

        setCommandListener(this);

        addCommand(midlet.backCmd);
        addCommand(updateCmd);
        addCommand(addStationCmd);

        // Channel
        channelScreen = new ChannelScreen(midlet);

        try {
            updateAlert = new Alert(Locale.get("updatealert.title"), Locale.get("updatealert.message"),
                    Image.createImage(Constants.APPLICATION_ICON), AlertType.CONFIRMATION);
            updateAlert.setTimeout(Alert.FOREVER);
        } catch (Exception ex) {

        }

    }


    public void setup() {
        // Check if this is the first time it is used
        ParameterStore parameterStore = ParameterStore.getInstance();
        if (parameterStore.getNumRecords() == 0) {
            newsConfigUpdater = new NewsConfigUpdater(midlet);
            newsConfigUpdater.start();
        } else {
            display();
        }
    }

    public void display() {
        deleteAll();

        //style stationBanner
        //append("", null);

        StationStore stationStore = StationStore.getInstance();
        try {
            OAbstractRecord[] stations = stationStore.getAll(new StationComparator(), null);
            for (int i = 0; i < stations.length; i++) {
                StationRecord station = (StationRecord) stations[i];
                //#style stationList
                append(station.getName(), ImageUtils.convertByteArrayToImage(station.getImageData()));
            }
            // Show the current screen
            Display.getDisplay(midlet).setCurrent(this);
        } catch (Exception ex) {
            //#debug
            System.out.println("Error displaying station." + ex.getMessage());
        }

    }

    /**
     * Command action.
     *
     * @param command
     * @param displayable
     */
    public void commandAction(Command command, Displayable displayable) {
        Display display = Display.getDisplay(midlet);

        if (command == midlet.backCmd) {
            display.setCurrent(midlet.mainScreen);
        } else if (command == updateCmd) {
            display.setCurrent(updateAlert, this);
            try {
                ParameterStore.getInstance().deleteAll();
            } catch (Exception ex) {
            }
            newsConfigUpdater = null;
            newsConfigUpdater = new NewsConfigUpdater(midlet);
            newsConfigUpdater.start();
        } else if (command == addStationCmd) {
            
        }

        if (command == List.SELECT_COMMAND) {
            try {
                int selectedItem = getSelectedIndex();
                switch (selectedItem) {
                    /*
                    case 0:
                        // Set advertisment
                        break;
                    */
                    default:
                        StationStore stationStore = StationStore.getInstance();
                        OAbstractRecord[] stations = stationStore.getAll(new StationComparator(), null);
                        StationRecord station = (StationRecord) stations[selectedItem];
                        channelScreen.setup(station);
                        break;
                }
            } catch (Exception ex) {

            }
        }
    }

}
