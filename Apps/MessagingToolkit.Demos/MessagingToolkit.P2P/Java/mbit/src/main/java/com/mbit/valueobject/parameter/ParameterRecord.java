package com.mbit.valueobject.parameter;

import com.nextel.rms.OAbstractRecord;

import java.io.IOException;
import java.io.DataOutputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 18, 2006 11:27:58 PM
 * @version : $Id:
 */
public class ParameterRecord extends OAbstractRecord {

    private int id;
    private String name;
    private String value;

    /**
     * Constructor
     */
    public ParameterRecord() {
    }

    /**
     * Constructor
     *
     * @param byteArray
     * @throws IOException
     */
    public ParameterRecord(byte [] byteArray) throws IOException {
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

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }

    /**
     * Writes the current record to the dataStream.
     *
     * @param dataStream The stream you wish to write the record to.
     */
    protected void writeStream(DataOutputStream dataStream) throws IOException {
        try {
            dataStream.writeInt(id);
            dataStream.writeUTF(name);
            dataStream.writeUTF(value);
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    /**
     * Populates the current record from the inputStream.
     *
     * @param inputStream The stream that contains record information
     * @throws IOException If there is a problem with the inputStream.
     */
    protected void populate(DataInputStream inputStream) throws IOException {
        id = inputStream.readInt();
        name = inputStream.readUTF();
        value = inputStream.readUTF();
    }
}
