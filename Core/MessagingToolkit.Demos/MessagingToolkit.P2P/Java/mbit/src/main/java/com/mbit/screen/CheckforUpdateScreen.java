package com.mbit.screen;

import com.mbit.MainMidlet;
import de.enough.polish.util.Locale;

import javax.microedition.lcdui.*;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 17, 2006 10:14:49 PM
 * @version : $Id:
 */
public class CheckforUpdateScreen extends List implements CommandListener {

    private MainMidlet midlet;

    /**
     * Constructor
     */
    public CheckforUpdateScreen(MainMidlet midlet) {

        //#style displayScreen
        super(Locale.get("station.title"), List.IMPLICIT);

        this.midlet = midlet;

        setCommandListener(this);

        //#style stationBanner
        append("", null);

        //#style stationList
        append(Locale.get("station.star"), null);

        //#style stationList
        append(Locale.get("station.theedge"), null);

        addCommand(midlet.backCmd);

    }

    /**
     * Command action.
     *
     * @param command
     * @param displayable
     */
    public void commandAction(Command command, Displayable displayable) {
        Display display = Display.getDisplay(midlet);
        if (command == midlet.backCmd) {
            display.setCurrent(midlet.mainScreen);
        }
    }
}

