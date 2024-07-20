package com.mbit.update;

import com.mbit.MainMidlet;
import com.mbit.TestMidlet;
import com.mbit.common.Constants;
import com.mbit.common.ErrorCode;
import com.mbit.common.Tag;
import com.mbit.screen.ProgressScreen;
import com.mbit.util.DownloadUtils;
import com.mbit.util.TextUtils;
import com.mbit.valueobject.rss.RssFeedRecord;
import com.mbit.valueobject.rss.RssItemRecord;
import com.mbit.valueobject.channel.ChannelRecord;
import com.mbit.valueobject.image.ImageStore;
import com.nextel.net.HttpRequest;
import de.enough.polish.util.Locale;
import org.kxml2.io.KXmlParser;
import org.xmlpull.v1.XmlPullParserException;

import javax.microedition.io.HttpConnection;
import javax.microedition.lcdui.Display;
import java.io.IOException;
import java.io.InputStream;
import java.util.Hashtable;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 4, 2006 6:21:44 PM
 * @version : $Id:
 */
public class FeedUpdate extends AbstractUpdate {

    private MainMidlet midlet;

    private RssFeedRecord rssFeed;

    private ChannelRecord channel;

    private Hashtable itemTable;

    /**
     * Constructor
     */
    public FeedUpdate(MainMidlet midlet, ChannelRecord channel, RssFeedRecord rssFeed) {
        super();
        this.rssFeed = rssFeed;
        this.channel = channel;
        this.midlet = midlet;
        itemTable = buildItemTable();
    }

    /**
     * Builds and return a Hashtable with the mapping
     * item title -> item.
     *
     * @return Hashtable with mapping item title -> item
     */
    private Hashtable buildItemTable() {
        if (rssFeed == null)
            return null;
        int noOfItems = rssFeed.getNoOfItems();
        Hashtable itemTable = new Hashtable(noOfItems);
        RssItemRecord rssItem;
        for (int i = 0; i < noOfItems; i++) {
            rssItem = rssFeed.getItem(i);
            itemTable.put(rssItem.getTitle(), rssItem);
        }
        return itemTable;
    }

    /**
     * Retrieve the channels
     */
    public void run() {
        int items = 0;
        int newItems = 0;
        int responseCode;

        ProgressScreen progressScreen = new ProgressScreen(Locale.get("progress.feedupdate"));
        progressScreen.setProgress(ProgressScreen.INITIAL_PROGRESS_VALUE);
        Display display = Display.getDisplay(midlet);
        display.setCurrent(progressScreen);

        //#debug
        System.out.println("Retrieve RSS from " + rssFeed.getUrl());
        HttpRequest httpRequest = new HttpRequest(rssFeed.getUrl(), Constants.HTTP_TIME_OUT, Constants.HTTP_TIME_OUT);
        try {
            progressScreen.setProgress(ProgressScreen.HALF_PROGRESS_VALUE);
            // Perhaps do a conditional GET request
            // Last-Modified and ETag
            // If-Modified-Since
            // If-None-Match
            // 304 Not Modified HTTP_NOT_MODIFIED
            if (rssFeed.getServerLastModified() != null && !rssFeed.getServerLastModified().equals("")) {
                httpRequest.getHttpConnection().setRequestMethod(HttpConnection.GET);
                httpRequest.getHttpConnection().setRequestProperty("Last-Modified",
                        rssFeed.getServerLastModified());
                httpRequest.getHttpConnection().setRequestProperty("If-None-Match",
                        rssFeed.getServerETag());
            }

            // create DataInputStream on top of the socket connection
            InputStream inputStream = httpRequest.get();
            KXmlParser parser = new KXmlParser();
            parser.setInput(inputStream, Constants.PAGE_ENCODING);

            responseCode = httpRequest.getHttpConnection().getResponseCode();
            if (responseCode != HttpConnection.HTTP_OK) {
                throw new IOException("HTTP response code: " + responseCode);
            }

            // Remove the feed
            if (responseCode == HttpConnection.HTTP_NOT_MODIFIED) {
                // Feed has not been modified
                return;
            }

            // Reset the image store
            if (!midlet.isOnline()) {
                midlet.setOnline(true);
                // Remove everything from the image store
                ImageStore imageStore = ImageStore.getInstance();
                try {
                    imageStore.deleteAll();
                } catch (Exception ex) {
                    //#debug
                    System.out.println(ex.getMessage());
                }
            }
            rssFeed.setServerLastModified(TextUtils.removeBlank(httpRequest.getHttpConnection().getHeaderField("Last-Modified")));
            rssFeed.setServerETag(TextUtils.removeBlank(httpRequest.getHttpConnection().getHeaderField("ETag")));
            rssFeed.setLastFeedLen(httpRequest.getHttpConnection().getLength());

            // Go through tags about the channel
            while (!Tag.RSS_ITEM_TAG.equals(parser.getName()) &&
                    (parser.getEventType() != KXmlParser.END_DOCUMENT)) {
                if (parser.getEventType() == KXmlParser.START_TAG) {
                    if (Tag.RSS_LAST_BUILD_DATE.equals(parser.getName()))
                        rssFeed.setLastBuildDate(parser.nextText());
                    else if (Tag.RSS_DESCRIPTION_TAG.equals(parser.getName()))
                        rssFeed.setDescription(parser.nextText());
                    else if (Tag.RSS_TITLE_TAG.equals(parser.getName()) &&
                            rssFeed.getTitle().equals(""))
                        // Set the title of the rssFeed if its nothing
                        rssFeed.setTitle(parser.nextText());
                    else if (Tag.RSS_IMAGE_TAG.equals(parser.getName())) {
                        String tagName;
                        String tagText;
                        parser.require(KXmlParser.START_TAG, null, Tag.RSS_IMAGE_TAG);
                        while (parser.nextTag() != KXmlParser.END_TAG) {
                            parser.require(KXmlParser.START_TAG, null, null);
                            tagName = parser.getName();
                            tagText = parser.nextText();
                            if (tagName.equals(Tag.RSS_IMAGE_TITLE_TAG)) {
                                rssFeed.setImageTitle(tagText);
                            } else if (tagName.equals(Tag.RSS_IMAGE_URL_TAG)) {
                                rssFeed.setImageUrl(tagText);
                            } else if (tagName.equals(Tag.RSS_IMAGE_LINK_TAG)) {
                                rssFeed.setImageLink(tagText);
                            } else if (tagName.equals(Tag.RSS_IMAGE_WIDTH_TAG)) {
                                //rssFeed.setImageWidth(tagText);
                            } else if (tagName.equals(Tag.RSS_IMAGE_HEIGHT_TAG)) {
                                //rssFeed.setImageHeight(tagText);
                            }
                            parser.require(KXmlParser.END_TAG, null, tagName);
                        }
                        parser.require(KXmlParser.END_TAG, null, Tag.RSS_IMAGE_TAG);

                    } else if (Tag.RSS_LINK_TAG.equals(parser.getName()))
                        rssFeed.setLink(parser.nextText());
                    else if (Tag.RSS_LANGUAGE_TAG.equals(parser.getName()))
                        rssFeed.setLanguage(parser.nextText());
                }
                try {
                    parser.next();
                } catch (Exception ex) {
                    //#debug
                    System.out.println(ex.getMessage());
                }
            }

            // We have found the first <item>
            RssItemRecord rssItem;
            String tagName;
            String tagText;
            long parseTime = System.currentTimeMillis();
            do {
                // For every item
                parser.require(KXmlParser.START_TAG, null, Tag.RSS_ITEM_TAG);
                rssItem = new RssItemRecord();
                while (parser.nextTag() != KXmlParser.END_TAG) {
                    // Go through all the tags within <item>
                    parser.require(KXmlParser.START_TAG, null, null);
                    tagName = parser.getName().toLowerCase();
                    //#debug
                    System.out.println("name " + tagName);
                    tagText = "";

                    while (true) {
                        int tokenType = parser.nextToken();
                        if (tokenType != KXmlParser.END_TAG) {
                            try {
                                if (tokenType == KXmlParser.TEXT)
                                    tagText += parser.getText();
                                else if (tokenType == KXmlParser.START_TAG)
                                    tagText += "<" + parser.getName() + ">";
                                else if (tokenType == KXmlParser.CDSECT)
                                    tagText += parser.getText();
                            } catch (Exception e1) {

                            }
                        } else {
                            if (tagName.equals(parser.getName().toLowerCase()))
                                break;
                            else
                                tagText += "</" + parser.getName() + ">";
                        }
                    }

                    //#debug
                    System.out.println("text " + tagText);
                    tagText = TextUtils.removeBlank(tagText);
                    if (Tag.RSS_PUB_DATE.equals(tagName)) {
                        rssItem.setPublishedDate(tagText);
                    } else if (Tag.RSS_TITLE_TAG.equals(tagName)) {
                        rssItem.setTitle(tagText);
                    } else if (Tag.RSS_AUTHOR_TAG.equals(tagName)) {
                        rssItem.setAuthor(tagText);
                    } else if (Tag.RSS_DESCRIPTION_TAG.equals(tagName)) {
                        rssItem.setDescription(tagText);
                    } else if (Tag.RSS_LINK_TAG.equals(tagName)) {
                        rssItem.setLink(tagText);
                    } else if (Tag.RSS_CATEGORY_TAG.equals(tagName)) {
                        rssItem.setCategory(tagText);
                    }
                    parser.require(KXmlParser.END_TAG, null, parser.getName());
                }

                parser.require(KXmlParser.END_TAG, null, Tag.RSS_ITEM_TAG);

                // Add the item if it's new (All items have titles)
                if (rssItem.getTitle() != null) {
                    items++;
                    if (itemTable.get(rssItem.getTitle()) == null) {
                        newItems++;
                        // A bit ugly hack for correct item ordering
                        rssItem.setParseTime(parseTime - items);
                        rssFeed.addItem(rssItem);
                    }
                }

            } while (parser.nextTag() != KXmlParser.END_TAG &&
                    Tag.RSS_ITEM_TAG.equals(parser.getName()));

            //#debug
            System.out.println("Feed update time: " + parseTime);

            // Save the rssFeed
            rssFeed.setLastUpdateTime(parseTime);

            inputStream.close();

            // Check if any image to retrieve
            if (rssFeed.getImageUrl() != null && !"".equals(rssFeed.getImageUrl())) {
                rssFeed.setImageData(DownloadUtils.getImage(rssFeed.getImageUrl()));
            }

            progressScreen.setProgress(ProgressScreen.COMPLETE_PROGRESS_VALUE);

            if (channel.getAdUrl() != null && !"".equals(channel.getAdUrl())) {
                AdUpdate adUpdate = new AdUpdate(midlet, channel);
                adUpdate.run();
            }
        } catch (IllegalArgumentException ex) {
            //#debug
            System.out.println(ex.getMessage());
            setErrorCode(ErrorCode.FEED_UPPDATE_ERROR);
            setErrorMsg(ex.getMessage());
        } catch (XmlPullParserException ex) {
            //#debug
            System.out.println(ex.getMessage());
            setErrorCode(ErrorCode.FEED_UPPDATE_ERROR);
            setErrorMsg(ex.getMessage());
        } catch (Exception ex) {
            //#debug
            System.out.println(ex.getMessage());
            setErrorCode(ErrorCode.FEED_UPPDATE_ERROR);
            setErrorMsg(ex.getMessage());
        } finally {
            httpRequest.cleanup();
        }

    }
}


