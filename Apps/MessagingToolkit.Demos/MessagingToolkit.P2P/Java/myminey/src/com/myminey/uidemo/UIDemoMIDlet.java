/*
 * Copyright © 2008 Sun Microsystems, Inc. All rights reserved.
 * Use is subject to license terms.
 *
 */
package com.myminey.uidemo;

import com.sun.lwuit.Button;
import com.sun.lwuit.animations.CommonTransitions;
import com.sun.lwuit.Command;
import com.sun.lwuit.Component;
import com.sun.lwuit.Dialog;
import com.sun.lwuit.Display;
import com.sun.lwuit.Font;
import com.sun.lwuit.Form;
import com.sun.lwuit.Graphics;
import com.sun.lwuit.Image;
import com.sun.lwuit.Label;
import com.sun.lwuit.TextArea;
import com.sun.lwuit.animations.Transition;
import com.sun.lwuit.events.ActionEvent;
import com.sun.lwuit.events.ActionListener;
import com.sun.lwuit.events.FocusListener;
import com.sun.lwuit.geom.Dimension;
import com.sun.lwuit.layouts.BorderLayout;
import com.sun.lwuit.layouts.BoxLayout;
import com.sun.lwuit.layouts.GridLayout;
import com.sun.lwuit.plaf.Border;
import com.sun.lwuit.plaf.Style;
import com.sun.lwuit.plaf.UIManager;
import com.sun.lwuit.util.Resources;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Hashtable;
import javax.microedition.io.Connector;
import javax.microedition.io.InputConnection;
import javax.microedition.rms.RecordStore;
import javax.microedition.rms.RecordStoreException;

/**
 * Bootstraps the UI toolkit demos 
 *
 * @author Shai Almog
 */
public class UIDemoMIDlet extends javax.microedition.midlet.MIDlet implements ActionListener {

    private static final int EXIT_COMMAND = 1;
    private static final int RUN_COMMAND = 2;
    private static final int BACK_COMMAND = 3;
    private static final int ABOUT_COMMAND = 4;
    private static final int DRAG_MODE_COMMAND = 5;
    private static final int SCROLL_MODE_COMMAND = 6;
    private static final Command runCommand = new Command("Run", RUN_COMMAND);
    private static final Command exitCommand = new Command("Exit", EXIT_COMMAND);
    private static final Command backCommand = new Command("Back", BACK_COMMAND);
    private static final Command aboutCommand = new Command("About", ABOUT_COMMAND);
    private static final Command dragModeCommand = new Command("Drag Mode", DRAG_MODE_COMMAND);
    private static final Command scrollModeCommand = new Command("Scroll Mode", SCROLL_MODE_COMMAND);
    private static final Demo[] DEMOS = new Demo[]{
        new ThemeDemo(), new RenderingDemo(), new AnimationDemo(), new ButtonsDemo(),
        new TransitionDemo(), new FontDemo(), new TabbedPaneDemo(), new DialogDemo(), new LayoutDemo(), new ScrollDemo()
    };
    private Demo currentDemo;
    private static Transition componentTransitions;
    private Hashtable demosHash = new Hashtable();
    private static MainScreenForm mainMenu;
    private int cols;
    private int elementWidth;
    
    protected void startApp() {
        try {
            Display.init(this);
            //set the theme
            Resources r1 = Resources.open("/LWUITtheme.res");
            UIManager.getInstance().setThemeProps(r1.getTheme(r1.getThemeResourceNames()[0]));
            //open the resources file that contains all the icons
            final Resources r2 = Resources.open("/resources.res");
            //although calling directly to setMainForm(r2) will work on
            //most devices, a good coding practice will be to allow the midp 
            //thread to return and to do all the UI on the EDT.
            Display.getInstance().callSerially(new Runnable() {
                public void run() {
                    setMainForm(r2);
                }
            });
            
        } catch (Throwable ex) {
            ex.printStackTrace();
            Dialog.show("Exception", ex.getMessage(), "OK", null);
        }
    }

    /**
     * Used instead of using the Resources API to allow us to fetch locally downloaded
     * resources
     * 
     * @param name the name of the resource
     * @return a resources object
     */
    public static Resources getResource(String name) throws IOException {
            return Resources.open("/" + name + ".res");
    }

    protected void pauseApp() {
    }

    protected void destroyApp(boolean arg0) {
    }

    public static void setTransition(Transition in, Transition out) {
        mainMenu.setTransitionInAnimator(in);
        mainMenu.setTransitionOutAnimator(out);
    }

    public static void setMenuTransition(Transition in, Transition out) {
        mainMenu.setMenuTransitions(in, out);
        UIManager.getInstance().getLookAndFeel().setDefaultMenuTransitionIn(in);
        UIManager.getInstance().getLookAndFeel().setDefaultMenuTransitionOut(out);
    }

    public static Form getMainForm() {
        return mainMenu;
    }

    public static void setComponentTransition(Transition t) {
        if (t != null) {
            mainMenu.setSmoothScrolling(false);
        }
        componentTransitions = t;
    }

    public static Transition getComponentTransition() {
        return componentTransitions;
    }

    void setMainForm(Resources r) {
        UIManager.getInstance().setResourceBundle(r.getL10N("localize", "en"));

        // application logic determins the number of columns based on the screen size
        // this is why we need to be aware of screen size changes which is currently
        // only received using this approach
        mainMenu = new MainScreenForm(this, "LWUIT Demo");

        int width = Display.getInstance().getDisplayWidth(); //get the display width 

        elementWidth = 0;

        mainMenu.setTransitionOutAnimator(CommonTransitions.createFade(400));

        Image[] selectedImages = new Image[DEMOS.length];
        Image[] unselectedImages = new Image[DEMOS.length];

        final ButtonActionListener bAListner = new ButtonActionListener();
        for (int i = 0; i < DEMOS.length; i++) {
            Image temp = r.getImage(DEMOS[i].getName() + "_sel.png");
            selectedImages[i] = temp;
            unselectedImages[i] = r.getImage(DEMOS[i].getName() + "_unsel.png");
            final Button b = new Button(DEMOS[i].getName(), unselectedImages[i]);
            b.setUIID("DemoButton");
            b.setRolloverIcon(selectedImages[i]);
            b.setAlignment(Label.CENTER);
            b.setTextPosition(Label.BOTTOM);
            mainMenu.addComponent(b);
            b.addActionListener(bAListner);
            b.addFocusListener(new  

                  FocusListener( ) {
                       public void focusGained(Component cmp) {
                    if (componentTransitions != null) {
                        mainMenu.replace(b, b, componentTransitions);
                    }
                }

                public void focusLost(Component cmp) {
                }
            });

            demosHash.put(b, DEMOS[i]);
            elementWidth = Math.max(b.getPreferredW(), elementWidth);
        }

        //Calculate the number of columns for the GridLayout according to the 
        //screen width
        cols = width / elementWidth;
        int rows = DEMOS.length / cols;
        mainMenu.setLayout(new GridLayout(rows, cols));

        mainMenu.addCommand(exitCommand);
        mainMenu.addCommand(aboutCommand);
        mainMenu.addCommand(dragModeCommand);
        mainMenu.addCommand(runCommand);

        mainMenu.setCommandListener(this);
        mainMenu.show();
    }

    /**
     * Invoked when a command is clicked. We could derive from Command but that would 
     * require 3 separate classes.
     */
    public void actionPerformed(ActionEvent evt) {
        Command cmd = evt.getCommand();
        switch (cmd.getId()) {
            case RUN_COMMAND:
                currentDemo = ((Demo) (demosHash.get(mainMenu.getFocused())));
                currentDemo.run(backCommand, this);
                break;
            case EXIT_COMMAND:
                notifyDestroyed();
                break;
            case BACK_COMMAND:
                currentDemo.cleanup();
                mainMenu.show();

                // for series 40 devices
                System.gc();
                System.gc();
                break;
            case ABOUT_COMMAND:
                Form aboutForm = new Form("About");
                aboutForm.setScrollable(false);
                aboutForm.setLayout(new BorderLayout());
                TextArea aboutText = new TextArea(getAboutText(), 5, 10);
                aboutText.setEditable(false);
                aboutForm.addComponent(BorderLayout.CENTER, aboutText);
                aboutForm.addCommand(new Command("Back") {

                    public void actionPerformed(ActionEvent evt) {
                        mainMenu.show();
                    }
                });
                aboutForm.show();
                break;
            case DRAG_MODE_COMMAND:
                mainMenu.setDragMode(true);
                mainMenu.removeCommand(dragModeCommand);
                mainMenu.addCommand(scrollModeCommand);
                break;
            case SCROLL_MODE_COMMAND:
                mainMenu.setDragMode(false);
                mainMenu.removeCommand(scrollModeCommand);
                mainMenu.addCommand(dragModeCommand);
                break;
        }
    }

    private String getAboutText() {
        return "LWUIT Demo shows the main features for developing a User " +
                "Interface (UI) in Java ME. " +
                "This demo contains inside additional different sub-demos " +
                "to demonstrate key features, where each one can be reached " +
                "from the main screen. For more details about each sub-demo, " +
                "please visit the demo help screen. For more details about LWUIT" +
                " feel free to contact us at lwuit@sun.com.";
    }

    private class ButtonActionListener implements ActionListener {

        public void actionPerformed(ActionEvent evt) {
            currentDemo = ((Demo) (demosHash.get(evt.getSource())));
            currentDemo.run(backCommand, UIDemoMIDlet.this);
        }
    }

    
}
