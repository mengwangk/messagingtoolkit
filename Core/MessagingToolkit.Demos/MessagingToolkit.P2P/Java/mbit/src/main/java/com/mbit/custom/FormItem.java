package com.mbit.custom;

import javax.microedition.lcdui.*;
import java.util.Timer;
import java.util.TimerTask;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 18, 2006 8:41:38 PM
 * @version : $Id:
 */
public class FormItem  extends CustomItem implements ItemCommandListener {

    private int width, height;

    private boolean ticking;

    private TickerTask tickerTask;

    private Timer timer;

    /**
     * The message set in this Ticker
     */
    private String message;

    /**
     * The message to display in this Ticker
     */
    private String displayedMessage;

    /**
     * The current location on screen of the scrolling message
     */
    private int messageLoc;

    /**
     * The pixel width of the message being displayed
     */
    private int messageWidth;

    /**
     * tickSpeed is the distance in pixels the message will
     * travel during one tick
     */
    private int tickSpeed;

    /**
     * TICK_RATE is the number of milliseconds between ticks
     */
    static final long TICK_RATE = 250;


    /**
     * Constructor.
     *
     * @param str
     * @param width
     */
    public FormItem(String str, int width) {
        super("");
        this.width = width;
        height = Font.getDefaultFont().getHeight();
        setupText(str);
        ticking = false;
    }

    /**
     * Sets the string to be displayed by this ticker. If this ticker is active
     * and is on the display, it immediately begins showing the new string.
     *
     * @param str string to be set for the <code>Ticker</code>
     * @throws NullPointerException if <code>str</code> is <code>null</code>
     * @see #getString
     */
    public void setString(String str) {
        setupText(str);
    }

    /**
     * Gets the string currently being scrolled by the ticker.
     *
     * @return string of the ticker
     * @see #setString
     */
    public String getString() {
        // SYNC NOTE: return of atomic value, no locking necessary
        return message;
    }

    protected int getMinContentWidth() {
        return width;
    }

    protected int getMinContentHeight() {
        return height;
    }

    protected int getPrefContentWidth(int i) {
        return width;
    }

    protected int getPrefContentHeight(int i) {
        return height;
    }

    protected void paint(Graphics graphics, int i, int i1) {
        if (ticking) {

            messageLoc -= tickSpeed;
            graphics.drawString(displayedMessage, messageLoc, 0,
                    Graphics.TOP | Graphics.LEFT);

            // Once the message is completely off the left side of
            // the screen, we reset its location to be the right side
            // of the screen
            if (messageLoc <= -messageWidth) {
                //messageLoc = displayable.getWidth();
                messageLoc = width;
            }
        } else {
            graphics.drawString(displayedMessage, 0, 0,
                    Graphics.TOP | Graphics.LEFT);
        }

    }

    public void commandAction(Command command, Item item) {

    }

    protected void keyPressed(int i) {
        super.keyPressed(i);
    }

    protected boolean traverse(int i, int i1, int i2, int[] ints) {
        boolean ret = super.traverse(i, i1, i2, ints);
        // Set ticking
        ticking = true;
        if (timer != null) {
            timer.cancel();
            timer = null;
            tickerTask = null;
        }
        tickerTask = new TickerTask(this);
        timer = new Timer();
        reset();
        timer.schedule(tickerTask, 0, TICK_RATE);
        return ret;
    }

    protected void traverseOut() {
        super.traverseOut();
        // Stop ticking
        ticking = false;
        if (timer != null) {
            timer.cancel();
            timer = null;
        }
    }

    /**
     * Initialize this Ticker with the given text
     *
     * @param message The text this Ticker will display
     */
    private void setupText(String message) {
        if (message == null) {
            throw new NullPointerException();
        }

        /*
        * Search the message for linebreak characters, and replace
        * with spaces.
        */
        StringBuffer msg = new StringBuffer(message);
        int offset = 0;
        boolean modified = false;
        while ((offset = message.indexOf('\n', offset)) != -1) {
            msg.setCharAt(offset, ' ');
            offset++;
            modified = true;
        }

        /*
        * Save the original unmodified message so that getString()
        * returns that. If the message is modified because it
        * contains linebreak characters, then set the display message
        * to the modified string.
        */
        this.message = message;
        this.displayedMessage = modified ? msg.toString() : message;

        messageWidth = Font.getDefaultFont().stringWidth(this.displayedMessage);

        if (messageWidth < 5) {
            tickSpeed = messageWidth;
        } else {
            tickSpeed = 5;
        }
        reset();
    }

    void reset() {
        //messageLoc = displayable.getWidth() / 2;
        messageLoc = width / 2;
    }


    class TickerTask extends TimerTask {
        FormItem ticker;
        public TickerTask(FormItem ticker) {
            this.ticker = ticker;
        }

        public void run() {
            ticker.repaint();
        }
    }

}
