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
package com.astrientlabs.fotomica;

import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Display;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Image;
import javax.microedition.midlet.MIDlet;
import javax.microedition.midlet.MIDletStateChangeException;

import com.astrientlabs.fotomica.ui.LogScreen;
import com.astrientlabs.fotomica.ui.SplashScreen;
import com.astrientlabs.fotomica.util.Images;
import com.astrientlabs.prefs.SystemPreferences;


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
