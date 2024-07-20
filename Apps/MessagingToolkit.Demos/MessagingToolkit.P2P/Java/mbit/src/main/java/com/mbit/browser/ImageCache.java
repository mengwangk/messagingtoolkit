package com.mbit.browser;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 11, 2006 10:20:48 PM
 * @version : $Id:
 */
public class ImageCache {

    private String url;
    private byte[] imageData;

    /**
     * Constructor
     */
    public ImageCache(){
        url = "";
        imageData = null;
    }

    /**
     * Constructor
     */
    public ImageCache(String url, byte[] imageData){
        this.url = url;
        this.imageData = imageData;
    }

    public String getUrl() {
        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public byte[] getImageData() {
        return imageData;
    }

    public void setImageData(byte[] imageData) {
        this.imageData = imageData;
    }


}
