package com.mbit.update;

import com.mbit.MainMidlet;
import com.mbit.common.ErrorCode;
import com.mbit.screen.ChannelScreen;
import com.mbit.valueobject.channel.ChannelRecord;
import com.mbit.valueobject.rss.*;
import com.nextel.rms.OAbstractRecord;


/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 8, 2006 11:16:49 PM
 * @version : $Id:
 */
public class NewsUpdater extends Thread {

    private MainMidlet midlet;
    private ChannelScreen channelScreen;
    private ChannelRecord channel;
    private RssFeedRecord rssFeed;

    /**
     * Constructor
     */
    public NewsUpdater(MainMidlet midlet, ChannelScreen channelScreen, ChannelRecord channel) {
        this.midlet = midlet;
        this.channelScreen = channelScreen;
        this.channel = channel;
    }

    public void run() {
        // Retrieve the existing feed for this channel
        RssFeedStore feedStore = RssFeedStore.getInstance();
        RssItemStore itemStore = RssItemStore.getInstance();
        try {
            OAbstractRecord[] feeds = feedStore.getAll(null, new RssFeedFilterByUrl(channel.getUrl()));
            OAbstractRecord[] items = null;

            if (feeds != null && feeds.length > 0) {
                rssFeed = (RssFeedRecord) feeds[0];
                items = itemStore.getAll(new RssItemComparator(), new RssItemFilterByFeedId(rssFeed.getRecordId()));

                // Check the session id
                if (rssFeed.getSessionId() == midlet.getSessionId()) {
                    //#debug
                    System.out.println("Retrieving from cache since in same session.");
                    for (int i = 0; i < items.length; i++) {
                        rssFeed.addItem((RssItemRecord) items[i]);
                    }
                    // Display the news
                    channelScreen.newsScreen.setup(channel);
                    channelScreen.newsScreen.display(rssFeed);
                    return;
                }
            }

            // Need to retrieve
            rssFeed = new RssFeedRecord();
            rssFeed.setUrl(channel.getUrl());
            FeedUpdate feedUpdate = new FeedUpdate(midlet, channel, rssFeed);
            //FeedUpdate feedUpdate = new FeedUpdate(null, rssFeed);
            feedUpdate.run();

            if (feedUpdate.getErrorCode() == ErrorCode.NO_ERROR) {

                // Remove the old feeds if there is any
                if (feeds != null && feeds.length > 0) {
                    for (int i = 0; i < feeds.length; i++) {
                        RssFeedRecord oldFeed = (RssFeedRecord) feeds[i];
                        feedStore.deleteRecord(oldFeed);
                    }
                    // Remove the old RSS items
                    if (items != null) {
                        for (int i = 0; i < items.length; i++) {
                            itemStore.deleteRecord(items[i]);
                        }
                    }
                }

                // Save the feed
                rssFeed.setSessionId(midlet.getSessionId());
                feedStore.addRecord(rssFeed);

                // Save the item
                for (int i = 0; i < rssFeed.getNoOfItems(); i++) {
                    RssItemRecord item = rssFeed.getItem(i);
                    item.setFeedId(rssFeed.getRecordId());
                    itemStore.addRecord(item);
                }
            } else {
                if (feeds != null && feeds.length > 0) {
                    rssFeed = (RssFeedRecord) feeds[0];
                    if (items != null && items.length > 0) {
                        for (int i = 0; i < items.length; i++) {
                            rssFeed.addItem((RssItemRecord) items[i]);
                        }
                    }
                } else {
                    // Display the error
                }

            }

            // Display the news
            channelScreen.newsScreen.setup(channel);
            channelScreen.newsScreen.display(rssFeed);


        } catch (Exception ex) {

        }
    }
}
