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

package com.nextel.util; // Generated package name

/**
 * Parses words from a String.
 * <p>
 * Words are parsed at space boundaries only - no other whitespace, such as
 * tabs, is recognized.
 *
 * @author Glen Cordrey
 */

public class WordParser 
{
  private final static char SPACE = ' ';
  protected String text;
  private int startIndex;
  private boolean parsedAll = false;

  /**
   * Creates a new <code>WordParser</code> instance.
   *
   * @param text Text containing words to be parsed.
   */
  public WordParser ( String text )
  {
    this.text = text.trim();
  } // constructor
  
  /**
   * Returns the next word in the text. 
   *
   * @return The next word in the text
   */
  public String nextWord () 
  {
    if ( Debugger.ON ) Logger.dev( "WordParser.nextWord ENTERED, text= " + text +
				" startIndex= " + startIndex );
      
    if ( parsedAll ) 
    {
      if ( Debugger.ON )
	Logger.dev( "WordParser.nextWord EXITTING, all words parsed" );
      return null;
    }
    
    int endIndex = text.indexOf( SPACE, startIndex );
    while( endIndex == startIndex ) 
    { // multiple whitespace, for interate forward until out of it
      startIndex++;
      endIndex = text.indexOf( SPACE, startIndex );
    }
      
    if ( endIndex > -1 ) 
    {
      if ( Debugger.ON ) Logger.dev( "WordParser.nextWord endIndex=" +
				  endIndex ); 
      String word = text.substring( startIndex, endIndex );
      startIndex = endIndex + 1;
      if ( Debugger.ON ) Logger.dev( "WordParser.nextWord EXITTING w/word=" +
				  word );
	 
      return word;
    }
    else  
    { // it's the last word
      parsedAll = true;
      if ( Debugger.ON ) Logger.dev( "WordParser.nextWord EXITTING. Last Word:" +
        text.substring( startIndex ) + "..." + startIndex );
      return text.substring( startIndex );
    }
      
  } // nextWord

}// WordParser


