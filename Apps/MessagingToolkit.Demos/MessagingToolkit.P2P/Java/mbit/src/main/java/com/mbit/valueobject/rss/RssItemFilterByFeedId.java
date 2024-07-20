package com.mbit.valueobject.rss;

import javax.microedition.rms.RecordFilter;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 7, 2006 8:04:31 PM
 * @version : $Id:
 */
public class RssItemFilterByFeedId implements RecordFilter {

    public static int ANY_FEEDID = -1;
    public static int NO_DEADLINE = 0;
    public static int READ_AND_UNREAD = 0;
    public static int READ = 1;
    public static int UNREAD = 2;

    private int feedId;
    private ByteArrayInputStream bin;
    private DataInputStream din;

    /**
     * Constructor for the class.
     *
     * @param feedId the id of the feed the item should be from
     *                  (Use class field if any feed)
     */
    public RssItemFilterByFeedId(int feedId) {
        this.feedId = feedId;
    }

    /**
     * Set the feed id.
     *
     * @param newFeedId the new feed id
     */
    public void setFeedId(int newFeedId) {
        feedId = newFeedId;
    }

    /**
     * Checks to see if the given item is a match.
     * Returns <code>TRUE</code> if feedId is set and it matches the
     * candidate items feedId.
     * If the deadline is set it checks that the candidate item is created
     * BEFORE the deadline (in millisecs since 1/1 1970).
     *
     * @param candidate the candidate item as a byte[]
     * @return <code>TRUE</code> if the given candidate item is a match.
     */
    public boolean matches(byte[] candidate) {
        try {
            boolean isMatch = true;
            bin = new ByteArrayInputStream(candidate);
            din = new DataInputStream(bin);
            // Check feed id
            if (feedId > ANY_FEEDID) {
                int id = din.readInt();
                isMatch = (id == feedId);
            } else {
                // Match if any feed id
                din.readInt();
            }
            din.close();
            bin.close();
            return isMatch;
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return false;
        }
    }
}

