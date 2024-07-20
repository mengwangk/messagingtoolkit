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
import javax.microedition.lcdui.*;
import com.nextel.util.*;
import com.nextel.exception.InvalidData;
import com.nextel.util.Logger;


/**
 *
 * This component allows the user to enter alphanumeric data.
 * <p><b>Example:</b> <img src="doc-files/TextField.jpg">
 * <p>
 * The component can be set to accept uppercase letters, lowercase letters,
 * numbers only, or any combination of the three. The desired behavior is
 * enabled via the {@link #OTextField constructor}. The component can
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
public class OTextField
  extends OFocusableComponent
  implements OTextDocument,
	     OThread // so composing timer is automatically stopped
{
  // anchor position for the field's text
  private static final int STRING_ANCHOR = Graphics.BOTTOM | Graphics.LEFT;

  // The number of characters that the field may contain (i.e., the field length)
  private int nbrOfChars;

  // The buffer containing the field's text
  private StringBuffer textBuff;

  // index in textBuff of the cursor
  private int cursorIdx;

  // handles input of the number keys
  private OInputComposer composer;

  // the constraints that apply to the field
  private int constraints;
  
  // special characters that are allowed to be input
  private String specialChars = new String();

  // whether spaces can be entered into the field
  private boolean allowSpaces = false;
  
  private Font font; // for display of the text
  private int charWidth; // max width in pixels of a character
  private int width; // width in pixels of the field
  private int height; // height in pixels of the field
  
  /** Constructs an instance of OTextField.
   *
   * @param nbrOfChars The total number of allowable characters.
   * @param font Font to be used to display the characters.
   * @param constraints {@link #ANY}, or any combination of
   * {@link #LOWERCASE}, {@link #UPPERCASE}, and/or {@link #NUMERIC}, as in
   * <pre><code>
   * OTextField field =
   *   new OTextField ( 5, OUILook.PLAIN_SMALL,
   *                            OTextField.UPPERCASE |
   *			        OTextField.NUMERIC);
   * </code></pre>
   */
  public OTextField(int nbrOfChars, Font font, int constraints)
  {
    this.nbrOfChars = nbrOfChars;
    this.font = font;
    this.constraints = constraints;
    textBuff = new StringBuffer( nbrOfChars );
    composer = new OInputComposer( this );
    
    // calculate the width from the widest character allowed in the field
    switch ( constraints ) 
    {
    case ANY: case  OInputComposer.LETTERS: case  UPPERCASE :
    case  OInputComposer.NUMERIC_UPPERCASE:
      {
	charWidth = font.charWidth( 'W' );
	break;
      }  
    case  LOWERCASE: case  OInputComposer.NUMERIC_LOWERCASE:
      {
	charWidth = Math.max( font.charWidth( '0' ), font.charWidth( 'w' ) );
	break;
      }
    default: // only case left is NUMERIC
      {
	charWidth = font.charWidth( '0' );
	break;
      }
    } // switch
    width = ( charWidth * nbrOfChars ) /* text width */ +
      ( PERIMETER_WIDTH * 2 ) /* left & right sidelines */;
    height = font.getHeight() + OUILook.STRING_SPACER_HEIGHT;

    // override normal use of 4-way navigation so you get into/out of this field
    // via the up/down keys.
    setTraverseDirections( UP | DOWN );
  } // constructor

  /**
   * Paints the component
   *
   * @param g Graphics object to paint to.
   */
  final public void paint( Graphics g )
  {
    if ( Debugger.ON ) Logger.dev( "OTextField.paint ENTERED" );
    
    int oldColor = g.getColor(); // save so we can restore it
    Font oldFont = g.getFont();
    
    paintBox( g, getX(), getY(), width, height );
    if ( textBuff != null && textBuff.length() > 0 )
    { // draw the text in the box
      g.setFont( font );
      if ( hasFocus() ) g.setColor( SELECTED_FOREGROUND );
      else  g.setColor( OUILook.TEXT_COLOR );
      g.drawString( textBuff.toString(), getX() + 2 /* outline + spacer */,
		    getY() + height, STRING_ANCHOR );
    }
    
    if ( hasFocus() )
    { // we'll need to draw the cursor or reverse-video the composing character
      int cursorX = getX() + 1 /* spacer */;
      if ( textBuff.length() > 0 ) 
      { // place the cursor after the last character
	char [] fieldB4Cursor = new char [ cursorIdx ];
	textBuff.getChars( 0, cursorIdx, fieldB4Cursor, 0 );
	if ( fieldB4Cursor.length > 0 ) 
	{ // there are 1 or mor characters in the field
	  cursorX += font.charsWidth( fieldB4Cursor, 0, fieldB4Cursor.length );
	}
      }
      if ( ! composer.isComposing() ) 
      { // draw the cursor
	g.setColor( SELECTED_FOREGROUND );
	g.drawLine( cursorX, getY() + 2, cursorX, getY() + height - 2 );
      }
      else  if ( cursorIdx > 0 && cursorIdx <= textBuff.length() ) 
      { // isComposing is true and the cursor is at a character (the cursor will
	// not be at a character if a number key that has no characters for the
	// constraints has been pressed, as when a 1 is pressed and the
	// constraints do not include NUMERIC)
	char composingChar = textBuff.charAt( cursorIdx - 1 );
	int composingX = cursorX - font.charWidth( composingChar );
	g.setColor( SELECTED_FOREGROUND );
	g.fillRect( composingX, getY() + 1,
		    font.charWidth( composingChar ) + 1,
		    height - 2 );
	g.setColor( OUILook.TEXT_COLOR );
	g.drawChar( composingChar, composingX + 1, getY() + height, STRING_ANCHOR );
      }
    }
    
    g.setColor( oldColor );
    g.setFont( oldFont );
    
    if ( Debugger.ON ) Logger.dev( "OTextField.paint EXITING" );
        
  } // paint


    /**
     * Gets the height of the component.
     * @return The height in pixels.
     */
  final public int getHeight()
  {
    return height;
  } // getHeight
    
  /**
   * Gets the width of the component
   * @return The width in pixels.
   */
  final public int getWidth()
  {
    if ( Debugger.ON ) Logger.dev( "OTextField.getWidth CALLED, returning " +
				width );
    return this.width;
  } // getWidth
  
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
    if ( Debugger.ON ) Logger.dev( "OTextField.allow ENTERED" );
    this.specialChars = specialChars;
    if ( Debugger.ON ) Logger.dev( "OTextField.allow EXITTING" );
  } // allow

  /**
   * Sets whether spaces are allowed in the field. The default behavior is
   * that spaces are allowed.
   *
   * @param value false if spaces are to not be allowed
   */
  public void allowSpaces( boolean value ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.allowSpaces ENTERED" );
    allowSpaces = value;
    if ( Debugger.ON ) Logger.dev( "OTextField.allowSpaces EXITTING" );
  } // allowSpaces

  /**
   * Sets the text to display in the field.
   *
   * @param value text to display in the field
   * @exception InvalidData if the text exceeds the length of the field
   * or contains invalid characters.
   */
  public void setText( String value ) 
    throws InvalidData
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.setText ENTERED" );
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
    repaint();
      
    if ( Debugger.ON ) Logger.dev( "OTextField.setText EXITTING" );
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
    if ( Debugger.ON ) Logger.dev( "OTextField.validate ENTERED" );
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
    if ( Debugger.ON ) Logger.dev( "OTextField.validate ENTERED" );
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
   * Gets the text that is in the field.
   *
   * @return The text in the field
   */
  public String getText() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.getText ENTERED" );
    return textBuff.toString();
  } // getText


    /**
   * Appends characters to the field.
   *
   * @param value Characters to append
   * @exception InvalidData If the characters would cause the field to
   * exceed its
   * maximum length, or if the any
   * character is not allowed in the field based upon
   * the field's constraints and special characters.
   */
  public void append( String value )
    throws InvalidData
  {
    if ( ( textBuff.length() + value.length() ) <= nbrOfChars )
    {
      validate( value );
      textBuff.append(value);
      cursorIdx = textBuff.length();
      repaint();
    }
    else 
    {
      OHandset.beep();
      throw new InvalidData( "Appending " + value +
			     " would exceed max nbr of chars=" + nbrOfChars );
    }
        
  } // append

  /**
   * Responds to a key press.
   * <p>
   * If the key is a number key and the constraint is {@link #NUMERIC} only,
   * no timing interval is used, the number is immediately inserted into the
   * field (if there is room).
   * If the key is a number key and the constraint IS NOT ONLY {@link #NUMERIC},
   * the timing interval begins so that
   * characters may be cycled through via repeated presses of the same key
   * within the timing interval.
   * <p>
   * Presses of the left and right sides of the 4-way navigation key move the
   * cursor one character left or right within the field. Pressing the <b>*</b>
   * key deletes the character to the left of the cursor. Pressing the <b>#</b>
   * key inserts a space at the cursor position.
   * <p>
   * You can override this method if you want a custom component to perform
   * additional filtering of key presses.  For example, if you want a field that
   * accepts only odd digits you could extend this class, ensure it is
   * {@link #NUMERIC}, then in this method check that the pressed key is an odd
   * digit before passing it to the parent's <code>keyPressed</code> method.
   * @param keyCode The code for the key that was pressed.
   */
  public void keyPressed( int keyCode ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.keyPressed ENTERED" );

    if ( keyCode >= Canvas.KEY_NUM0 && keyCode <= Canvas.KEY_NUM9 ) 
    { // one of the number keys was pressed
      if ( composer.keyPressed( keyCode ) ) repaint();
      else  OHandset.beep();
      return;
    } // if on number key
    else // not a number key
    {  
      composer.stopComposing( );
      if ( keyCode == OAbstractScreen.LEFT_KEY
	   && cursorIdx > 0 ) 
      { // they want to move back one character
	cursorIdx--;
	repaint();
	return;
      } // else  on left key

      else  if ( keyCode == OAbstractScreen.RIGHT_KEY 
		 && cursorIdx < textBuff.length() ) 
      {
	cursorIdx++;
	repaint();
	return;
      } // else  on right key

      else  if ( ( keyCode == Canvas.KEY_STAR || keyCode == 8 /* ASCII code */ )
		 && cursorIdx > 0 ) 
      { // they want to backspace/delete
	textBuff.deleteCharAt( --cursorIdx );
	repaint();
	return;
      } // else  on backspace/delete

      else  if ( keyCode == Canvas.KEY_POUND
		 && allowSpaces
		 && textBuff.length() < nbrOfChars ) 
      { // ok to insert a space
	textBuff.insert( cursorIdx++, ' ' );
	repaint();
	return;
      } // else  on pound (space) key
      else  if ( keyCode >= 32 && keyCode <= 126
		 && isValid( ( char ) keyCode ) ) 
      { // ASCII code
	insertChar( ( char ) keyCode );
	repaint();
	return;
      }

    } 

    // all successful branches complete with a return, so we only got here if an
    // invalid key was pressed
    OHandset.beep();
    if ( Debugger.ON ) Logger.dev( "OTextField.keyPressed EXITTING" );
  } // keyPressed
  
  /**
   * Called to indicate that the component has gained or lost focus.
   *
   * @param event {@link com.nextel.ui.OFocusEvent#FOCUS_GAINED} or
   * {@link com.nextel.ui.OFocusEvent#FOCUS_LOST}
   */
  public void setFocus( int event ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.setFocus ENTERED" );
    if ( event == OFocusEvent.FOCUS_LOST || composer.isComposing() ) 
    {
      composer.stopComposing( );
    }
    else  // event==FOCUS_GAINED
    {
      cursorIdx = textBuff.length();
    }
    
    super.setFocus( event );
    
    if ( Debugger.ON ) Logger.dev( "OTextField.setFocus EXITTING" );
  } // setFocus


  //// implementation of OThread //////////////////////////////
  
  /**
   * Does nothing except satisfy the {@link com.nextel.ui.OThread} interface.
   *
   */
  public void start() 
  { /* do nothing; just satisfies OThread interface */ }

  /**
   * Stops the timer for the key press interval.
   *
   */
  public void stop() 
  {
    if ( composer.isComposing() ) composer.stopComposing( );
  } // stop

  ///////// Implement OTextDocument

  /**
   * Called to indicate that the document has been changed.
   *
   */
  public void changed() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.composingCompleted ENTERED" );
    repaint();
    if ( Debugger.ON ) Logger.dev( "OTextField.composingCompleted EXITTING" );
  } // composingCompleted

  /**
   * Inserts a character into the document.
   *
   * @param value The character to insert
   */
    public void insertChar( char value ) 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.insert ENTERED" );
       textBuff.insert( cursorIdx++, value );
    if ( Debugger.ON ) Logger.dev( "OTextField.insert EXITTING" );
  } // insertChar

  /**
   * Deletes the character before the document's cursor.
   *
   */
  public void deleteChar() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.delete ENTERED" );
    textBuff.deleteCharAt( --cursorIdx );
    if ( Debugger.ON ) Logger.dev( "OTextField.delete EXITTING" );
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

    public int getMaxNbrOfChars() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.getNbrOfChars ENTERED" );
    return nbrOfChars;
  } // getMaxNbrOfChars

    public int getConstraints() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.getConstraints ENTERED" );
    return constraints;
  } // getConstraints

  public String getSpecialChars() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.getSpecialChars ENTERED" );
    return specialChars;
  } // getSpecialChars

  public boolean getAllowSpaces() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.getAllowSpaces ENTERED" );
    return allowSpaces;
  } // getAllowSpaces

    public void advanceCursor() 
  { 
    if ( Debugger.ON ) Logger.dev( "OTextField.advanceCursor ENTERED" );
    cursorIdx++;
  } // advanceCursor

} //OTextField

  
