package com.mbit.valueobject.advertisement;

import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OAbstractStore;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 10, 2006 12:08:37 PM
 * @version : $Id:
 */
public class AdStore extends OAbstractStore {

    private static AdStore adStore;

    /**
     * Constructor
     */
    private AdStore() {
        super("advertisement", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>AdStore</code>.
     *
     * @return An instance of a AdStore
     */
    public static AdStore getInstance() {
        if (adStore == null)
            adStore = new AdStore();

        return adStore;
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new AdRecord.
     */
    protected OAbstractRecord createRecord() {
        return new AdRecord();
    }

}
