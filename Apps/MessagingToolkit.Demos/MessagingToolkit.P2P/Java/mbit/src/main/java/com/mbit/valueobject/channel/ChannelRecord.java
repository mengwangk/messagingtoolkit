package com.mbit.valueobject.channel;

import com.nextel.rms.OAbstractRecord;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 18, 2006 10:05:33 PM
 * @version : $Id:
 */
public class ChannelRecord extends OAbstractRecord {

    private int id;
    private String name;
    private String url;
    private String adUrl;
    private String image;
    private String stationName;

    private int imageLength;
    private byte[] imageData;


    /**
     * Constructor
     */
    public ChannelRecord() {
    }

    /**
     * Constructor
     *
     * @param byteArray
     * @throws IOException
     */
    public ChannelRecord(byte [] byteArray) throws IOException {
        super(byteArray);
    }


    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getUrl() {
        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public String getAdUrl() {
        return adUrl;
    }

    public void setAdUrl(String adUrl) {
        this.adUrl = adUrl;
    }

    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public String getStationName() {
        return stationName;
    }

    public void setStationName(String stationName) {
        this.stationName = stationName;
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
            dataStream.writeUTF(name);
            dataStream.writeUTF(url);
            dataStream.writeUTF(adUrl);
            dataStream.writeUTF(image);
            dataStream.writeUTF(stationName);
            if (image != null && !"".equals(image)) {
                dataStream.writeInt(imageData.length);
                dataStream.write(imageData);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    protected void populate(DataInputStream inputStream) throws IOException {
        id = inputStream.readInt();
        name = inputStream.readUTF();
        url = inputStream.readUTF();
        adUrl = inputStream.readUTF();
        image = inputStream.readUTF();
        stationName = inputStream.readUTF();
        if (image != null && !"".equals(image)) {
            imageLength = inputStream.readInt();
            imageData = new byte[imageLength];
            inputStream.readFully(imageData);
        }
    }

}
