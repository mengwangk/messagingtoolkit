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

import com.nextel.util.Logger;
import javax.microedition.rms.RecordFilter;

/**
 * A simple RecordFilter that filters out the 
 * {@link com.nextel.examples.expensepad.ExpenseRecord ExpenseRecord}
 * if the uploaded flag is true. Used in writeExternal() of the 
 * {@link com.nextel.examples.expensepad.ExpenseStore ExpenseStore}.
 * @author Ryan Wollmuth
 */
public class ExpenseRecordFilter implements RecordFilter
{
  /**
   * Creates a new <code>ExpenseRecordFilter</code> instance.
   */
  public ExpenseRecordFilter()
  {}
  
  /** 
   * Looks at a record in byte form, reads it in, and determines 
   * whether it matches or not.
   * <P>
   * If the uploaded flag in the record is
   * true, this method will return false.
   */
  public boolean matches(byte[] values)
  {
    ExpenseRecord record = null;
    try
    {
      record = new ExpenseRecord( values );
    } 
    catch ( Exception e ) 
    {
      Logger.ex( e );
      return false;
    }
    return ! record.getUploaded();
  }
}
