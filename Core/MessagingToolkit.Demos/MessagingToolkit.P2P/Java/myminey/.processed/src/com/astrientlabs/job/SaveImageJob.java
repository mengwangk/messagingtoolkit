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

import com.astrientlabs.fotomica.comm.PhotoStore;
import com.astrientlabs.fotomica.comm.Photo;
import com.astrientlabs.fotomica.ui.LogScreen;

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
