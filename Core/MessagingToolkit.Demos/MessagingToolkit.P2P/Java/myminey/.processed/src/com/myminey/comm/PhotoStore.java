
package com.myminey.comm;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.InputStream;
import java.util.Timer;
import java.util.TimerTask;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.rms.RecordComparator;
import javax.microedition.rms.RecordEnumeration;
import javax.microedition.rms.RecordStore;
import javax.microedition.rms.RecordStoreException;

import com.myminey.Slideshow;
import com.myminey.job.JobRunner;
import com.myminey.job.SaveImageJob;
import com.myminey.prefs.SystemPreferences;
import com.myminey.ui.LogScreen;
import com.myminey.ui.PhotoCanvas;



public class PhotoStore extends TimerTask implements RecordComparator
{
	private int id = 0;
	private static Timer timer = null;
	public static final String STORE_NAME = "slideshow-images";
	private boolean running = false;
	private int cacheSize ;

	public PhotoStore()
	{			
		cacheSize = Integer.parseInt(Slideshow.instance.getAppProperty("myminey-cachesize"));
	}
	
	public void start()
	{
		if (timer == null)
		{
		    int freq = SystemPreferences.instance.getInt("dl.f",30);
			timer = new Timer();
			timer.scheduleAtFixedRate(this, 0, 1000 * freq);
		}
	}

	public int add(Photo ss)
	{
		return add(ss.toPersistentFormat());
	}

	public int add(byte[] data)
	{
		RecordStore store = null;
		try
		{
			store = RecordStore.openRecordStore(STORE_NAME, true);
			return store.addRecord(data, 0, data.length);
		}
		catch (RecordStoreException e)
		{
			LogScreen.instance.log(e);
		}
		finally
		{
			try
			{
				if (store != null)
					store.closeRecordStore();
			}
			catch (Exception e)
			{
			}
		}

		return -1;
	}

	public void delete(Photo ss)
	{
		delete(ss.recordId);
	}

	public void delete(int id)
	{
		RecordStore store = null;
		try
		{
			store = RecordStore.openRecordStore(STORE_NAME, true);
			store.deleteRecord(id);
		}
		catch (RecordStoreException e)
		{
			LogScreen.instance.log(e);
			//e.printStackTrace();
		}
		finally
		{

		}
	}


	public void run()
	{
		if ( !running )
		{
		    deleteOldImage();
		    getNewImage();
		}
	}
	
	
	public void deleteOldImage()
	{
	    RecordStore store = null;
	    try
	    {
			store = RecordStore.openRecordStore(STORE_NAME, true);
			
			
			if ( store.getNumRecords() > cacheSize )
			{
			    
				RecordEnumeration renum = store.enumerateRecords(null, this, false);
				if (renum.hasNextElement())
				{
					try
					{					
						delete(renum.nextRecordId());
					}
					catch (Exception e)
					{
						LogScreen.instance.log(e);
					}
				}
				renum.destroy();			    
			}
	    }
		catch (Exception e)
		{
			LogScreen.instance.log(e);
		}
		finally
		{
			try
			{
				if (store != null)
					store.closeRecordStore();
			}
			catch (Exception e)
			{
			}
		}
	}
	
	public void getNewImage()
	{
		running = true;
		HttpConnection c = null;
		InputStream is = null;
		
		String category = SystemPreferences.instance.get("c");
		String height = String.valueOf(PhotoCanvas.instance.getHeight());
		String width = String.valueOf(PhotoCanvas.instance.getWidth());

		String uploadURL = Slideshow.instance.getAppProperty("myminey-host") + "/slideshow?c=" + category + "&li=" + (id++) + "&h=" + height + "&w=" + width;

		try
		{	        
		    c = (HttpConnection) Connector.open(uploadURL,Connector.READ_WRITE);
		    c.setRequestMethod(HttpConnection.GET);
		    int rc = c.getResponseCode();
		    LogScreen.instance.log("Image dld reponse code is " + rc);
		    
		    if (  rc == HttpConnection.HTTP_OK )
		    {
				is = c.openInputStream();
				 
				
			    ByteArrayOutputStream baos = new ByteArrayOutputStream(1024);
			    
			    int i;
			    while ( ( i = is.read()) != -1 )
			    {
			        baos.write(i);
			    }
				
				
				JobRunner.instance.run(new SaveImageJob(baos.toByteArray(), this));		        
		    }
		}
		catch (Exception e)
		{
			LogScreen.instance.log(e);
		}
		finally
		{
			running = false;
			
			if (is != null)
			{
				try
				{
					is.close();
				}
				catch (Exception e)
				{
				}
			}
			if (c != null)
			{
				try
				{
					c.close();
				}
				catch (Exception e)
				{
				}
			}
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
	}
	
	
    public int compare(byte[] rec1, byte[] rec2)
    {
        try
        {
            long t1 = new DataInputStream(new ByteArrayInputStream(rec1)).readLong();
            long t2 = new DataInputStream(new ByteArrayInputStream(rec2)).readLong();
            
            if ( t1 < t2 )
            {
                return PRECEDES;
            }
            else
            {
                return FOLLOWS;
            }
        }
        catch (Exception e)
        {
            return PRECEDES;
        }
    }
}
