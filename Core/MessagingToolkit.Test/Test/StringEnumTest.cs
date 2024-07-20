﻿//===============================================================================
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
using System.Linq;
using System.Text;

using NUnit.Framework;
using MessagingToolkit.Core;

namespace MessagingToolkit.Core.Test
{
    #region Class StringEnumTest

    /// <summary>
    /// StringEnum Tests (Static and instance tests).
    /// </summary>
    [TestFixture]
    internal class StringEnumTest
    {

        #region Enums

        enum EnumWithStrings
        {
            [StringValue("First Value")]
            Quick,
            [StringValue("Second Value")]
            Brown = 1,
            [StringValue("Third Value")]
            Fox = 8,
            [StringValue("Fourth Value")]
            Lazy,
            [StringValue("Fifth Value")]
            Dog = 7
        }

        enum IdenticalEnumWithDifferentStrings
        {
            [StringValue("1st Value")]
            Quick,
            [StringValue("2nd Value")]
            Brown = 1,
            [StringValue("3rd Value")]
            Fox = 8,
            [StringValue("4th Value")]
            Lazy,
            [StringValue("5th Value")]
            Dog = 7
        }

        enum EnumWithoutStrings
        {
            She = 0,
            Sells = 1,
            Seashells = 4,
            Seashore = 5
        }

        enum EnumPartialStrings
        {
            Jumping,
            [StringValue("Jack be nimble")]
            Jack,
            Frost
        }

        #endregion

        #region Private Fields

        #endregion

        #region Constructor

        public StringEnumTest()
        {
            //TODO: Enter Test Fixture construction implementation
        }

        #endregion

        #region Setup / TearDown

        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void TearDown()
        {

        }

        #endregion

        #region Static Tests

        /// <summary>
        /// Tests GetStringValue (Static implementation)
        /// </summary>
        [Test]
        public void TestStaticGetStringValue()
        {
            //Expect to retrieve a string value
            Assert.AreEqual("Third Value", StringEnum.GetStringValue(EnumWithStrings.Fox));
            //No string value to retrieve
            Assert.IsNull(StringEnum.GetStringValue(EnumWithoutStrings.Seashells));
            //String values exist but not for this enum value
            Assert.IsNull(StringEnum.GetStringValue(EnumPartialStrings.Frost));

        }

        /// <summary>
        /// Tests GetStringValue caching (Static implementation)
        /// </summary>
        [Test]
        public void TestStaticGetStringValueCaching()
        {
            //Expect to retrieve a string value (and cache this value)
            Assert.AreEqual("Third Value", StringEnum.GetStringValue(EnumWithStrings.Fox));

            //Expect to retrieve a different value (as this is from a different enum)
            Assert.AreEqual("3rd Value", StringEnum.GetStringValue(IdenticalEnumWithDifferentStrings.Fox));

            //Expect to retrieve both values again (cached)
            Assert.AreEqual("Third Value", StringEnum.GetStringValue(EnumWithStrings.Fox));
            Assert.AreEqual("3rd Value", StringEnum.GetStringValue(IdenticalEnumWithDifferentStrings.Fox));

        }

        /// <summary>
        /// Parse a string value to retrieve an enum value
        /// </summary>
        [Test]
        public void TestStaticParse()
        {
            //Case Sensitive (not found)
            Assert.IsNull(StringEnum.Parse(typeof(EnumPartialStrings), "jacK be nImbLe"));
            //Case Sensitive (found)
            Assert.AreEqual(EnumPartialStrings.Jack, StringEnum.Parse(typeof(EnumPartialStrings), "Jack be nimble"));
            //Case insensitive (found)
            Assert.AreEqual(EnumPartialStrings.Jack, StringEnum.Parse(typeof(EnumPartialStrings), "jacK be nImbLe", true));

        }


        /// <summary>
        /// Test whether a given string is defined within the given enum
        /// </summary>
        [Test]
        public void TestStaticIsStringDefined()
        {
            Assert.IsFalse(StringEnum.IsStringDefined(typeof(EnumWithStrings), "My fair Lady"));
            Assert.IsTrue(StringEnum.IsStringDefined(typeof(EnumPartialStrings), "jack BE NIMble", true));
        }

        #endregion

        #region Instance Tests

       
        /// <summary>
        /// Create new instance and return enum value from the given string value
        /// </summary>
        [Test]
        public void TestInstanceGetStringValue()
        {
            StringEnum stringEnum = new StringEnum(typeof(EnumWithStrings));
            Assert.AreEqual("Fourth Value", stringEnum.GetStringValue("Lazy"));
            //Expect null as this value doesn't exist
            Assert.IsNull(stringEnum.GetStringValue("clearly not there"));
        }

        /// <summary>
        /// Test retrieving an array of string values from a supplied enum
        /// </summary>
        [Test]
        public void TestInstanceGetStringValues()
        {
            StringEnum stringEnum = new StringEnum(typeof(EnumWithoutStrings));
            Assert.AreEqual(0, stringEnum.GetStringValues().Length);

            stringEnum = new StringEnum(typeof(EnumPartialStrings));
            Assert.AreEqual(1, stringEnum.GetStringValues().Length);
            Assert.AreEqual("Jack be nimble", stringEnum.GetStringValues().GetValue(0).ToString());

        }

        /// <summary>
        /// Test whether the given string is defined using instance methods
        /// </summary>
        [Test]
        public void TestInstanceIsStringDefined()
        {
            StringEnum stringEnum = new StringEnum(typeof(EnumWithStrings));
            Assert.IsFalse(stringEnum.IsStringDefined("Something that's not there"));
            Assert.IsFalse(stringEnum.IsStringDefined("first value"));
            Assert.IsTrue(stringEnum.IsStringDefined("First Value"));
        }

        /// <summary>
        /// Test basic property get access
        /// </summary>
        [Test]
        public void TestInstancePropertyEnum()
        {
            StringEnum stringEnum = new StringEnum(typeof(EnumWithoutStrings));
            Assert.IsTrue(stringEnum.EnumType.IsEnum);

        }

        [Test]
        public void TestStructImplementation()
        {
            string myString = "My Value 1";
            switch (myString)
            {
                case MyStringEnum.MyValue1:
                    break;
                case MyStringEnum.MyValue2:
                    Assert.Fail();
                    break;
            }
        }

        #endregion

    }

    #endregion

    #region Struct Implementation

    /// <summary>
    /// Struct implementation of String Enum
    /// </summary>
    public struct MyStringEnum
    {
        public const string MyValue1 = "My Value 1";
        public const string MyValue2 = "My Value 2";

    }
    #endregion

}
