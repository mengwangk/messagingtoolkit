package com.mbit.valueobject.station;

import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OAbstractStore;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 29, 2006 5:35:20 PM
 * @version : $Id:
 */
public class StationStore extends OAbstractStore {

    private static StationStore stationStore;

    /**
     * Constructor
     */
    private StationStore() {
        super("station", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>StationStore</code>.
     *
     * @return An instance of a StationStore
     */
    public static StationStore getInstance() {
        if (stationStore == null)
            stationStore = new StationStore();

        return stationStore;
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new StationRecord.
     */
    protected OAbstractRecord createRecord() {
        return new StationRecord();
    }

}