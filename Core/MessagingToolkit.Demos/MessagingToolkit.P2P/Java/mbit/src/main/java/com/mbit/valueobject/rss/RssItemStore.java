package com.mbit.valueobject.rss;

import com.nextel.rms.OAbstractStore;
import com.nextel.rms.OAbstractRecord;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 5, 2006 5:43:02 PM
 * @version : $Id:
 */
public class RssItemStore extends OAbstractStore {

    private static RssItemStore itemStore;

    /**
     * Constructor
     */
    private RssItemStore() {
        super("rssitem", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>RssItemStore</code>.
     *
     * @return An instance of a RssItemStore
     */
    public static RssItemStore getInstance() {
        if (itemStore == null)
            itemStore = new RssItemStore();

        return itemStore;
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new RssItemRecord.
     */
    protected OAbstractRecord createRecord() {
        return new RssItemRecord();
    }

}

