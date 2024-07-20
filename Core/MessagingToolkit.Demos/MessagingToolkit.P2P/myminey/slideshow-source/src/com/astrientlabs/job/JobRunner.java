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
package com.astrientlabs.job;

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