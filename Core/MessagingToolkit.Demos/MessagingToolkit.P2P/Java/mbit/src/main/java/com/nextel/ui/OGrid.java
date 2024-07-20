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
import javax.microedition.lcdui.Image;

/**
 * <p>
 * Focusable component that displays a scrollable list of rows that can be
 * divided into columns.
 * <p><b>Example:</b><img src="doc-files/Grid.jpg">
 * <p> 
 * The component supports vertical scrolling and enables the
 * scrollbar when the data populated exceeds the maximum number
 * of viewport rows.  Other features include:
 * </p>
 * <p>
 * <li> 
 * The currently selected grid row is reverse-videoed while unselected rows have
 * normal video.  
 * </li>
 * <li>
 * The currently selected row may be configured to render using a different than 
 * the unselected items.
 * </li>
 * <li>
 * The OGrid class contains a default rendering implementation 
 * ({@link com.nextel.ui.OGridRowRenderer}) used to render grid 
 * rows for cell objects (Strings, int, short, Image).
 *</li>
 *<li>
 * The default {@link com.nextel.ui.OGridRowRenderer} may be configured for 
 * a single OGrid instance.  
 *</li>
 *<li>
 * The {@link com.nextel.ui.OGridRow} interface provides the {@link com.nextel.ui.OGridRowRenderer} with the capability
 * to allocate, deallocate, and retrieve cell object values and cell renderers for
 * a specific grid row.
 * <li> Grid Rows and Cell columns may have a specific rendering mechanism 
 * configured using the {@link com.nextel.ui.OGridRow}, {@link com.nextel.ui.OGridRowRenderer}, 
 * {@link com.nextel.ui.OCell} and {@link com.nextel.ui.OCellRenderer} interfaces.  
 * <li>The {@link com.nextel.ui.OGridInfo} has rendering attributes and contains 
 * methods used to calculate X/Y coordinates based on grid and cell rendering 
 * information.
 * </li>
 * </p>
 *
 * @author Anthony Paper
 */
public class OGrid extends OFocusableComponent 
{
  private OGridInfo gridInfo;
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
  
  // width of the scroll grid, including the scrollbar width
  private int width;

  // width of the list in pixels, excluding the scrollbar width
  private int listWidth;
  private int listHeight;
  private int columnHeadingsHeight;

  // the height of the list in pixels
//  private int height;

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
  // into the list of items, but is the index of the row in the scroll grid
  // display.  e.g., if 3 items can be displayed in the scroll grid window, this
  // will have a value of 0, 1, or 2
  private int lineSelected;

  // the set of items which may be displayed
//  private Object [] items;
  OGridRow [] items;
  
  private OScrollBar scrollBar;

  private boolean firstTime = true;
  
  // renderer used to render the list items
  private OGridRowRenderer itemORenderer;

  /** 
      Indicates whether the scrollbar should only draw the DOWN Arrow.
      The default value is false and both arrows shall be displayed.
   */
  private boolean downArrowOnlyFlag = false;
  
  public void setDownArrowOnlyFlag(boolean flag)
  {
      downArrowOnlyFlag = flag;
  }
  
  /**
   * Default Renderer used to render grid rows and columns
   */
  static private OGridRowRenderer defaultORenderer =
      new OGridRowRenderer()
      {
 
        /**
         * Method used to render columns based on the left X
         * and top Y coordinates.
         */
        public void renderCol(Graphics g, 
                              int leftX, int topY, 
                              int rowNum,    
                              int colNum,
                              OCell cell,
                              int cellWidth, 
                              int cellAlignment,
                              OGridInfo gridInfo)
 	{
          if ((cell == null || cell.value == null)) return;
          
          //
          // Calculate X based on cell width, cell alignment,
          // scrollbar width, and horizontal grid border width
          //
          int x = gridInfo.calculateX(
                        g, 
                        leftX,
                        cell.value,
                        colNum, 
                        gridInfo.numberOfColumns, 
                        gridInfo.getHorizontalBorderWidth(),
                        gridInfo.getScrollBarWidth(),
                        cellWidth, 
                        cellAlignment);
          //
          // Calculate the Y based on cell height, font,
          // and image height.  This routine only adjusts
          // Y value based on the image height
          //
          int y =  gridInfo.calculateY(
                              g,
                              topY,
                              cell.value,
                              colNum,
                              gridInfo.numberOfColumns);
          
          if (cell.value instanceof Image) {
              Image image = (Image) cell.value;
              g.drawImage(image,x,y,Graphics.TOP | cellAlignment);
          }              
          else {
              String value = ( cell.value instanceof String
                               ? ( String ) cell.value
                               : cell.value.toString() );
              g.drawString(value, x, y,Graphics.TOP | cellAlignment);
          }
	} // render

        /**
         * Method used to render  rows based on the left X
         * and top Y coordinates.
         */
        public void renderRow(Graphics g, 
                              int x, int y,
                              int rowNum, 
                              int rowWidth,
                              OGridRow gridRow, 
                              OGridInfo gridInfo)
        {
            OCell [] cells = gridRow.getCells();
            if (cells != null) {
                
                //
                // For the number of columns let's render it
                // to the screen based on the column width
                // alignment, calculated 
                int x1 = x;
                for (int cellIndex = 0; cellIndex < cells.length; cellIndex++)
                {
                    OCell cell = cells[cellIndex];
                    
                    // If we should NOT use default renderer
                    if (cell.renderer != null)
                    {
                        cell.renderer.renderCol(
                                    g,x1,y,rowNum,cellIndex,
                                    cell,           
                                    gridInfo.columnWidths[cellIndex],
                                    gridInfo.columnAlignments[cellIndex],
                                    gridInfo);
                    }
                    else
                    {
                        renderCol(g,x1,y,rowNum,cellIndex,
                                   cell,
                                   gridInfo.columnWidths[cellIndex],
                                   gridInfo.columnAlignments[cellIndex],
                                   gridInfo);
                    }
                    x1 = x1 + gridInfo.columnWidths[cellIndex];
                }
            }
        }
  

        /**
         * Method used to render row.  The method renders a string row or
         * grid row based on the type of object to be rendered.  This method
         * is designed to support OScrolledList and OGrid components.
         */

	public void render( Graphics g, int x, int y, int width ,
			    Object     item,
                            int       rowNumber,
                            OGridInfo gridInfo)
	{
          if (item == null) return;
          if (item instanceof OGridRow) {
             renderRow(g, x, y, rowNumber, width,
                       (OGridRow) item, gridInfo);
          }    
          else {
	     String value = ( item instanceof String
			   ? ( String ) item
			   : item.toString() );
	     g.drawString( value, x, y, Graphics.TOP | Graphics.LEFT );
          }
	} // render
      };
 
  /**
   * Creates a new <code>OGrid</code> instance.
   *
   * @param gridInfo Contains grid information used to allocate the grid object.
   */
  public OGrid (OGridInfo gridInfo)
  {
    super( );
    if ( Debugger.ON ) Logger.dev( "OGrid.ctor.3 ENTERED" );
    
    // default traverseDirections for OFocusableComponent include UP and DOWN, so
    // limit to right and left only
    setTraverseDirections( OFocusableComponent.RIGHT | OFocusableComponent.LEFT );
    setFocusedComponentBackground( OColor.TRANSPARENT );
    setGridInfo(gridInfo);
    if ( Debugger.ON ) Logger.dev( "OGrid.init EXITTING" );

  } // OGrid
  

  /**
   * Sets the column headings and column fonts.  The width and height associated
   * with the column headings are recalculated based on the parameters.  The
   * current implementation supports a single line for each column heading.
   * 
   * 
   * @param columnHeadings The array of strings to be displayed for each column
   *                       cell and Null is passed for no column headings to be
   *                       displayed.
   * @param columnHeadingsFont The font used to display column headings.
   */
  public void setColumnHeadings(String [] columnHeadings,
                                Font      columnHeadingsFont)
  {
      this.columnHeadingsHeight   = 0;
      if ((columnHeadings != null) && (columnHeadings.length > 0)) {
        gridInfo.columnHeadings = columnHeadings;
        gridInfo.columnHeadingsFont = columnHeadingsFont;
        this.columnHeadingsHeight = 
            columnHeadingsFont.getHeight() + OUILook.STRING_SPACER_HEIGHT;        
      }
  }
  /**
   * Method used to Retrieve the <code>OGridInfo</code> instance.  
   *
    * @return The grid information used to allocate and render the grid object.
   *
   */
  public OGridInfo getGridInfo()
  { return gridInfo;}

  /** 
   * Method used to set the <code>OGridInfo</code> instance.  
   *
   * @param gridInfo Contains grid information used to render the grid object.
   */
  public void setGridInfo(OGridInfo newGridInfo)
  {
    gridInfo = newGridInfo;
    this.width          = gridInfo.maxWidth;
    this.listWidth      = gridInfo.maxWidth;
    this.selectedFont   = gridInfo.selectedFont;
    this.unselectedFont = gridInfo.unselectedFont;
    setColumnHeadings(gridInfo.columnHeadings,gridInfo.columnHeadingsFont);
    this.selectedTextHeight = selectedFont.getHeight();
    this.unselectedTextHeight = unselectedFont.getHeight();
    this.listHeight = ( OUILook.STRING_SPACER_HEIGHT - 1 ) + selectedTextHeight +
      ( unselectedTextHeight * ( gridInfo.viewportRows - 1 ) );
     
    this.nbrOfDisplayableLines = gridInfo.viewportRows;
    this.bottomLineIndex = nbrOfDisplayableLines - 1;
  }
  
  /**
   * Populates the grid with the items to display.
   * You might want to use this version of populate, rather than
   * {@link #populate( OGridRow [] ) populate( OGridRow[] )}, if you need to align
   * columns within your list, or if you want to display in the list things
   * other than strings, both of which you could do by supplying a custom
   * renderer.
   *
   * @param items The items to display.
   * @param renderer A renderer to use for rendering the items to the screen.
   */
  public void populate ( OGridRow [] items, OGridRowRenderer renderer) 
  {
    if ( Debugger.ON ) Logger.dev( "OGrid.populate Object[] ENTERED" );
    this.itemORenderer = renderer;
    layOut( items );
    if ( Debugger.ON ) Logger.dev( "OGrid.populate Object[] EXITTING" );
    return; 
  } // populate

  /**
   * Populates the grid with a set of items.  Default Rendering information may
   * be specified in the <code>OGridInfo</code> instance.  All columns
   * <code>OCell</code> may have a specific <code>OCellRenderer</code>
   *
   * @param items The items to be displayed in the list.  The items will be
   * rendered by a default string renderer.
   */
  public void populate ( OGridRow [] items ) 
  {
    if ( Debugger.ON ) Logger.dev( "OGrid.populate String[] ENTERED" );
    populate( items, defaultORenderer );
    if ( Debugger.ON ) Logger.dev( "OGrid.populate String[] EXITTING" );
  } // populate

  /**
   * Lays out the grid list display.
   *
   * @param items The items to be displayed.
   */
  protected void layOut ( OGridRow [] items ) 
  {
    if ( Debugger.ON ) Logger.dev( "OGrid.layOut ENTERED" );
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
    
    this.listHeight = ( OUILook.STRING_SPACER_HEIGHT - 1 ) + selectedTextHeight +
      ( unselectedTextHeight * ( nbrOfDisplayedLines - 1 ) );
    
    //
    // Adjust information for grid border and scrollbar widths 
    //
    if (items.length > gridInfo.viewportRows) gridInfo.setScrollBarWidth(12);
    gridInfo.setGridBorderType(gridInfo.gridBorderType);
    
    if ( Debugger.ON ) Logger.dev( "OGrid.layOut EXITTING" );
    
    return; 
  } // layOut

  /**
   * Gets the height of the scrolled grid.
   * @return Height of the scrolled grid. The height will be 0 until
   * {@link #populate populate} is called.
   */
  public int getHeight() 
  {
    if ( Debugger.ON ) Logger.dev( "OGrid.getHeight CALLED, returning " +
				columnHeadingsHeight + listHeight );    
//    return this.height;
    return (columnHeadingsHeight + listHeight);
  }
  
  /**
   * Gets the width of the scrolled grid, including the scroll bar.
   * @return Width of the scrolled grid.
   */
  public int getWidth() 
  {
    if ( Debugger.ON ) Logger.dev( "OGrid.getWidth CALLED, returning " +
				this.width );
    return this.width;
  }
  
  /**
   * Paints the scrolled grid on the screen.
   *
   * @param g The graphics context
   */
  public void paint ( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "OGrid.paint ENTERED" );
   
    // width of the list portion depends upon whether a scrollbar is needed
    this.listWidth = this.width;
    if ( nbrOfDisplayableLines < items.length ) 
    {
      if ( firstTime ) 
      { // create scrollbar
	firstTime = false;
	this.scrollBar = new OScrollBar( );
        this.scrollBar.setDownArrowOnlyFlag(downArrowOnlyFlag);
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
    
    //
    // Let's draw the column headings
    //
    if (columnHeadingsHeight > 0) {
        
        for (int hi = 0; hi < gridInfo.columnHeadings.length; hi++)
        {
            g.setFont(gridInfo.columnHeadingsFont);
            int myDiff = Math.abs(
                       gridInfo.columnWidths[hi] -
		       gridInfo.columnHeadingsFont.stringWidth(
                       gridInfo.columnHeadings[hi]));
            int myX = x+1;
            if (myDiff > 2) myX = x + (myDiff/2);
            g.drawString(gridInfo.columnHeadings[hi], myX, y,
			 Graphics.TOP | Graphics.LEFT );
            x = x + gridInfo.columnWidths[hi];
        }
        //
        // Reset X/Y starting values
        //
        x = getX();
        y = getY() + columnHeadingsHeight - OUILook.STRING_SPACER_HEIGHT;
    }
    
    // now draw the box containing the list
    int boxLeftEdge = x + labelWidth;
    int boxTopEdge = y;
    paintBox( g, boxLeftEdge, y, listWidth,
              listHeight);
          //   this.height );

    // prepare to draw the items in the box
    //
    // adjust y to top of where string is to be
    y +=  OUILook.STRING_SPACER_HEIGHT;
    
    int hGridCount = 0;
    
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
      //
      // Get the row grid and cells to render
      // based on the current list index
      //
      itemORenderer.render(g,x + OUILook.H_GAP,y,
			   getListWidth() - OUILook.H_GAP,
			   items[listIndex],listIndex,gridInfo);
      
      y += itemFont.getHeight();
      
      //
      // If we need to draw the horizontal lines
      // and we have not already drawn all of the
      // horizontal grid lines.  The horizontal
      // line will be drawn AFTER Y is incremented
      // therefore, it is below the item.  
      //
      if  (((gridInfo.gridBorderType == gridInfo.GRID_BORDER_TYPE_HORIZONTAL) ||
            (gridInfo.gridBorderType == gridInfo.GRID_BORDER_TYPE_BOTH)) &&
           (hGridCount < (gridInfo.viewportRows - 1)))
      {
        g.setColor(OColor.BLACK);
        g.drawLine(x,y-1, x+ getListWidth(),y-1);
        hGridCount++;
      }
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
		       listHeight, 
                       //height, 
                       sliderStart, percentageDisplayed );
    }
    
    //
    // If we need to draw the vertical lines
    //
    if (((gridInfo.gridBorderType == gridInfo.GRID_BORDER_TYPE_VERTICAL) ||
         (gridInfo.gridBorderType == gridInfo.GRID_BORDER_TYPE_BOTH)) &&
         (gridInfo.columnWidths != null) && 
         (gridInfo.columnWidths.length > 1))
    {
        int x1 = boxLeftEdge;
        int x2 = 0;
        int y1 = boxTopEdge;
        int y2 = boxTopEdge + listHeight;
        for (int index=1; index < gridInfo.columnWidths.length; index++)
        {
           x1 = x1 + gridInfo.columnWidths[index-1];
           x2 = x1;
           g.setColor(OColor.BLACK);
           g.drawLine(x1,y1,x2,y2);
        }
    }
    
    g.setColor( oldColor );
    g.setFont( oldFont );
    if ( Debugger.ON ) Logger.dev( "OGrid.paint EXITTING" );
    
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
      Logger.dev( "OGrid.getSelectedValue CALLED, returning " + value );
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
    if ( Debugger.ON ) Logger.dev( "OGrid.setSelectedValue ENTERED w/value=" +
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
      throw new NotFoundException( "OGrid.setSelectedValue value " +
				   value + " not found" );
    }
    repaint();
    if ( Debugger.ON ) Logger.dev( "OGrid.setSelectedValue EXITTING" );
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
    if ( Debugger.ON ) Logger.dev( "OGrid.setSelectedValue ENTERED " +
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
    if ( Debugger.ON ) Logger.dev( "OGrid.setSelectedValue EXITTING" );
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
    if ( Debugger.ON ) Logger.dev( "OGrid.scrollTo ENTERED w/itemIndex=" +
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
    if ( Debugger.ON ) Logger.dev( "OGrid.scrollTo EXITTING, returning " +
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
      Logger.dev( "OGrid.getSelectedIndex CALLED, returning " +
		  topLineIndex + lineSelected );

    return topLineIndex + lineSelected;
  } // getSelectedIndex


  /**
   * Processes a key press while the scroll grid has focus.
   * <p>
   * If the key pressed is the up- or down- key the list will be scrolled
   * appropriately and the selected item will change.  If the currently selected
   * item is the first or last item then a beep will ensue.  Any key other than
   * the up- or down- key is ignored.
   * @param keyCode The code of the pressed key.
   */
  public void keyPressed ( int keyCode ) 
  {
    if ( Debugger.ON ) Logger.dev( "OGrid.keyPressed w/keyCode= " + keyCode );

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
     * Gets the width of the list portion of the scroll grid, which excludes the
     * width of the scroll bar if one is displayed.
     *
     * @return The width of the list portion.
     */
  public int getListWidth () 
  { 
    return listWidth; 
  } // getListWidth
  

}// OGrid
