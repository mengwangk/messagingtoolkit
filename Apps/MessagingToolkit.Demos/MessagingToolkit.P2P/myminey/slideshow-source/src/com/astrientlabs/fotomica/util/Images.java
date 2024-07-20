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
package com.astrientlabs.fotomica.util;

import java.io.IOException;

import javax.microedition.lcdui.Image;

public class Images
{
	public static Image TITLE;
	public static Image SAVE;
	public static Image SAVEON;
	
	public static void init()
	{
		try
		{				
			TITLE = Image.createImage("/logo.png");
			SAVE = Image.createImage("/save.png");
			SAVEON = Image.createImage("/saveon.png");
		}
		catch (IOException ioe)
		{
			ioe.printStackTrace();
		}
	}
}
