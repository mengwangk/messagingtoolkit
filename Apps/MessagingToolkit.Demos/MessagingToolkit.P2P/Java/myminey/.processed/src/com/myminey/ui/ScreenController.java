
package com.myminey.ui;

import com.myminey.Slideshow;
import com.myminey.prefs.SystemPreferences;


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
