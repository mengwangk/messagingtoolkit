
package com.myminey.job;

import java.util.NoSuchElementException;
import java.util.Timer;
import java.util.TimerTask;
import java.util.Vector;


public class JobRunner extends TimerTask
{
	private Vector queue = new Vector();
	private boolean executing = false;
	
	public static final JobRunner instance = new JobRunner();

	private Timer timer = null;


	private JobRunner()
	{
		if (timer == null)
		{
			timer = new Timer();
			timer.scheduleAtFixedRate(this, 0, 1000 * 3);
		}
	}
	
	
	public void run(Runnable command)
	{
		queue.addElement(command);
	}


	public void run()
	{
		processCommands();
	}

	public void processCommands()
	{
		if (!executing)
		{
			try
			{
				executing = true;
				
				for ( int i = queue.size(); i > 0; i-- )
				{
					try
					{
						((Runnable) queue.elementAt(0)).run();
					}
					catch (NoSuchElementException nsee)
					{
						//nsee.printStackTrace();
						break;
					}
					catch (Exception e)
					{
						//e.printStackTrace();
					}
					finally
					{
						queue.removeElementAt(0);
					}
				}

			}
			finally
			{
				executing = false;
			}
		}
	}
}
