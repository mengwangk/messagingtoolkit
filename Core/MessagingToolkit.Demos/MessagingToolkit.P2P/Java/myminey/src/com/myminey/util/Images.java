
package com.myminey.util;

import java.io.IOException;

import javax.microedition.lcdui.Image;

public class Images
{
	public static Image TITLE;
	public static Image SAVE;
	public static Image SAVEON;
	
	public static void init()
	{
		try
		{				
			TITLE = Image.createImage("/logo.png");
			SAVE = Image.createImage("/save.png");
			SAVEON = Image.createImage("/saveon.png");
		}
		catch (IOException ioe)
		{
			ioe.printStackTrace();
		}
	}
}
