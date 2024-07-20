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

/**
 *  An event that signals that there has been a change as to which component is
 *  currently focused.
 *
 */
public class OFocusEvent 
{
  /**
   * Indicates that the component receiving the event has gained focus.
   */
  public final static int FOCUS_GAINED = 1;

  /**
   * Indicates that the component receiving the event has loast focus.
   */
  public final static int FOCUS_LOST = -FOCUS_GAINED;
  
  // The source of the focus change.  for example, if the component at rank 0
  // has focus and the user presses the the right key, the rank 0 component is
  // the source
  private OFocusableComponent source;

  // One of the FOCUS_* int values above.
  private int id;
  
  /**
   * Creates a new <code>OFocusEvent</code> instance.
   *
   * @param source The component which has focus when the focus change begins.
   * @param id {@link #FOCUS_GAINED} or {@link #FOCUS_LOST}
   */
  public OFocusEvent ( OFocusableComponent source,
		       int id )
  {
    if ( Debugger.ON ) Logger.dev( "OFocusEvent ctor CALLED w/source= " + source +
				", id=" + id );
    
    this.source = source;
    this.id = id;
  } // constructor

  /**
   * Gets the value describing whether focus was gained or lost.
   *
   * @param id {@link #FOCUS_GAINED} or {@link #FOCUS_LOST}
   */
  public int getId () 
  {
    if ( Debugger.ON )
      Logger.dev( "OFocusEvent.getID CALLED, returning id= " + id  );
    return this.id;
  } // getId

  /**
   * Gets the component which had focus when the focus change begins.
   *
   * @return a <code>OFocusableComponent</code> value
   */
  public OFocusableComponent getSource () 
  {
    if ( Debugger.ON ) Logger.dev( "OFocusEvent.getSource CALLED, returning " +
				this.source );
    return this.source;
  } // getSource

}// OFocusEvent
