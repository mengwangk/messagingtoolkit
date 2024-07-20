package com.mbit.valueobject.advertisement;

import com.nextel.rms.OAbstractRecord;

import java.io.IOException;
import java.io.DataOutputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 10, 2006 12:08:18 PM
 * @version : $Id:
 */
public class AdRecord extends OAbstractRecord {

    private int id;
    private String url;
    private String text;
    private String image;
    private String actionId;
    private long sessionId;

    private int imageLength;
    private byte[] imageData;

    /**
     * Constructor
     */
    public AdRecord() {
    }

    /**
     * Constructor
     *
     * @param byteArray
     * @throws java.io.IOException
     */
    public AdRecord(byte [] byteArray) throws IOException {
        super(byteArray);
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getUrl() {
        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }

    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public String getActionId() {
        return actionId;
    }

    public void setActionId(String actionId) {
        this.actionId = actionId;
    }

    public long getSessionId() {
        return sessionId;
    }

    public void setSessionId(long sessionId) {
        this.sessionId = sessionId;
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

    protected void writeStream(DataOutputStream dataStream) throws IOException {
        try {
            dataStream.writeInt(id);
            dataStream.writeUTF(url);
            dataStream.writeUTF(text);
            dataStream.writeUTF(image);
            dataStream.writeUTF(actionId);
            dataStream.writeLong(sessionId);
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
        id = inputStream.readInt();
        url = inputStream.readUTF();
        text = inputStream.readUTF();
        image = inputStream.readUTF();
        actionId = inputStream.readUTF();
        sessionId = inputStream.readLong();
        imageLength = inputStream.readInt();
        if (imageLength > 0) {
            imageData = new byte[imageLength];
            inputStream.readFully(imageData);
        }
    }
}

