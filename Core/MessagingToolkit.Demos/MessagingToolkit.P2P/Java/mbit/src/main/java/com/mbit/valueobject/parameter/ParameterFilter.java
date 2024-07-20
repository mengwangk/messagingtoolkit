package com.mbit.valueobject.parameter;

import javax.microedition.rms.RecordFilter;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 29, 2006 10:09:13 AM
 * @version : $Id:
 */
public class ParameterFilter implements RecordFilter {

    private ByteArrayInputStream bin;
    private DataInputStream din;
    private String parameterName;

    public ParameterFilter(String name) {
        parameterName = name;
    }
    public boolean matches(byte[] bytes) {
        try {
            bin = new ByteArrayInputStream(bytes);
            din = new DataInputStream(bin);
            din.readInt();
            String name = din.readUTF();
            din.close();
            bin.close();
            return (parameterName.equals(name));
        } catch (Exception e) {
            //#debug
            System.out.println(e.getMessage());
            e.printStackTrace();
            return false;
        }
    }
}
