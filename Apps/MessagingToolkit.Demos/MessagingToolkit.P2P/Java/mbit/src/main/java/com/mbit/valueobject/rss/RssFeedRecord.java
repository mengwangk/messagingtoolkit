package com.mbit.valueobject.rss;

import com.nextel.rms.OAbstractRecord;

import java.io.*;
import java.util.Vector;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 5, 2006 5:42:32 PM
 * @version : $Id:
 */
public class RssFeedRecord extends OAbstractRecord {

    // In minutes between updates
    protected final int STANDARD_UPDATE = 60;

    private String title;
    private String description;
    private String link;
    private String language;
    private String lastBuildDate;

    // Update time in milliseconds since January 1, 1970 UTC
    private long lastUpdateTime;

    // Last RSS feed (the parsed rss xml document) length in bytes
    private long lastFeedLen;

    private int minsBetweenUpdates;

    private String url;

    // Header fields
    private String serverLastModified;

    private String serverETag;

    // All items for this feed
    private Vector items;


    private String imageTitle;
    private String imageUrl;
    private String imageLink;

    private int imageLength;
    private byte[] imageData;

    private long sessionId;

    /**
     * Constructor for the class.
     */
    public RssFeedRecord() {
        title = "";
        description = "";
        language = "";
        link = "";
        lastBuildDate = "";
        lastUpdateTime = 0;
        lastFeedLen = 0;
        minsBetweenUpdates = STANDARD_UPDATE;
        serverLastModified = "";
        serverETag = "";
        url = "";
        items = new Vector();
        imageTitle = "";
        imageUrl = "";
        imageLink = "";
        imageLength = 0;
        imageData = null;
        sessionId = 0;
    }


    /**
     * Constructor
     *
     * @param byteArray
     * @throws IOException
     */
    public RssFeedRecord(byte [] byteArray) throws IOException {
        super(byteArray);
    }


    /**
     * Returns the number of items.
     *
     * @return the number of items
     */
    public int getNoOfItems() {
        return items.size();
    }

    /**
     * Adds the given item to the feed.
     *
     * @param item item to be added to the feed
     */
    public void addItem(RssItemRecord item) {
        item.setFeedId(getRecordId());
        items.addElement(item);
    }

    /**
     * Returns the item with the given internal id
     *
     * @param itemId the id of the item
     * @return the item with the given id.
     *         <code>NULL</code> if an error occurs or if no item
     *         exists with that title.
     */
    public RssItemRecord getItem(int itemId) {
        try {
            return (RssItemRecord) items.elementAt(itemId);
        } catch (Exception e) {
            return null;
        }
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getLink() {
        return link;
    }

    public void setLink(String link) {
        this.link = link;
    }

    public String getLanguage() {
        return language;
    }

    public void setLanguage(String language) {
        this.language = language;
    }

    public String getLastBuildDate() {
        return lastBuildDate;
    }

    public void setLastBuildDate(String lastBuildDate) {
        this.lastBuildDate = lastBuildDate;
    }

    public long getLastUpdateTime() {
        return lastUpdateTime;
    }

    public void setLastUpdateTime(long lastUpdateTime) {
        this.lastUpdateTime = lastUpdateTime;
    }

    public long getLastFeedLen() {
        return lastFeedLen;
    }

    public void setLastFeedLen(long lastFeedLen) {
        this.lastFeedLen = lastFeedLen;
    }

    public int getMinsBetweenUpdates() {
        return minsBetweenUpdates;
    }

    public void setMinsBetweenUpdates(int minsBetweenUpdates) {
        this.minsBetweenUpdates = minsBetweenUpdates;
    }

    public String getUrl() {
        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public String getServerLastModified() {
        return serverLastModified;
    }

    public void setServerLastModified(String serverLastModified) {
        this.serverLastModified = serverLastModified;
    }

    public String getServerETag() {
        return serverETag;
    }

    public void setServerETag(String serverETag) {
        this.serverETag = serverETag;
    }

    public Vector getItems() {
        return items;
    }

    public void setItems(Vector items) {
        this.items = items;
    }

    public String getImageTitle() {
        return imageTitle;
    }

    public void setImageTitle(String imageTitle) {
        this.imageTitle = imageTitle;
    }

    public String getImageUrl() {
        return imageUrl;
    }

    public void setImageUrl(String imageUrl) {
        this.imageUrl = imageUrl;
    }

    public String getImageLink() {
        return imageLink;
    }

    public void setImageLink(String imageLink) {
        this.imageLink = imageLink;
    }

    public int getImageLength() {
        return imageLength;
    }

    public void setImageLength(int imageLength) {
        this.imageLength = imageLength;
    }

    public byte[] getImageData() {
        return imageData;
    }

    public void setImageData(byte[] imageData) {
        this.imageData = imageData;
    }

    public long getSessionId() {
        return sessionId;
    }

    public void setSessionId(long sessionId) {
        this.sessionId = sessionId;
    }

    protected void writeStream(DataOutputStream dataStream) throws IOException {
        try {
            dataStream.writeUTF(url);
            dataStream.writeUTF(title);
            dataStream.writeUTF(description);
            dataStream.writeUTF(link);
            dataStream.writeUTF(language);
            dataStream.writeUTF(lastBuildDate);
            dataStream.writeLong(lastUpdateTime);
            dataStream.writeLong(lastFeedLen);
            dataStream.writeInt(minsBetweenUpdates);
            dataStream.writeUTF(serverLastModified);
            dataStream.writeUTF(serverETag);
            dataStream.writeLong(sessionId);

            dataStream.writeUTF(imageTitle);
            dataStream.writeUTF(imageUrl);
            dataStream.writeUTF(imageLink);

            if (imageData != null) {
                dataStream.writeInt(imageData.length);
                dataStream.write(imageData);
            }


        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
        }

    }

    protected void populate(DataInputStream inputStream) throws IOException {
        url = inputStream.readUTF();
        title = inputStream.readUTF();
        description = inputStream.readUTF();
        link = inputStream.readUTF();
        language = inputStream.readUTF();
        lastBuildDate = inputStream.readUTF();
        lastUpdateTime = inputStream.readLong();
        lastFeedLen = inputStream.readLong();
        minsBetweenUpdates = inputStream.readInt();
        serverLastModified = inputStream.readUTF();
        serverETag = inputStream.readUTF();
        sessionId = inputStream.readLong();

        imageTitle = inputStream.readUTF();
        imageUrl = inputStream.readUTF();
        imageLink = inputStream.readUTF();


        if (imageUrl != null && !"".equals(imageUrl)) {
            imageLength = inputStream.readInt();
            imageData = new byte[imageLength];
            inputStream.readFully(imageData);
        }
    }
}
