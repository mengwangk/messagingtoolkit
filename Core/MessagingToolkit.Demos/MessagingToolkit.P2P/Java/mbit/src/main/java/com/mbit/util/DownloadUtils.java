package com.mbit.util;

import com.mbit.common.Constants;
import com.nextel.net.HttpRequest;

import javax.microedition.lcdui.Image;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.InputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 20, 2006 10:56:52 PM
 * @version : $Id:
 */
public class DownloadUtils {


    /**
     * Retrieve the image data.
     *
     * @param url
     * @return byte[]
     */
    public static byte[] getImage(String url){
        HttpRequest httpRequest = new HttpRequest(url, Constants.HTTP_TIME_OUT, Constants.HTTP_TIME_OUT);
        try {
            byte[] imageData;
            InputStream inputStream = httpRequest.get();
            DataInputStream dataInputStream = new DataInputStream(inputStream);
            int length = (int) httpRequest.getHttpConnection().getLength();
            if (length != -1) {
                imageData = new byte[length];
                // Read the image into an array
                dataInputStream.readFully(imageData);
            } else {
                // Length not available...
                ByteArrayOutputStream bStrm = new ByteArrayOutputStream();
                int ch;
                while ((ch = dataInputStream.read()) != -1)
                    bStrm.write(ch);
                imageData = bStrm.toByteArray();
                bStrm.close();
            }
            return imageData;
        } catch (Exception ex){
            //#debug
            System.out.println(ex.getMessage());
            return null;
        } finally {
            httpRequest.cleanup();
        }
    }

  

}
