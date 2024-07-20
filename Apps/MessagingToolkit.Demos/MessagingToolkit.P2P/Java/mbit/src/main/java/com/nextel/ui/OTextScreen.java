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
import com.nextel.util.Debugger;
import com.nextel.util.Logger;
import javax.microedition.lcdui.Canvas;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import com.nextel.exception.InvalidData;

/**
 *  A screen that allows the entry of text.
 * <p>
 * The component can be set to accept uppercase letters, lowercase letters,
 * numbers only, or any combination of the three. The desired behavior is
 * enabled via the {@link #OTextScreen constructor}. The component can
 * also be set to accept special characters, such as @ and $, via
 * {@link #allow}.
 * <p>
 * The character sets for each number
 * key, and the behavior exhibited when a number key is pressed,
 * are defined in {@link com.nextel.ui.OInputComposer}
 * <p>
 * <p>
 * The user can move within the text field
 * by pressing the left (previous character) and right (next character)
 * sides of the 4-way navigation key. The top and bottom sides of the 4-way
 * navigation key are therefore used to move to the previous and next component
 * on the screen, whereas with most other components the left and right sides of
 * the 4-way navigation key are used to move to the previous and next components.
 * <p>
 * A vertical cursor is displayed before the position where the next character
 * is to be placed.
 * <p>
 * The asterisk <b>*</b> key is used to perform backspace/delete
 * operations on the
 * text field.  When the asterisk key is depressed, the preceding character is
 * erased and the remaining characters are shifted left.  The cursor position
 * is updated to reflect the deleted character.
 * <p>
 * The pound <b>#</b> key is used to insert spaces into the
 * text field.  
 * <p>
 * If the text exceeds the vertical size of the display area up and down arrows,
 * as appropriate, will display at the bottom of the screen, and
 * the display will scroll vertically when the cursor is moved to a line that is
 * not displayed.
 * <p>
 * An optional
 * screen title can be supplied, and both the title and text can have their text
 * font specified.  The text will be wrapped automatically to ensure each line
 * fits width-wise into the display area.  You may also place '\n'
 * characters where line breaks are desired.
 * <p>
 * The field will also accept input when the handset is connected to the iBoard,
 * Motorola's foldable keyboard. However, because there is no way for the
 * component to know that it is attached to the iBoard, keys that issue
 * identical codes on the handset and iBoard will have handset behavior even
 * when pressed on the iBoard. For example, since the handset include number
 * keys, and multiple presses of a number key on the handset cycle through
 * available characters as described in {@link com.nextel.ui.OTextDocument},
 * pressing a number key on the iBoard will cycle through the same set of
 * character. Likewise, pressing the * and # keys on the iBoard result in the
 * insertion of a space or deletion of a character, respectively, because that
 * is the function of those keys on the handset.
 * 
 * @author Glen Cordrey
*/
public class OTextScreen extends OAbstractScreen implements OTextDocument
{
  // the maximum number of lines that can be placed in a screen. This number was
  // arbitrarily chosen for sizing purposes
  private static final int MAX_NBR_OF_LINES = 50;

  // anchor point for writing text
  private final static int ANCHOR = Graphics.BOTTOM | Graphics.LEFT;
  
  /** Number of pixels for a left margin */
  private static final int LEFT_MARGIN = 2;

  // number of pixels the cursor is offset from the left
  private static final int CURSOR_X_OFFSET = 1;
  private static Font textFont;

  private char widestChar = 'W';
  
  // the number of lines that can be displayed in the screen's body area
  private int nbrDisplayableLines;
    
  // the index in the list of the first line displayed 
  private int firstDisplayedLineIdx = 0;

  // the index in the list of the last line displayed
  private int lastDisplayedLineIdx = 0;

  private StringBuffer textBuff; // the text in the box

  // the index in textBuff of the cursor
  private int cursorIdx;

  // array of the end indices of the lines in the text. only the indices of the
  // displayed lines are kept current, the indices for the other lines get
  // recalculated when those lines get displayed
  private int [] lineEndIndices = new int [ MAX_NBR_OF_LINES ] ;

  // the index of the line that the cursor is on
  private int editingLineIdx;

  // the minimum number of characters in a line, calculated from the font and
  // the widest character
  private int minCharsInLine;
  
  private int nbrOfChars; // max # of characters in the box

  // whether the text box can be edited
  private boolean editable = true;

  private final int screenWidth = getWidth();

  // the constraints on the input characters, defined in OTextDocument
  private int constraints;
  
  // special characters, other than letters and digits,
  // that are allowed to be input
  private String specialChars = new String();

  // whether spaces can be entered into the box
  private boolean allowSpaces = false;

  // processes presses of the number keys 
  private OInputComposer composer;
  
  /**
   * Creates a new <code>OTextScreen</code> instance.
   *
   * @param title Screen title, which can be null.
   * @param titleFont Font for screen title, which should be null if the title
   * is null but not if it isn't.
   * @param text The text to display. The text may contain '\n' characters to
   * specify line breaks.
   * @param textFont The font to use for displaying the font.
   * @param nbrOfChars Maximum number of characters in the text box.
   * @param constraints {@link com.nextel.ui.OTextDocument#ANY}, or a
   * bitwise-or of any of {@link com.nextel.ui.OTextDocument#UPPERCASE},
   *{@link com.nextel.ui.OTextDocument#LOWERCASE}, and
   * {@link com.nextel.ui.OTextDocument#NUMERIC}
   * 
   */
  public OTextScreen ( String title, Font titleFont,
		    String text, Font textFont, int nbrOfChars,
		    int constraints )
  { 
    super( title, titleFont );
    if ( Debugger.ON ) Logger.dev( "OTextScreen.OTextScreen ENTERED" );
    this.textFont = textFont;
    this.nbrDisplayableLines = getBodyHeight() / textFont.getHeight();  
    this.minCharsInLine = getWidth() / textFont.charWidth( widestChar );
    this.nbrOfChars = nbrOfChars;
    this.constraints = constraints;
    
    textBuff = new StringBuffer( nbrOfChars );
    if ( text != null )
    { // text was supplied
      textBuff.insert( 0, text );
      updateLineIndices( 0 );
    }

    composer = new OInputComposer( this );
    if ( Debugger.ON ) Logger.dev( "OTextScreen.init EXITTING" );

  } // constructor

  /**
   * Creates a non-editable text screen.
   * <p>
   * This method calls {@link #OTextScreen(String, Font, String, Font, int, int )}
   * with <code>nbrOfChars</code> equal to the length of <code>text</code> and
   * <code>constraints</code> equal to
   * {@link com.nextel.ui.OTextDocument#ANY}, and calls
   * {@link #setEditable setEditable( false )}.
   *
   * @param title Screen title, which can be null.
   * @param titleFont Font for screen title, which should be null if the title
   * is null but not if it isn't.
   * @param text The text to display. The text may contain '\n' characters to
   * specify line breaks.
   * @param textFont The font to use for displaying the font.
   */
  public OTextScreen ( String title, Font titleFont,
		    String text, Font textFont )
  { 
    this( title, titleFont, text, textFont, text.length(),
	  OTextDocument.ANY );
    setEditable( false );
  } // OTextScreen
  
    /**
   * Sets whether the text is editable. The default behavior is that the text IS
   * editable. 
   *
   * @param value true if the text is to be editable, false otherwise
   */
  public void setEditable( boolean value ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.setEditable ENTERED" );
    this.editable = value;
    if ( Debugger.ON ) Logger.dev( "OTextScreen.setEditable EXITTING" );
  } // setEditable

  protected void paintBody( Graphics g ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.paintBody ENTERED" );
    
    int oldColor = g.getColor(); // save so we can restore it
    Font oldFont = g.getFont();

    super.paintBodyBackground( g );
    g.setFont( textFont );
    g.setColor( OUILook.TEXT_COLOR );
    
    // draw each text line within the window of lines to display
    int y = getBodyRow();
    String nextLine = null;
    if ( textBuff.length() > 0 ) 
    {
      for ( int idx = firstDisplayedLineIdx;
	    idx <= lastDisplayedLineIdx; idx++ ) 
      {
	int lineStart = 0;
	if ( idx > 0 ) lineStart = getLineStart( idx );
	int lineEnd = lineEndIndices[ idx ];
	if ( lineEnd >= lineStart
	     && lineEnd < textBuff.length() ) 
	{
	  char [] lineChars = new char[ lineEnd - lineStart + 1 ];
	  textBuff.getChars( lineStart, lineEnd + 1, lineChars, 0 );
	  nextLine = new String( lineChars );
	  y += textFont.getHeight();
	  g.drawString( nextLine, LEFT_MARGIN, y, ANCHOR );
	}
	else  break;
      }
    }

    // draw the cursor or, if the character is still composing, reverse-video
    // the character
    int cursorX = getCursorX();
    int cursorY = getCursorY();
    g.setColor( OColor.BLACK );
    if ( ! composer.isComposing() ) 
    { // draw the cursor
      g.drawLine( cursorX, cursorY + 1,
		  cursorX, cursorY + textFont.getHeight() - 2 );
    }
    else  if ( cursorIdx <= textBuff.length() ) 
    { // isComposing is true and the cursor is at a character (the cursor will
      // not be at a character if a number key that has no characters for the
      // constraints has been pressed, as when a 1 is pressed and the
      // constraints do not include NUMERIC)
      char composingChar = textBuff.charAt( cursorIdx - 1 );
      int composingX =
	cursorX - CURSOR_X_OFFSET - textFont.charWidth( composingChar );
      g.fillRect( composingX, cursorY - 1,
		  textFont.charWidth( composingChar ) + 2,
		  textFont.getHeight() );
      g.setColor( OUILook.BACKGROUND_COLOR );
      g.drawChar( composingChar, composingX,
		  cursorY + textFont.getHeight(), ANCHOR );
    }
    g.setColor( oldColor );
    g.setFont( oldFont );
 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.paintBody EXITTING" );
  } // paintBody

  /**
   * Determines whether scrolling is needed and in which directions.
   * <p>
   * This overrides the parent method because the parent method determines
   * scrolling via the components the class contains, and this class doesn't
   * contain any components.
   *
   * @return an <code>int</code> value
   */
  protected int getScrollDirections() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getScrollDirections ENTERED" );

    // if text can be scrolled indicate so 
    int directions = 0;
    
    if ( ! lastLine( lastDisplayedLineIdx ) )
    { // there are lines after the last displayed line, so need down scroller
      directions += OComponent.DOWN;
    }
    if ( firstDisplayedLineIdx > 0 ) 
    { // they've scrolled down, so need indicator they can scroll up
      directions += OComponent.UP;
    }
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getScrollDirections EXITTING" );
    return directions;
 
  } // getScrollDirections

  /**
   * Handles key presses.
   *
   * @param keyCode The code of the key that was pressed
   */
  public void keyPressed ( int keyCode ) 
  {
    if ( Debugger.ON ) Logger.dev( "OTextScreen.keyPressed w/keyCode= " + keyCode );
    if ( ! editable
	 && ( keyCode == Canvas.KEY_POUND || keyCode == Canvas.KEY_STAR
	      || ( keyCode >= Canvas.KEY_NUM0 && keyCode <= Canvas.KEY_NUM9 ) ) )
    { // tried to enter something in an uneditable text box
      OHandset.beep();
    }
    else  if ( keyCode >= Canvas.KEY_NUM0 && keyCode <= Canvas.KEY_NUM9 ) 
    { // one of the number keys was pressed
      if ( composer.keyPressed( keyCode ) ) repaint();
      else  OHandset.beep(); // key was rejected
    }
    else  // not a number key
    {
      composer.stopComposing();
      if ( keyCode == DOWN_KEY ) 
      {
	if ( lineEndIndices[ editingLineIdx ] < textBuff.length() - 1 )
	{
	  moveVertically( 1 );
	  repaint();
	}
	else  OHandset.beep(); // at last line
      }
      else  if ( keyCode == UP_KEY ) 
      {
	if ( editingLineIdx > 0 ) 
	{
	  moveVertically( -1 );
	  repaint();
	}
	else  OHandset.beep(); // at 1st line
      }
      else  if ( keyCode == LEFT_KEY )
      {
	if ( cursorIdx > 0 ) 
	{
	  moveLeft();
	  repaint();
	}
	else  OHandset.beep(); // at beginning of text
      }
      else  if ( keyCode == RIGHT_KEY )
      {
	if ( cursorIdx < textBuff.length() ) 
	{
	  advanceCursor();
	  repaint();
	}
	else  OHandset.beep(); // at end of text
      }
      else  if ( keyCode == Canvas.KEY_POUND )
      {
	insertChar( ' ' );
	repaint();
      }
      else  if ( keyCode == Canvas.KEY_STAR || keyCode == 8 /* ASCII code */ )
      {
	if ( cursorIdx > 0 )
	{ // they want to backspace/delete
	  deleteChar( );
	  repaint();
	}
	else  OHandset.beep(); // can't backspace past 1st position
      }    
      else  if ( keyCode >= 32 && keyCode <= 126
		 && isValid( ( char ) keyCode ) ) 
      { // ASCII code
	insertChar( ( char ) keyCode );
      }
      else // see if the parent can handle it
      {
	super.keyPressed( keyCode );
      }
    }
    
  } // keyPressed


  /**
   * Moves the cursor left 1 character;
   *
   */
  private void moveLeft() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.moveLeft ENTERED" );
    cursorIdx--;
    if ( editingLineIdx > 0 
	 && cursorIdx <= lineEndIndices[ editingLineIdx - 1 ] + 1 ) 
    { // we crossed a line boundary
      editingLineIdx--;
      if ( editingLineIdx < firstDisplayedLineIdx ) 
      {
	scroll( -1 );
      }
    }
 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.moveLeft EXITTING" );
  } // moveLeft



  /**
   * Moves the cursor vertically.
   *
   * @param nbrLines The number of lines to move. A negative value indicates
   * move up, a positive value means move down.
   */
  private void moveVertically( int nbrLines ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.moveVertically ENTERED" );
    int newLineIdx = editingLineIdx + nbrLines;
    if ( nbrLines > 0 && lineEndIndices[ newLineIdx ] == 0 ) 
    { // this is the first time this line is being displayed, so we need to
      // determine its last character
      lineEndIndices[ newLineIdx ] =
	getLineEnd( lineEndIndices[ editingLineIdx ] + 1 );
    }
    char [] line = getLine( newLineIdx );

    int cursorX = getCursorX();
    // the lowest possible number of characters that will fit in the width
    // defined by the cursor's x position is minimum of: 1- the cursor x position
    // divided by the widest possible character, and 1- the line length
    int charOffset =
      Math.min( cursorX / textFont.charWidth( widestChar ), line.length );
    charOffset = findCharIdx( line, charOffset, cursorX );
    int lineBegin = 0;
    if ( newLineIdx > 0 ) 
    {
      lineBegin = lineEndIndices[ newLineIdx - 1 ] + 1;
    }
    cursorIdx = lineBegin + charOffset;
    // don't change editingLineIdx until the end, because it is used
    // by getCursorX in finding the x coordinate before moving
    editingLineIdx += nbrLines;
    if ( editingLineIdx < firstDisplayedLineIdx  ||
	 editingLineIdx > lastDisplayedLineIdx ) scroll( nbrLines );

    if ( Debugger.ON ) Logger.dev( "OTextScreen.moveVertically EXITTING" );
  } // moveVertically

  /**
   * Gets the line at the specified index.
   *
   * @param idx Index of the line to get
   * @return a <code>char[]</code> value
   */
  private char [] getLine( int idx )
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getLine ENTERED" );
    // copy the line into a character buffer
    int lineBegin = 0;
    if ( idx > 0 ) lineBegin = lineEndIndices[ idx - 1 ] + 1;
    int lineEnd = lineEndIndices[ idx ];
    char [] line = new char[ lineEnd - lineBegin + 1 ];
    textBuff.getChars( lineBegin, lineEnd + 1, line, 0 );
 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getLine EXITTING" );
    return line;
  } // getLine

  /**
   * Gets the x coordinate of the cursor
   *
   * @return an <code>int</code> value
   */
  private int getCursorX() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getCursorX ENTERED" );
    int cursorX = CURSOR_X_OFFSET; 
    int lineStart = 0;
    if ( cursorIdx > 0 )
    {
      if ( editingLineIdx > 0 ) 
      {
	lineStart = lineEndIndices[ editingLineIdx - 1 ] + 1;
      }
      
      int offsetLen = cursorIdx -  lineStart;
      if ( offsetLen > 0 ) 
      {
	char [] offsetChars = new char[ offsetLen ];
	textBuff.getChars( lineStart, cursorIdx, offsetChars, 0 );
	cursorX += textFont.charsWidth( offsetChars, 0, offsetLen );
      }
    }
 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getCursorX EXITTING" );
    return cursorX;
  } // getCursorX
  
  /**
   * Finds the index of the last character that will fit in a width
   *
   * @param charString The character string to check
   * @param startingIdx The index within charString to start checking at
   * @param width The maximum width in pixels of the resulting string
   * @return an <code>int</code> value
   */
  private int findCharIdx( char [] charString,
			   int startingIdx, // index to start checking after
			   int width ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.findCharIdx ENTERED" );

    // check the width of the substring, adding one character until the width
    // exceeds the desired width or there are no more characters
    
    int endingIdx = startingIdx;
    for ( int extraChars = 1; endingIdx  <= charString.length &&
	    startingIdx + extraChars <= charString.length &&
	    textFont.charsWidth( charString, 0, startingIdx + extraChars )
	    <=  width; extraChars++ )
    {
      endingIdx++;
    }
    return endingIdx;
  } // findCharIdx

  /**
   * Gets the y coordinate of the cursor
   *
   * @return an <code>int</code> value
   */
  private int getCursorY() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getCursorY ENTERED" );
    int cursorY = getBodyRow();
    if ( cursorIdx > 0 )
    {
      cursorY +=
	( editingLineIdx - firstDisplayedLineIdx )  * textFont.getHeight();
    }
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getCursorY EXITTING" );
    return cursorY;

  } // getCursorY
  
  
  /**
   * Scrolls the display.
   *
   * @param nbrLines Number of lines to scroll. A negative number means scroll
   * up, a positive number means scroll down.
   */
  private void scroll( int nbrLines ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.scrollU ENTERED" );

    firstDisplayedLineIdx += nbrLines;
    lastDisplayedLineIdx += nbrLines;
    updateLineIndices( firstDisplayedLineIdx );
    if ( Debugger.ON ) Logger.dev( "OTextScreen.scroll EXITTING" );
  } // scroll

  /**
   * Updates lineEndIndices, starting at the supplied index and continuing to
   * the last displayable (meaning it will fit on the screen) line.
   *
   * @param startingLineIdx The index of the first line to update
   */
  private  void  updateLineIndices ( int startingLineIdx )
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.updateLineIndices ENTERED " );
    int textBuffEnd = textBuff.length() - 1;
    if ( textBuffEnd < 0 ) return; // no text to display
    
    // update the index for each displayed line after and including
    // startingLIneIdx 
    int lineStart = -1;
    int lineEnd = -1;
    for ( lastDisplayedLineIdx = startingLineIdx;
	  lineEnd != textBuffEnd &&
	    lastDisplayedLineIdx - firstDisplayedLineIdx < nbrDisplayableLines;
	  lastDisplayedLineIdx++)
    {
      lineStart = getLineStart( lastDisplayedLineIdx );
      lineEnd = getLineEnd( lineStart );
      lineEndIndices[ lastDisplayedLineIdx ] = lineEnd;
    }
    lastDisplayedLineIdx--;  // remove last increment
    
    if ( Debugger.ON ) Logger.dev( "OTextScreen.updateLineIndices EXITTING " );
    
  } // updateLineIndices

  /**
   * Gets the index in textBuff of the beginning of a line.
   *
   * @param lineIdx The index in lineEndIndices of the line.
   * @return an <code>int</code> value
   */
  private int getLineStart( int lineIdx ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getLineStart ENTERED" );
    int lineStart = 0;
    // get the beginning index, within the text buffer, of the line
    if ( lineIdx > 0 )  
    { // this line starts at the first character after the end of the last line,
      // unless the first character is a space and the second character isn't a
      // space, in which case we ignore the space so line's don't start with a
      // space (but a line can start with multiple spaces, since that may be
      // indentation)
      lineStart = lineEndIndices[ lineIdx - 1 ] + 1;
      int textBuffEnd = textBuff.length() - 1;
      if ( lineStart < textBuffEnd
	   && textBuff.charAt( lineStart ) == ' '
	   && lineStart + 1 < textBuffEnd
	   && textBuff.charAt( lineStart + 1 ) != ' ' )
      {
	lineStart++;
      }
    }
    // else  lineIdx == 0 and line always starts on 0
 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getLineStart EXITTING" );

    return lineStart;
  } // getLineStart

  /**
   * Gets the index in textBuff of the last character in a line.
   *
   * @param lineStart The beginning index, in textBuff, of the line.
   * @return an <code>int</code> value
   */
  private int getLineEnd( int lineStart ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getLineEnd ENTERED" );
    
    int textBuffEnd = textBuff.length() - 1;

    // find the last character that will fit in the screen width
    int lineEnd = lineStart;
    int lineLength = 0;
    while ( true ) 
    {
      lineLength += textFont.charWidth( textBuff.charAt( lineEnd ) );
      if ( lineLength > screenWidth )
      {
	lineEnd--;
	break;
      }
      else  if ( lineEnd == textBuffEnd ) break; 
      else  lineEnd++;
    }

    // we don't want to break in the middle of any contiguous characters, so if
    // the line isn't followed by a space back up to the last space
    if ( lineEnd < textBuffEnd && textBuff.charAt( lineEnd + 1 ) != ' ' ) 
    { 
      int testEnd = lineEnd;
      for ( ; testEnd > lineStart; testEnd-- )
      {
	if ( textBuff.charAt( testEnd ) == ' ' )
	{ // found space
	  lineEnd = testEnd;
	  break;
	}
      }
    }
    
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getLineEnd EXITTING" );
    return lineEnd;
  } // getLineEnd

  /**
   * Determines whether a line is the last line in textBuff.
   *
   * @param lineIdx The index in lineEndIndices of the line to check.
   * @return true if the line is the last line
   */
  private boolean lastLine( int lineIdx ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.lastLine CALLED" );
    return ( textBuff.length() == 0 ||
	     lineEndIndices[ lineIdx ] == textBuff.length() - 1 );
  } // lastLine

  /**
   * Allows entry into the field of any of the specified special characters.
   *
   * @param specialChars The special characters to allow in the field. Any
   * combination of
   * <pre> + - * / = < > # . ? ! , @ & : ; " _ ( ) { } [ ] ' % $ ^ { } | \ ~ </pre>
   * or {@link #ALL_SPECIAL_CHARACTERS}
   */
  public void allow( String specialChars ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.allow ENTERED" );
    this.specialChars = specialChars;
    if ( Debugger.ON ) Logger.dev( "OTextScreen.allow EXITTING" );
  } // allow

  
  ///////// Implement OTextDocument
  
  /**
   * Inserts a character into the document.
   *
   * @param value The character to insert
   */
  public void insertChar( char value ) 
  {
    if ( Debugger.ON ) Logger.dev( "OTextScreen.insert ENTERED" );
    textBuff.insert( cursorIdx, value );
    updateLineIndices( editingLineIdx );
    advanceCursor();
    repaint();
 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.insert EXITTING" );
  } // insertChar

  
  /**
   * Deletes the character before the cursor.
   *
   */
  public void deleteChar()
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.delete ENTERED" );
    textBuff.deleteCharAt( cursorIdx - 1 ); 
    if ( lineEndIndices[ editingLineIdx ] == textBuff.length() - 1 ) 
    { // we deleted the last line, so adjust
      lastDisplayedLineIdx = editingLineIdx;
    }

    // delete near beginning of line may cause word to unwrap to previous line
    int updateLine = 0;
    if ( editingLineIdx > 0 ) updateLine = editingLineIdx - 1; 
    
    updateLineIndices( updateLine );
    moveLeft();
 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.delete EXITTING" );
  } // deleteChar

  /**
   * Gets the number of characters in the document
   *
   * @return an <code>int</code> value
   */
  public int length() 
  {
    return textBuff.length();
  } // length

  /**
   * Gets the maximum number of characters the document can contain.
   *
   * @return an <code>int</code> value
   */
  public int getMaxNbrOfChars() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getMaxNbrOfChars ENTERED" );
    return nbrOfChars;
  } // getMaxNbrOfChars

  /**
   * Gets the constraints that apply to the document.
   *
   * @return A bitwise or of
   * {@link com.nextel.ui.OTextDocument#UPPERCASE},
   *{@link com.nextel.ui.OTextDocument#LOWERCASE}, and/or
   * {@link com.nextel.ui.OTextDocument#NUMERIC}
   */
  public int getConstraints() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getConstraints ENTERED" );
    return constraints;
  } // getConstraints

  /**
   * Gets the special characters that the document will accept.
   *
   * @return the special characters that the document will accept
   */
  public String getSpecialChars() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getSpecialChars ENTERED" );
    return specialChars;
  } // getSpecialChars

  /**
   * Sets whether spaces are allowed in the field. The default behavior is
   * that spaces are allowed.
   *
   * @param value false if spaces are to not be allowed
   */
  public void allowSpaces( boolean value ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.allowSpaces ENTERED" );
    allowSpaces = value;
    if ( Debugger.ON ) Logger.dev( "OTextScreen.allowSpaces EXITTING" );
  } // allowSpaces

  /**
   * Sets the text in the document.
   *
   * @param value The document text
   * @exception InvalidData if the text exceeds the length of the field or
   * contains invalid characters.
   */
  public void setText( String value ) 
    throws InvalidData
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.setText ENTERED" );
    if ( value.length() > nbrOfChars ) 
    {
      throw new InvalidData( "'" + value + "' > " + nbrOfChars +
			     " characters" );
    }

    if ( value.length() > 0 ) 
    {
      validate( value ); // throws exception if invalid
      textBuff = new StringBuffer( value );
      textBuff.ensureCapacity( nbrOfChars );
    }
    else  textBuff = new StringBuffer( nbrOfChars );
    cursorIdx = 0;
    firstDisplayedLineIdx = 0;
    lastDisplayedLineIdx = 0;
    lineEndIndices[0] = 0;
    editingLineIdx = 0;
    updateLineIndices( 0 );
    repaint();
    if ( Debugger.ON ) Logger.dev( "OTextScreen.setText EXITTING" );
  } // setText

  /**
   * Validate that a string conforms to the constraints.
   *
   * @param value String to validate
   * @exception InvalidData if <code>value</code> does not conform to the
   * constraints (including special characters).
   */
   private void validate( String value )
    throws InvalidData
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.validate ENTERED" );
    StringBuffer sBuff = new StringBuffer( value );
    char nextChar;
    for ( int idx=0; idx < sBuff.length(); idx++ ) 
    {
      nextChar = sBuff.charAt( idx );
      if ( ! isValid( nextChar ) )
      {
	throw new InvalidData( "'" + nextChar + "' violates constraints" );
      }
    }
  } // validate

    /**
     * Tests whether a character is valid given the constraints.
     *
     * @param value Character to test
     * @return true if the character is allowed under the constraints
     */
   private boolean isValid( char value ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.validate ENTERED" );
    return ( ( ( constraints &  OTextDocument.NUMERIC ) > 0
		&& Character.isDigit( value ) )
	      || ( ( constraints & OTextDocument.UPPERCASE ) > 0
		   && Character.isUpperCase( value ) )
	      || ( ( constraints & OTextDocument.LOWERCASE ) > 0
		   && Character.isLowerCase( value ) )
	      || ( ( specialChars.length() > 0
		     && ( specialChars.indexOf( value ) > -1 ) ) )
	      || ( allowSpaces && value == ' ' )
	     );
  } // isValid  


  /**
   * Gets the document's text.
   *
   * @return a <code>String</code> value
   */
  public String getText() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getText ENTERED" );
    return textBuff.toString();
  } // getText

  /**
   * Gets whether the document allows the insertion of spaces.
   *
   * @return true if spaces can be inserted
   */
  public boolean getAllowSpaces() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.getAllowSpaces ENTERED" );
    return allowSpaces;
  } // getAllowSpaces

  /**
   * Advances the cursor to the next character
   *
   */
  public void advanceCursor() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.advanceCursor ENTERED" );
    if ( cursorIdx  > lineEndIndices[ editingLineIdx ] )
    { // we crossed a line boundary
      editingLineIdx++;
      if ( editingLineIdx > lastDisplayedLineIdx )
      { // line being edited is not displayed (it's a new line)
	if ( editingLineIdx - firstDisplayedLineIdx  ==
	     nbrDisplayableLines ) 
	{ // line is off-screen
	  scroll( 1 );
	}
	else  lastDisplayedLineIdx++;
      }
    }
    cursorIdx++;
    if ( Debugger.ON ) Logger.dev( "OTextScreen.advanceCursor EXITTING" );
  } // advanceCursor
  
  /**
   * Called to indicate that the document has been changed.
   *
   */
  public void changed() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextScreen.changed ENTERED" );
    repaint();
    if ( Debugger.ON ) Logger.dev( "OTextScreen.changed EXITTING" );
  } // changed
  
}// OTextScreen
