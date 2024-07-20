using System;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Represents the type of data encoded by a barcode -- from plain text, to a
    /// URI, to an e-mail address, etc.
    /// </summary>
    public enum ParsedResultType
    {

        AddessBook, EmailAddress, Product, Uri, Text, Geo, Tel, Sms, Calendar, Wifi, Isbn,

    }


}