package com.mbit.custom;

import com.mbit.valueobject.rss.RssItemRecord;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 9, 2006 12:24:13 AM
 * @version : $Id:
 */
public class NewsItem extends LineTicker {

    private String key;
    private String title;
    private String description;

    /**
     * Constructor
     *
     * @param rssItem
     */
    public NewsItem(RssItemRecord rssItem, int width) {
        super(rssItem.getTitle(), width);
        this.title = rssItem.getTitle();
        this.description = rssItem.getDescription();
        this.key = rssItem.getTitle();
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

    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }
}

