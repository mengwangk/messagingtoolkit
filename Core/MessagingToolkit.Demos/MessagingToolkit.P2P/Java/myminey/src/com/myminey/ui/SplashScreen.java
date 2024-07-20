
package com.myminey.ui;

import java.util.Timer;
import java.util.TimerTask;

import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;

import com.myminey.Slideshow;

public class SplashScreen extends Canvas
{
	protected Timer timer = new Timer();
	protected Image image;
	
	public SplashScreen(Image image)
	{
		this.image = image;
		setFullScreenMode(true);
		Slideshow.display.setCurrent(this);	
	}

	protected void paint(Graphics g)
	{
		int h = getHeight();
		int w = getWidth();
		g.setColor(0x000000);
		g.fillRect(0,0,w,h);
		g.drawImage(image, w >> 1, h >> 1, Graphics.VCENTER | Graphics.HCENTER);
	}

	protected void showNotify()
	{
		timer.schedule(new DisplayTimeout(), 3 * 1000);
	}

	private void dismiss()
	{
		if ( timer != null ) 
		{
		    timer.cancel();
		    timer = null;
		}

		ScreenController.instance.next();
	}

	private class DisplayTimeout extends TimerTask
	{
		public void run()
		{
			dismiss();
		}
	}
}
