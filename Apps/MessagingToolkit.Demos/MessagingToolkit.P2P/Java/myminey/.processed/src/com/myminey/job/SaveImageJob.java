
package com.myminey.job;

import com.myminey.comm.Photo;
import com.myminey.comm.PhotoStore;
import com.myminey.ui.LogScreen;

public class SaveImageJob implements Runnable
{
	private Photo snapShot;
	private PhotoStore imageStore;

	public SaveImageJob(byte[] data, PhotoStore imageStore)
	{
		this.imageStore = imageStore;
		this.snapShot = new Photo(data);
	}

	public void run()
	{
		try
		{
			imageStore.add(snapShot);
		}
		catch (Throwable t)
		{
			LogScreen.instance.log(t);
		}
	}
}
