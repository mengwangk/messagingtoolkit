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

/**
 * Interface for any class that starts a new thread.
 * <p>
 * By implementing this interface a class may enable some automatic lifecycle
 * maintenance.  For example, when an {@link com.nextel.ui.OCompositeScreen} is used,
 * any {@link com.nextel.ui.OComponent}s that it contains and that implement
 * this interface will have their {@link #start} method called whenever that
 * {@link com.nextel.ui.OCompositeScreen} displays (i.e., whenever the screen's inherited
 * {@link javax.microedition.lcdui.Canvas#showNotify} is called by the kvm)
 * and their
 * {@link #stop} method will be
 * called whenever that {@link com.nextel.ui.OCompositeScreen}
 * is hidden (i.e., whenever the screen's inherited
 * {@link javax.microedition.lcdui.Canvas#hideNotify} is called by the kvm).
 * @author Glen Cordrey
 */
public interface OThread 
{
   /**
    * Starts the thread.
    */
   public void start();

   /**
    * Stops the thread.
    */
   public void stop();

}// OThread
