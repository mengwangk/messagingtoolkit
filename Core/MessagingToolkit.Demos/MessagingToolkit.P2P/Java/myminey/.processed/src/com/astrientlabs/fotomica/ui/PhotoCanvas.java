/*
 * Copyright (C) 2005 Astrient Labs, LLC
 * 
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 2 of the License, or (at your option) any later
 * version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 * 
 * You should have received a copy of the GNU General Public License along with
 * this program; if not, write to the Free Software Foundation, Inc., 59 Temple
 * Place - Suite 330, Boston, MA 02111-1307, USA.
 * 
 * Astrient Labs, LLC. 
 * www.astrientlabs.com 
 * rashid@astrientlabs.com
 * Rashid Mayes 2005
 */
package com.astrientlabs.fotomica.ui;

import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;

import com.astrientlabs.fotomica.Slideshow;
import com.astrientlabs.fotomica.comm.Photo;
import com.astrientlabs.fotomica.comm.PhotoStore;
import com.astrientlabs.fotomica.comm.Slider;

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
