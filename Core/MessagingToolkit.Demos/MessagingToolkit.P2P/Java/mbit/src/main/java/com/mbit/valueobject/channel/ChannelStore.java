package com.mbit.valueobject.channel;

import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OAbstractStore;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 4, 2006 10:26:11 AM
 * @version : $Id:
 */
public class ChannelStore extends OAbstractStore {

    private static ChannelStore channelStore;

    /**
     * Constructor
     */
    private ChannelStore() {
        super("channel", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>ChannelStore</code>.
     *
     * @return An instance of a ChannelStore
     */
    public static ChannelStore getInstance() {
        if (channelStore == null)
            channelStore = new ChannelStore();

        return channelStore;
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new ChannelRecord.
     */
    protected OAbstractRecord createRecord() {
        return new ChannelRecord();
    }

}
