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
import javax.microedition.lcdui.Image;
import javax.microedition.lcdui.Graphics;
import com.nextel.util.Debugger;
import com.nextel.util.Logger;

/**
 * Non-Focusable component used to display PNG Image Formats.
 * <p>
 *<ul>
 *<li>
 * Images aren't focusable, i.e. they can't receive key presses.
 *</li>
 *<li>
 *All images are displayed using normal video.
 *</li>
 *<li>
 *Pixel height and width are calculated based on the image size.
 *</li>
 *</ul>
 * @author Anthony Paper
 */

public class OImage extends OComponent //extends OFocusableComponent
{
    
    private Image image;
    
    /** Constructor method used to create an image component using the image name.
     * The image name should include the resource path and file name (e.g. "/resources/myImage").
     * @param imageName Contains the string value used to specify resource path and file name.
     * @throws IOException if the resource does not exist, the data cannot be loaded, or the image data cannot be decoded
     *
     */
    public OImage(String imageName) throws java.io.IOException
    {
        if ( Debugger.ON ) Logger.dev( "OImage.1 ENTERED" );
        image = Image.createImage(imageName);
        if ( Debugger.ON ) Logger.dev( "OImage.2 EXITTING" );
    } // constructor
    
    /** Constructor method used to create an image component using the specified image.
     * @param imageObject Contains the image value.
     */
    public OImage(Image imageObject)
    {
        if ( Debugger.ON ) Logger.dev( "OImage.1 ENTERED" );
        image = imageObject;
        if ( Debugger.ON ) Logger.dev( "OImage.2 EXITTING" );
    } // constructor
    
    /** Method used to paint the image.
     * @param g Contains the graphics object used to repaint the pushbutton
     */
    public void paint( Graphics g )
    {
        if ( Debugger.ON ) Logger.dev( "OImage.paint ENTERED" );
        g.drawImage(image,getX(),getY(),Graphics.TOP | Graphics.LEFT );
        if ( Debugger.ON ) Logger.dev( "OImage.paint EXITTING" );
    } // paint
    
    /** Method used to return the pixel height of the image.
     * @return Height value in pixels
     */
    public int getHeight()
    {
        if ( Debugger.ON ) Logger.dev( "OImage.getHeight ENTERED" );
        return image.getHeight();
    } // getHeight
    
    /** Method used to return the pixel width of the image.
     * @return Width value in pixels
     */
    public int getWidth()
    {
        if ( Debugger.ON ) Logger.dev( "OImage.getWidth ENTERED" );
        return image.getWidth();
    } // getWidth
    
    /**
     * Sets a new image object to be displayed for the next repainting.
     * @param imageObject 
     */
    public void setImage(Image imageObject)
    {
        image = imageObject;
    }
    
}// OImage
