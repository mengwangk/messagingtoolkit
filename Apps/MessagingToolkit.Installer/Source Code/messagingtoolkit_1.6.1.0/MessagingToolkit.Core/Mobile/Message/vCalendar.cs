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
using System.Linq;
using System.Text;
using System.Collections;

using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Helper;

namespace MessagingToolkit.Core.Mobile.Message
{
    #region ================================ vCalendar Class ===================================

    /// <summary>
    /// vCalendar message
    /// </summary>
    [global::System.Serializable]
    public class vCalendar : Sms
    {

        /// <summary>
        /// vCalendar destination port
        /// </summary>
        private const int vCalendarDestinationPort = 9205;

        /// <summary>
        /// vCalendar source port
        /// </summary>
        private const int vCalendarSourcePort = 0;


        /// <summary>
        /// Load vCalendar from string
        /// </summary>
        private bool loadFromString;

        /// <summary>
        /// Private constructor
        /// </summary>
        private vCalendar():base()
        {
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.SourcePort = vCalendarSourcePort;
            this.DestinationPort = vCalendarDestinationPort;
            this.Events = new vEvents();
            this.loadFromString = false;
        }

        /// <summary>
        /// Private Constructor
        /// </summary>
        /// <param name="Value"></param>
        private vCalendar(vEvent Value):base()
        {
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.SourcePort = vCalendarSourcePort;
            this.DestinationPort = vCalendarDestinationPort;
           
            this.Events = new vEvents();
            this.Events.Add(Value);
            this.loadFromString = false;
            this.TimeZone = string.Empty;            
        }


        /// <summary>
        /// Calendar events
        /// </summary>
        /// <value>Events</value>
        public vEvents Events
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time zone.
        /// This property is identified by the property name TZ. This property specifies the standard time zone of the "home" 
        /// system that created the vCalendar object. The property value is specified in a manner consistent with ISO 8601. 
        /// The property value is a signed numeric indicating the number of hours and possibly minutes from UTC. 
        /// Time zones east of UTC are positive numbers. Time zones west of UTC are negative numbers. 
        /// <para>
        /// The following are examples of this property:
        /// TZ:-05
        /// TZ:+05:30
        /// Support for this property is optional for implementations conforming to this specification.
        /// </para>
        /// </summary>
        /// <value>The time zone.</value>
        public string TimeZone 
        {
            get;
            set;
        }

               
        /// <summary>
        /// Load vCalendar content from a string
        /// </summary>
        /// <param name="value">vCalendar</param>
        /// <returns>true if loaded successfully</returns>
        public bool LoadString(string value)
        {
            this.Content = value;
            this.loadFromString = true;
            return true;
        }

        /// <summary>
        /// Create the vCalendar
        /// </summary>
        /// <returns>vCalendar content</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("BEGIN:VCALENDAR{0}", System.Environment.NewLine);

            // The following two lines seem to be required by Outlook to get the alarm settings
            result.AppendFormat("VERSION:1.0{0}", System.Environment.NewLine);
            //result.AppendFormat("METHOD:PUBLISH{0}", System.Environment.NewLine);

            if (!string.IsNullOrEmpty(this.TimeZone))
            {
                result.AppendFormat("TZ:{0}{1}", this.TimeZone, System.Environment.NewLine);
            }

            foreach (vEvent item in Events)
            {
                result.Append(item.ToString());
            }

            //result.Append("\r\nDALARM:201101031T000000;P1D;7;Mom's birthday on the 28th.  Get a card!\r\n"); 
            result.AppendFormat("END:VCALENDAR{0}", System.Environment.NewLine);
            return result.ToString();
        }


        /// <summary>
        /// Encode the vCalendar content
        /// </summary>
        /// <returns>Encoded vCalendar content</returns>
        internal override byte[] GetPdu()
        {
            if (!this.loadFromString)
                this.Content = ToString();
            return Encoding.GetEncoding("iso-8859-1").GetBytes(this.Content);

            //return MessagingToolkit.Pdu.PduUtils.StringToUnencodedSeptets(this.Content);           
        }


        #region ============== Factory method   =====================================================

        /// <summary>
        /// Static factory to create the vCalendar instance
        /// </summary>
        /// <param name="e">vCalendar event</param>
        /// <returns>A new instance of the vCalendar object</returns>
        public static vCalendar NewInstance(vEvent e)
        {
            return new vCalendar(e);
        }

        /// <summary>
        /// Static factory to create the vCalendar instance
        /// </summary>
        /// <returns>A new instance of the vCalendar object</returns>
        public static vCalendar NewInstance()
        {
            return new vCalendar();
        }

        #endregion ===================================================================================
    } 


    #endregion ===============================================================================

    #region =================================== vAlarm Class =================================


    /// <summary>
    /// vAlarm class
    /// </summary>
    [global::System.Serializable]
    public class vAlarm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="vAlarm"/> class.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="snoozeDuration">Duration of the snooze.</param>
        /// <param name="noOfRepeat">The no of repeat.</param>
        /// <param name="reminder">The reminder.</param>
        public vAlarm(DateTime startTime, TimeSpan snoozeDuration, int noOfRepeat, string reminder)
        {
            this.StartTime = startTime;
            this.SnoozeDuration = snoozeDuration;
            this.NoofRepeat = noOfRepeat;
            this.Reminder = reminder;
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the duration of the snooze.
        /// </summary>
        /// <value>The duration of the snooze.</value>
        public TimeSpan SnoozeDuration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reminder.
        /// </summary>
        /// <value>The reminder.</value>
        public string Reminder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of repeat for the snooze
        /// </summary>
        /// <value>The repeat.</value>
        public int NoofRepeat
        {
            get;
            set;
        }
        /*
        /// <summary>
        /// Amount of time before event to display alarm
        /// </summary>
        /// <value></value>
        public TimeSpan Trigger
        {
            get;
            set;
        }


        /// <summary>
        /// Action to take to notify user of alarm
        /// </summary>
        /// <value></value>
        public string Action
        {
            get;
            set;
        }

       
        /// <summary>
        /// Description of the alarm
        /// </summary>
        /// <value></value>
        public string Description
        {
            get;
            set;

        }

        /// <summary>
        /// Constructor
        /// </summary>
        public vAlarm()
        {
            Trigger = TimeSpan.FromDays(1);
            Action = "DISPLAY";
            Description = "Reminder";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setTrigger"></param>
        public vAlarm(TimeSpan setTrigger)
        {
            Trigger = setTrigger;
            Action = "DISPLAY";
            Description = "Reminder";
        }

        /// <summary>
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="action"></param>
        /// <param name="description"></param>
        public vAlarm(TimeSpan trigger, string action, string description)
        {
            Trigger = trigger;
            Action = action;
            Description = description;
        }
        */


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            /*
            result.AppendFormat("BEGIN:VALARM{0}", System.Environment.NewLine);
            result.AppendFormat("TRIGGER:P{0}DT{1}H{2}M{3}", Trigger.Days, Trigger.Hours, Trigger.Minutes, System.Environment.NewLine);

            if (!string.IsNullOrEmpty(Action))
            {

                if (QuotedPrintable.NeedQuotedPrintable(Action))
                    result.AppendFormat("ACTION;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", Action, System.Environment.NewLine);
                else
                    result.AppendFormat("ACTION:{0}{1}", Action, System.Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(Description))
            {
                if (QuotedPrintable.NeedQuotedPrintable(Description))
                    result.AppendFormat("DESCRIPTION;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", Description, System.Environment.NewLine);
                else
                    result.AppendFormat("DESCRIPTION:{0}{1}", Description, System.Environment.NewLine);
            }
            
            result.AppendFormat("END:VALARM{0}", System.Environment.NewLine);
            */ 
             
            // DALARM:initial run time;duration of snoozes;number of times to repeat snoozes;Display string
            // Example
            // Reminder of Mom's birthday to be displayed one week before her birthday on August 28th, repeated daily.
            // DALARM:20020821T000000;P1D;7;Mom's birthday on the 28th.  Get a card! 

            if (QuotedPrintable.NeedQuotedPrintable(Reminder))
            {
                result.AppendFormat("DALARM;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0};P{1}DT{2}H{3}M;{4};{5}{6}",
                    StartTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ"),
                    SnoozeDuration.Days, SnoozeDuration.Hours, SnoozeDuration.Minutes,
                    NoofRepeat,
                    Reminder,
                    Environment.NewLine);
            }
            else
            {                
                result.AppendFormat("DALARM:{0};P{1}DT{2}H{3}M;{4};{5}{6}",
                    StartTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ"),
                    SnoozeDuration.Days, SnoozeDuration.Hours, SnoozeDuration.Minutes,
                    NoofRepeat,
                    Reminder,
                    Environment.NewLine);                 
            }

            return result.ToString();
        }
    }
    #endregion


    #region ==================================== vRecurrenceRule Class ===========================================

    /// <summary>
    /// Repeat frequency
    /// </summary>
    public enum EventRepeat
    {
        /// <summary>
        /// Repeat daily
        /// </summary>
        Daily,
        /// <summary>
        /// Repeat weekly
        /// </summary>
        Weekly,
        /// <summary>
        /// Repeat monthly
        /// </summary>
        Monthly,
        /// <summary>
        /// Repeat yearly
        /// </summary>
        Yearly
    }


    /// <summary>
    /// vEvent recurring rule.
    /// 
    /// </summary>
    [global::System.Serializable]
    public class vRecurrenceRule
    {        
        private DateTime endTime;
        private EventRepeat repeat;

        /// <summary>
        /// Initializes a new instance of the <see cref="vRecurrenceRule"/> class.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public vRecurrenceRule(string rule)
        {
            this.Rule = rule;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vRecurrenceRule"/> class.
        /// </summary>
        /// <param name="repeat">The repeat.</param>
        /// <param name="endTime">The end time.</param>
        public vRecurrenceRule(EventRepeat repeat, DateTime endTime)
        {
            this.repeat = repeat;
            this.endTime = endTime;
            this.Rule = string.Empty;
        }

        /// <summary>
        /// Gets or sets the rule.
        /// </summary>
        /// <value>The rule.</value>
        public string Rule
        {
            get;
            internal set;
        }
                       
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrEmpty(this.Rule))
            {
                result.AppendFormat("RRULE:{0}{1}", this.Rule, Environment.NewLine);
            }
            else
            {
                if (repeat == EventRepeat.Daily)
                {
                    result.AppendFormat("RRULE:{0} {1}{2}", "D1", endTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ"), Environment.NewLine);
                }
                else if (repeat == EventRepeat.Weekly)
                {
                    result.AppendFormat("RRULE:{0} {1}{2}", "W1", endTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ"), Environment.NewLine);
                }
                else if (repeat == EventRepeat.Monthly)
                {
                    result.AppendFormat("RRULE:{0} {1}{2}", "M1", endTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ"), Environment.NewLine);
                }
                else if (repeat == EventRepeat.Yearly)
                {
                    result.AppendFormat("RRULE:{0} {1}{2}", "Y1", endTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ"), Environment.NewLine);
                }
            }
            return result.ToString();
        }
        

        #region ============== Factory method   =====================================================


        /// <summary>
        /// Create the recurrence rule using the rule value, e.g. RRULE:D1 20110117T145600
        /// </summary>
        /// <returns>A new instance of the vEvent object</returns>
        public static vRecurrenceRule NewInstance(string rule)
        {
            return new vRecurrenceRule(rule);
        }

        /// <summary>
        /// Create the recurrence rule
        /// </summary>
        /// <param name="repeat">The repeat frequency</param>
        /// <param name="endTime">The end time.</param>
        /// <returns></returns>
        public static vRecurrenceRule NewInstance(EventRepeat repeat, DateTime endTime)
        {
            return new vRecurrenceRule(repeat, endTime);
        }

        #endregion ===================================================================================
    }



    #endregion ==========================================================================================
        

    #region ==================================== vEvent Class ===========================================

    /// <summary>
    /// vCalendar event
    /// </summary>
    [global::System.Serializable]
    public class vEvent
    {

        /// <summary>
        /// Private constructor
        /// </summary>
        private vEvent()
        {
            Alarms = new vAlarms();
            DtEnd = DateTime.MinValue;
            DtStamp = DateTime.MinValue;
            DtStart = DateTime.MinValue;
            RecurrenceRule = null;
        }

        /// <summary>
        /// Unique identifier for the event
        /// </summary>
        /// <value></value>              
        public string Uid
        {
            get;
            set;
        }
        

        /// <summary>
        /// Start date of event. Will be automatically converted to GMT
        /// </summary>
        /// <value></value>
        public DateTime DtStart
        {
            get;
            set;
        }


        /// <summary>
        /// End date of event. Will be automatically converted to GMT
        /// </summary>
        /// <value></value>
        public DateTime DtEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Timestamp. Will be automatically converted to GMT
        /// </summary>
        /// <value>The dt stamp.</value>
        public DateTime DtStamp
        {
            get;
            set;
        }
        


        /// <summary>
        /// </summary>
        /// <value></value>      
        public string Summary
        {
            get;
            set;
        }
        


        /// <summary>
        /// Can be mailto: url or just a name
        /// </summary>
        /// <value></value>    
        public string Organizer
        {
            get;
            set;
        }
        
        

        /// <summary>
        /// </summary>
        /// <value></value>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get;
            set;
        }
         

        /// <summary>
        /// Alarms needed for this event
        /// </summary>
        /// <value>The alarms.</value>
        public vAlarms Alarms
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the recurrence rule.
        /// 
        /// <para>
        /// This property is identified by the property name RRULE. 
        /// This property defines a rule or repeating pattern for a recurring vCalendar entity, 
        /// based on the Basic Recurrence Rule Grammar of XAPIA's CSA. 
        /// The value for the property is a pattern specification for the recurrence. 
        /// The following is an example of this property:
        ///
        ///     W2 TU TH			Every other week, on Tuesday and Thursday
        ///     D1 #10				Daily for 10 occurrences
        ///     YM1 6 7 #8			Yearly in June and July for 8 occurrences
        /// 
        /// Support for this property is optional for implementations conforming to this specification.
        /// </para>
        /// </summary>
        /// <value>The recurrence rule.</value>
        public vRecurrenceRule RecurrenceRule
        {
            get;
            set;
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("BEGIN:VEVENT{0}", System.Environment.NewLine);

            if (!string.IsNullOrEmpty(Uid))
            {
                if (QuotedPrintable.NeedQuotedPrintable(Uid))
                    result.AppendFormat("UID;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", QuotedPrintable.Encode(Uid), System.Environment.NewLine);
                else
                    result.AppendFormat("UID:{0}{1}", Uid, System.Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(Summary))
            {
                if (QuotedPrintable.NeedQuotedPrintable(Summary))
                    result.AppendFormat("SUMMARY;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", QuotedPrintable.Encode(Summary), System.Environment.NewLine);
                else
                    result.AppendFormat("SUMMARY:{0}{1}", Summary, System.Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(Organizer))
            {
                if (QuotedPrintable.NeedQuotedPrintable(Organizer))
                    result.AppendFormat("ORGANIZER;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", QuotedPrintable.Encode(Organizer), System.Environment.NewLine);
                else
                    result.AppendFormat("ORGANIZER:{0}{1}", Organizer, System.Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(Location))
            {
                if (QuotedPrintable.NeedQuotedPrintable(Location))
                    result.AppendFormat("LOCATION;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", QuotedPrintable.Encode(Location), System.Environment.NewLine);
                else
                    result.AppendFormat("LOCATION:{0}{1}", Location, System.Environment.NewLine);
            }

            if (DtStart > DateTime.MinValue)
            {
                string dtStart = DtStart.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                result.AppendFormat("DTSTART:{0}{1}", dtStart, Environment.NewLine);
            }

            if (DtEnd > DateTime.MinValue)
            {
                string dtEnd = DtEnd.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                result.AppendFormat("DTEND:{0}{1}", dtEnd, Environment.NewLine);
            }

            if (DtStamp > DateTime.MinValue)
            {
                string dtStamp = DtStamp.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                result.AppendFormat("DTSTAMP:{0}{1}", dtStamp, System.Environment.NewLine);
            }

            
            if (!string.IsNullOrEmpty(Description))
            {
                if (QuotedPrintable.NeedQuotedPrintable(Description))
                    result.AppendFormat("DESCRIPTION;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", QuotedPrintable.Encode(Description), System.Environment.NewLine);
                else
                    result.AppendFormat("DESCRIPTION:{0}{1}", Description, System.Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(Url) && Url.Length > 0)
            {
                if (QuotedPrintable.NeedQuotedPrintable(Url))
                    result.AppendFormat("URL;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:{0}{1}", QuotedPrintable.Encode(Url), System.Environment.NewLine);
                else
                    result.AppendFormat("URL:{0}{1}", Url, System.Environment.NewLine);
            }
                     
            foreach (vAlarm item in Alarms)
            {
                result.Append(item.ToString());
            }

            if (RecurrenceRule != null)
            {
                result.Append(RecurrenceRule.ToString());               
            }
            
            result.AppendFormat("END:VEVENT{0}", Environment.NewLine);
            return result.ToString();
        }


        #region ============== Factory method   =====================================================


        /// <summary>
        /// Static factory to create the vEvent instance
        /// </summary>
        /// <returns>A new instance of the vEvent object</returns>
        public static vEvent NewInstance()
        {
            return new vEvent();
        }

        #endregion ===================================================================================
    }

    #endregion ============================================================================================

    #region  ================================= vAlarms Class ==============================================

    /// <summary>
    /// </summary>
    [global::System.Serializable]
    public class vAlarms : CollectionBase
    {
        // The first thing to do when building a CollectionBase class is to inherit from System.Collections.CollectionBase

        public vAlarm Add(vAlarm Value)
        {
            // After you inherit the CollectionBase class, you can access an intrinsic object
            // called InnerList that represents your collection. InnerList is of type ArrayList.
            this.InnerList.Add(Value);
            return Value;
        }

        public vAlarm Item(int index)
        {
            // To retrieve an item from the InnerList, pass the index of that item to the .Item property.
            return (vAlarm)this.InnerList[index];
        }

        public void Remove(int index)
        {
            // This Remove expects an index.
            vAlarm cust = default(vAlarm);

            cust = (vAlarm)this.InnerList[index];
            if ((cust != null))
            {
                this.InnerList.Remove(cust);
            }
        }
    }
    
    #endregion ===========================================================================================

    #region ===================================== vEvents Class ==========================================

    /// <summary>
    /// Collection of vCalendar events
    /// </summary>
    [global::System.Serializable]
    public class vEvents : CollectionBase
    {
        public vEvent Add(vEvent Value)
        {
            // After you inherit the CollectionBase class, you can access an intrinsic object
            // called InnerList that represents your collection. InnerList is of type ArrayList.
            this.InnerList.Add(Value);
            return Value;
        }

        public vEvent Item(int index)
        {
            // To retrieve an item from the InnerList, pass the index of that item to the .Item property.
            return (vEvent)this.InnerList[index];
        }

        public void Remove(int index)
        {
            // This Remove expects an index.
            vEvent cust = default(vEvent);
            cust = (vEvent)this.InnerList[index];
            if ((cust != null))
            {
                this.InnerList.Remove(cust);
            }
        }
    }
    #endregion =============================================================================================
}
