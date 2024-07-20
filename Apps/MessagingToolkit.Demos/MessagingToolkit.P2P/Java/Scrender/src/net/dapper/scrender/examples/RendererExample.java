/**
 * 
 */
package net.dapper.scrender.examples;

import java.io.File;

import net.dapper.scrender.Scrender;

/**
 * @author Ohad Serfaty
 *
 */
public class RendererExample
{

	
	public static void main(String[] args) throws Exception
	{
		// Create a standard self-disposing scrender object :
		Scrender scrender = new Scrender();
		scrender.init();		
		// render it ( and dispose when finish):
		scrender.render("http://www.twit88.com/blog", new File("./scrender.index.html.jpg"));
		
		// Creae a second scrender object , one that is continuous ( meaning , you can make all the screenshots
		// you want but you have to dispose it yourself )
		Scrender scrender2 = new Scrender(true);
		scrender2.init();
		scrender2.render("http://www.google.com", new File("./google.com.jpg"));
		scrender2.render("http://www.yahoo.com", new File("./yahoo.com.jpg"));
		scrender2.dispose();
	
	}
}
