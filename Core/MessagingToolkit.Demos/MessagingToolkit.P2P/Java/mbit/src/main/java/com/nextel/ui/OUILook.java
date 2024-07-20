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
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;

/**
 * Defines the default look of the displays.
 * <p>
 * All of these are default values which may be overridden by individual
 * displays. 
 * @author Glen Cordrey
 */

public class OUILook 
{
  /** Plain style small size font */
  public static final Font PLAIN_SMALL =
    Font.getFont( Font.FACE_PROPORTIONAL, Font.STYLE_PLAIN, Font.SIZE_SMALL );
    
  /** Plain style medium size font */
  public static final Font PLAIN_MEDIUM =
    Font.getFont( Font.FACE_PROPORTIONAL, Font.STYLE_PLAIN, Font.SIZE_MEDIUM );

  /** Plain style medium size font */
  public static final Font PLAIN_LARGE =
    Font.getFont( Font.FACE_PROPORTIONAL, Font.STYLE_PLAIN, Font.SIZE_LARGE );

  /** Bold style small size font */
  public static final Font BOLD_SMALL =
    Font.getFont( Font.FACE_PROPORTIONAL, Font.STYLE_BOLD, Font.SIZE_SMALL );

  /** Bold style medium size font */
  public static final Font BOLD_MEDIUM =
    Font.getFont( Font.FACE_PROPORTIONAL, Font.STYLE_BOLD, Font.SIZE_MEDIUM );

  /** Bold large font **/
  public static final Font BOLD_LARGE =
    Font.getFont( Font.FACE_PROPORTIONAL, Font.STYLE_BOLD, Font.SIZE_LARGE );
  
  /** Default font to use for text */
  public final static Font TEXT_FONT = PLAIN_SMALL;

  /** Color to use for text display */
  public static final int TEXT_COLOR = OColor.BLACK;

  /** Spacer for the top and left sides of strings. Strings include inter-line
      and inter-character spacing to the right and below the string (see
      {@link javax.microedition.lcdui.Graphics}) but the top and left sides have
      no inter-line or inter-character spacing. Therefore, to keep these sides of
      text from being right next to the previous component you need to add some
      extra space **/
  public static  final int STRING_SPACER_HEIGHT = 2;
  public static  final int STRING_SPACER_WIDTH = 2;
  
  /** Vertical gap, in pixels, between components */
  public static final int V_GAP = 2;

  /** Horizontal gap, in pixels, between components */
  public static final int H_GAP = 3;

  /** Color to give backgrounds. {@link com.nextel.ui.OColor#LT_GRAY} **/
  public static final int BACKGROUND_COLOR = OColor.LT_GRAY;
  
  /**
   * Creates a new <code>OUILook</code> instance.
   *
   */
  private OUILook ()
  { } // constructor
  
}// OUILook



