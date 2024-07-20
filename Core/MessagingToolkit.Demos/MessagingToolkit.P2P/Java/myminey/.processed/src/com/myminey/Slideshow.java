
package com.myminey;

import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Display;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Image;
import javax.microedition.midlet.MIDlet;
import javax.microedition.midlet.MIDletStateChangeException;

import com.myminey.prefs.SystemPreferences;
import com.myminey.ui.LogScreen;
import com.myminey.ui.SplashScreen;
import com.myminey.util.Images;


public class Slideshow extends MIDlet implements CommandListener
{
	public static Display display;
	public static Slideshow instance;
	
	private SplashScreen splashScreen;

	public Slideshow()
	{
		instance = this;
	}


	protected void destroyApp(boolean unconditional) throws MIDletStateChangeException
	{
		exitMIDlet();
	}

	protected void pauseApp()
	{
	}

	protected void startApp() throws MIDletStateChangeException
	{
		if (display == null)
		{ 
			display = Display.getDisplay(this);
			try
			{
				splashScreen = new SplashScreen(Image.createImage("/splash.jpg"));
				SystemPreferences.instance.refresh();
				Images.init();
			}
			catch (Exception e)
			{
				LogScreen.instance.log(e);
			}
		}
	}

	public void exitMIDlet()
	{
		try
		{
			notifyDestroyed();			
		}
		catch (Exception e)
		{
			
		}
	}

	public void commandAction(Command c, Displayable d)
	{
		exitMIDlet();
	}
}
