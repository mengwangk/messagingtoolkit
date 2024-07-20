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
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import com.nextel.util.Logger;
import com.nextel.util.Debugger;

/**
 * Focusable component used to display pushbuttons with text labels.
 * <p><b>Example:</b><img src="doc-files/PushButton.jpg">
 * <p>
 *<ul>
 * <li>A pushbutton is displayed as text within an ellipse</li>
 *<li>
 *The currently selected pushbutton item is displayed using reverse-video.
 *</li>
 *<li>
 *All unselected pushbuttons have normal video.
 *</li>
 *<li>Multiple-line labels are not supported. You should also keep the button
 *label string short, as it needs to fit in an ellipse. This version of the
 *toolkit does not guarantee that the ellipse will not intersect the text, so
 *you need to visually verify correctness.
 *</li>
 *</ul>
 * <p>
 * When the push button has focus a string, specified by an argument to this
 * class's constructors, will be displayed above the right soft key, and
 * pressing the soft key will result in the performance of the action that was
 * previously set via {@link #setAction}.
 * <p>
 * Here is an example of code that places a push button on a screen.  This
 * code would typically be within an instance of
 * {@link com.nextel.ui.OCompositeScreen}. When the push button is displayed
 * it has the label "Edit Record".  When the push button has
 * focus the word "EDIT" appears above the right soft key.  Pushing the right
 * soft key while the push button has focus results in the execution of the
 * code within <code>editAction.performAction</code>
 * <pre><code>
 *  private OCommandAction editAction =
 *    new OCommandAction ()
 *    {
 *	  public void performAction()
 *	 { // create screen for editing the record
 *	 }
 *    };
 *  private OPushButton editButton =
 *     new OPushButton( "Edit Record", myFont, "EDIT" );
 *  ...
 *  editButton.setAction( editAction );
 *  add( editButton, ...
 * </code></pre>
 * @author Anthony Paper
 */
public class OPushButton extends OFocusableComponent
{
  // extra pixels in height and width to try to prevent ellipse from
  // intersecting text
  private final static int ELLIPSE_OVERAGE = 2;

  private String [] labels;
  private Font   labelFont;
  //private Image  image;
  private int labelWidth;
  private int labelHeight;
  private int maxHeight;
  private int maxWidth;
  private int rowDelta = 2;
  private int colDelta = 2;

  private OSoftKey pushKey;

  private void adjust(int w,int h)
  {
    // Set max height and width based on current labels
    this.maxHeight = labelHeight + 4;
    this.maxWidth  = labelWidth  + 4;

    // Reset max height and width if it fits
    int deltaHeight = (h - labelHeight);
    int deltaWidth  = (w - labelWidth);
    if ((h > maxHeight) && (deltaHeight > 2)){
      maxHeight = h;
      rowDelta = (deltaHeight/ 2);
    }
    maxHeight += ELLIPSE_OVERAGE;
    if ((w > maxWidth) && (deltaWidth > 2)) {
      maxWidth = w;
      colDelta = (deltaWidth/2);
    }
    maxWidth += ELLIPSE_OVERAGE;
  }

  private void init(String myLabel, Font myFont, int w, int h,
                    String rOSoftKeyLabel )
  {
    this.pushKey = new OSoftKey( rOSoftKeyLabel );
    this.labelFont = myFont;
    this.labelWidth = 0;

    // Only single-line labels are allowed for now so that they fit correctly
    // into the ellipse drawn for the pushbutton. However, leve in the code for
    // multi-line labels so we can revisit later and see if we can make it work
    //this.labels = StringUtils.breakIntoLines(myLabel);
    this.labels = new String [1];
    this.labels[0] = myLabel;

    for ( int idx=0; idx < labels.length; idx++ )
    {
      int stringWidth = this.labelFont.stringWidth( labels[ idx ] );
      if ( stringWidth > this.labelWidth ) this.labelWidth = stringWidth;
    }
    // Set default height and width and adjust if it fits
    this.labelHeight = (labels.length * labelFont.getHeight());
    adjust(w,h);
  }

  /*
    private void init(Image myImage,int w, int h, String rOSoftKeyLabel )
    {
    pushKey = new OSoftKey( rOSoftKeyLabel );
    this.image = myImage;
    this.labelWidth = image.getWidth()+4;
    this.labelHeight = image.getHeight()+4;
    // Set default height and width and adjust if it fits
    adjust(w,h);
    }
  */

  /** Constructor method used to create a push button using the specified label and font.  The label string may use "\n" characters to seperate lines.
   * @param myLabel Contains the string value used for the pushbutton label.  Use "\n" character to seperate lines.
   * @param myLabelFont Contains the Font used to format and display the pushbutton label string.
   * @param rOSoftKeyLabel String that will be displayed above the right soft key
   * when the button has focus
   */
  public OPushButton ( String myLabel, Font myLabelFont, String rOSoftKeyLabel )
  {
    init(myLabel,myLabelFont,0,0, rOSoftKeyLabel );
  } // constructor

  /** Constructor method used to create a push button using the specified label, font, width, and height.  The label string may use "\n" characters to seperate lines.  The height and width parameters shall be used to generate the pixel size of the pushbutton.
   * @param w Contains the int value used to specify the pixel width of the pushbutton.
   * @param h Contains the int value used to specify the pixel height of the pushbutton.
   * @param myLabel Contains the string value used for the pushbutton label.  Use "\n" character to seperate lines.
   * @param myLabelFont Font used to format and display the pushbutton label string.
   * @param rOSoftKeyLabel String that will be displayed above the right soft key
   * when the button has focus
   */
  public OPushButton(String myLabel, Font myLabelFont, int w, int h,
                     String rOSoftKeyLabel)
  {
    init(myLabel,myLabelFont,w,h, rOSoftKeyLabel );
  }


  /** Constructor method used to create a push button using the specified image.
   * @param myImage Contains the PNG image value used for the pushbutton.
   * @param rOSoftKeyLabel String that will be displayed above the right soft key
   * when the button has focus
   */
  /*
    public OPushButton(Image myImage, String rOSoftKeyLabel)
    {
    init(myImage,0,0, rOSoftKeyLabel );
    }
  */

  /** Constructor method used to create a push button using the specified image, width, and height.  The height and width parameters shall be used to generate the pixel size of the pushbutton.
   * @param myImage Contains the PNG image value used for the pushbutton.
   * @param w Contains the int value used to specify the pixel width of the pushbutton.
   * @param h Contains the int value used to specify the pixel height of the pushbutton.
   * @param rOSoftKeyLabel String that will be displayed above the right soft key
   * when the button has focus
   */
  /*
    public OPushButton(Image myImage, int w, int h, String rOSoftKeyLabel )
    {
    init(myImage,w,h, rOSoftKeyLabel );
    }
  */

  /** Method used to return the pixel height of the pushbutton.
   * @return Height value in pixels
   */
  public int getHeight()
  {
    if ( Debugger.ON ) Logger.dev( "OPushButton.getHeight CALLED" );
    return (maxHeight + 1 + ELLIPSE_OVERAGE);
  } // getHeight

  /** Method used to return the pixel width of the pushbutton.
   * @return Width value in pixels
   */
  public int getWidth()
  {
    if ( Debugger.ON ) Logger.dev( "OPushButton.getWidth CALLED" );
    return (maxWidth+1 + ELLIPSE_OVERAGE);
  } // getWidth

  /** Method used to paint the pushbutton.
   * @param g Contains the graphics object used to repaint the pushbutton
   */
  public void paint( Graphics g)
  {
    if ( Debugger.ON ) Logger.dev( "OPushButton.paint ENTERED" );

    Font oldFont = g.getFont();
    int oldColor = g.getColor();

    int backgroundColor = OUILook.BACKGROUND_COLOR;
    int foregroundColor = OUILook.TEXT_COLOR;
    if ( hasFocus() )
    { // fill the background if we have focus
      backgroundColor = SELECTED_BACKGROUND_W_FOCUS;
      foregroundColor = SELECTED_FOREGROUND;
    }

    // draw the label background and set highlight area for focus
    //
    // NOTE1: AMP/29MAY01 
    // Changed getX() and getY() to getX()-1 and getY()-1 
    // to support the widget's deployment to i85s/i50sx.
    //
    // NOTE2: AMP/30MAY01 
    // Changed 2 to 5, to adjust more rounded buttons
    //
    g.setColor( backgroundColor );
    //g.fillRoundRect(getX(),getY(), maxWidth, maxHeight,7,7);
    g.fillArc(getX() - 1,getY() - 1, maxWidth, maxHeight,0,360);

    // draw the label text
    g.setColor( foregroundColor );
    g.setFont( labelFont );

    if (labels != null) {
      int myY = getY();
      for ( int idx=0; idx < labels.length; idx++ )
      {
    g.drawString(labels[idx],colDelta+getX(),rowDelta+myY,
             Graphics.TOP | Graphics.LEFT );
    myY += labelFont.getHeight();
      }
    }
    /*
      if (image != null) {
      g.drawImage(image,getX()+colDelta+2,rowDelta+getY()+2,Graphics.TOP | Graphics.LEFT );
      }
    */

    // draw the outlined rounded rectangular
    if (hasFocus()== false) {
      g.setColor( OUILook.TEXT_COLOR );
      //
      // NOTE1: AMP/29MAY01 
      // Changed getX() and getY() to getX()-1 and getY()-1 
      // to support the widget's deployment to i85s/i50sx.
      // Also changed so that the outlined rounded border
      // is ONLY displayed when field is NOT focused
      //
      // NOTE2: AMP/30MAY01 
      // Changed 2 to 5, to adjust more rounded buttons
      // Changed maxWidth to maxWidth + 1 to account for
      // the extra pixels
      //
      //
      g.setColor( OUILook.TEXT_COLOR );
      //g.drawRoundRect(getX()-1,getY()-1, maxWidth, maxHeight,7,7);
      g.drawArc(getX()-2,getY()-2, maxWidth, maxHeight,0,360);
    }


    // reset to original values
    g.setFont( oldFont );
    g.setColor( oldColor );
    if ( Debugger.ON ) Logger.dev( "OPushButton.paint EXITTING" );
  } // paint

  /**
   * Set the focus based on the value passed.
   *
   * @param id {@link com.nextel.ui.OFocusEvent#FOCUS_GAINED} or
   * {@link com.nextel.ui.OFocusEvent#FOCUS_LOST}
   */
  public void setFocus( int id)
  {
    if ( Debugger.ON ) Logger.dev( "OPushButton.setFocus ENTERED" );
    super.setFocus( id );
    if ( id == OFocusEvent.FOCUS_GAINED )
    {
      getContainer().getScreen().addSoftKey( pushKey, Graphics.RIGHT  );
    }
    else if ( id == OFocusEvent.FOCUS_LOST )
    {
      {
    getContainer().getScreen().removeSoftKey( Graphics.RIGHT );
      }
    }

    if ( Debugger.ON ) Logger.dev( "OPushButton.setFocus EXITTING" );
  } // setFocus

  /** Method is not currently implemented but is required from the parent class.
   * @param g Parameter is not currently used.
   * @param x Parameter is not currently used.
   * @param y Parameter is not currently used.
   * @param width Parameter is not currently used.
   * @param height Parameter is not currently used.
   */
  protected void paintBox( Graphics g, int x, int y, int width,
                           int height )
  {
    if ( Debugger.ON ) Logger.dev( "OPushButton.paintBox ENTERED" );
    // we don't need a perimeter for radio buttons
    if ( Debugger.ON ) Logger.dev( "OPushButton.paintBox EXITTING" );
  } // paintBox

  /**
   * This method does nothing, but is supplied because it is declared abstract
   * by the parent component.
   * <p>
   * The only key press that applies to this component
   * is a press of the right soft key while this component has focus, and that
   * press is handled by any
   * {@link com.nextel.ui.OCommandAction OCommandAction} that was
   * passed to {@link #setAction}. If the action was never set then nothing will
   * happen.
   * <p>
   * This approach was taken because push buttons are usually used for actions
   * that apply to the entire screen, in which case it is the screen, and not
   * this component, that is best positioned to take the action.  An example
   * of this is shown in {@link #setAction}.
   * @param keyCode  The code for the key that was pressed.
   */
  protected void keyPressed( int keyCode )
  {
    if ( Debugger.ON ) Logger.dev( "OPushButton.keyPressed ENTERED" );
    // do nothing; processing is handled by the container to which the button
    // was added
    if ( Debugger.ON ) Logger.dev( "OPushButton.keyPressed EXITTING" );
  } // keyPressed

  /**
   * Sets an action that is to be performed when the right soft key is pressed
   * while this pushbutton has focus.
   * <p>
   * Here is an example of code that places a push button on a screen.  This
   * code would typically be within an instance of
   * {@link com.nextel.ui.OCompositeScreen}. When the push button is displayed the
   * it reads "Edit Record".  When the push button has
   * focus the word "EDIT" appears above the right soft key.  Pushing the right
   * soft key while the push button has focus results in the execution of the
   * code within <code>editAction.performAction</code>
   * <pre><code>
   *  private OCommandAction editAction =
   *    new OCommandAction ()
   *    {
   *	  public void performAction()
   *	 { // create screen for editing the record
   *	 }
   *    };
   *  private OPushButton editButton =
   *     new OPushButton( "Edit Record", myFont, "EDIT" );
   *  ...
   *  editButton.setAction( editAction );
   *  add( editButton, ...
   *
   * </code></pre>
   *
   * @param action The action to be performed then the right soft key is pressed
   * while this pushbutton has focus.
   */
  public void setAction( OCommandAction action )
  {
    if ( Debugger.ON ) Logger.dev( "OPushButton.setAction ENTERED" );
    pushKey.setAction( action );
    if ( Debugger.ON ) Logger.dev( "OPushButton.setAction EXITTING" );
  } // setAction


}// OPushButton
