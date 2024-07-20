using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Query to retrieve and filter the messages.
    /// </summary>
    public sealed class GetMessageQuery
    {
        internal static string ParameterTo = "to";
        internal static string ParameterFrom = "from";
        internal static string ParameterDateSent = "datesent";
        internal static string ParameterPage = "page";
        internal static string ParameterPageSize = "pagesize";
        internal static string ParameterThreadId = "threadid";
        internal static string ParameterStatus = "status";

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMessageQuery"/> class.
        /// </summary>
        public GetMessageQuery()
        {
            this.Id = -1;
            this.To = string.Empty;
            this.From = string.Empty;
            this.DateSent = string.Empty;
            this.Page = -1;
            this.PageSize = -1;
            this.ThreadId = -1;
            this.Status = -1;
        }

        /// <summary>
        /// Gets or sets the unique message identifier.
        /// </summary>
        /// <value>
        /// The unique message identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets message to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the date sent.
        /// </summary>
        /// <value>
        /// The date sent.
        /// </value>
        public string DateSent { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the thread ID.
        /// </summary>
        /// <value>
        /// The thread ID.
        /// </value>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public int Status { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "Id = ", Id, "\r\n");
            str = String.Concat(str, "To = ", To, "\r\n");
            str = String.Concat(str, "From = ", From, "\r\n");
            str = String.Concat(str, "DateSent = ", DateSent, "\r\n");
            str = String.Concat(str, "Page = ", Page, "\r\n");
            str = String.Concat(str, "PageSize = ", PageSize, "\r\n");
            str = String.Concat(str, "ThreadId = ", ThreadId, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            return str;
        }

    }
}
