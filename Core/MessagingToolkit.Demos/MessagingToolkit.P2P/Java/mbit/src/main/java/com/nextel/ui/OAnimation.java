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


package com.nextel.ui;

import javax.microedition.lcdui.*;
import java.util.Timer;
import java.util.TimerTask;
import java.io.IOException;
import com.nextel.util.Debugger;

/**
 *  This class provides animation by cycling through a set of images at a
 *  specified rate.
 * <p>
 * Because this class implements {@link com.nextel.ui.OThread} the animation
 * thread is
 * automatically started and stopped whenever the {@link com.nextel.ui.OCompositeScreen}
 * containing the animation is displayed or hidden.
*/
public class OAnimation extends OComponent implements OThread
{
  private Image[] imageList;

  /** Timer service to schedule a timer task, and to execute the timer task periodically. */
  private Timer timerService;
  /** Timer task */
  private TimerTask timerClient;

  /** Index of currently displaying image. */
  private int imageIndex;

  /** Tells whether the HourGlass has been activated or not.  */
  boolean isActivated;

  // height and width of the component. Since multiple images are involved, the
  // component's height and width are the maximum of the height and width of
  // all images
  private int height, width;

  // the duration, in milliseconds, that each image is displayed
  private long imageDuration;

  /**
   * This class gets scheduled to be executed by Timer when the given period
   * in seconds gets expired.
   * The usage is: <br>
   * <pre>
   * <code>
   *            timerClient = new TimerClient();
   *            timerService = new Timer();
   *            timerService.schedule(timerClient, delay);
   * </code>
   * </pre>
   * <br>
   *
   * The timerClient and timerService both need to be reinstanciated to
   * launch new Timer.
   *
   *
   */
  class TimerClient extends TimerTask
  {
    public final void run()
    {
      imageIndex++;
      if (imageIndex == imageList.length) imageIndex = 0;
      repaint();
    }
  } // end of TimerClient class definition


    /**
     * Creates a new <code>OAnimation</code> instance.
     * @param images An array of images to display. The array is treated as
     * circular. 
     * @param imageDuration The duration, in milliseconds, that each image is
     * displayed.
     */
  public OAnimation( Image[] images, long imageDuration )
  {
    imageList = images;
    init( imageDuration );
  } // OAnimation

  /**
   * Creates a new <code>OAnimation</code> instance.
   * @param imageNames The names of .png files containing the images that
   * provide the animation. The array is treated as circular.
   * These names should include the paths to the .png
   * files within the MIDlet jar. For example, if in the jar an image is in
   * resources/someImage.png, <code>imageNames</code> should contain
   * "<code>/resources/someImage.png</code>".
   * @param imageDuration The duration, in milliseconds, that each image is
   * displayed.
   * @exception IOException An error occured accessing one of the files specified
   * in <code>imageNames</code>. The
   * exception's message will contain the name of the missing file.
   */
  public OAnimation( String[] imageNames, long imageDuration )
    throws IOException
  {
    int length = imageNames.length;
    if (imageNames != null && length > 0)
    {
      imageList = new Image[length];

      int imageIdx = 0;
      try {
    for ( ; imageIdx < length; imageIdx++)
    {
      imageList[imageIdx] = Image.createImage(imageNames[imageIdx]);
    }

      }
      catch( IOException ex) {
    // just rethrow it with the name of the file it failed on
    throw new IOException( ex.getMessage() + " : " +
                   imageNames[ imageIdx ] );
      }

      init( imageDuration );
    }
    else
      imageList = null;
  } // OAnimation


    /**
     * Initializes the object.
     */
  private void init( long imageDuration )
  {
    this.imageDuration = imageDuration;
    imageIndex = 0;
    isActivated = false;
    for ( int idx=0; idx < imageList.length; idx++ )
    {
      width = Math.max( width, imageList[ idx ].getWidth() );
      height = Math.max( height, imageList[ idx ].getHeight() );
    }

  } // init


    /**
     * Method used to paint the image.
     * @param g Contains the graphics object to draw the Screen.
     */
  public void paint( Graphics g )
  {
    // first refresh the background color, which ensures proper display if the
    // images are not rectangular
    int oldColor = g.getColor();
    g.setColor( OUILook.BACKGROUND_COLOR );
    g.fillRect( getX(), getY(), getWidth(), getHeight() );
    g.setColor( oldColor );

    g.drawImage(imageList[imageIndex], getX(), getY(),
        Graphics.TOP | Graphics.LEFT);
  } // paint

    /**
     * Returns the pixel height of the component. Since the component uses
     * multiple images, the maximum height of all images is returned.
     * @return Height value in pixels
     */
  public int getHeight()
  {
    return height;
  } // getHeight

    /**
     * Returns the pixel width of the image. Since the component uses multiple
     * images, the maximum width of all the images is returned.
     * @return Width value in pixels
     */
  public int getWidth()
  {
    return width;
  } // getWidth

    /**
     * Starts the animation.
     */
  public void start()
  {
    if (isActivated) return; // already activated.
    isActivated = true;

    initTask();

    try {
      timerClient = new TimerClient();
      timerService = new Timer();
      timerService.schedule(timerClient, imageDuration, imageDuration);
    }
    catch(IllegalStateException ex) {
      if (Debugger.ON) ex.printStackTrace();
      initTask();
    }
  }

  /**
   * Stops the animation.
   */
  public void stop()
  {
    if ( !isActivated ) return; // already unactivated.        
    isActivated = false;
    initTask();
  }

  /**
   *  Initialize the timer task and timer service.
   */
  private void initTask()
  {
    if (timerClient != null)
    {
      timerClient.cancel();
      timerClient = null;
    }

    if (timerService != null)
    {
      timerService.cancel();
      timerService = null;
    }

    imageIndex = 0;
  }
}


