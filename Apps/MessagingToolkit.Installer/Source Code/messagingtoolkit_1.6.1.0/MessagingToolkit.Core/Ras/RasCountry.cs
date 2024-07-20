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

namespace MessagingToolkit.Core.Ras
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Represents country or region specific dialing information.
    /// </summary>
    /// <example>
    /// The following example shows how to retrieve all countries from the Windows Telephony list.
    /// <code lang="C#">
    /// <![CDATA[
    /// ReadOnlyCollection<RasCountry> countries = RasCountry.GetCountries();
    /// foreach (RasCountry country in countries)
    /// {
    ///     // Do something useful.
    /// }
    /// ]]>
    /// </code>
    /// <code lang="VB.NET">
    /// <![CDATA[
    /// Dim countries As ReadOnlyCollection(Of RasCountry) = RasCountry.GetCountries();
    /// For Each country As RasCountry in countries
    ///     ' Do something useful.
    /// Next
    /// ]]>
    /// </code>
    /// </example>
    [Serializable]
    [DebuggerDisplay("Id = {Id}, Name = {Name}")]
    public sealed class RasCountry
    {
        #region Fields

        private int _id;
        private int _code;
        private string _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasCountry"/> class.
        /// </summary>
        /// <param name="id">The TAPI identifier.</param>
        /// <param name="code">The country or region code.</param>
        /// <param name="name">The name of the country.</param>
        internal RasCountry(int id, int code, string name)
        {
            this._id = id;
            this._code = code;
            this._name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the TAPI identifier.
        /// </summary>
        public int Id
        {
            get { return this._id; }
        }

        /// <summary>
        /// Gets the country or region code.
        /// </summary>
        public int Code
        {
            get { return this._code; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves country/region specific dialing information from the Windows Telephony list of countries/regions for a specific country id.
        /// </summary>
        /// <param name="countryId">The country id to retrieve.</param>
        /// <returns>A new <see cref="DotRas.RasCountry"/> object.</returns>
        public static RasCountry GetCountryById(int countryId)
        {
            int nextCountryId = 0;
            return RasHelper.Instance.GetCountry(countryId, out nextCountryId);
        }

        /// <summary>
        /// Retrieves country/region specific dialing information from the Windows Telephony list of countries/regions.
        /// </summary>
        /// <returns>A collection of <see cref="RasCountry"/> objects.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should not be a property.")]
        public static ReadOnlyCollection<RasCountry> GetCountries()
        {
            Collection<RasCountry> tempCollection = new Collection<RasCountry>();

            // The country id must be set to 1 to initiate retrieval of all countries in the list.
            int countryId = 1;

            do
            {
                RasCountry country = RasHelper.Instance.GetCountry(countryId, out countryId);
                if (country != null)
                {
                    tempCollection.Add(country);
                }
            }
            while (countryId != 0);

            return new ReadOnlyCollection<RasCountry>(tempCollection);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="DotRas.RasCountry"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="DotRas.RasCountry"/>.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}