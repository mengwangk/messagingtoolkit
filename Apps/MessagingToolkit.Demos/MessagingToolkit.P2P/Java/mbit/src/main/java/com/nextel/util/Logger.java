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
import java.util.Calendar;

/**
 * Sends messages to the system output stream.
 * <p>
 * All methods are guarded by {@link com.nextel.util.Debugger#ON}; if
 * it's value is false then all methods simply return without doing anything.
 * However, to save the size and performance cost of calls to this method
 * callers should also guard those calls with {@link com.nextel.util.Debugger#ON}, as
 * in
 * <pre><code>
 *  if ( Debug.ON ) Logger.dev( "Some message" );
 * </code></pre>
 *
 * @author Glen Cordrey
 */
public class Logger
{
  // Delimiter between message fields
  private final static char DELIMITER = ' ';

  // Indicators included in the message to indicate the severity.
  private final static String EXCEPTION = "X";
  private final static String DEVELOPMENT = "D";

  /**
   * Creates a new <code>Logger</code> instance.
   *
   */
  private Logger ()
  {
  } // constructor

  /**
   * Logs an exception and its stack trace.
   *
   * @param ex The exception
   */
  public final static void  ex ( Throwable ex )
  {
    if ( ! Debugger.ON ) return;
    logMessage( ex.toString(), EXCEPTION );
    ex.printStackTrace();
  } // exception

  /**
   * Logs an exception and its stack trace.
   *
   * @param ex Exception to log
   * @param message Additional message to log
   */
  public final static void  ex ( Throwable ex, String message )
  {
    if ( ! Debugger.ON ) return;
    StringBuffer messageBuff = new StringBuffer();
    messageBuff.append( ex.getMessage() ).
      append( " : " ).
      append( message );

    logMessage( messageBuff.toString(), EXCEPTION );
    ex.printStackTrace();

  } // exception


  /**
   * Logs a Development message.
   *
   * @param message The message to log.
   */
  public final static void  dev ( String message )
  {
    logMessage( message, DEVELOPMENT );
  } // development

  /**
   * Logs the message, adding the timestamp and severity indicator.
   *
   * @param message Message to log.
   * @param severity  Message severity.
   */
  protected final static void logMessage ( String message, String severity )
  {
    Calendar now = Calendar.getInstance();
    StringBuffer textBuffer = new StringBuffer();
    textBuffer.append( now.get( Calendar.HOUR ) ).append( ':' ).
      append( now.get( Calendar.MINUTE ) ).append( ':' ).
      append( now.get( Calendar.SECOND ) ).append( ':' ).
      append( now.get( Calendar.MILLISECOND ) ).
      append( DELIMITER ).
      append( '[' ).
      append( severity ).
      append( ']' ).
      append( DELIMITER ).
      append( message );
    System.out.println( textBuffer.toString() );

  } // logMessage

}// Logger
