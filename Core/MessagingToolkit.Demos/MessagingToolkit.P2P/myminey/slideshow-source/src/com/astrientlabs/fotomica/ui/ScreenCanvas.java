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

import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;

import com.astrientlabs.fotomica.Slideshow;
import com.astrientlabs.fotomica.util.Images;

public abstract class ScreenCanvas extends Canvas
{
	public abstract void initialize();
	
	public static final Font SMALL = Font.getFont(Font.FACE_PROPORTIONAL,	Font.STYLE_PLAIN, Font.SIZE_SMALL);

	
	protected int background = 0xD1FDCF;
	protected static int top = SMALL.getHeight()+14;
	private int fontHeight = SMALL.getHeight();


    public static final int TEXT = 0x93A1B7;
    public static final int SCREEN_TEXT = 0x333333;
	
	
	public void drawHeaderAndFooter(Graphics g, String text)
	{		
		int w = getWidth();
		int h = getHeight();
			
		g.setFont(SMALL);
		g.setColor(background);
		g.fillRect(0,0,w,h);
		
		g.setColor(ScreenCanvas.TEXT);
		g.drawLine(0,fontHeight+3,w,fontHeight+3);
		g.setColor(0x000000);
		g.fillRect(0,0,w,fontHeight+3);
		g.setColor(ScreenCanvas.TEXT);
		g.drawString("Slideshow", w >> 1, 2, Graphics.TOP | Graphics.HCENTER);
		
		
		g.drawImage(Images.TITLE,w>>1,h>>1,Graphics.VCENTER | Graphics.HCENTER);
		
		g.setColor(ScreenCanvas.TEXT);
		g.drawLine(0,h-fontHeight-4,w,h-fontHeight-4);
		g.setColor(0x000000);
		g.fillRect(0,h-fontHeight-3,w,fontHeight+3);
		g.setColor(ScreenCanvas.TEXT);
		g.drawString(text, w >> 1, h-fontHeight, Graphics.TOP | Graphics.HCENTER);		
	}
	
	public void keyRepeated(int keyCode)
	{
		keyPressed(keyCode);
	}
	
	public void keyPressed(int keyCode)
	{
		if (keyCode == KEY_STAR)
		{
			Slideshow.instance.exitMIDlet();
			return;
		}
		
		int action = getGameAction(keyCode);
		
		switch (action)
		{
			case UP :  
			          break;
			case LEFT : ScreenController.instance.prev(this); 
			            break;
			case DOWN : 
			            break;
			case RIGHT : ScreenController.instance.next(this);
			            break;
			case FIRE :	break;
		}
	}
}
