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
package com.astrientlabs.text;

import java.util.Enumeration;
import java.util.NoSuchElementException;

import javax.microedition.lcdui.*;


public class LineEnumeration implements Enumeration
{
	private Font font;
	private String text;
	
	private int width;
	private int position;
	private int length;
	private int start = 0;

	public LineEnumeration(Font font, String text, int width)
	{
	    this.font = font;
		this.text = text;
		this.width = width;
		this.length = text.length();
	}
	
	
	public boolean hasMoreElements()
	{
	    return (position < (length-1));
	}
	
	public Object nextElement() throws NoSuchElementException
	{
	    try
	    {
	        int next = next();
	        
	        String s = text.substring(start,next);        
	        start = next;
	        
	        if ( text.length()-1 > start && ( text.charAt(start) == ' ' || text.charAt(start) == '\n' ) )
	        {
	            position++;
	            start++;
	        }
	                
	        return s;
	    }
	    catch ( IndexOutOfBoundsException e )
	    {
	        throw new NoSuchElementException(e.getMessage());
	    }
	    catch ( Exception e )
	    {
	        throw new NoSuchElementException(e.getMessage());
	    }
	}
	

	private int next()
	{	    
		int i = position;	
		int lastBreak = -1;
		
		for ( ;i < length && font.stringWidth(text.substring(position,i)) <= width; i++ )
		{
		    if ( text.charAt(i) == ' ' )
		    {
		        lastBreak = i;
		    }
		    else if ( text.charAt(i) == '\n' )
		    {
		        lastBreak = i;
		        break;
		    }  
		}
		
		
		if ( i == length )
		{
		    position = i;
		}
		else if ( lastBreak == position )
		{
		    position++;
		}
		else if ( lastBreak < position )
		{
		    position = i;
		}
		else
		{
		    position = lastBreak;
		}
				
		return position;
	}
	
	
	public int writeTo(Graphics g, int startx, int starty, Font font)
	{
	    int fontHeight = font.getHeight()+1;
	    
		while ( hasMoreElements() ) 
		{	
			g.drawString(String.valueOf(nextElement()), startx, starty, Graphics.TOP | Graphics.LEFT);
			starty += fontHeight;
		}
		
	    return starty;
	}
	
	public void reset()
	{
	    start = 0;
	    position = 0;
	}
}