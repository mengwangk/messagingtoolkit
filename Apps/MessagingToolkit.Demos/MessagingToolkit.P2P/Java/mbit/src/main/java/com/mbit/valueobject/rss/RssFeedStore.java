package com.mbit.valueobject.rss;

import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OAbstractStore;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 5, 2006 5:42:56 PM
 * @version : $Id:
 */
public class RssFeedStore extends OAbstractStore {

    private static RssFeedStore feedStore;

    /**
     * Constructor
     */
    private RssFeedStore() {
        super("rssfeed", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>RssFeedStore</code>.
     *
     * @return An instance of a RssFeedStore
     */
    public static RssFeedStore getInstance() {
        if (feedStore == null)
            feedStore = new RssFeedStore();

        return feedStore;
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new RssFeedRecord
     */
    protected OAbstractRecord createRecord() {
        return new RssFeedRecord();
    }

}

