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

import java.util.Vector;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;


/**
 * <p>
 * The OGridObjectRow class implements the OGridRow interface used to allocate
 * and manage {@link com.nextel.ui.OCell} objects for shorts, ints, strings, 
 * images, and any other object values.  It is a data bank used to generically 
 * store object and primitives associated with a single {@link com.nextel.ui.OGridRow}.  
 * The Grid component shall invoke the following life-cycle methods:
 * </p>
 * <p>
 * <li>
 *    OCell [] getCells[] - Method To retrieve a list of cell objects.
 * </li>
 * <li>
 *    allocateCells       - Method to allocate the column cell objects.
 * </li>
 * <li>
 *    deallocateCells     - Method to deallocate the column cell objects.
 * </li>
 * <li>
 *    hasAllocatedCells   - Method to indicate whether the column cell objects have been allocated.
 * </li>
 * </p>
 * @author Anthony Paper
 */

public class OGridObjectRow implements OGridRow 
{
  int           [] colTypes;
  short         [] shortCols;
  int           [] intCols;
  Object        [] objectCols;
  OCellRenderer [] renderCols;
  OCell         [] cells;


  /**
   * Indicates the Column Type is UNDEFINED.  This value is used to
   * designate the default initialization value from the KVM and 
   * should not be passed as a parameter when invoking addCell methods.
   */
  private static final int TYPE_UNDEFINED = 0;
  
  /**
   * Indicates the Column Type is BYTE.  <b>Not currently supported.</b>
   */
  private static final int TYPE_BYTE      = -1; // T.B.D
  
  /**
   * Indicates the Column Type is a primitive short.  The equivalent
   * <b>java.lang.Short</b> wrapper object is created using
   * {@link com.nextel.ui.OGridObjectRow#allocateCells} and
   * returned using {@link com.nextel.ui.OGridObjectRow#getCells}.
   */
  public static final int TYPE_SHORT     = 1;
  
  /**
   * Indicates the Column Type is a primitive int.  The equivalent
   * <b>java.lang.Integer</b> wrapper object is created using
   * {@link com.nextel.ui.OGridObjectRow#allocateCells} and
   * returned using {@link com.nextel.ui.OGridObjectRow#getCells}.
   */
  public static final int TYPE_INT       = 2;
  
  /**
   * Indicates the Column Type is a Java Object.  The object should support
   * a toString() method used to render text or be an instance of the
   * {@link javax.microedition.lcdui.Image} object.  Otherwise a customized
   * {@link com.nextel.ui.OCellRenderer} shall need to be configured 
   * for the cell column object using the {@link com.nextel.ui.OGridObjectRow#addCellRenderer}
   * method.  An alternative approach is to customize rendering at the 
   * {@link com.nextel.ui.OGrid} instance using the {@link com.nextel.ui.OGridRowRenderer}
   * interface.
   * 
   */
  public static final int TYPE_OBJECT    = 3; // T.B.D

     
  /**
   * Constructor used to create the grid object row based on the total
   * number of columns.
   *
   * @param totalColumns Indicates the number of cell columns to be managed.
   */
  public OGridObjectRow(int totalColumns)
  {
     colTypes = new int [totalColumns];
  }

  /** 
   * Method used to add a cell primitive short value for the specified column.
   *
   * @param index Specifies the column index.
   * @param value Contains the primitive short value.
   */
  public void addCell(int index, short value) {
     colTypes[index] = TYPE_SHORT;
     if (shortCols == null) shortCols = new short[colTypes.length];
     shortCols[index] = value;
  }
  
  /** 
   * Method used to add a cell primitive int value for the specified column.
   *
   * @param index Specifies the column index.
   * @param value Contains the primitive int value.
   */
  public void addCell(int index, int value) {
     colTypes[index] = TYPE_INT;
     if (intCols == null) intCols = new int[colTypes.length];
     intCols[index] = value;
  }

  /** 
   * Method used to add a cell object value for the specified column.
   * The object should support a <b>toString()</b> method used to render text 
   * <b>or</b> be an instance of the {@link javax.microedition.lcdui.Image} object.  
   * Otherwise a customized {@link com.nextel.ui.OCellRenderer} shall need to be 
   * configured for the cell column object using the {@link com.nextel.ui.OGridObjectRow#addCellRenderer}
   * method.  See also {@link com.nextel.ui.OGrid} for customized grid rendering using the {@link com.nextel.ui.OGridRowRenderer}
   * interface.
   * @param index Specifies the column index.
   * @param value Contains the cell object value.
   */
  public void addCell(int index, Object value) {
     colTypes[index] = TYPE_OBJECT;
     if (objectCols == null) objectCols = new Object[colTypes.length];
     objectCols[index] = value;
  }

  /** 
   * Method used to add a specific renderer for a cell column
   * for the specfied column.  This method is used to overide
   * the default OCellRenderer for a specific cell column value.
   * It should be used only when custom rendering is required.
   *
   * @param index Specifies the column index.
   * @param value Contains the cell renderer value.
   */
  public void addCellRenderer(int index, OCellRenderer value) {
     colTypes[index] = TYPE_OBJECT;
     if (renderCols == null) renderCols = new OCellRenderer [colTypes.length];
     renderCols[index] = value;
  }
  

  /** 
   * Method used to allocate cell object values.  
   */  
  public void allocateCells()
  {
     cells = new OCell[colTypes.length];
     for (int x = 0; x < colTypes.length; x++)
     {
        Object o = null;
        if (colTypes[x] == TYPE_SHORT)  o = new Short(shortCols[x]);
        if (colTypes[x] == TYPE_INT  )  o = new Integer(intCols[x]);
        if (colTypes[x] == TYPE_OBJECT) o = objectCols[x];
        OCell cell = null;
        if ((renderCols == null) || (renderCols[x] == null)) {
            cells[x] = new OCell(o);
        }
        else {
            cells[x] = new OCell(o,renderCols[x]);
        }
     }
  }


  /** 
   * Method used to deallocate cell object values.
   */
  public void deallocateCells()
  {
     cells = null;
  }

  /**
   * Method used to determine if the cell object value has been allocated.
   *
   * @returns The boolean flag indicating whether the cell object values have
   *           been allocated.
   */
  public boolean hasAllocatedCells()
  { return (cells != null);}

  /**
   * Creates an array of cell columns used to manage, display, and scroll grid rows.
   *
   * @returns An array of cell columns
   */
   public  OCell [] getCells() {
     if (cells == null) allocateCells();
     return (cells);
  }

} // OGridObjectRow