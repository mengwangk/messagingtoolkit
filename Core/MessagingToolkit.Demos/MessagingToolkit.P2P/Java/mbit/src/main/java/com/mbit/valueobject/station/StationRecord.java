package com.mbit.valueobject.station;

import com.nextel.rms.OAbstractRecord;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 18, 2006 9:10:03 PM
 * @version : $Id:
 */
public class StationRecord extends OAbstractRecord {

    private int id;
    private String name;
    private String image;
    private String channel;

    private int imageLength;
    private byte[] imageData;

    /**
     * Constructor
     */
    public StationRecord() {
    }

    /**
     * Constructor
     *
     * @param byteArray
     * @throws IOException
     */
    public StationRecord(byte [] byteArray) throws IOException {
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

    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public String getChannel() {
        return channel;
    }

    public int getImageLength() {
        return imageLength;
    }

    public void setImageLength(int imageLength) {
        this.imageLength = imageLength;
    }

    public void setChannel(String channel) {
        this.channel = channel;
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
            dataStream.writeUTF(image);
            dataStream.writeUTF(channel);
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
        name = inputStream.readUTF();
        image = inputStream.readUTF();
        channel = inputStream.readUTF();
        imageLength = inputStream.readInt();
        if (imageLength > 0) {
            imageData = new byte[imageLength];
            inputStream.readFully(imageData);
        }
    }


}
