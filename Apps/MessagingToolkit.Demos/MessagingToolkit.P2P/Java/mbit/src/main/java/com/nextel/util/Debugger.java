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

import javax.microedition.midlet.MIDlet;

/**
 * Provides facilities for debugging.
 * <p>
 * A number of <code>final static boolean</code> variables are defined and
 * should be used to guard debug statements, as in
 * <code>if ( Debug.ON ) do something </code>.  Then, if the code is
 * recompiled with ON set to <code>false</code>,
 * any code guarded as shown above will not
 * be included in the compiled class file, because the compiler recognizes the
 * the 'do something' is unreachable and will eliminate it. By setting all of
 * these values to <code>false</code> when compiling for deployment you will
 * reduce the size of your class files.
 *
 * @author Glen Cordrey
 */

public class Debugger
{
  /** Whether logging  facilities are enabled. Use this to guard calls to
   * {@link com.nextel.util.Logger}
   */
  public final static boolean ON = false;


  /**
   * Whether debug code other than <code>Logger</code> calls is compiled in.
   * This should be renamed if/when {@link #ON} is renamed to LOG_ON.
   */
  public final static boolean CODE_ON = false;

  /** Whether the monitoring of memory is enabled. All calls to {@link #logMem}
   * should be conditional on this variable.
   *
   */
  public static final boolean LOG_MEM = false;

  /**
   * Whether unit test code is compiled.
   */
  public static final boolean UNIT_TEST = false;
  private static StringBuffer MEM_MSGS = new StringBuffer();
  private static final long TOTAL_MEM = Runtime.getRuntime().totalMemory();
  private static int MEM_MSG_COUNTER = 1;

  /**
   * Creates a new <code>Debug</code> instance.
   *
   */
  private Debugger ()
  {
  } // constructor


    /**
     * Logs a message showing the amount of memory used and and the amount
     * available.
     * <p>
     * The message will be saved in a string buffer that can be accessed via
     * {@link #getMemMsgs}, and will also be written via
     * {@link com.nextel.util.Logger#dev}.
     * <p>
     * The message has the form:
     * <pre><code>[sequence number] u:# f:# [user-supplied message]</code></pre>
     * <b>u:</b> and <b>f:</b> are kilobytes, followed by 'K', if their sizes
     * are 1 or more kilobytes.If their size is
     * less than 1 kilobyte then the total number of bytes is shown.
     * @param message Message to append to the memory statistics.
     */
  public static void logMem( String message )
  {
    long free = Runtime.getRuntime().freeMemory();

    StringBuffer logMsg = new StringBuffer();
    logMsg.append( Integer.toString( MEM_MSG_COUNTER++ ) ).
      append( " u:" ).append( formString( TOTAL_MEM - free ) ).
      append( " f:" ).append( formString( free ) ).
      append( ' ' ).append( message ).append( '\n' );
    MEM_MSGS.insert( 0, logMsg.toString() );
    Logger.dev( logMsg.toString() );

  } // logMem

  // forms the string to use for the number of bytes in an amount. The number of
  // kilobytes followed by a 'K' is returned if <code>amount</code> is 1
  // kilobyte or more, else  the actual number of bytes is returned.
  private static String formString( long amount )
  {
    String value;
    if ( amount >= 1024 )
    {
      value = ( amount / 1024 ) + "K";
    }
    else
    {
      value = Long.toString( amount );
    }
    return value;
  } // formString

    /**
     * Gets all messages that were created via calls to
     * {@link #logMem}
     *
     * @return The messages showing memory use. Messages are separated from one
     * another via the <code>\n</code> control character.
     */
  public static StringBuffer getMemMsgs()
  {
    return MEM_MSGS;
  } // getMemMsgs

}// Debug


