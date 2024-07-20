package com.mbit.screen;

import javax.microedition.lcdui.*;

import de.enough.polish.util.Locale;

import com.mbit.MainMidlet;
import com.mbit.valueobject.parameter.ParameterStore;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 11, 2006 11:58:27 PM
 * @version : $Id:
 */
public class MainScreen extends List implements CommandListener {

    private MainMidlet midlet;

    public Command preferenceCmd = new Command(Locale.get("cmd.preference"), Command.ITEM, 20);
    public Command aboutCmd = new Command(Locale.get("cmd.about"), Command.ITEM, 30);


    /**
     * Constructor
     */
    public MainScreen(MainMidlet midlet) {

        //#style displayScreen
        super(Locale.get("main.title"), List.IMPLICIT);

        this.midlet = midlet;

        setCommandListener(this);

        //#style mainList
        append(Locale.get("main.myspace"), null);

        //#style mainList
        append(Locale.get("main.todaynews"), null);

        //#style mainList
        append(Locale.get("main.recent"), null);

        //#style mainList
        append(Locale.get("main.hot"), null);

        //#style mainList
        append(Locale.get("main.checkforupdate"), null);

        addCommand(midlet.exitCmd);
        addCommand(preferenceCmd);
        addCommand(aboutCmd);

    }

    /**
     * Command action.
     *
     * @param command
     * @param displayable
     */
    public void commandAction(Command command, Displayable displayable) {
        if (command == midlet.exitCmd) {
            midlet.quit();
        } else if (command == preferenceCmd) {

        } else if (command == aboutCmd) {

        }

        if (command == List.SELECT_COMMAND) {
            int selectedItem = getSelectedIndex();
            Display display = Display.getDisplay(midlet);
            switch (selectedItem) {
                case 0:
                    midlet.mySpaceScreen.setup();
                    midlet.mySpaceScreen.display();
                    break;
                case 1:
                    midlet.stationScreen.setup();
                    break;
                case 2:
                    display.setCurrent(midlet.recentScreen);
                    break;
                case 3:
                    display.setCurrent(midlet.hotScreen);
                    break;
                case 4:
                    display.setCurrent(midlet.checkforUpdateScreen);
                    break;
                default:
                    midlet.notifyDestroyed();
            }
        }
    }
}