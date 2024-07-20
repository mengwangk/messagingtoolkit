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
import java.util.Timer;
import java.util.TimerTask;
import javax.microedition.lcdui.Canvas;

/**
 * Composes input entered via the number keys.
 * <p>
 * Characters are entered by pressing a number key 
 * within 1.4 seconds of a previous press of the same key, which will cycle
 * through a sequence of available characters.
 * While the component is in this 1.4 second interval the current
 * character is reverse-videoed. If a key other than the last pressed key is
 * pressed during this interval, or if the interval elapses without another key
 * press,  the current character assigned to the key is
 * placed in the field and the cursor position is advanced.
 * <p>Here are the possible characters for the number keys, depending upon which
 * constraints have been set:
 * <pre>
 * 0 + - * / = < > # % $
 * 1 . ? ! , @ & : ; " _ ( ) { } [ ] ^| \ ~
 * 2 a b c A B C            
 * 3 d e f D E F            
 * 4 g h i G H I            
 * 5 j k l J K L            
 * 6 m n o M N O            
 * 7 p q r s P Q R S  
 * 8 t u v T U V            
 * 9 w x y z W X Y Z
 </pre>
 * <p>
 * For example, if the field is created with the
 * {@link com.nextel.ui.OTextDocument#UPPERCASE} constraint
 * and the key numbered 2 is pressed a letter A will be placed in the field and
 * displayed in reverse video. If
 * the key numbered 2 is pressed again within 1.4 seconds of the previous press
 * the letter A displayed in the field will be replaced with the letter B, 
 * again displayed in reverse video. If 1.4 seconds from the second key presses
 * then elapses without another key press, the letter B remains in the field and
 * is no longer reverse videoed, and the vertical cursor will advance to be to
 * the right of the letter B.
 * @author Glen Cordrey
 */
public class OInputComposer 
{
  // Other combinations for internal use
  static final int  NUMERIC_UPPERCASE =
    OTextDocument.NUMERIC | OTextDocument.UPPERCASE;
  static final int  NUMERIC_LOWERCASE =
    OTextDocument.NUMERIC | OTextDocument.LOWERCASE;
  static final int LETTERS =
    OTextDocument.UPPERCASE | OTextDocument.LOWERCASE;

  // the characters, mapped to the number keys
  private final static char CHARACTERS [][] =
  {
    {'0', '+', '-', '*', '/', '=', '<', '>', '#', '%', '$' },
    {'1', '.', '?', '!', ',', '@', '&', ':', ';', '"', '\'',
     '_', '(', ')', '{', '}', '[', ']', '^', '|', '\\', '~' },
    {'2', 'a', 'b', 'c', 'A', 'B', 'C'},            
    {'3', 'd', 'e', 'f', 'D', 'E', 'F'},            
    {'4', 'g', 'h', 'i', 'G', 'H', 'I'},            
    {'5', 'j', 'k', 'l', 'J', 'K', 'L'},            
    {'6', 'm', 'n', 'o', 'M', 'N', 'O'},            
    {'7', 'p', 'q', 'r', 's', 'P', 'Q', 'R', 'S'},  
    {'8', 't', 'u', 'v', 'T', 'U', 'V'},            
    {'9', 'w', 'x', 'y', 'z', 'W', 'X', 'Y', 'Z'}   
  };
  
  // number of milliseconds during which a repeat press of a key will cause
  // cycling through the characters associated with that key
  private static long composingInterval = 1400;

  // the combination of OTextDocument.UPPERCASE, OTextDocument.LOWERCASE,
  // and/or  OTextDocument.NUMERIC that define what
  // characters, other than special characters, the field will accept
  private int constraints;

  private long lastTime; // the time of the last key press
  private long thisTime; // the time of the current key press

  // timer to time when the composingInterval has elapsed
  private Timer composingTimer;

  // whether we are within the timing interval defined by composingInterval
  private boolean isComposing = false;
  
  // key code of the last pressed numeric key. start with an arbitrary negative
  // number 
  private int lastKey = -123;
  
  // index into row of values for a key of the last alphanumeric used.
  // start at -1 because we increment it before we use it
  private int lastCharIdx = -1;

  // the number of characters that the document can hava
  private int nbrOfChars;

  // The text document we are managing input for
  private  OTextDocument document;
  
  /**
   * Creates a new <code>OInputComposer</code> instance.
   *
   * @param document The document to manage input for.
   */
  public OInputComposer(  OTextDocument document )
  { 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.OInputComposer ENTERED" );
    this.document = document;
    this.constraints = document.getConstraints();
    nbrOfChars = document.getMaxNbrOfChars();
    if ( Debugger.ON ) Logger.dev( "OInputComposer.OInputComposer EXITTING" );
  } // OInputComposer

  /**
   * Composes a character when repeated presses of a key are to cycle through
   * the set of available characters for that key.
   *
   * @param keyCode Code of the key that was pressed
   * @returns true if the key code is for a valid key given the constraints
   */
  boolean compose( int keyCode ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.compose ENTERED" );
    int keyIdx = keyCode - '0'; // index of the key within CHARACTERS
    // save the times of the last 2 key presses so we can determine if
    // we're within the timing window
    lastTime = thisTime;
    thisTime = System.currentTimeMillis();
    if ( lastKey == keyIdx && 
	 thisTime - lastTime <= composingInterval ) 
    { // this is a repeat press of a key within the composing interval,
      startComposing( );

      // get the next character that we use for this key,
      // traversing the array for this key circularly so it rolls
      // back to the first character after the last character
      lastCharIdx =
	getNextCharIdx( keyIdx,
			( ( lastCharIdx + 1 ) % CHARACTERS[ keyIdx ].length ) );
      // we got a character, so delete the current character at the cursor
      if ( lastCharIdx > -1 ) document.deleteChar( );
    }
    else // this is the first press of this key
    {
      if ( document.length() < nbrOfChars )
      {
	startComposing();
	lastCharIdx = getNextCharIdx( keyIdx, 0 );
      }
      else 
      {
	OHandset.beep();
	return false;
      }
    }
    lastKey = keyIdx;
    if ( lastCharIdx > -1 ) 
    { // the pressed key has a valid value given the constraints
      document.insertChar( CHARACTERS[ keyIdx ] [ lastCharIdx ] );
    }
    else // the key has no value given the constraints, as when the key is 1
    {  // and the constraints don't include  OTextDocument.NUMERIC
      cleanupComposing(  );
    }
    
    if ( Debugger.ON ) Logger.dev( "OInputComposer.compose EXITTING" );
    return true;
  } // compose
  
  /**
   * Initiates the composing interval.
   *
   */
  private void startComposing( ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.startComposing ENTERED" );
    stopComposing();
    if ( composingTimer != null ) composingTimer.cancel();

    // create task to run at completion of the composing interval
    TimerTask timerTask = new TimerTask()
      {
	public void run()
	{ 
	  cleanupComposing(  );
	}
      };
    composingTimer = new Timer();
    composingTimer.schedule( timerTask, composingInterval );
    isComposing = true;
 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.startComposing EXITTING" );
  } // startComposing

  /**
   * Stops a composing interval.
   *
   */
  void stopComposing(  ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.stopComposing ENTERED" );
    if ( composingTimer != null ) composingTimer.cancel();    
    cleanupComposing(  );
    if ( Debugger.ON ) Logger.dev( "OInputComposer.stopComposing EXITTING" );
  } // stopComposing

  /**
   * Cleans up when stopping composing.
   * <p>
   * If a character was input then the cursor index is incremented. A character
   * may not have been input in some instances, such as when the constraint is
   * OTextDocument.UPPERCASE only, no special characters are allowed, and the number 1 is
   * pressed. Because no letters are associated with 1 pressing it would
   * initiate a composing interval but not result in a character being input.
   *
   */
  synchronized private void cleanupComposing( )
  { 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.cleanupComposing ENTERED" );
    // We only want to perform this processing if composing is in effect.
    if ( isComposing ) 
    {
      isComposing = false;
      document.changed();
    }
 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.cleanupComposing EXITTING" );
  } // cleanupComposing
  
  /**
   * Gets the index within CHARACTERS of the next character for the pressed key.
   *
   * @param keyIdx Index within CHARACTERS of the pressed key.
   * @param beginningIdx Index, within the CHARACTERS
   * subarray for the pressed key,
   * of the next character to check to see if it is to be used for input.
   * @return the index within the CHARACTERS subarray for the pressed key of the
   * next character to input, or -1 if there is no character for the
   * key (for example, if the key is 1 and the field is OTextDocument.UPPERCASE).
   */
  private int getNextCharIdx( int keyIdx, int beginningIdx ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.getNextCharIdx ENTERED" );
    int startingIdx = beginningIdx;
    char nextChar; // the next character to check
    while ( true ) // exit when found the right character 
    { 
      nextChar = CHARACTERS[ keyIdx ] [ beginningIdx ];
      if ( ( ( constraints &  OTextDocument.NUMERIC ) > 0
	     && Character.isDigit( nextChar ) )
	   || ( ( constraints & OTextDocument.LOWERCASE ) > 0
		&& Character.isLowerCase( nextChar ) )
	   || ( ( constraints & OTextDocument.UPPERCASE ) > 0
		&& Character.isUpperCase( nextChar ) )
	   || ( document.getSpecialChars().indexOf( nextChar ) > -1 ) )
      { // this character matches the constraints
	break;
      }
      else 
      { // next check will be for the next character, wrapping back around to
	// the beginning of the array entry if necessary
	beginningIdx = ++beginningIdx % CHARACTERS[ keyIdx ].length;

	if ( beginningIdx == startingIdx )
	{ // we've wrapped around for this key and not found a match, e.g. the
	  // key is 1 but no numeric characters are allowed
	  OHandset.beep();
	  beginningIdx = -1;
	  break;
	}
      }
    }
    if ( Debugger.ON ) Logger.dev( "OInputComposer.getNextCharIdx EXITTING" );
    return beginningIdx;
  } // getNextCharIdx

  /**
   * Returns true if we are in a composing interval
   *
   * @return a <code>boolean</code> value
   */
  boolean isComposing( ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OInputComposer.isComposing ENTERED" );
    return isComposing;
  } // isComposing

  /**
   * Processes a key press.
   *
   * @param keyCode The code of the pressed key
   * @return true if the key was accepted (i.e., it was a valid key)
   */
  public boolean keyPressed( int keyCode ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.keyPressed ENTERED" );

    boolean status = false;
    if ( keyCode >= Canvas.KEY_NUM0 && keyCode <= Canvas.KEY_NUM9 ) 
    { // one of the number keys was pressed
      if ( document.length() < nbrOfChars || isComposing ) 
      { // there is room for another character
	if ( constraints != document.NUMERIC ||
	     document.getSpecialChars().length() > 0 ) 
	{
	  status = compose( keyCode );
	}
	else  // field is numeric only, so we don't use the timing window
	{
	  document.insertChar( ( char ) keyCode );
	  status = true;
	}
      }
    } // if on number key
    return status;
  } // keyPressed
  
} // OInputComposer


