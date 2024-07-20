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
package com.nextel.examples.rms;

import com.nextel.rms.OAbstractRecord;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;

/**
 * A simple example of a "record" in the RMS toolkit
 * <p/>
 * This record contains 2 fields: A String ( name ) and an int ( salary ).
 */
public class SalaryRecord extends OAbstractRecord {
    // Our record definition: name and salary.
    private String name;
    private String salary;

    /**
     * Creates a <code>SalaryRecord</code> instance.
     */
    public SalaryRecord() {
    }

    /**
     * Creates a <code>SalaryRecord</code> instance.
     *
     * @param byteArray The array to populate the record with.
     */
    public SalaryRecord(byte[] byteArray) throws IOException {
        super(byteArray);
    }

    /**
     * Setter for the name
     *
     * @param name The person's name.
     */
    public void setName(String name) {
        this.name = name;
    }

    /**
     * Getter for the name
     *
     * @return The name for this record ( null if not set ).
     */
    public String getName() {
        return name;
    }

    /**
     * Setter for the salary.
     *
     * @param salary The amount of the person's salary.
     */
    public void setSalary(String salary) {
        this.salary = salary;
    }

    /**
     * Getter for the salary.
     *
     * @return The salary for this record ( null if not set ).
     */
    public String getSalary() {
        return salary;
    }

    /**
     * Writes the current record to the dataStream.
     *
     * @param dataStream The stream you wish to write the record to.
     */
    protected void writeStream(DataOutputStream dataStream) {
        try {
            dataStream.writeUTF(name);
            dataStream.writeUTF(salary);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Populates the current record from the inputStream.
     *
     * @param inputStream The stream that contains record information
     * @throws IOException If there is a problem with the inputStream.
     */
    protected void populate(DataInputStream inputStream)
            throws IOException {
        name = inputStream.readUTF();
        salary = inputStream.readUTF();
    }
}
