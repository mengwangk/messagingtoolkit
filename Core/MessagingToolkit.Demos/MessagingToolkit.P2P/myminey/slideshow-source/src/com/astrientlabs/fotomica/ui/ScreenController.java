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

import com.astrientlabs.fotomica.Slideshow;
import com.astrientlabs.prefs.SystemPreferences;


public class ScreenController
{
	public static final ScreenController instance = new ScreenController();
	
	private ScreenController()
	{
	}
	
	public void next()
	{
		if ( SystemPreferences.instance.get("setup") == null )
		{  
			PreferenceScreen.instance.initialize();
			Slideshow.display.setCurrent(PreferenceScreen.instance);		
		}
		else
		{
			PhotoCanvas.instance.initialize();
			Slideshow.display.setCurrent(PhotoCanvas.instance);	
		}
	}
	
	
	public void next(ScreenCanvas canvas)
	{
		if ( canvas == PhotoCanvas.instance )
		{
			LogScreen.instance.initialize();
			Slideshow.display.setCurrent(LogScreen.instance);
		}
		else if ( canvas == LogScreen.instance )
		{
			Slideshow.display.setCurrent(AboutScreen.instance);
		}
		else if ( canvas == AboutScreen.instance )
		{
			PreferenceScreen.instance.initialize();	
			Slideshow.display.setCurrent(PreferenceScreen.instance);
		}
		else if ( canvas == PreferenceScreen.instance )
		{
			Slideshow.display.setCurrent(PhotoCanvas.instance);
		}
	}
	
	public void prev(ScreenCanvas canvas)
	{
		if ( canvas == PhotoCanvas.instance )
		{
			PreferenceScreen.instance.initialize();	
			Slideshow.display.setCurrent(PreferenceScreen.instance);
		}
		else if ( canvas == LogScreen.instance )
		{
			Slideshow.display.setCurrent(PhotoCanvas.instance);
		}
		else if ( canvas == AboutScreen.instance )
		{
			Slideshow.display.setCurrent(LogScreen.instance);
		}
		else if ( canvas == PreferenceScreen.instance )
		{
			Slideshow.display.setCurrent(AboutScreen.instance);
		}
	}
}