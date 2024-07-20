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
package com.nextel.examples.expensepad;

import javax.microedition.midlet.MIDlet;
import javax.microedition.midlet.MIDletStateChangeException;
import com.nextel.ui.OHandset;
import com.nextel.examples.ui.*;

/**
 * This is the main MIDlet class for the example program ExpensePad
 * This example program demonstrates the functionality of parts of all 3
 * of the toolkits ( UI, RMS, Network).
 * <P>
 * This program demonstrates how to create, update and delete a record 
 * using the UI widgets and storing it through the RMS toolkit. An example of 
 * this is demonstrated in the 
 * {@link com.nextel.examples.expensepad.ExpensePadInputScreen InputScreen}.
 * <P>
 * Using the networking toolkit, it also demonstrates how to upload and download 
 * data to a networked resource (in our case, a servlet provided in directory
 * <code>servlets/com/nextel/examples/expensepad</code>). By calling 
 * {@link com.nextel.rms.OSynchStore#upload upload()} or 
 * {@link com.nextel.rms.OSynchStore#download download()} on the ExpenseStore,
 * data is sent to or received from the servlet.
 * <p>
 * The functionality demonstrated by ExpensePad is:
 * <UL>
 *  <LI>Ability to store, retrieve, update and delete a record using the RMS.
 *  <LI>Ability to list expenses in a sorted order and display the total
 * of all the expenses.
 *  <LI>Ability to upload expenses to an external servlet.
 *  <LI>Ability to download expenses from an external servlet.
 * </UL>
 * <P>
 * A simple way to test the functionality of the ExpensePad would be to do the 
 * following:
 * <UL>
 *  <LI>Install in a J2EE application server (e.g., Tomcat, Weblogic) the
 *  servlet in <code>servlets/com/nextel/examples/expensepad/ExpenseServlet</code>
 *  <LI>Install this MIDlet on a network-aware handset (i.e., a handset with a
 *  routable IP address)
 *  <LI>Select 'New Expense' from the main menu.
 *  <LI>Enter in information for an amount, an expense type and a date.
 *  <LI>Press the 'SAVE' softkey.
 *  <LI>Press the 'OK' softkey.
 *  <LI>Select 'List All' from the main menu.
 *  <LI>The record that was created will be shown in the grid component.
 *  <LI>Press the 'SELECT' softkey.
 *  <LI>Choose the 'EDIT' pushbutton and press the 'OK' softkey.
 *  <LI>Edit the record and press the 'SAVE' softkey.
 *  <LI>Select 'Net Config' from the main menu. (It is very important that you
 *  specify the network configuration BEFORE you upload or download, or the
 *  upload and download functions will generate exceptions. No attempt is made
 *  to handle this condition for the sake of brevity in this example code, but
 *  if you were to use this MIDlet as the bases for a production application you
 *  would handle this condition gracefully, perhaps by displaying a message 
 *  <LI>Enter the upload and download URI and press the 'SAVE' softkey.
 *  <LI>Select 'Upload' from the main menu.
 *  <LI>Press the 'DONE' softkey.
 *  <LI>Select 'Delete All' from the main menu. All the records will be deleted.
 *  <LI>Select 'Download' from the main menu. 
 *  <LI>Press the 'DONE' softkey.
 *  <LI>Select 'List All' from the main menu. Your record that was uploaded
 *  previously should be shown.
 * </UL>
 * <P>
 * To see how to view the expenses through a browser, see the javadoc
 * for the <code>ExpenseServlet</code>.
 * <h2>Deploying</h2>
 * <h3>Including the Cache</h3>
 * The Expense Pad uses the default caching mechanism, discussed in
 * {@link com.nextel.rms.OAbstractStore}, when storing the expense records.
 * Because the cache class in instantiated via
 * <code>forName( className ).newInstance()</code>,
 * when you create a MIDlet suite that contains the Expense Pad you must
 * ensure that the MIDlet's jar contains the class fiel for
 * {@link com.nextel.rms.ODataCacheImpl}
 * and that the MIDlet's jad file contains the line
 * <pre><code>iDEN-Install-Class-1=com.nextel.rms.DataCacheImpl</code></pre>
 * Additionally, if you obfuscate when creating the MIDlet jar you must ensure
 * that {@link com.nextel.rms.ODataCacheImpl} is not renamed, i.e. that it
 * retains its original class name. How you do this depends upon the obfuscator
 * that you use.
 * <h3>Include .png files</h3>
 * While uploading or downloading expense data this MIDlet displays an animation
 * of a phone. The images that provide this animation are included in the source
 * code directory for this MIDlet, and are:
 * <ul>
 * <li>Phone1.png</li>
 * <li>Phone2.png</li>
 * <li>Phone3.png</li>
 * <li>Phone4.png</li>
 * </ul>
 * These files must be included in the
 * MIDlet's jar file, and at the top level of the directory tree in the jar
 * (i.e., within the code these files are referenced as
 * <code>/Phone1.png</code>, and the <code>/</code> that begins each reference
 * indicates that the files are at the root level of the directory tree in the
 * jar file).
 * <h3>Use Web JAL</h3>
 * Because this MIDlet is network-aware (the MIDlet allows expense info to be
 * up/downloaded to/from a web site) you must use <b>Web JAL</b> (available at
 * <a href="http://www.idendev.com">www.idendev.com</a>), and not JAL Lite,
 * to load this MIDlet to a handset.
 * @author Ryan Wollmuth
 */
public class ExpensePad extends MIDlet
{
  private ExpensePadMainScreen screen;

  /**
   * Creates a new <code>ExpensePad</code> instance.
   *
   */
  public ExpensePad() 
  { // give OHandset a reference to this MIDlet so we can use OHandset's
    // convenience functions
    OHandset.setMIDlet( this );
  } // ExpensePad

  /** Called when the MIDlet is first started <b>and also whenever it is resumed
   * after having been paused</b>
   */
  protected void startApp() 
    throws MIDletStateChangeException
  {
    // we want to create the screen only once, the first time the MIDlet is
    // started. 
    if ( screen == null ) screen = new ExpensePadMainScreen();

    // ScreenNavigator is a utility class that makes traversal of screens
    // easier.
    // This call will display the initial screen that is used for navigation
    // through the MIDlet
    ScreenNavigator.goForward( screen );
  } // startApp
  
  /**
   * This method satisfies the base class by implementing a do-nothing version
   * of this method.  Subclasses should override this definition if they do in
   * fact have processing that should be performed in this method.=
   *
   */
  public void pauseApp()
  { /* do nothing except satisfy parent's abstraction */ }
  
  
  /**
   * This method satisfies the base class by implementing a do-nothing version
   * of this method.  Subclasses should override this definition if they do in
   * fact have processing that should be performed in this method.=
   *
   * @param param1   {@see javax.microedition.midlet.MIDlet#destroyApp}
   * @exception javax.microedition.midlet.MIDletStateChangeException
   * {@see javax.microedition.midlet.MIDlet#destroyApp}
   */
  public void destroyApp(boolean param1) throws MIDletStateChangeException
  { /* do nothing except satisfy parent's abstraction */ }

} // ExpensePad
