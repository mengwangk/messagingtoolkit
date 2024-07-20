
package com.myminey.prefs;

import java.util.Enumeration;
import java.util.Hashtable;

import javax.microedition.rms.RecordEnumeration;
import javax.microedition.rms.RecordStore;

import com.myminey.ui.LogScreen;

public class SystemPreferences
{
	public static final String RECORD_STORE_NAME = "pdb";
	public static final char RECORD_DELIMITER = '=';
	
	
    private Hashtable preferences; 
    
    public static final SystemPreferences instance = new SystemPreferences();
    
    
    private SystemPreferences()
    {
    	preferences = new Hashtable();

    }  
    
    public String get(String name)
    {
    	return (String)preferences.get(name);
    }
    
    
    public String get(String name, String def)
    {
    	Object value = preferences.get(name);
    	return  ( value == null ) ? def : String.valueOf(value);	
    }
    
    public boolean getBoolean(String name)
    {
        String val = get(name);
        return ( val == null ) ? false : val.equals("true");
    }
    
    
    public int getInt(String name, int def)
    {
        int rc = def;
        String val = get(name);
        if ( val != null )
        {
            try
            {
                rc = Integer.parseInt(val);
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
        }
        return rc;
    }
    
    
    public void set(String name, String value)
    {
        if ( value == null ) return;
    	preferences.put(name,value);
    }
    
      
	public void refresh()
	{
		RecordStore preferenceDB = null;
		try
		{
			preferenceDB = RecordStore.openRecordStore(RECORD_STORE_NAME, true);
			RecordEnumeration renum = preferenceDB.enumerateRecords(null, null, false);

			int split = 0;
			String temp;

			while (renum.hasNextElement())
			{
				temp = new String(renum.nextRecord());
				
				split = temp.indexOf(RECORD_DELIMITER);
				if ( split != -1 )
				{
					set(temp.substring(0,split),temp.substring(split+1));
				}
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
				if (preferenceDB != null)
					preferenceDB.closeRecordStore();
			}
			catch (Exception e)
			{
			}
		}
	}

	public void save()
	{
		RecordStore preferenceDB = null;
	    try
	    {
            RecordStore.deleteRecordStore(RECORD_STORE_NAME);
	    }
	    catch (Exception e)
	    {
	        LogScreen.instance.log(e);
	    }
		
		try
		{
			preferenceDB = RecordStore.openRecordStore(RECORD_STORE_NAME, true);
			
			Object key;
			byte[] bytes;
			
			StringBuffer buffer = new StringBuffer();
			Enumeration renum = preferences.keys();
			while (renum.hasMoreElements())
			{
				key = renum.nextElement();
				buffer.append(key);
				buffer.append(RECORD_DELIMITER);
				buffer.append(preferences.get(key));
				bytes = buffer.toString().getBytes();
				
				preferenceDB.addRecord(bytes, 0, bytes.length);
				buffer.setLength(0);
			}
			
			LogScreen.instance.log("Preferences saved.");
		}
		catch (Exception e)
		{
			LogScreen.instance.log(e);
		}
		finally
		{
			try
			{
				if (preferenceDB != null)
					preferenceDB.closeRecordStore();
			}
			catch (Exception e)
			{
			    LogScreen.instance.log(e);
			}
		}
	}
}