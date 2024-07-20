
package com.myminey.comm;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;

import com.myminey.ui.LogScreen;


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
