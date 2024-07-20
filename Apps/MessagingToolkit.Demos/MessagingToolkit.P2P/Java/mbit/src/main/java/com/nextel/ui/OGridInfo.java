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
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Image;

/**
  * Class used to contain the grid information.
  * The OGridInfo class is passed as a parameter
  * to the OGrid constructor class.  It serves
  * as a container for grid parameters required
  * by the OGrid, OGridRowRenderer, and OCellRender
  * interfaces.  The Grid information includes:
  * <p>
  * <li>Grid Border Type</li>
  * <li>Number Of Grid Viewport Rows</li>
  * <li>Number Of Grid Columns</li>
  * <li>Column Cell Headings</li>
  * <li>Column Alignments</li>
  * <li>Column Widths (pixels)</li>
  * <li>Fonts for Column Headings and Column Cells</li>
  * <li>Helper Methods used to adjust X/Y coordinate rendering values</li>
  * </p>
  * @author Anthony Paper
  */

public class OGridInfo
{
  /**
   * The {@link com.nextel.ui.OGridInfo#gridBorderType} constant value used 
   * to indicate that <b>no horizontal</b> or <b>vertical</b> grid border lines 
   * should be displayed.
   */
  static final public int GRID_BORDER_TYPE_NONE       = 1;

  /**
   * The {@link com.nextel.ui.OGridInfo#gridBorderType} constant value used 
   * to indicate that <b>only horizontal</b> grid border lines should be displayed.
   */
  static final public int GRID_BORDER_TYPE_HORIZONTAL = 2;

  /**
   * The {@link com.nextel.ui.OGridInfo#gridBorderType} constant value used 
   * to indicate that <b>only vertical</b> grid border lines should be displayed.
   */
  static final public int GRID_BORDER_TYPE_VERTICAL   = 3;

  /**
   * The {@link com.nextel.ui.OGridInfo#gridBorderType} constant value used 
   * to indicate that <b>both horizontal</b> and <b>vertical</b> grid border 
   * lines should be displayed.
   */
  static final public int GRID_BORDER_TYPE_BOTH       = 4; 

  /**
   * Contains the Grid Border Type variable used to render horizontal
   *  and vertical grid lines.  It <b>should contain</b> one of the following
   *  values:
   * <p>
   * <li>
   * {@link com.nextel.ui.OGridInfo#GRID_BORDER_TYPE_NONE}       - Neither horizontal or vertical
   * </li>
   * <li>
   * {@link com.nextel.ui.OGridInfo#GRID_BORDER_TYPE_HORIZONTAL} - Only horizontal
   * </li>
   * <li>
   * {@link com.nextel.ui.OGridInfo#GRID_BORDER_TYPE_VERTICAL}   - Only vertical
   * </li>
   * <li>
   * {@link com.nextel.ui.OGridInfo#GRID_BORDER_TYPE_BOTH}       - Both horizontal and vertical
   * </li>
   */
  public int       gridBorderType;    

  /**
   * Contains the number of viewport rows to be displayed.
   */
  public int       viewportRows;

  /**
   * Contains the number of columns to be displayed.
   */
  public int       numberOfColumns;

  /**
   * Contains the column headings to be displayed.
   */
  public String [] columnHeadings;

  /**
   * Contains an array of column pixel alignments to
   * be used when rendering cell columns.  It should
   * contain one of the following values:
   * <p>
   * <li>
   *     {@link javax.microedition.lcdui.Graphics#RIGHT}
   * </li>
   * <li>
   *     {@link javax.microedition.lcdui.Graphics#LEFT}
   * </li>
   * <li>
   *     {@link javax.microedition.lcdui.Graphics#HCENTER}
   * </li>
   */
  public int    [] columnAlignments;

  /**
   * Contains an array of column pixel width values
   * to be used when rendering cell columns.  
   */
  public int    [] columnWidths;

  /**
   * Contains the maximum width size of the grid in pixels
   */
  public int       maxWidth;

  /**
   * Contains the maximum height size of the grid in pixels
   */
//  public int       maxHeight;

  /**
   * Contains the selected font value
   */
  public Font      columnHeadingsFont;
  
  /**
   * Contains the selected font value
   */
  public Font      selectedFont;
  /**
   * Contains the unselected font value
   */
  public Font      unselectedFont;
  
  /**
   * Contains the scrollbar width.  This {@link com.nextel.ui.OGrid}
   * attribute is automatically populated by the {@link com.nextel.ui.OGrid}
   * component and should be used as a read-only value when performing 
   * customized cell rendering for {@link javax.microedition.lcdui.Graphics#RIGHT} 
   * alignment. If scrollbar is not displayed the value shall be zero.
   */
  public int scrollBarWidth = 0;
  
  /**
   * Contains the horizontal border width.  This OGrid attribute
   * is automatically populated by the OGrid component and
   * should be used as a read-only value when performing 
   * customized cell rendering for Graphics.RIGHT alignment.
   * If the {@link com.nextel.ui.OGridInfo#gridBorderType} 
   * is set to {@link com.nextel.ui.OGridInfo#GRID_BORDER_TYPE_NONE}
   * or {@link com.nextel.ui.OGridInfo#GRID_BORDER_TYPE_HORIZONTAL}
   * the value shall be zero.
   */
  private int horizontalBorderWidth = 0;
    
  /**
   * Constructor used to create grid information.
   *
   * @param gridBorderType Contains the grid border type.
   * @param viewportRows Contains the number of grid viewport rows to be displayed.
   * @param numberOfColumns Contains the number of column headings to be displayed.
   * @param columnHeadings Contains an array of cell column headings to be displayed.
   * @param columnAlignments Contains an array of cell column alignment values used for rendering.
   * @param columnWidths Contains an array of cell column width (pixels) values used for rendering.
   * @param maxWidth Contains the maximum width (pixels) of the grid component.
   * @param columnHeadingsFont Contains the Font used to render cell column headings.
   * @param selectedFont Contains the selected Font used to render cell values.
   * @param unselectedFont Contains the unselected Font used to render cell values.
   */
  public OGridInfo(
                    int       gridBorderType,
                    int       viewportRows,
                    int       numberOfColumns,
                    String [] columnHeadings,
                    int    [] columnAlignments,
                    int    [] columnWidths,   //TBD
                    int       maxWidth,       //TBD
//                    int       maxHeight,      //TBD
                    Font      columnHeadingsFont,
                    Font      selectedFont,
                    Font      unselectedFont)
  {
     this.gridBorderType     = gridBorderType;
     this.viewportRows       = viewportRows;
     this.numberOfColumns    = numberOfColumns;    
     this.columnHeadings     = columnHeadings;
     this.columnAlignments   = columnAlignments;
     this.columnWidths       = columnWidths;
     this.maxWidth           = maxWidth;
//     this.maxHeight        = maxHeight;
     this.columnHeadingsFont = columnHeadingsFont;
     this.selectedFont       = selectedFont;
     this.unselectedFont     = unselectedFont;
     setGridBorderType(gridBorderType);
     setScrollBarWidth(0);
  }
  
   /**
   * Package protected method used by OGridInfo constructor and OGrid to specify 
   * the grid border type and adjust the horizontal grid border width.
   *
   * @param newGridBorderType Contains the grid border type
   */
  void setGridBorderType(int newGridBorderType)
  {
    this.gridBorderType = newGridBorderType;
    this.horizontalBorderWidth = 0;
    if ((newGridBorderType == GRID_BORDER_TYPE_BOTH) ||
        (newGridBorderType == GRID_BORDER_TYPE_VERTICAL)) {
        this.horizontalBorderWidth = OUILook.H_GAP;
    }  
  }

  /**
   * Public method used by cell renderers to get the scrollbar width.
   * It should be used to adjust the X coordinate when rendering the 
   * last column for the Graphics.RIGHT cell alignment.
   *
   * @returns The scrollbar width in pixels.
   */
  public int getScrollBarWidth(){return scrollBarWidth;}
  
  /**
   * Package protected method used by OGrid to specify the scrollbar width.
   *
   * @param newScrollBarWidth Contains the scrollbar width in pixels
   */
   void setScrollBarWidth(int newScrollBarWidth){scrollBarWidth = newScrollBarWidth;}
   
   
  /**
   * Public method used by cell renderers to get the horizontal grid border width.
   * It should be used by rendering mechanisms to handle horizontal border width.
   *
   * @returns The Horizontal Border Width in pixels.
   */  
  public int getHorizontalBorderWidth(){return horizontalBorderWidth;}
  
  /**
   * Public method used by cell renders to calculate the Y value
   * based on the grid and alignment information.  No vertical
   * adjustment is performed for string objects.  Vertical
   * adjustments are performed for Image objects.
   *
   *  @param g Contains the graphics context used to retrieve fonts and colors.
   *  @param topY Contains the top-most Y coordinate value.
   *  @param o Contains the column cell to be rendered.
   *  @param colNum Contains the column number to be rendered.
   *  @param maxCols Contains the maximum number of columns to be displayed.
   *
   *  @returns The adjusted Y coordinate value that was calculated.
   */
  static public int calculateY(Graphics g,
                               int      topY,
                               Object   o,
                               int      colNum,
                               int      maxCols)
  {
      int y = topY;
      if (o instanceof Image) {
          Image i = (Image)o;
          y = y + (Math.abs(g.getFont().getHeight() - i.getHeight())/2); 
      }
      return y;
  }

  /**
   * Public method used by cell renderers to calculate X coordinate
   * based on the cell object, cell width, cell alignment, scrollbar,
   * and grid border.   This method can be used to adjust the X coordinate 
   * when rendering the cell column for the Graphics.RIGHT or Graphics.HCENTER 
   * alignment.  No adjustment is performed for Graphics.LEFT alignment.
   * Adjustments are calculated differently for String and Image objects.
   * The last cell column shall be adjusted correctly to support scrollbar
   * display properties.
   *
   *  @param g Contains the graphics context used to retrieve fonts and colors.
   *  @param leftX Contains the left-most X coordinate value.
   *  @param o Contains the column cell to be rendered.
   *  @param colNum Contains the column number to be rendered.
   *  @param maxCols Contains the maximum number of columns.
   *  @param horizontalBorderWidth Contains the maximum border width.
   *  @param scrollBarWidth Contains the scrollbar width that is displayed.
   *  @param cellWidth Contains the cell width in pixels.
   *  @param cellAlignment Contains the cell alignment used for rendering.
   *
   *  @returns The adjusted X coordinate value that was calculated.
   */
  static public int calculateX(Graphics g,
                           int leftX, 
                           Object o,
                           int colNum, 
                           int maxCols, 
                           int horizontalBorderWidth,
                           int scrollBarWidth,
                           int cellWidth, 
                           int cellAlignment) {
      
    if (cellAlignment == g.LEFT) return leftX;
    int x = leftX;
    int w = 0;    
    //
    // Calculate the width for image, string, or object
    //
    if (o instanceof Image) {
        Image i = (Image) o;
        w = i.getWidth();
    }
    else {
        String value = ( o instanceof String
                         ? ( String ) o
                         : o.toString() );
        w = g.getFont().stringWidth(value);                 
    }
    
    //
    // Adjust X coordinate based on alignment, grid border width, and scrollbar width
    //
    if (cellAlignment == Graphics.RIGHT) {
        x = x + cellWidth-horizontalBorderWidth;
        if (colNum == (maxCols -1)) {
            x = x - scrollBarWidth;
        }
    }
    if (cellAlignment == Graphics.HCENTER) x = (x + (Math.abs(cellWidth-w)/2));
    return x;
  }
  
}// OGridInfo