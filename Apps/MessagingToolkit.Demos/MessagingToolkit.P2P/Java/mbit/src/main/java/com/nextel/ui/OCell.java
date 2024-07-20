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
  * Class used to contain the cell object value and cell renderer.
  * It represents a cell column within a grid. This holder object is 
  * specified by {@link com.nextel.ui.OGridRow#getCells()} interface
  * method.  It is used to support both default and custom rendering 
  * mechanisms for the {@link com.nextel.ui.OGrid} component.  The 
  * {@link com.nextel.ui.OGridRow} interface describes the allocation, 
  * deallocation, and retrieval mechanisms for managing OCell objects.
  *
  * @author Anthony Paper
  */

public class OCell
{
    

  /**
   * Constructor used to allocate an OCell object.
   * The default OCell renderer shall be used by
   * the {@link com.nextel.ui.OGrid} component.  
   * This constructor is the preferred way to 
   * create OCell objects.
   *
   * @param v Contains the OCell object value. 
   */
   public OCell(Object v)
   { value = v; renderer = null;}
   
  /**
   * Constructor used to allocate an OCell object.
   * Null renderer may be passed to the object and
   * the {@link com.nextel.ui.OGrid} component shall 
   * use the default OCell renderer.  
   *
   * @param v Contains the OCell object value.
   * @param r Contains the OCell renderer value. 
   */
   public OCell(Object v, OCellRenderer r)
   { value = v; renderer =r;}

  /**
   * Contains the object value used to render the cell column
   */
  public Object value;
  /**
   * Contains the renderer used to render the cell column.  
   * This value may be null and the default OCell renderer
   * shall be used to render the object value.
   */
  public OCellRenderer renderer;


}// OCell
