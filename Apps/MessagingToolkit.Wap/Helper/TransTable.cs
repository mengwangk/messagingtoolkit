//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Globalization;

using MessagingToolkit.Wap.Log;

namespace MessagingToolkit.Wap.Helper
{
    /// <summary>
    /// This class represent a translation table that may be used to convert
    /// well-known integer values to their corresponding string representation.
    /// </summary>
    public class TransTable
    {    
        public const string ResourcePkg = "Resources";

        private static Hashtable tables = Hashtable.Synchronized(new Hashtable());
        private Hashtable mib2str;
        private Hashtable str2mib;

        private TransTable()
        {
            mib2str = Hashtable.Synchronized(new Hashtable());
            str2mib = Hashtable.Synchronized(new Hashtable());
        }

        /// <summary>
        /// Get a specific translation table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a TransTable object</returns>
        /// <throws>RuntimeException if the resource cannot be loaded </throws>
        public static TransTable GetTable(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                return null;
            }

            // If the resource is not fully qualified, locate it in the resource
            // package
            if (!table.StartsWith("/"))
            {
                table = new StringBuilder(ResourcePkg).Append(".").Append(table).ToString();
            }

            TransTable tbl = (TransTable)tables[table];

            if (tbl == null)
            {
                lock (tables.SyncRoot)
                {
                    tbl = (TransTable)tables[table];

                    if (tbl == null)
                    {
                        tbl = new TransTable();
                        tables[table] = tbl;

                        Stream input = GetEmbeddedFile(table);

                        if (input == null)
                        {
                            throw new Exception(table + ": Unable to load translation table");
                        }
                        NameValueCollection p = null;
                        try
                        {
                            try
                            {
                                p = ReadPropertiesFile(input);
                            }
                            finally
                            {
                                input.Close();
                            }
                        }
                        catch (IOException unknown)
                        {
                            throw new Exception(table + ": Unable to load translation table");
                        }

                        for (IEnumerator e = p.Keys.GetEnumerator(); e.MoveNext(); )
                        {
                            string key = (string)e.Current;
                            string val = p.Get(key).Trim();

                            try
                            {
                                int code = 0;
                                if (key.StartsWith("0x"))
                                {
                                    key = key.Replace("0x", string.Empty);
                                    code = Int32.Parse(key, NumberStyles.AllowHexSpecifier);
                                }
                                else
                                {
                                    code = Int32.Parse(key);
                                }
                                
                                tbl.mib2str[code] = val;
                                tbl.str2mib[val.ToLower()] = code;
                            }
                            catch (FormatException nfe)
                            {                               
                               Logger.LogThis(new StringBuilder("TransTable '").Append(table).Append("': Skipping non-integer key ").Append(key).ToString(), LogLevel.Warn);
                            }
                        }
                    }
                }
            }

            return tbl;
        }

        /// <summary>
        /// Lookup the string representation of a code in the translation table
        /// </summary>
        /// <param name="mib">the code to lookup</param>
        /// <returns>
        /// the string representation or null if unknown
        /// </returns>
        public virtual string Code2Str(int mib)
        {
            if (mib2str.ContainsKey(mib))
                return (string)mib2str[mib];
            else
                return null;
        }

        /// <summary>
        /// Lookup the integer representation of a code
        /// </summary>
        /// <param name="str">the code to lookup</param>
        /// <returns>
        /// the integer representation or null if unknown
        /// </returns>
        public virtual int Str2Code(string str)
        {
            if (str2mib.ContainsKey(str.ToLower()))
                return (int)str2mib[str.ToLower()];
            else
                return -1;
        }

        /// <summary>
        /// Extracts an embedded file out of a given assembly.
        /// </summary>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <returns>A stream containing the file data.</returns>
        public static Stream GetEmbeddedFile(string fileName)
        {
            string assemblyName = typeof(TransTable).Assembly.GetName().Name;
            try
            {                
                Assembly a =  Assembly.Load(assemblyName);
                Stream str = a.GetManifestResourceStream(assemblyName + "." + fileName);                
                if (str == null)
                    throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" + assemblyName + "'");
                return str;
            }
            catch (Exception e)
            {
                throw new Exception(assemblyName + ": " + e.Message);
            }
        }

        private static NameValueCollection ReadPropertiesFile(Stream stream)
        {
            NameValueCollection nvc = new NameValueCollection();
            StreamReader sr = new StreamReader(stream);
            string content = sr.ReadToEnd();
            string[] lines = content.Split(new char[]{'\r','\n'});
            foreach (string line in lines)
            {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (line.Contains('=')))
                {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    nvc.Add(key, value);
                }
            }
            sr.Close();
            return nvc;
        }


        public static XmlDocument GetEmbeddedXml(string fileName)
        {
            Stream str = GetEmbeddedFile(fileName);
            XmlTextReader tr = new XmlTextReader(str);
            XmlDocument xml = new XmlDocument();
            xml.Load(tr);
            return xml;
        }

        /*
        public static Bitmap GetEmbeddedImage(Type type, string fileName)
        {
            Stream str = GetEmbeddedFile(type, fileName);
            return new Bitmap(str);
        }
        */
      
    }
}
