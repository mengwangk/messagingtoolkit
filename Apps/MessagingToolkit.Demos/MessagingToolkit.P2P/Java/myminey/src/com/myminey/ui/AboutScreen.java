
package com.myminey.ui;

import javax.microedition.lcdui.Graphics;

import com.myminey.Slideshow;


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