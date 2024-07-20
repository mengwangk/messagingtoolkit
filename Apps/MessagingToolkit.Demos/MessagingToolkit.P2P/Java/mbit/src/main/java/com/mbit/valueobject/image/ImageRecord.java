package com.mbit.valueobject.image;

import com.nextel.rms.OAbstractRecord;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 14, 2006 10:14:14 AM
 * @version : $Id:
 */
public class ImageRecord extends OAbstractRecord {

    private String name;
    private int imageLength;
    private byte[] imageData;

    /**
     * Constructor
     */
    public ImageRecord() {
        name = "";
        imageLength = 0;
        imageData = null;
    }

    /**
     * Constructor
     *
     * @param byteArray
     * @throws java.io.IOException
     */
    public ImageRecord(byte [] byteArray) throws IOException {
        super(byteArray);
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
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

            dataStream.writeUTF(name);

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
        name = inputStream.readUTF();

        if (name != null && !"".equals(name)) {
            imageLength = inputStream.readInt();
            imageData = new byte[imageLength];
            inputStream.readFully(imageData);
        }
    }
}
