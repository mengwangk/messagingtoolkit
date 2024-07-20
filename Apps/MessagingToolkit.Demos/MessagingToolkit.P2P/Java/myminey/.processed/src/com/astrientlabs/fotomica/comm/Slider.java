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
package com.astrientlabs.fotomica.comm;

import java.util.Timer;
import java.util.TimerTask;

import javax.microedition.rms.InvalidRecordIDException;
import javax.microedition.rms.RecordEnumeration;
import javax.microedition.rms.RecordStore;

import com.astrientlabs.fotomica.ui.LogScreen;
import com.astrientlabs.fotomica.ui.PhotoCanvas;
import com.astrientlabs.prefs.SystemPreferences;


public class Slider extends TimerTask
{
	private static Timer timer = null;
	private boolean running = false;
	
	RecordStore store;
	RecordEnumeration renum = null;


	public Slider()
	{
	}
	
	public void start()
	{
		if (timer == null)
		{
			try
			{
	            store = RecordStore.openRecordStore(PhotoStore.STORE_NAME, true);            
	            renum = store.enumerateRecords(null, null, false);            
	            //renum.keepUpdated(true);
			}
			catch (Exception e)
			{
				LogScreen.instance.log(e);
			}
			
			int freq = SystemPreferences.instance.getInt("s.f",4);
			timer = new Timer();
			timer.scheduleAtFixedRate(this, 0, 1000 * freq);
		}
	}


	public void run()
	{
		if ( !running )
		{
		    changePic();
		}
	}
	
	
	public void changePic()
	{
	    try
	    {	    
			if ( !renum.hasNextElement() )
			{
			    //renum.reset();
			    renum.rebuild();
			}
			
			if ( renum.hasNextElement() )
			{
			    Photo p = new Photo();
			    p.fromPersistentFormat(renum.nextRecord());
			    PhotoCanvas.instance.setImage(p);
			}
	    }
	    catch (InvalidRecordIDException e)
	    {
	        renum.rebuild();
	    }
		catch (Exception e)
		{
			LogScreen.instance.log(e);
		}
	}


	public void shutDown()
	{
		if ( timer != null )
		{
		    timer.cancel();
			timer = null;		
			running = false;
		}
		
		try
		{
			if (store != null)
				store.closeRecordStore();
			
			if (renum != null)
				renum.destroy();
		}
		catch (Exception e)
		{
		}
	}
}
