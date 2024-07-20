package com.mbit.screen;

import de.enough.polish.util.Locale;

import javax.microedition.lcdui.Form;
import javax.microedition.lcdui.Gauge;
import javax.microedition.lcdui.ImageItem;
import javax.microedition.lcdui.Image;

import com.mbit.common.Constants;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 22, 2006 10:59:35 PM
 * @version : $Id:
 */
public class ProgressScreen extends Form {

    public static int INITIAL_PROGRESS_VALUE = 2;

    public static int HALF_PROGRESS_VALUE = 5;

    public static int THREE_QUARTER_PROGRESS_VALUE = 7;

    public static int COMPLETE_PROGRESS_VALUE = 10;

    private Gauge gauge;


    /**
     * Constructor.
     */
    public ProgressScreen() {
        //#style progressScreen
        super(Locale.get("progress.title"));

        try {
            ImageItem appImage =
                    new ImageItem
                            ("", Image.createImage(Constants.APPLICATION_ICON),
                                    ImageItem.LAYOUT_CENTER | ImageItem.LAYOUT_NEWLINE_BEFORE
                                            | ImageItem.LAYOUT_NEWLINE_AFTER, "In progress");
            append(appImage);
        } catch (Exception ex) {
            //#debug
            System.out.println(ex.getMessage());
        }

        //#style gaugeStyle
        gauge = new Gauge("", false, 10, 0);
        append(gauge);
        gauge.setValue(INITIAL_PROGRESS_VALUE);
    }

    /**
     * Constructor
     *
     * @param title
     */
    public ProgressScreen(String title) {
        //#style progressScreen
        super(title);

        try {
            ImageItem appImage =
                    new ImageItem
                            ("", Image.createImage(Constants.APPLICATION_ICON),
                                    ImageItem.LAYOUT_CENTER | ImageItem.LAYOUT_NEWLINE_BEFORE
                                            | ImageItem.LAYOUT_NEWLINE_AFTER, "In progress");
            append(appImage);
        } catch (Exception ex) {
            //#debug
            System.out.println(ex.getMessage());
        }
        //#style gaugeStyle
        gauge = new Gauge("", false, 10, 0);
        append(gauge);
        gauge.setValue(INITIAL_PROGRESS_VALUE);
    }


    /**
     * Set the progress status.
     *
     * @param value
     */
    public void setProgress(int value) {
        gauge.setValue(value);
    }

}
