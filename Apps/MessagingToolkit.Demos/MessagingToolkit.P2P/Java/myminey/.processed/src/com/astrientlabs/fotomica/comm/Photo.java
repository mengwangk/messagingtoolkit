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

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;

import com.astrientlabs.fotomica.ui.LogScreen;


public class Photo
{
	public static final int STATUS_NEWIMAGE = 0;
	public static final int STATUS_ERROR = 2;
	public static final int STATUS_RETRY = 3;
	
	public int recordId;
	public byte[] data;
	public long date = System.currentTimeMillis();
	public int status = STATUS_NEWIMAGE;
	
	public Photo()
	{
	}

	public Photo(byte[] data)
	{
		this.data = data;
	}

	public byte[] toPersistentFormat()
	{
		ByteArrayOutputStream baos = new ByteArrayOutputStream();
		DataOutputStream dos = new DataOutputStream(baos);
		
		try
		{
			dos.writeLong(date);
			dos.writeInt(data.length);
			dos.writeInt(status);
			dos.write(data);
			dos.flush();					
		}
		catch (Exception e)
		{
			LogScreen.instance.log(e);
		}
		
		return baos.toByteArray();
	}

	public void fromPersistentFormat(byte[] bytes)
	{
		ByteArrayInputStream bais = new ByteArrayInputStream(bytes);
		DataInputStream dis = new DataInputStream(bais);
		
		try
		{
			date = dis.readLong();
			
			dis.readInt();
			status = dis.readInt();
	
			ByteArrayOutputStream temp = new ByteArrayOutputStream();
			byte[] ba = new byte[1*1024];
			
			int read;
			while ( (read = dis.read(ba)) != -1 )
			{
				temp.write(ba,0,read);
			}
			
			data = temp.toByteArray();
		}
		catch (Exception e)
		{
			LogScreen.instance.log(e);
		}
	}
}
