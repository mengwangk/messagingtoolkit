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

import com.nextel.util.*;
import java.util.Enumeration;
import java.util.Hashtable;
import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Graphics;
import java.util.Vector;

/**
 * A container of components.
 * <p>
 * A component's rank is the order in which it was added to the container.
 * Components should be added as they appear on the screen, left-to-right and
 * top-to-bottom.
 *
 */
public interface OContainer
{
  /**
   * Gets the component of the specified rank.
   *
   * @param rank Rank of the desired component.
   * @return component of the specified rank
   */
  public OComponent getComponent (int rank);

  /**
   * Gets the number of components in the container.
   *
   * @return The number of components in the container
   */
  public int getComponentCount ();

  /**
   * Gets the components in the container
   *
   * @return The components in the container.
   */
  public OComponent [] getComponents();
  
  /**
   * Gets the screen that contains this container.
   *
   * @return The screen containing this container, or a null if this container
   * has not yet been added to a screen.
   */
  public OCompositeScreen getScreen();

  /**
   * Sets the screen that contains this container.
   *
   * @param screen The screen that contains this container.
   */
  public void setScreen( OCompositeScreen screen );

  /**
   * Paints this container to the screen.
   *
   * @param g The Graphics object to which the container's contents are painted.
   * @param all True if all of the container's components are to be painted,
   * false if you want to paint only those components whose appearance may
   * have changed since the screen was last painted.  Painting only those
   * changed components will reduce screen flicker. However, if an action
   * results in a change to more than the component involved - for example, if
   * an expandable component such as a pop-up list is collapsed - then the
   * entire screen may need to be painted.
   */
  public void paint( Graphics g, boolean all );


  /**
   * Paints to the screen the components whose appearance may have changed.
   * Equivalent to paint( g, false );
   *
   * @param g The Graphics object to which the components are painted.
   */
  public void paint( Graphics g );

  /**
   * Gets the width of the container.
   *
   * @return an <code>int</code> value
   */
  public int getWidth( );

  /**
   * Gets the height of the container.
   *
   * @return an <code>int</code> value
   */
  public int getHeight();

}// OContainer
