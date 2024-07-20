package com.mbit.browser;

import com.mbit.base.BaseMidlet;
import com.mbit.browser.FormSelectOption;
import com.mbit.browser.FormWidget;

import javax.microedition.lcdui.*;


/**
 * Allow a user to choose an entry from a select menu
 */
public class MIDPFormSelectList extends javax.microedition.lcdui.List
        implements CommandListener {
    /**
     *
     */

    BaseMidlet app;
    FormWidget widget;

    private Command cancel = new Command("Cancel", Command.CANCEL, 1);
    private Command done = new Command("Done", Command.OK, 1);


    /**
     * Constructor for a SELECT list
     */
    public MIDPFormSelectList(String title, BaseMidlet app) {
        super(title, List.IMPLICIT);
        this.app = app;
        setCommandListener(this);
        addCommand(cancel);
        addCommand(done);
    }

    /**
     * Takes a picomidp FormWidget, which can have a list of options.
     * If options is non-null, then add the option's content (text) to
     * this MIDP select widget.
     */
    public void setWidget(FormWidget w) {
        widget = w;
        if (widget.options != null) {
            for (int i = 0; i < widget.options.size(); i++) {
                FormSelectOption option =
                        (FormSelectOption) (widget.options.elementAt(i));
                String content = option.content;
                this.append(content, null);
            }
        }
    }

    /**
     * Indicates that a command event has occurred.
     */
    public void commandAction(Command c, Displayable d) {
        if (c == cancel) {
            //Display.getDisplay(app).setCurrentDisplay(app.browserCanvas());
        } else if (c == done || c == List.SELECT_COMMAND) {
            int selection = getSelectedIndex();
            widget.selection = selection;
            returnToBrowser();
        }
    }

    void returnToBrowser() {
        /*
        int selection = getSelectedIndex();
        widget.selection = selection;
        BrowserScreen browser = app.browserCanvas();
        Display.getDisplay(app).setCurrentDisplay(browser);
        */
    }

}
