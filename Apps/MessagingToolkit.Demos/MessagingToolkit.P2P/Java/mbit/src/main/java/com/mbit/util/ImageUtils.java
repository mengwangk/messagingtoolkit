package com.mbit.util;

import javax.microedition.lcdui.Image;
import javax.microedition.lcdui.Graphics;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 30, 2006 12:06:21 AM
 * @version : $Id:
 */
public class ImageUtils {

    /**
     * Convert byte array to image.
     *
     * @param imageData
     * @return Image
     */
    public static Image convertByteArrayToImage(byte[] imageData) {
        // Create the image from the byte array
        return Image.createImage(imageData, 0, imageData.length);
    }

    private static int[] reScaleArray(int[] ini, int x, int y, int x2, int y2) {
        int out[] = new int[x2 * y2];

        for (int yy = 0; yy < y2; yy++) {
            int dy = yy * y / y2;
            for (int xx = 0; xx < x2; xx++) {
                int dx = xx * x / x2;
                out[(x2 * yy) + xx] = ini[(x * dy) + dx];
            }
        }

        return out;
    }


    public static Image resizeImage(Image image, int newX, int newY) {

        int rgb[] = new int[image.getWidth() * image.getHeight()];
        /*
        image.getRGB(rgb, 0, image.getWidth(), 0, 0, image.getWidth(), image.getHeight());

        if (newX == -1) {
            newX = (image.getWidth() * newY / image.getHeight());

        }
        if (newY == -1) {
            newY = (image.getHeight() * newX / image.getWidth());
        }*/

        /*
        int rgb2[] = reScaleArray(rgb, image.getWidth(), image.getHeight(), newX, newY);
        return (Image.createRGBImage(rgb2, newX, newY, true));
        */
        return null;

    }

    public static Image createThumbnail(Image image) {
        int sourceWidth = image.getWidth();
        int sourceHeight = image.getHeight();
        int thumbWidth = 36;
        int thumbHeight = 36;

        if (thumbHeight == -1)
            thumbHeight = thumbWidth * sourceHeight / sourceWidth;

        Image thumb = Image.createImage(thumbWidth, thumbHeight);
        Graphics g = thumb.getGraphics();
        for (int y = 0; y < thumbHeight; y++) {
            for (int x = 0; x < thumbWidth; x++) {
                g.setClip(x, y, 1, 1);
                int dx = x * sourceWidth / thumbWidth;
                int dy = y * sourceHeight / thumbHeight;
                g.drawImage(image, x - dx, y - dy, Graphics.LEFT | Graphics.TOP);
            }
        }
        Image immutableThumb = Image.createImage(thumb);
        return immutableThumb;
    }
}
