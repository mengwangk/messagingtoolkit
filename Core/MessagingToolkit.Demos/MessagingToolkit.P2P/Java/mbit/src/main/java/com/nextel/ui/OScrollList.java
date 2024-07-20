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
import com.nextel.exception.NotFoundException;
import com.nextel.util.Debugger;
import com.nextel.util.Logger;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;

/**
 * A list of items which may not all be displayed at once and whose display is
 * scrollable if not all items are displayed.
 * <p><b>Example:</b><img src="doc-files/ScrollList.jpg">
 * <p>
 * If not all items are displayed then a vertical scroll bar is displayed to the
 * right of the list; if all items are displayed (i.e., all items fit within
 * the height available for the display) then no scroll bar is displayed.
 * <p>
 * The currently selected item is reverse-videoed while unselected items have
 * normal video.  The currently selected item may optionally be in a different
 * font than the unselected items.
 *
 * @author Glen Cordrey
 */
public class OScrollList extends OFocusableComponent
{
  private String label;
  private Font labelFont;
  private int labelWidth;

  // The height of text for the selected item.
  private int selectedTextHeight;

  // The height of text for items which are not selected.
  private int unselectedTextHeight;

  // The font to use for the selected item
  private Font selectedFont;

  // The font to use for unselected items
  private Font unselectedFont;

  // width of the scroll list, including the scrollbar width
  private int width;

  // width of the list in pixels, excluding the scrollbar width
  private int listWidth;

  // the height of the list in pixels
  private int height;

  // the number of lines which can be displayed in the available space  
  private int nbrOfDisplayableLines;

  // the number of lines which are displayed in the available space; equal to or
  // less than nbrOfDisplayableLines
  private int nbrOfDisplayedLines;

  // the index in the list of the first line displayed 
  private int topLineIndex;

  // the index in the list of the last line displayed
  private int bottomLineIndex;

  // 0-based index of the selected displayed row.  this index is NOT an index
  // into the list of items, but is the index of the row in the scroll list
  // display.  e.g., if 3 items can be displayed in the scroll list window, this
  // will have a value of 0, 1, or 2
  private int lineSelected;

  // the set of items which may be displayed
  private Object [] items;

  private OScrollBar scrollBar;

  private boolean firstTime = true;

  // renderer used to render the list items
  private ORenderer itemORenderer;

  // default renderer, which simply displays the item as a String.  This can
  static private ORenderer defaultORenderer =
    new ORenderer()
      {
    public void render( Graphics g, int x, int y, int width ,
                        Object item )
    {
      String value = ( item instanceof String
               ? ( String ) item
               : item.toString() );
      g.drawString( value, x, y, Graphics.TOP | Graphics.LEFT );
    } // render
      } /* defaultORenderer */ ;

  /**
   * Creates a new <code>OScrollList</code> instance.
   *
   * @param width The width of the scroll list.  If any items in the list are
   * wider than this their display will be truncated.
   * @param maxHeight The height available for the scroll list.  The
   * actual height of the list may be less than this, and is derived from the
   * number of lines which can be displayed in the maxHeight.
   * @param selectedFont Font to use for the selected item
   * @param unselectedFont Font to use for the unselected items
   */
  public OScrollList ( int width,
                       int maxHeight,
                       Font selectedFont,
                       Font unselectedFont )
  {
    super( );
    if ( Debugger.ON ) Logger.dev( "OScrollList.ctor.3 ENTERED" );

    // default traverseDirections for OFocusableComponent include UP and DOWN, so
    // limit to right and left only
    setTraverseDirections( OFocusableComponent.RIGHT | OFocusableComponent.LEFT );
    setFocusedComponentBackground( OColor.TRANSPARENT );

    this.width = width;
    this.listWidth = width;

    this.selectedTextHeight = selectedFont.getHeight();
    this.unselectedTextHeight = unselectedFont.getHeight();
    int heightAvailableForNotSelectedText = maxHeight -
      OUILook.STRING_SPACER_HEIGHT - selectedTextHeight;

    this.nbrOfDisplayableLines = 1 /* selected line */ +
      ( heightAvailableForNotSelectedText / unselectedTextHeight );

    this.selectedFont = selectedFont;
    this.unselectedFont = unselectedFont;
    this.bottomLineIndex = nbrOfDisplayableLines - 1;

    if ( Debugger.ON ) Logger.dev( "OScrollList.init EXITTING" );

  } // OScrollList

  /**
   * Populates the list with the items to display.
   * You might want to use this version of populate, rather than
   * {@link #populate( String [] ) populate( String[] )}, if you need to align
   * columns within your list, or if you want to display in the list things
   * other than strings, both of which you could do by supplying a custom
   * renderer.
   *
   * @param items The items to display.
   * @param renderer A renderer to use for rendering the items to the screen.
   */
  public void populate ( Object [] items, ORenderer renderer)
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.populate Object[] ENTERED" );
    this.itemORenderer = renderer;
    layOut( items );
    if ( Debugger.ON ) Logger.dev( "OScrollList.populate Object[] EXITTING" );
    return;
  } // populate

  /**
   * Populates the list with a set of items. 
   *
   * @param items The items to be displayed in the list.  The items will be
   * rendered by a default string renderer.
   */
  public void populate ( String [] items )
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.populate String[] ENTERED" );
    populate( items, defaultORenderer );
    if ( Debugger.ON ) Logger.dev( "OScrollList.populate String[] EXITTING" );
  } // populate

  /**
   * Lays out the list display.
   *
   * @param items The items to be displayed.
   */
  protected void layOut ( Object [] items )
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.layOut ENTERED" );
    this.items = items;
    nbrOfDisplayedLines = Math.min( items.length, nbrOfDisplayableLines );
    if ( bottomLineIndex >= items.length )
    { // bottomLineIndex is leftover from previous deletion of the last row, so
      // correct it
      bottomLineIndex = items.length - 1;
      topLineIndex = bottomLineIndex - nbrOfDisplayedLines + 1;
    }
    else
    {
      bottomLineIndex = topLineIndex + nbrOfDisplayedLines - 1;
    }

    if ( lineSelected > nbrOfDisplayedLines - 1 )
    { // the number of items displayed has decreased
      lineSelected = nbrOfDisplayedLines - 1;
    }

    this.height = (OUILook.STRING_SPACER_HEIGHT - 1) + selectedTextHeight +
      ( unselectedTextHeight * ( nbrOfDisplayedLines - 1 ) );

    if ( Debugger.ON ) Logger.dev( "OScrollList.layOut EXITTING" );

    return;
  } // layOut

  /**
   * Gets the height of the scroll list.
   * @return Height of the scroll list. The height will be 0 until
   * {@link #populate populate} is called.
   */
  public int getHeight()
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.getHeight CALLED, returning " +
                this.height );
    return this.height;
  }

  /**
   * Gets the width of the scroll list, including the scroll bar.
   * @return Width of the scroll list.
   */
  public int getWidth()
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.getWidth CALLED, returning " +
                this.width );
    return this.width;
  }

  /**
   * Paints the scroll list on the screen.
   *
   * @param g The graphics context
   */
  public void paint ( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.paint ENTERED" );

    // width of the list portion depends upon whether a scrollbar is needed
    this.listWidth = this.width;
    if ( nbrOfDisplayableLines < items.length )
    {
      if ( firstTime )
      { // create scrollbar
    firstTime = false;
    this.scrollBar = new OScrollBar( );
      }
      this.listWidth -= scrollBar.getWidth();
    }

    int oldColor = g.getColor(); // save so we can restore it
    Font oldFont = g.getFont();

    int x = getX();
    int y = getY();

    if ( label != null )
    { // draw the label on the left, if their is one
      g.setFont( labelFont );
      g.drawString( label, x, y, Graphics.TOP | Graphics.LEFT );
    }

    // now draw the box containing the list
    int boxLeftEdge = x + labelWidth;
    int boxTopEdge = y;
    paintBox( g, boxLeftEdge, y, listWidth, this.height );

    // prepare to draw the items in the box
    //
    // adjust y to top of where string is to be
    y +=  OUILook.STRING_SPACER_HEIGHT;

    // display list items in the box
    g.setColor( OUILook.TEXT_COLOR );
    for ( int listIndex = topLineIndex;
          listIndex < topLineIndex + nbrOfDisplayedLines &&
            listIndex < items.length; listIndex++ )
    {
      Font itemFont = unselectedFont;
      int foreground = OUILook.TEXT_COLOR;
      if ( listIndex == ( topLineIndex + lineSelected ) )
      { // this is the selected line; reverse-video the background
    g.setColor( hasFocus() ? OColor.BLACK  : OColor.DK_GRAY );
    g.fillRect( getX(), y - 1, listWidth, selectedTextHeight );

    foreground = OColor.TRANSPARENT;
    itemFont = selectedFont;
      }

      g.setFont( itemFont );
      g.setColor( foreground );

      itemORenderer.render( g, x + OUILook.H_GAP, y,
                getListWidth() - OUILook.H_GAP, items[ listIndex ] );
      y += itemFont.getHeight();
    }

    // paint the scroll bar;
    if ( scrollBar != null)
    {
      int percentageDisplayed = ( nbrOfDisplayedLines * 100 ) / items.length;
      int sliderStart = ( topLineIndex * 100 ) / items.length;
      if ( bottomLineIndex == items.length - 1 )
      { // the end of the list is displayed, make sure scroll bar slider covers
    // full remaining area
    sliderStart = 100 - percentageDisplayed;
      }

      scrollBar.paint( g, boxLeftEdge + listWidth + 1, boxTopEdge,
               height, sliderStart, percentageDisplayed );
    }

    g.setColor( oldColor );
    g.setFont( oldFont );
    if ( Debugger.ON ) Logger.dev( "OScrollList.paint EXITTING" );

  } // paint

  /**
   * Gets the list item that was selected
   *
   * @return The selected list item.
   */
  public Object getSelectedValue ()
  {
    Object value = items[ topLineIndex + lineSelected ];
    if ( Debugger.ON )
      Logger.dev( "OScrollList.getSelectedValue CALLED, returning " + value );
    return value;
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
    if ( Debugger.ON ) Logger.dev( "OScrollList.setSelectedValue ENTERED w/value=" +
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
      throw new NotFoundException( "OScrollList.setSelectedValue value " +
                   value + " not found" );
    }
    repaint();
    if ( Debugger.ON ) Logger.dev( "OScrollList.setSelectedValue EXITTING" );
  } // setSelectedValue

  /**
   * Sets the item that is selected. If necessary the list will be scrolled so
   * that the item is displayed.
   *
   * @param itemIndex The index, in the list of items, of the selected item.
   * @exception NotFoundException if no item with the specified index exists
   */
  public void setSelectedIndex ( int itemIndex )
    throws NotFoundException
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.setSelectedValue ENTERED " +
                " w/itemIndex= " + itemIndex );
    if ( itemIndex < topLineIndex || itemIndex > bottomLineIndex )
    {
      lineSelected = scrollTo( itemIndex );
    }
    else
    {
      lineSelected = itemIndex - topLineIndex;
    }
    repaint();
    if ( Debugger.ON ) Logger.dev( "OScrollList.setSelectedValue EXITTING" );
  } // setSelectedIndex


  /**
   * Scroll the list to display an item.
   *
   * @param itemIndex Index of the item to scroll to
   * @return The index, within the displayed rows, of the row scrolled to.
   * e.g., if the row scrolled to is the second of 3 displayed rows, the 0-based
   * index of 1 will be returned
   * @exception NotFoundException if no item with the specified index exists
   */
  public int scrollTo ( int itemIndex )
    throws NotFoundException
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.scrollTo ENTERED w/itemIndex=" +
                itemIndex );
    if ( itemIndex > items.length - 1 )
    {
      throw new NotFoundException( "requested index " + itemIndex +
                   " but there are only " + items.length + " items" );
    }
    if ( itemIndex < topLineIndex || itemIndex > bottomLineIndex )
    {
      int lastIndex = items.length - 1;
      if ( ( lastIndex - itemIndex + 1 ) >= nbrOfDisplayedLines )
      { // place the item at the top of the scroll display
    topLineIndex = itemIndex;
      }
      else  // not enough items including and below the desired item to place it
      { // at the top, so place item at bottom of scroll display
    topLineIndex = itemIndex - nbrOfDisplayedLines + 1;
      }
      bottomLineIndex = topLineIndex + nbrOfDisplayedLines - 1;
      repaint();
    }
    if ( Debugger.ON ) Logger.dev( "OScrollList.scrollTo EXITTING, returning " +
                ( itemIndex - topLineIndex ) );
    return itemIndex - topLineIndex;
  } // scrollTo

  /**
   * Gets the index of the item that is currently selected.
   *
   * @return the index of the currently selected item
   */
  public int getSelectedIndex ()
  {
    if ( Debugger.ON )
      Logger.dev( "OScrollList.getSelectedIndex CALLED, returning " +
          topLineIndex + lineSelected );

    return topLineIndex + lineSelected;
  } // getSelectedIndex


  /**
   * Processes a key press while the scroll list has focus.
   * <p>
   * If the key pressed is the up- or down- key the list will be scrolled
   * appropriately and the selected item will change.  If the currently selected
   * item is the first or last item then a beep will ensue.  Any key other than
   * the up- or down- key is ignored.
   * @param keyCode The code of the pressed key.
   */
  public void keyPressed ( int keyCode )
  {
    if ( Debugger.ON ) Logger.dev( "OScrollList.keyPressed w/keyCode= " + keyCode );

    int currentSelectionIndex = topLineIndex + lineSelected;
    OCompositeScreen screen = getContainer().getScreen();
    if ( keyCode == OAbstractScreen.DOWN_KEY )
    {
      if ( currentSelectionIndex < items.length - 1 )
      { // there is a row below the current selection
    if ( lineSelected < ( nbrOfDisplayedLines - 1 ) )
    {
      lineSelected++;
    }
    if ( ++currentSelectionIndex > bottomLineIndex )
    { // scroll down
      bottomLineIndex++;
      topLineIndex++;
    }
    repaint();
      }
      else
      { // at bottom of the list
    OHandset.beep();
      }
    }
    else  if ( keyCode == OAbstractScreen.UP_KEY )
    {
      if ( currentSelectionIndex > 0 )
      { // there is a row above the current selection
    if ( lineSelected > 0 )
    {
      lineSelected--;
    }

    if ( --currentSelectionIndex < topLineIndex )
    { // scroll up
      topLineIndex--;
      bottomLineIndex--;
    }
    repaint();
      }
      else
      { // at the top of the list, so play a sound
    OHandset.beep();
      }
    }
  } // keyPressed

    /**
     * Gets the width of the list portion of the scroll list, which excludes the
     * width of the scroll bar if one is displayed.
     *
     * @return The width of the list portion.
     */
  public int getListWidth ()
  {
    return listWidth;
  } // getListWidth


}// OScrollList
