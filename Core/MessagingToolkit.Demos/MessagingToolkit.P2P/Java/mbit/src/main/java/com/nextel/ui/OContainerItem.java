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

/**
 * An item in an OContainer.  The item consists of a component and information
 * about where to place the component on the display.
 *
 * @author Glen Cordrey
 */
class OContainerItem 
{  
  /**
   * The component contained in the container.
   */
  public OComponent component;
  
  /**
   * The grid's x-coordinate of the component (the leftmost column of the grid
   * has x-coordinate 0).
   */
  public int gridX;

  /**
   * The grid's y-coordinate of the component (the top row of the grid
   * has y-coordinate 0).
   */
  public int  gridY;

  /**
   * The horizontal alignment of the component in the grid cell.  One of
   * {@link javax.microedition.lcdui.Graphics#LEFT Graphics.LEFT} or 
   * {@link javax.microedition.lcdui.Graphics#HCENTER Graphics.HCENTER} or 
   * {@link javax.microedition.lcdui.Graphics#RIGHT Graphics.RIGHT}
   */
  public int h_alignment;

  /**
   * Creates a new <code>OContainerItem</code> instance.
   *
   * @param component The component contained in the container.
   * @param gridX The grid's x-coordinate of the component (the leftmost column
   * of the grid has x-coordinate 0).
   * @param gridY The grid's y-coordinate of the component (the top row
   * of the grid has y-coordinate 0).
   * @param h_alignment The horizontal alignment of the component in the grid
   * cell.  One of
   * {@link javax.microedition.lcdui.Graphics#LEFT Graphics.LEFT} or 
   * {@link javax.microedition.lcdui.Graphics#HCENTER Graphics.HCENTER} or 
   * {@link javax.microedition.lcdui.Graphics#RIGHT Graphics.RIGHT}
   */
  public OContainerItem ( OComponent component,
			  int gridX, int gridY, int h_alignment )
  {
    this.component = component;
    this.h_alignment = h_alignment;
    this.gridX = gridX;
    this.gridY = gridY;
  } // constructor
}// OContainerItem
