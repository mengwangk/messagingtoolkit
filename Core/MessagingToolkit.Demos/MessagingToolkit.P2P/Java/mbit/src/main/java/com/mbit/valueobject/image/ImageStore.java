package com.mbit.valueobject.image;

import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OAbstractStore;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 14, 2006 10:14:23 AM
 * @version : $Id:
 */
public class ImageStore extends OAbstractStore {

    private static ImageStore imageStore;

    /**
     * Constructor
     */
    private ImageStore() {
        super("image", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>ImageStore</code>.
     *
     * @return An instance of a ImageStore
     */
    public static ImageStore getInstance() {
        if (imageStore == null)
            imageStore = new ImageStore();

        return imageStore;
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new ImageRecord.
     */
    protected OAbstractRecord createRecord() {
        return new ImageRecord();
    }

}
