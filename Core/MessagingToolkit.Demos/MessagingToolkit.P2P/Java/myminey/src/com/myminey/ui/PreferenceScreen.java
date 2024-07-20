
package com.myminey.ui;

import javax.microedition.lcdui.Choice;
import javax.microedition.lcdui.Command;
import javax.microedition.lcdui.CommandListener;
import javax.microedition.lcdui.Displayable;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.List;
import javax.microedition.lcdui.TextBox;
import javax.microedition.lcdui.TextField;

import com.myminey.Slideshow;
import com.myminey.prefs.SystemPreferences;
import com.myminey.text.LineEnumeration;
import com.myminey.util.Images;

public class PreferenceScreen extends ScreenCanvas implements CommandListener
{
	public static final PreferenceScreen instance = new PreferenceScreen();
	private int focus;

	private String fprompt = "Keyword:";
	
	private int plen;
	private int fontHeight = SMALL.getHeight();
	
	private TextBox textBox = new TextBox("Edit", "", 160, TextField.ANY);
	private Command OK = new Command("OK", Command.BACK, 1);
	

	private String category;
	
	private String[] categories = { 
	        "animals",
	        "architecture",
	        "art",
	        "beach",
	        "car",
	        "china",
	        "friends",
	        "girl",
	        "hot",
	        "interesting",
	        "italy",
	        "japan",
	        "london",
	        "me",
	        "mountains",
	        "nyc",
	        "paris",
	        "party",
	        "portrait",
	        "sanfrancisco",
	        "sunset",
	        "tokyo",
	        "wedding",
	        "other"
	        };
	
	private List categoryList;
	LineEnumeration e;
	
	public PreferenceScreen()
    {		
        setFullScreenMode(true);

        textBox.addCommand(OK);
        textBox.setCommandListener(this);

        
        categoryList = new List("Keyword", Choice.IMPLICIT, categories, null);
        categoryList.addCommand(OK);
        categoryList.setCommandListener(this);
        
        
		e = new LineEnumeration(SMALL,"Click below and select a keyword for the slideshow.",getWidth()-8);
		
    }
	
	public void initialize()
	{
		focus = 0;
		category = SystemPreferences.instance.get("c",categories[2]);
	}

	protected void paint(Graphics g)
	{
	    drawHeaderAndFooter(g,"<  Preferences  >");
	    
		int w = getWidth();
		int h = getHeight();
		
		int statusY = h-fontHeight-2;
		
		int textX = 6;
		int textY = top;
		
		int boxX = textX + plen + 2;
		int boxY = textY - 4;
		int boxWidth = w - boxX - 4;
		int boxHeight = fontHeight + 4;
		
		int lineSize = boxHeight+6;
		
		g.setFont(SMALL);
		g.setColor(ScreenCanvas.SCREEN_TEXT);
		
		e.reset();
		textY = e.writeTo(g,4,textY,SMALL);
			
		g.drawString(fprompt,4,textY,Graphics.TOP | Graphics.LEFT);
		textY += lineSize;
		boxY = textY-4;
		
		if ( focus == 0 )
		{
		    g.setColor(0xFFBC53);
			g.fillRect(4,boxY+1,w-8,boxHeight-1);
			g.setColor(ScreenCanvas.SCREEN_TEXT);
		}
		g.drawRect(4,boxY+1,w-8,boxHeight-1);

		g.drawString(category,7,textY,Graphics.TOP | Graphics.LEFT);

			

		//save button
		textY += lineSize;
		
		if ( focus == 1 )
		{
		    g.drawImage(Images.SAVEON,w>>1,textY,Graphics.TOP | Graphics.HCENTER);
		}
		else
		{
		    g.drawImage(Images.SAVE,w>>1,textY,Graphics.TOP| Graphics.HCENTER);
		}		
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
			case UP : moveFocus(false);
			          break;
			case LEFT : ScreenController.instance.prev(this); 
			            break;
			case DOWN : moveFocus(true);
			            break;
			case RIGHT : ScreenController.instance.next(this);
			            break;
			case FIRE :	performAction();
		}
	}
	
	
	public void performAction()
	{
		if ( focus == 0 )
		{
			Slideshow.display.setCurrent(categoryList);
		}
		else if ( focus == 1 )
		{
			SystemPreferences.instance.set("c",category);
			SystemPreferences.instance.set("setup","true");
			SystemPreferences.instance.save();
			ScreenController.instance.next();
		}
	}
	
	
	public void moveFocus(boolean forward)
	{
		if ( forward )
		{
			focus++;
			if ( focus > 1) focus = 0;
		}
		else
		{
			focus--;
			if ( focus < 0 ) focus = 1;
		}
		
		repaint();
		
	}
	
	public void commandAction(Command c, Displayable d)
	{
		if ( c == OK )
		{
			if ( d ==  categoryList )
			{
			    int i = categoryList.getSelectedIndex();
			    
			    if ( i == (categories.length-1) )
			    {
			        textBox.setString(category);
			        Slideshow.display.setCurrent(textBox);
			    }
			    else
			    {
			        category = categories[i];
					   
					
					repaint();
					Slideshow.display.setCurrent(this);
			    }
			}
			else if ( d == textBox )
			{
			    category = textBox.getString();
				   
				
				repaint();
				Slideshow.display.setCurrent(this);
			}
		}
	}
}