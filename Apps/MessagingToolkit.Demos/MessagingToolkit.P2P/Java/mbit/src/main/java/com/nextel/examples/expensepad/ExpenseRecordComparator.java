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

import javax.microedition.rms.RecordComparator;
import java.io.IOException;

import com.nextel.util.Logger;

/**
 * A simple record comparator that sorts on the ExpenseRecord's
 * date field (stored as a long).
 * @author Ryan Wollmuth
 */
public class ExpenseRecordComparator implements RecordComparator
{
  private ExpenseRecord record1;
  private ExpenseRecord record2;
  
  /** 
   * Creates a new <code>ExpenseRecordComparator</code> instance.
   */
  public ExpenseRecordComparator()
  {}
  
  /** 
   * Compares 2 records in byte[] format and determines
   * whether the first record precedes, follows or is
   * equivalent to the second record.
   */
  public int compare(byte[] values, byte[] values1)
  {
    try
    {
      record1 = new ExpenseRecord( values );
      record2 = new ExpenseRecord( values1 );
    } catch ( IOException io ) {
      Logger.ex( io );
    }
    
    if ( record1.getRawDate() > record2.getRawDate() )
      return RecordComparator.FOLLOWS;
    else if ( record1.getRawDate() < record2.getRawDate() )
      return RecordComparator.PRECEDES;
    else if ( record1.getRawDate() == record2.getRawDate() )
      return RecordComparator.EQUIVALENT;
    // Should never get to this point...
    return RecordComparator.PRECEDES;
  }
  
}
