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

import java.util.Timer;
import java.util.TimerTask;

import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;

import com.astrientlabs.fotomica.Slideshow;

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
