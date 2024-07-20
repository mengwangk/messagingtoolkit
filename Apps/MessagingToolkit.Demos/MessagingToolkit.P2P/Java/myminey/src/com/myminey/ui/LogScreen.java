
package com.myminey.ui;

import java.util.Calendar;
import java.util.Date;
import java.util.Vector;

import javax.microedition.lcdui.Alert;
import javax.microedition.lcdui.AlertType;
import javax.microedition.lcdui.Graphics;

import com.myminey.Slideshow;
import com.myminey.text.LineEnumeration;

public class LogScreen extends ScreenCanvas
{
	public static final LogScreen instance = new LogScreen();
	public Vector messages = new Vector();
	private int position;
	private int MAX_LENGTH = 15;
	
	private Alert alert = new Alert("");
	
	private int w;
	private int h;
	private int fontHeight;
	
	private int statusY;
	
	private int boxStartX;
	private int boxStartY;
	private int boxWidth; 
	private int boxHeight;
	
	private int scrollbarX;
	private int scrollbarY;
	private int scrollbarWidth;
	private int scrollbarHeight;
	
	private int dateStartY;
	
	private int messageStartY;
	private int messageStartX;
	private int messageWidth;
	private int messageHeight;
	
	
	public LogScreen()
	{
		setFullScreenMode(true);
		
		w = getWidth();
		h = getHeight();
		fontHeight = SMALL.getHeight();
		
		statusY = h-fontHeight-2;
		
		boxStartX = 4;
		boxStartY = fontHeight + 4;
		boxWidth = (w-2*4-8); 
		boxHeight = statusY - boxStartY - 4;
		
		scrollbarX = w-2*4-4;
		scrollbarY = boxStartY;
		scrollbarWidth = 8;
		scrollbarHeight = boxHeight;
		
		dateStartY = boxStartY + 4;
		
		messageStartY = fontHeight + dateStartY + 4;
		messageStartX = boxStartX + 4;
		messageWidth = boxWidth-12;
		messageHeight = boxHeight - 8;
	}
	
	public void initialize()
	{
		position = 0;
	}

	protected void paint(Graphics g)
	{
		int messageCount = messages.size();
		String indexStr  = ( messageCount > 0 ) ? (position+1) + " of " + messageCount : "0 of 0";
		
		drawHeaderAndFooter(g,"<  Log " + indexStr + "  >");
		
		g.setFont(SMALL);
		
		g.setColor(ScreenCanvas.SCREEN_TEXT);
		g.drawRect(boxStartX,boxStartY,boxWidth,boxHeight);
		g.drawRect(scrollbarX, scrollbarY, scrollbarWidth, scrollbarHeight);
		

		if ( messageCount > 0 )
		{
			LogMessage message = (LogMessage)messages.elementAt(position);
			
			int sliceHeight = scrollbarHeight / messageCount;
			
			int sliceY = (position * sliceHeight) + scrollbarY + 1;
			
			int slide =  scrollbarHeight % messageCount ;
			if ( slide > 1 )
			{
				sliceHeight+=slide; 
			} 
			
			g.fillRect(scrollbarX,sliceY,scrollbarWidth,sliceHeight);
			
			g.drawString(message.dateString, messageStartX,dateStartY, Graphics.TOP | Graphics.LEFT);
			g.setClip(0,0,scrollbarX,boxStartY+boxHeight-4);

			LineEnumeration e = new LineEnumeration(SMALL,message.message,messageWidth);
			e.writeTo(g,messageStartX,messageStartY,SMALL);
		}
	}
	
		
	public void log(Throwable t)
	{
	    t.printStackTrace();
	    log(t.getClass().getName() + "\n" + t.getMessage());
	}
	
	public void log(String message)
	{
	    //TODO: uncomment for debug
	    //System.out.println(message);
		messages.insertElementAt(new LogMessage(message),0);
	
		if ( messages.size() > MAX_LENGTH )
		{
			messages.setSize(MAX_LENGTH);
		}
	}
	
	public void prevMessage()
	{
		if ( position > 0 )
		{
			position--;
		}
		
		repaint();
	}
	
	public void nextMessage()
	{
		if 	( position < messages.size() - 1 )
		{
			position++;
		}
		
		repaint();
	}
	
	
	public void keyRepeated(int keyCode)
	{
		keyPressed(keyCode);
	}
	
	public void keyPressed(int keyCode)
	{
		if (keyCode == KEY_STAR)
		{
			Slideshow.instance.exitMIDlet();
			return;
		}
		
		int action = getGameAction(keyCode);
		
		switch (action)
		{
			case UP : prevMessage(); 
			          break;
			case LEFT : ScreenController.instance.prev(this); 
			            break;
			case DOWN : nextMessage();
			            break;
			case RIGHT : ScreenController.instance.next(this);
			            break;
			case FIRE :	zoom(); break;
		}
	}
	
	
	public void zoom()
	{
	    try
	    {
	        LogMessage message = (LogMessage)messages.elementAt(position);

            alert.setType(AlertType.INFO);
            alert.setTimeout(Alert.FOREVER);
            alert.setTitle(message.dateString);
            alert.setString(message.message);
            Slideshow.display.setCurrent(alert);	
	    }
	    catch (Exception e)
	    { 
	    }
	}
}
class LogMessage
{
	public long timeStamp = System.currentTimeMillis();
	public String message;
	public String dateString;
	
	public LogMessage(String message)
	{
		this.message = ( message == null ) ? "Empty message" : message;
		
		Calendar c = Calendar.getInstance();
		c.setTime(new Date(timeStamp));
		
		StringBuffer buffer = new StringBuffer();
		buffer.append(c.get(Calendar.MONTH)+1)
		.append("-")
		.append(c.get(Calendar.DAY_OF_MONTH))
		.append(" ")
		.append(c.get(Calendar.HOUR_OF_DAY))
		.append(":");
		
		int min = c.get(Calendar.MINUTE);
		if ( min > 10 )
		{
			buffer.append(min);
		}
		else
		{
			buffer.append("0").append(min);
		}
		
		this.dateString = buffer.toString();
	}
}