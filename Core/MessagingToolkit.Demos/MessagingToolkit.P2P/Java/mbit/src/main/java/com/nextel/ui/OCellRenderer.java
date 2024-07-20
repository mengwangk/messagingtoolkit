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

/**
 * Interface used to render a cell columns to the screen.
 * The {@link com.nextel.ui.OGrid} component uses this interface to render
 * {@link com.nextel.ui.OCell} columns.
 *
 * @author Anthony Paper
 */

public interface OCellRenderer 
{
   /**
    * This method renders a cell column based on the x/y coordinates,
    * cell width and cell alignment.  The grid information is passed
    * to provide additional information for customized components.
    * The Row/Col number(s) are provided as additional information
    * that may used to help with customized render options.  The
    * current implementation handles text Strings, images, and
    * invokes the toString method for java.lang.Object(s) that
    * are not images or strings.
    *
    * @param g The graphics context.
    * @param x The pixel column at which to begin the display.
    * @param y The pixel row at which to begin the display.
    * @param rowNum The current row number being rendered. (Informational)
    * @param colNum The current column number being rendered. (Informational)
    * @param OCell  Cell object containing value to be rendered.
    * @param cellWidth The number of pixels available for the cell
    * @param cellAlignment The cell alignment values used for rendering
    * @param gridInfo Contains additional grid information
    */
   public void renderCol(Graphics g, 
                         int x, int y, 
                         int rowNum,    
                         int colNum,
                         OCell cell,
                         int cellWidth, 
                         int cellAlignment,
                         OGridInfo gridInfo);

//  public void render( Graphics g, int x, int y, int width, OCell cell);
}// OCellRenderer 
