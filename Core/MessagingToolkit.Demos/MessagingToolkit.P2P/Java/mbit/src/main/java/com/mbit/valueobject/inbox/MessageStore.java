package com.mbit.valueobject.inbox;

import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OAbstractStore;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 11, 2006 11:41:24 AM
 * @version : $Id:
 */
public class MessageStore extends OAbstractStore {

    private static MessageStore messageStore;

    /**
     * Constructor
     */
    private MessageStore() {
        super("message", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>MessageStore</code>.
     *
     * @return An instance of a MessageStore
     */
    public static MessageStore getInstance() {
        if (messageStore == null)
            messageStore = new MessageStore();

        return messageStore;
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new MessageRecord.
     */
    protected OAbstractRecord createRecord() {
        return new MessageRecord();
    }

}
