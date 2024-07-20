package com.mbit.browser;

import com.mbit.base.BaseMidlet;
import com.mbit.browser.FormWidget;

import javax.microedition.lcdui.*;


/**
 * Allow a user to enter a single text input field
 */
public class MIDPFormTextField extends javax.microedition.lcdui.Form
        implements CommandListener {
    /**
     *
     */

    BaseMidlet app;
    FormWidget widget;

    private Command cancel = new Command("Cancel", Command.CANCEL, 1);
    private Command done = new Command("Done", Command.OK, 1);


    TextField textField;

    /**
     * This constructor creates new BaseDemo with the given title & creates
     * the standard commands for the demos.
     */
    public MIDPFormTextField(String title, BaseMidlet app) {
        super(title);
        this.app = app;
        setCommandListener(this);
        //setItemStateListener(new StateChangeListener(done, this));
        addCommand(cancel);
        addCommand(done);

        textField = new TextField("", "", 25, TextField.ANY);
        //append(textField);

    }

    public void setWidget(FormWidget w, int maxSize, int constraints) {
        widget = w;
        textField.setMaxSize(maxSize);
        textField.setString(w.getValue());
        textField.setConstraints(constraints);
    }

    /**
     * Indicates that a command event has occurred.
     */
    public void commandAction(Command c, Displayable d) {
        if (c == cancel) {
            //Display.getDisplay(app).setCurrentDisplay(app.browserCanvas());
        } else if (c == done) {
            returnToBrowser();
        }
    }

    void returnToBrowser() {
        /*
        String newval = textField.getString();
        widget.setValue(newval);
        BrowserScreen browser = app.browserCanvas();
        Display.getDisplay(app).setCurrentDisplay(browser);
        */
    }


}
