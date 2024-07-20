package com.mbit.valueobject.inbox;

import com.nextel.rms.OAbstractRecord;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 11, 2006 10:15:19 AM
 * @version : $Id:
 */
public class MessageRecord extends OAbstractRecord {

    private long msgTime;
    private String title;
    private String body;
    private int msgType;

    private String image1;
    private String image2;
    private String image3;

    private int imageLength1;
    private int imageLength2;
    private int imageLength3;

    private byte[] imageData1;
    private byte[] imageData2;
    private byte[] imageData3;


    /**
     * Constructor
     */
    public MessageRecord() {
        msgTime = 0;
        title = "";
        body = "";
        msgType = 0;
        image1 = "";
        image2 = "";
        image3 = "";
        imageLength1 = 0;
        imageLength2 = 0;
        imageLength3 = 0;
        imageData1 = null;
        imageData2 = null;
        imageData3 = null;
    }

    /**
     * Constructor
     *
     * @param byteArray
     * @throws java.io.IOException
     */
    public MessageRecord(byte [] byteArray) throws IOException {
        super(byteArray);
    }

    public long getMsgTime() {
        return msgTime;
    }

    public void setMsgTime(long msgTime) {
        this.msgTime = msgTime;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getBody() {
        return body;
    }

    public void setBody(String body) {
        this.body = body;
    }

    public int getMsgType() {
        return msgType;
    }

    public void setMsgType(int msgType) {
        this.msgType = msgType;
    }

    public String getImage1() {
        return image1;
    }

    public void setImage1(String image1) {
        this.image1 = image1;
    }

    public String getImage2() {
        return image2;
    }

    public void setImage2(String image2) {
        this.image2 = image2;
    }

    public String getImage3() {
        return image3;
    }

    public void setImage3(String image3) {
        this.image3 = image3;
    }

    public int getImageLength1() {
        return imageLength1;
    }

    public void setImageLength1(int imageLength1) {
        this.imageLength1 = imageLength1;
    }

    public int getImageLength2() {
        return imageLength2;
    }

    public void setImageLength2(int imageLength2) {
        this.imageLength2 = imageLength2;
    }

    public int getImageLength3() {
        return imageLength3;
    }

    public void setImageLength3(int imageLength3) {
        this.imageLength3 = imageLength3;
    }

    public byte[] getImageData1() {
        return imageData1;
    }

    public void setImageData1(byte[] imageData1) {
        this.imageData1 = imageData1;
    }

    public byte[] getImageData2() {
        return imageData2;
    }

    public void setImageData2(byte[] imageData2) {
        this.imageData2 = imageData2;
    }

    public byte[] getImageData3() {
        return imageData3;
    }

    public void setImageData3(byte[] imageData3) {
        this.imageData3 = imageData3;
    }

    protected void writeStream(DataOutputStream dataStream) throws IOException {
        try {
            dataStream.writeLong(msgTime);
            dataStream.writeUTF(title);
            dataStream.writeUTF(body);
            dataStream.writeInt(msgType);
            dataStream.writeUTF(image1);
            dataStream.writeUTF(image2);
            dataStream.writeUTF(image3);

            if (imageData1 != null) {
                dataStream.writeInt(imageData1.length);
                dataStream.write(imageData1);
            }
            if (imageData2 != null) {
                dataStream.writeInt(imageData2.length);
                dataStream.write(imageData2);
            }

            if (imageData3 != null) {
                dataStream.writeInt(imageData3.length);
                dataStream.write(imageData3);
            }

        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
        }

    }

    protected void populate(DataInputStream inputStream) throws IOException {
        msgTime = inputStream.readLong();
        title = inputStream.readUTF();
        body = inputStream.readUTF();
        msgType = inputStream.readInt();
        image1 = inputStream.readUTF();
        image2 = inputStream.readUTF();
        image3 = inputStream.readUTF();

        if (image1 != null && !"".equals(image1)) {
            imageLength1 = inputStream.readInt();
            imageData1 = new byte[imageLength1];
            inputStream.readFully(imageData1);

        }
        if (image2 != null && !"".equals(image2)) {
            imageLength2 = inputStream.readInt();
            imageData2 = new byte[imageLength2];
            inputStream.readFully(imageData2);

        }
        if (image3 != null && !"".equals(image3)) {
            imageLength3 = inputStream.readInt();
            imageData3 = new byte[imageLength3];
            inputStream.readFully(imageData3);

        }
    }


}
