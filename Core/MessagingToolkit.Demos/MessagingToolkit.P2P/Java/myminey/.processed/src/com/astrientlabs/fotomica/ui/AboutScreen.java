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

import com.astrientlabs.fotomica.Slideshow;


public class AboutScreen extends ScreenCanvas
{
	public static final AboutScreen instance = new AboutScreen();
	int w;
	int h;
	int fontHeight;
	
	int textX;
	int textY;
	
	public AboutScreen()
	{
		setFullScreenMode(true);
		
		w = getWidth();
		h = getHeight();
		fontHeight = SMALL.getHeight()+4;		
	}
	
	public void initialize()
	{
	}

	protected void paint(Graphics g)
	{  
		drawHeaderAndFooter(g,"<  About  >");
		
		textX = w>>1;
		textY = fontHeight + 10;

		g.setFont(SMALL);
		g.setColor(ScreenCanvas.SCREEN_TEXT);
		
		g.drawString("Slideshow " + "v" + Slideshow.instance.getAppProperty("MIDlet-Version"), textX,textY, Graphics.TOP | Graphics.HCENTER);


		textY+=(fontHeight>>1);
		textY+=fontHeight;
		g.drawString("Astrient Labs, LLC 2005", textX,textY, Graphics.TOP | Graphics.HCENTER);

		textY+=fontHeight;
		g.drawString("www.astrientlabs.com", textX,textY, Graphics.TOP | Graphics.HCENTER);
	}
}
