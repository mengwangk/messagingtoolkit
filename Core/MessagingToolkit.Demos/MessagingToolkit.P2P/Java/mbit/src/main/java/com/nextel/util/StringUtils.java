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
import java.util.Vector;
import javax.microedition.lcdui.*;

/**
 * Utilities for manipulating strings.
 *
 * @author Glen Cordrey
 */
public class StringUtils
{
  public final static char HUNDREDS_DELIMITER = ',';

  /**
   * The number of digits in a hundred. Used for inserting comma delimiters
   */
  private final static int DIGITS_IN_HUNDREDS = 3;

  /**
   * Creates a new <code>StringUtils</code> instance.
   *
   */
  private StringUtils ()
  { /* all methods are static */ } // constructor


  /**
   * Breaks a string into lines no longer than the number of pixels available.
   * Line breaks are placed at word boundaries; if a string contains no spaces
   * and is wider than the
   * available pixels the break will occur within the string.
   *
   * @param text The text to break into lines
   * @param textFont The font used for displaying the text
   * @param availablePixels The number of pixels available width-wise. 
   * @return Array of strings, each string is a line.
   */
  public static String []  breakIntoLines ( String text, Font textFont,
                                            int availablePixels )
  {
    if ( Debugger.ON ) Logger.dev( "StringUtil.breakIntoLines.1 ENTERED w/text= " +
                text + " textFont=" + textFont +
                " availablePixels=" + availablePixels );
    Vector lines = new Vector(); // temp storage of the line
    String nextLine = null; // the next line to be stored
    WordParser parser = new WordParser( text );
    String word  = parser.nextWord();
    StringBuffer candidateLineBuff = new StringBuffer(); // candidate next line
    String candidateLine = null;
    while ( word != null )
    {
      if ( candidateLineBuff.length() > 0 )
      { // there's already a line in the buffer, so insert a space
    candidateLineBuff.append( ' ' );
      }
      candidateLineBuff.append( word );

      // see if the line will fit in the available space
      candidateLine = candidateLineBuff.toString();
      if ( textFont.stringWidth( candidateLine ) <= availablePixels )
      { // it will fit, so prepare for the next word
    nextLine = candidateLine;
    word = parser.nextWord();
      }
      else  // won't fit, so save line and start a new one
      {
    if ( nextLine != null && nextLine.length() > 0 )
    { // the line isn't empty, so save it and start next line
      lines.addElement( nextLine.trim() );
      nextLine = null;
      candidateLineBuff.delete( 0, candidateLineBuff.length() );
     }
    else  // the next line is empty and the word is longer than a line
    { // prepare to truncate the word
      boolean tooLong = true;
      int truncateAt = word.length();
      String line = null;
      while ( tooLong )
      { // truncate characters until it fits
        line = word.substring( 0, --truncateAt );
        if ( textFont.stringWidth( line ) <= availablePixels )
        { // this fragment will fit
          tooLong = false;
        }
      } // finished truncating

      lines.addElement( line );
      candidateLineBuff.delete( 0, candidateLineBuff.length() );
      word = word.substring( truncateAt ); // start with remaining fragment
    }
      }
    } // while

    if ( candidateLine != null && candidateLine.length() > 0 )
    { // add the last line
      lines.addElement( candidateLine );
    }
    String [] returnValue = null;
    if ( lines.size() > 0 )
    { // make an array from the vector of lines
      returnValue = new String[ lines.size() ];
      lines.copyInto( returnValue );
    }

    if ( Debugger.ON ) Logger.dev( "StringUtil.breakIntoLines.1 EXITTING w/lines=" +
                returnValue );

    return returnValue;
  } // breakIntoLines


  /**
   * Breaks a string into lines at embeddd new-line characters (\n).
   *
   * @param text The text to break into lines
   * @return Each array element is a line.
   */
  public static String [] breakIntoLines( String text )
  {
    if ( Debugger.ON ) Logger.dev( "StringUtils.breakIntoLines ENTERED" );

    Vector linesVector = new Vector();

    int beginSubstring = 0;
    int endSubstring = -1;
    int lineIdx = 0;
    while ( ( endSubstring = text.indexOf( '\n', beginSubstring ) ) > -1 )
    {
      linesVector.addElement( text.substring( beginSubstring, endSubstring ) );
      beginSubstring = endSubstring + 1;
      lineIdx++;
    }

    String [] lines;
    if ( lineIdx > 0 )
    { // embedded new line character(s) were found, so process the last line
      linesVector.addElement( text.substring( beginSubstring ) );
      lines = new String[ linesVector.size() ];
      linesVector.copyInto( lines );
    }
    else  // only a single line
    {
      lines = new String[ 1 ];
      lines[ 0 ] = text;
    }
    if ( Debugger.ON ) Logger.dev( "StringUtils.breakIntoLines EXITTING" );

    return lines;
  } // breakIntoLines

  /**
   * Breaks a string into lines no longer than the number of pixels available.
   * Line breaks are placed at word boundaries; if a string contains no spaces
   * and is wider than the
   * available pixels the break will occur within the string.
   *
   * @param textLines An array of strings.
   * @param textFont The font used for displaying the text
   * @param availablePixels The number of pixels available width-wise. 
   * @return Array of strings, each string is a line.
   */
  public static String []  breakIntoLines( String [] textLines,
                                           Font textFont, int availablePixels )
  {
    if ( Debugger.ON ) Logger.dev( "StringUtils.breakIntoLines ENTERED" );
    Vector linesVector = new Vector( textLines.length );
    for ( int idx = 0; idx < textLines.length; idx++ )
    { // break each line by width
      String [] lines =
    breakIntoLines( textLines[ idx ], textFont, availablePixels );
      if ( lines != null)
      {
    for ( int newIdx = 0; newIdx < lines.length; newIdx++ )
    {
      linesVector.addElement( lines[ newIdx ] );
    }
      }

    }
    String [] returnValue = new String[ linesVector.size() ];
    linesVector.copyInto( returnValue );

    if ( Debugger.ON ) Logger.dev( "StringUtils.breakIntoLines EXITTING" );

    return returnValue;
  } // breakIntoLines

  /**
   * Returns true if a text string is nothing, i.e. empty, all blanks, or
   * all 0's, as in, "", "  ", "0", "00", etc.
   *
   * @param text Text to check
   * @return true if the text is nothing
   */
  public static boolean isNothing ( String text )
  {
    if ( Debugger.ON ) Logger.dev( "StringUtils.isNothing ENTERED w/text " + text );
    if ( text == null )
    {
      return true;
    }

    boolean returnValue = ( text.trim().length() <= 0 ||
                Integer.parseInt( text ) <= 0 );
    if ( Debugger.ON ) Logger.dev( "StringUtils.isNothing returning " +
                returnValue );
    return returnValue;
  } // isNothing

    /**
     * Delimits the hundreds in a number by inserting commas.
     * @param buff Text in which commas are to be inserted.
     */
  public static void delimitHundreds( StringBuffer buff )
  {
    if (buff == null) return;

    if (buff.toString().indexOf( HUNDREDS_DELIMITER ) >= 0)
      remove( buff, HUNDREDS_DELIMITER );

    if ( buff.length() > DIGITS_IN_HUNDREDS )
    {
      // comma(s) need to be inserted
      for ( int i = buff.length() - DIGITS_IN_HUNDREDS;
            i > 0; i -= DIGITS_IN_HUNDREDS )
      {
          buff.insert( i, HUNDREDS_DELIMITER );
      }
    }
  } // delimitHundreds


    /** Removes all of the given character from the given StringBuffer.
     * @param buff A StringBuffer to be operated on.  The change in this buffer
     * will be persistent after this function call.
     * @param ch A character to be repeatly deleated from the giving
     * StringBuffer.
     */
  public static void remove( StringBuffer buff, char ch )
  {
    for (int i=0; i < buff.length(); i++)
    {
      if (buff.charAt(i) == ch)  buff.deleteCharAt(i);
    }
  }

  /**
   * Given a string and a delimiter, returns a Vector of Strings
   * broken up by the delimiter.
   * Note that the delimiter is not removed.
   * Returns an empty Vector if the delimiter is not found.
   * @param parseString String to parse
   * @param delimiter The delimiter
   * @return A Vector of Strings broken up by the delimiter.
   */
  public static Vector tokenize( String parseString, String delimiter )
  {
    Vector v = new Vector();
    int pos = 0;
    while ( parseString.indexOf( delimiter, pos ) != -1 )
    {
      int curPos = parseString.indexOf( delimiter, pos );
      v.addElement( parseString.substring( pos, curPos + delimiter.length() ) );
      pos = curPos + delimiter.length();
    }
    return v;
  }

}// StringUtils




