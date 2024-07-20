
package com.myminey.ui;

import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;

import com.myminey.Slideshow;
import com.myminey.comm.Photo;
import com.myminey.comm.PhotoStore;
import com.myminey.comm.Slider;

public class PhotoCanvas extends ScreenCanvas
{
    protected PhotoStore imageStore = new PhotoStore();
    protected Slider slider = new Slider();
    public static final PhotoCanvas instance = new PhotoCanvas();
    
    protected int height;
    protected int width;
    protected Image image;
    private boolean initialized = false;


    public PhotoCanvas()
    {
        setFullScreenMode(true);
       
        height = getHeight();
        width = getWidth();
    }
    

    public void initialize()
    {
        if (!initialized)
        {
            imageStore.start();
            slider.start();
            initialized = true;
        }
    }


    public void paint(Graphics g)
    {
		g.setColor(0x000000);
		g.fillRect(0,0,width,height);
        
        if ( image != null )
        {
            try
            {
                g.drawImage(image, width >> 1, height >> 1, Graphics.VCENTER | Graphics.HCENTER);
            }
            catch (Exception e)
            {
                LogScreen.instance.log(e);
            }  
        }
        else
        {
            g.setFont(SMALL);
            g.setColor(0x93A1B7);
            g.drawString("Loading...Please wait.", width >> 1, height >> 1, Graphics.TOP | Graphics.HCENTER);
        }
    }


    public void keyPressed(int keyCode)
    {
        if (keyCode == KEY_STAR)
        {
            Slideshow.instance.exitMIDlet();
        }
        else
        {
            int action = getGameAction(keyCode);

            switch (action)
            {
                case UP:
                    break;
                case LEFT:
                    ScreenController.instance.prev(this);
                    break;
                case DOWN:
                    break;
                case RIGHT:
                    ScreenController.instance.next(this);
                    break;
            }
        }
    }

   
    public void setImage(Photo s)
    {
        try
        { 
            if ( isShown() ) 
            {
                image = Image.createImage(s.data,0,s.data.length);
                
                //remove this line based on performance
                System.gc();
                repaint();
                //Slideshow.display.flashBacklight(1000);
            }
        }
        catch (Exception e)
        {
            LogScreen.instance.log(e);
        }
    }
}