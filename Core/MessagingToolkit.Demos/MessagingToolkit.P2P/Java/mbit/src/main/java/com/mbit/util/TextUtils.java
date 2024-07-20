package com.mbit.util;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 9, 2006 8:14:20 PM
 * @version : $Id:
 */
public class TextUtils {

    public static final String STRING_EMPTY = "";

    public static boolean isNotBlank(String value) {
        if (value == null) return false;
        if (value.equals(STRING_EMPTY)) return false;
        return true;
    }

    public static String removeBlank(String value) {
        if (value == null) return STRING_EMPTY;

        return value.trim();
    }

    public static String stripHTMLTags(String message) {
        StringBuffer returnMessage = new StringBuffer(message);
        int startPosition = message.indexOf("<"); // encountered the first opening brace
        int endPosition = message.indexOf(">"); // encountered the first closing braces
        while (startPosition != -1) {
            returnMessage.delete(startPosition, endPosition + 1); // remove the tag
            startPosition = (returnMessage.toString()).indexOf("<"); // look for the next opening brace
            endPosition = (returnMessage.toString()).indexOf(">"); // look for the next closing brace
        }
        return returnMessage.toString().trim();
    }

}
