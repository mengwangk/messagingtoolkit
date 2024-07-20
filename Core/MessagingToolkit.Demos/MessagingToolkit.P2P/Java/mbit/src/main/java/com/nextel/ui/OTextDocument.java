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
import com.nextel.exception.InvalidData;

/**
 * Interface for a class that accepts text.
 * @author Glen Cordrey
 */
public interface OTextDocument 
{
  /** Allows uppercase letters in the document **/
  public static final int UPPERCASE  = 1;

  /** Allows lowercase letters in the document **/
  public static final int LOWERCASE = (1<<1);

  /** Allow numbers in the document **/
  public static final int NUMERIC = (1<<2);

  /** Allow uppercase and lowercase letters, and numbers, in the document **/
  public static final int ANY = NUMERIC | LOWERCASE | UPPERCASE;

  /** All the special characters that can be input **/
  public static final String ALL_SPECIAL_CHARACTERS =
    "+-*/=<>#.?!,@&:;\"_(){}[]'%$^{}|\\~";

  /**
   * Allows entry into the document of any of the specified special characters.
   *
   * @param specialChars The special characters to allow in the document. Any
   * combination of
   * <pre> + - * / = < > # . ? ! , @ & : ; " _ ( ) { } [ ] ' % $ ^ { } | \ ~</pre>,
   * or {@link #ALL_SPECIAL_CHARACTERS}
   */
  public void allow( String specialChars );

  /**
   * Sets whether spaces are allowed in the document. The default behavior is
   * that spaces are allowed.
   *
   * @param value false if spaces are to not be allowed
   */
  public void allowSpaces( boolean value );

  /**
   * Gets whether the document allows the insertion of spaces.
   *
   * @return true if spaces can be inserted
   */
  public boolean getAllowSpaces();

  /**
   * Sets the text to display in the document.
   *
   * @param value text to display in the document
   * @exception InvalidData if the text exceeds the length of the document.
   */
  public void setText( String value ) 
    throws InvalidData;

  /**
   * Gets the text that is in the document.
   *
   * @return The text in the document
   */
  public String getText();

  /**
   * Inserts a character into the document.
   *
   * @param value The character to insert
   */
  public void insertChar( char value );

  /**
   * Deletes the character before the document's cursor.
   *
   */
  public void deleteChar();

  /**
   * Gets the number of characters in the document
   *
   * @return an <code>int</code> value
   */
  public int length();

  /**
   * Gets the maximum number of characters the document can contain.
   *
   * @return an <code>int</code> value
   */
  public int getMaxNbrOfChars();

  /**
   * Gets the constraints that apply to the document.
   *
   * @return A bitwise or of
   * {@link #UPPERCASE}, {@link #LOWERCASE}, and/or {@link #NUMERIC}
   */
  public int getConstraints();

  /**
   * Gets the special characters that the document will accept.
   *
   * @return the special characters that the document will accept
   */
  public String getSpecialChars();

  /**
   * Advances the cursor to the next character
   *
   */
  public void advanceCursor();

  /**
   * Called to indicate that the document has been changed.
   *
   */
  public void changed();
}
