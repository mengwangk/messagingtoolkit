
package com.myminey.comm;

import java.util.Timer;
import java.util.TimerTask;

import javax.microedition.rms.InvalidRecordIDException;
import javax.microedition.rms.RecordEnumeration;
import javax.microedition.rms.RecordStore;

import com.myminey.prefs.SystemPreferences;
import com.myminey.ui.LogScreen;
import com.myminey.ui.PhotoCanvas;


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
