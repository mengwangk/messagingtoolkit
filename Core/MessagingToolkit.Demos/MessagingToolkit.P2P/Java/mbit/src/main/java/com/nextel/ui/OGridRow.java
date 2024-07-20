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
 * Interface for a grid row object used to create and manage cell columns.
 * This interface is used by the {@link com.nextel.ui.OGrid} component
 * to render grid rows and columns.  Please refer to the {@link com.nextel.ui.OGridObjectRow}
 * class for an example implementation that may be used with the
 * {@link com.nextel.ui.OGrid} component.
 *
 * @author Anthony Paper
 */

public interface OGridRow 
{
  /**
   * Allocates the cell column objects used to render grid rows.
   */
  public void allocateCells();
  
  /**
   * Deallocates the cell column objects used to render grid rows.
   */
  public void deallocateCells();

  /**
   * Indicates whether the cells have been allocated
   * @return Flag indicating whether cells have been allocated.
   */
  public boolean hasAllocatedCells();

  /**
   * Creates and array of cell columns used to manage, display, and scroll grid rows.
   *
   * @return An array of cell columns
   */
   public  OCell [] getCells();

}// ORenderer
