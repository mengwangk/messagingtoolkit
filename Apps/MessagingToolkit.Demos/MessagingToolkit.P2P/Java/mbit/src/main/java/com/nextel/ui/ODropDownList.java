/* Copyright (c) 2001 Nextel Communications, Inc.
 * All rights reserved.
 *  
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met: 
 *  
 *   - Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer. 
 *   - Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution. 
 *   - Neither the name of Nextel Communications, Inc. nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission. 
 *  
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
 * IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL NEXTEL
 * COMMUNICATIONS, INC. OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
package com.nextel.ui; // Generated package name
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import com.nextel.exception.NotFoundException;
import com.nextel.util.Logger;
import com.nextel.util.Debugger;

/**
 * A drop down list of scrollable items are displayed.
 * <p>
 * If the component gains focus, a drop down vertical scroll bar and list is 
 * to allow the user to select an item using UP/DOWN arrow keys.  The drop
 * down list scrolled area is dynamically calculated based on the component
 * location (X/Y coordinates), screen size, number of items, and item font.
 * If the component loses focus, the selected item is displayed.
 * <p>
 * The currently selected item is reverse-videoed while unselected items have
 * normal video.
 *
 * @author Anthony Paper
 */
public class ODropDownList extends OFocusableComponent
{
  private int selectedIndex = 0;
  private String [] items;
  private Font itemFont;
  private int itemWidth;
  O3DScrollList focusList;
  private boolean firstTime  = true;
  private boolean adjustFlag = true;
  
  
  /** font to use for calculating scrollbar size */
  private static final Font SCROLLBAR_FONT = OUILook.PLAIN_MEDIUM;

  /** width of a vertical scrollbar */
  private static  int SCROLLBAR_WIDTH = SCROLLBAR_FONT.charWidth( 'W' );
  static
  { // ensure scrollbar width is odd so arrow centers
    if ( SCROLLBAR_WIDTH % 2 == 0 ) SCROLLBAR_WIDTH--;
  }
  
  /** Height of the end boxes in a vertical scrollbar */
  private static final int SCROLLBAR_END_BOX_HEIGHT =
    SCROLLBAR_FONT.getHeight( ) / 2;

  /** Width of an arrow in a vertical scrollbar */
  private static int SCROLLBAR_ARROW_WIDTH = SCROLLBAR_WIDTH - 2;
  private static int SCROLLBAR_ARROW_HEIGHT = SCROLLBAR_END_BOX_HEIGHT - 2;
  
  /** Color of scrollbar arrow */
  private static final int SCROLLBAR_ARROW_COLOR = OColor.BLACK;

  
  /** Constructor method used to create a drop down list using the specified items and font.  
   *  Multiple text lines are not currently supported.
   * @param items Contains the string values for the drop down list
   * @param itemFont Contains the Font used to format and display the item strings.
   */      
  public ODropDownList (String [] items, 
                        Font      itemFont )
  {
    this.selectedIndex = 0;
    this.itemFont =  itemFont;
    this.items = items;
    init();
    setTraverseDirections( OFocusableComponent.RIGHT | OFocusableComponent.LEFT );
    setFocusedComponentBackground( OColor.TRANSPARENT );
  } // constructor
  
  /**
   * Method used to initialize focused and non-focused components
   * to be displayed in the paint method.  The adjustFlag and 
   * firstTime flags are used to indicate whether list parameters
   * need to be recalculated.
   */
  private void init()
  {
    if ( Debugger.ON ) Logger.dev( "ODropDownList.init ENTERED" );
    
    //
    // If this is the first we are calling this routine
    // and the layout manager has given us the X/Y 
    // coordinate positions.
    //
    if (adjustFlag) {
        int maxViewportRows =2;
        int rowsBelow       = 1;
        int rowsAbove       = 0;
        int adjustHeight    = 0;        
        //
        // Calculate the maximum height available
        //
        if (firstTime == false) {
            
           OCompositeScreen screen = getContainer().getScreen();
           int startY = screen.getBodyRow() + 2;
           int stopY  = screen.getBodyRow() + screen.getBodyHeight() - 3 - O3DScrollList.shadowThickness;
          /*
           maxViewportRows = (screen.getBodyHeight()/itemFont.getHeight());
           if (items.length < maxViewportRows) maxViewportRows = items.length;
           */
           int deltaRows = ((Math.abs(stopY - getY()))/itemFont.getHeight());
           if (deltaRows > rowsBelow) rowsBelow = deltaRows;
           deltaRows = ((Math.abs(getY()-startY))/itemFont.getHeight());
           if (deltaRows > rowsAbove) rowsAbove = deltaRows;
           //
           // Adjust based on the data to be displayed
           //
           if (items.length < rowsBelow) { 
               rowsBelow = items.length;
           }
           maxViewportRows = rowsBelow;
        }
        firstTime = false;

        //
        // Calculate the maximum width required
        //
        itemWidth = 0;
        for (int x = 0; x < items.length; x++)
        {
            int w = itemFont.stringWidth(items[x]);
            if (w > itemWidth) itemWidth = w;
        }
        itemWidth = itemWidth + 15;

        int tempHeight = ( OUILook.STRING_SPACER_HEIGHT - 1) + itemFont.getHeight() +
                         ( itemFont.getHeight() * (maxViewportRows - 1 ) );
        
        //
        // If we need to pop-up, let's setup adjustment for
        // the height and starting Y-axis coordinate so that
        // we can popup.
        //
        if ((maxViewportRows < 2)  && (items.length > 1)){
           adjustHeight = OUILook.STRING_SPACER_HEIGHT + itemFont.getHeight();
        }
        focusList = new O3DScrollList(itemWidth, 
                                      tempHeight + adjustHeight, 
                                      itemFont,itemFont);
        focusList.populate(items);
        adjustFlag   = false;
        focusList.setX(getX());
        focusList.setY(getY()-adjustHeight);
        try {
            focusList.setSelectedIndex(selectedIndex);
        } catch (Exception e){ e.printStackTrace();}
        
    }
    if ( Debugger.ON ) Logger.dev( "ODropDownList.keyPressed EXITTING" );
  }


  /**
   * Sets the  focus state to that indicated by <code>id</code>.
   * This method is used to set the active components to be
   * displayed.
   *
   * @param id {@link com.nextel.ui.OFocusEvent#FOCUS_GAINED} or 
   * {@link com.nextel.ui.OFocusEvent#FOCUS_LOST}
   */
  
  public void setFocus( int id ) 
  { 
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setFocus ENTERED" );
    super.setFocus(id);
    if ( id == OFocusEvent.FOCUS_LOST )
    {
     this.repaintAll();
    }
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setFocus EXITTING" );
  } // setFocus

  /**
   * Gets the list item that was selected
   *
   * @return The selected list item.
   */
  public Object getSelectedValue () 
  {
    if ( Debugger.ON ) Logger.dev( "ODropDownList.getSelectedValue CALLED.");
    return this.items[selectedIndex];
  } // getSelectedValue

  /**
   * Sets the item that is selected. If necessary the list will be scrolled so
   * that the item is displayed.
   *
   * @param value The item that is selected
   * @exception NotFoundException if the selected item does not exist
   */
  public void setSelectedValue ( Object value ) 
    throws NotFoundException
  {
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setSelectedValue ENTERED w/value=" +
				value );
    boolean found = false;
    for ( int idx=0; idx < items.length; idx++ ) 
    {
      if ( items[ idx ].equals( value ) ) 
      {
	setSelectedIndex( idx );
	found = true;
	break;
      }
    }
    if ( ! found ) 
    {
      throw new NotFoundException( "ODropDownList.setSelectedValue value " +
				   value + " not found" );
    }
    repaint();
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setSelectedValue EXITTING" );
  } // setSelectedValue

  
  
  /**
   * Method to get the selected index.
   * @return The selected index
   */
  public int getSelectedIndex()
  { 
      if ( Debugger.ON ) Logger.dev( "ODropDownList.getSelectedIndex CALLED" );
      return selectedIndex;
  }
  /** 
   * Method used to set the selected index.
   */
  public void setSelectedIndex(int index)
  {
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setSelectedIndex ENTERED" );
    selectedIndex = index;
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setSelectedIndex EXITTING" );
  }
    
  /** 
   * Method used to capture X-axis coordinate value passed from the
   * container during component layout.  The adjustment flag is set
   * to true and recalculations are performed when painting the
   * component.  Adjustments are performed only when the change
   * in delta-X is greater than one pixel.
   */
  public void setX(int newX)
  {
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setX ENTERED" );
    if (Math.abs(getX()- newX) > 1) {
       adjustFlag = true;
       firstTime  = false;
    }
    super.setX(newX);
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setX EXITTING" );
  }
  /** Method used to capture Y-axis coordinate value passed from the
   * container during component layout.  The adjustment flag is set
   * to true and recalculations are performed when painting the
   * component.  Adjustments are performed only when the change
   * in delta-Y is greater than one pixel.
   */
  public void setY(int newY)
  {
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setY ENTERED" );
    if (Math.abs(getY()- newY) > 1) {
       adjustFlag = true;
       firstTime  = false;
    }
    super.setY(newY);
    if ( Debugger.ON ) Logger.dev( "ODropDownList.setY EXITTING" );
  }
 
  /** Method used to return the pixel height of the drop down list.
   *  The height value is used by the container for the non-focused component.
   * @return Height value in pixels
   */   
  public int getHeight() 
  { 
    if ( Debugger.ON ) Logger.dev( "ODropDownList.getHeight CALLED" );
    return (OUILook.STRING_SPACER_HEIGHT + itemFont.getHeight());
  } // getHeight
   
  /** Method used to return the pixel width of the drop down list.
   *  The height value is used by the container for the non-focused component.
   * @return Width value in pixels
   */   
  public int getWidth() 
  { 
    if ( Debugger.ON ) Logger.dev( "ODropDownList.getWidth CALLED" );
    return focusList.getWidth();
  } // getWidth
   
  /** Method used to paint the drop down list.
   * @param g Contains the graphics object used to repaint the checkbox
   */   
  public void paint( Graphics g) 
  { 
    if ( Debugger.ON ) Logger.dev( "ODropDownList.paint ENTERED" );
    
    Font oldFont = g.getFont();
    int oldColor = g.getColor();
    if (adjustFlag) init();
    
    if (hasFocus()) {
        // since we'return delegating to scroll list, make sure it thinks it has 
        // has focus
        focusList.setFocus( true );
       focusList.paint(g);
    }
    else {
        int x = getX();
        int y = getY();
        int height = getHeight();
        int width  = getWidth();
        
        // fill the scrollbar background
        g.setColor( OUILook.BACKGROUND_COLOR );
        g.fillRect(x,y,width,height+2);
     //   g.fillRect( x, y, SCROLLBAR_WIDTH, height );
          
        x = x + itemWidth - 10;
    
        // draw the up and down arrows
        g.setColor( SCROLLBAR_ARROW_COLOR );
        int arrowPointColumn = x + ( SCROLLBAR_WIDTH / 2 );
        OComponent.drawTriangle( g, arrowPointColumn, y + height - OUILook.V_GAP,
                                SCROLLBAR_ARROW_WIDTH,
                                SCROLLBAR_ARROW_HEIGHT, OComponent.DOWN );
        
        // outline the scrollbar 
        int scrollBarLeftEdge = x - 1;
  //      g.drawRect( scrollBarLeftEdge, y, SCROLLBAR_WIDTH, height );
        g.drawLine(scrollBarLeftEdge,y,scrollBarLeftEdge,y+height);
        g.drawRect(getX(),getY(), getWidth()-1- O3DScrollList.shadowThickness,height);
        g.setFont(itemFont);
        g.drawString(items[selectedIndex],getX() + OUILook.H_GAP,getY() + OUILook.STRING_SPACER_HEIGHT, Graphics.TOP | Graphics.LEFT );
    }
   
    
    // reset to original values
    g.setFont( oldFont );
    g.setColor( oldColor );
    if ( Debugger.ON ) Logger.dev( "ODropDownList.paint EXITTING" );
  } // paint


  /**
   * Responds to key presses when the drop down list has focus.
   * Method shall handle up and down arrow keys to support
   * vertical scrolling.  
   *
   * @param keyCode The pressed key.
   */
  protected void keyPressed( int keyCode ) 
  { 
    if ( Debugger.ON ) Logger.dev( "ODropDownList.keyPressed ENTERED" );
    OCompositeScreen screen = getContainer().getScreen();
    if ((keyCode == OAbstractScreen.UP_KEY ) ||
	(keyCode == OAbstractScreen.DOWN_KEY )) {
        this.setNeedsRepaint(true);
        focusList.setContainer(getContainer());
        focusList.keyPressed(keyCode);   
        focusList.setNeedsRepaint(true);
        selectedIndex = focusList.getSelectedIndex();
        try {
            focusList.setSelectedIndex(selectedIndex);
        } catch (Exception e){e.printStackTrace();}
    }
    
    if ( Debugger.ON ) Logger.dev( "ODropDownList.keyPressed EXITTING" );
  } // keyPressed

}// ODropDownList
