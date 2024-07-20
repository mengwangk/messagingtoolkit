package com.mbit.valueobject.rss;

import com.nextel.rms.OAbstractRecord;

import java.io.*;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 5, 2006 5:42:41 PM
 * @version : $Id:
 */
public class RssItemRecord extends OAbstractRecord {

    // The feed id in the RMS that this item belongs to
    private int feedId;

    // The item id in the RMS
    private int itemId;

    // Parsed time in milliseconds since January 1, 1970 UTC
    private long parseTime;
    private String title;
    private String link;
    private String description;
    private String publishedDate;
    private String author;
    private String category;

    /**
     * Constructor.
     */
    public RssItemRecord() {
        publishedDate = "";
        title = "";
        description = "";
        link = "";
        author = "";
        category = "";
        parseTime = System.currentTimeMillis();
        itemId = -1;
    }

    /**
     * Constructor
     *
     * @param byteArray
     * @throws IOException
     */
    public RssItemRecord(byte [] byteArray) throws IOException {
        super(byteArray);
    }


    public int getFeedId() {
        return feedId;
    }

    public void setFeedId(int feedId) {
        this.feedId = feedId;
    }

    public int getItemId() {
        return itemId;
    }

    public void setItemId(int itemId) {
        this.itemId = itemId;
    }

    public long getParseTime() {
        return parseTime;
    }

    public void setParseTime(long parseTime) {
        this.parseTime = parseTime;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getLink() {
        return link;
    }

    public void setLink(String link) {
        this.link = link;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getPublishedDate() {
        return publishedDate;
    }

    public void setPublishedDate(String publishedDate) {
        this.publishedDate = publishedDate;
    }

    public String getAuthor() {
        return author;
    }

    public void setAuthor(String author) {
        this.author = author;
    }

    public String getCategory() {
        return category;
    }

    public void setCategory(String category) {
        this.category = category;
    }

    protected void writeStream(DataOutputStream dataStream) throws IOException {
        try {
            dataStream.writeInt(feedId);
            dataStream.writeLong(parseTime);
            dataStream.writeUTF(title);
            dataStream.writeUTF(link);
            dataStream.writeUTF(description);
            dataStream.writeUTF(publishedDate);
            dataStream.writeUTF(author);
            dataStream.writeUTF(category);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    protected void populate(DataInputStream inputStream) throws IOException {
        feedId = inputStream.readInt();
        parseTime = inputStream.readLong();
        title = inputStream.readUTF();
        link = inputStream.readUTF();
        description = inputStream.readUTF();
        publishedDate = inputStream.readUTF();
        author = inputStream.readUTF();
        category = inputStream.readUTF();
    }
}
