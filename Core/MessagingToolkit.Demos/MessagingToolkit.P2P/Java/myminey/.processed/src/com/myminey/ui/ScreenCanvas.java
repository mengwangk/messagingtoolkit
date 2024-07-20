
package com.myminey.ui;

import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;

import com.myminey.Slideshow;
import com.myminey.util.Images;

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
