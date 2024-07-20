package com.mbit.update;

import com.mbit.MainMidlet;
import com.mbit.common.ErrorCode;
import com.mbit.valueobject.channel.ChannelStore;
import com.mbit.valueobject.station.StationRecord;
import com.mbit.valueobject.station.StationStore;
import com.nextel.rms.OAbstractRecord;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 18, 2006 11:27:09 PM
 * @version : $Id:
 */
public class NewsConfigUpdater extends Thread {

    private MainMidlet midlet;


    /**
     * Constructor
     *
     * @param midlet
     */
    public NewsConfigUpdater(MainMidlet midlet) {
        this.midlet = midlet;
    }


    public void run() {
        // Update parameter store
        ParameterUpdate parameterUpdate = new ParameterUpdate(midlet);
        parameterUpdate.run();
        if (parameterUpdate.getErrorCode() == ErrorCode.NO_ERROR) {

            // Update station store
            StationUpdate stationUpdate = new StationUpdate(midlet);
            stationUpdate.run();
            if (stationUpdate.getErrorCode() != ErrorCode.NO_ERROR) {
                //#debug
                System.out.println("Error: " + stationUpdate.getErrorMsg());
                //#style stationList
                midlet.stationScreen.append(stationUpdate.getErrorMsg(), null);

            } else {
                // For each station update the channel
                StationStore stationStore = StationStore.getInstance();
                try {
                    //OAbstractRecord[] stations = stationStore.getAll(new StationComparator(), null);
                    OAbstractRecord[] stations = stationStore.getAll();
                    //#debug
                    System.out.println("No of stations: " + stationStore.getNumRecords());

                    try {
                        ChannelStore channelStore = ChannelStore.getInstance();
                        channelStore.deleteAll();  // to clean up
                    } catch (Exception ex) {
                        //#debug
                        System.out.println(ex.getMessage());
                    }

                    for (int i = 0; i < stations.length; i++) {
                        StationRecord station = (StationRecord) stations[i];
                        ChannelUpdate channelUpdate = new ChannelUpdate(midlet, station);
                        channelUpdate.run();
                        if (channelUpdate.getErrorCode() != ErrorCode.NO_ERROR) {
                            //#style stationList
                            midlet.stationScreen.append(channelUpdate.getErrorMsg(), null);
                        }
                    }

                } catch (Exception ex) {
                    //#debug
                    System.out.println("Error retrieving station from store." + ex.getMessage());
                }
            }
        } else {
            //#style stationList
            midlet.stationScreen.append(parameterUpdate.getErrorMsg(), null);
        }
        midlet.stationScreen.display();
    }
}
