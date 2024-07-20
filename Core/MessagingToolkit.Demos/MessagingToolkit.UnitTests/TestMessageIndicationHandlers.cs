using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using MessagingToolkit.Core.Mobile;
using System.Text.RegularExpressions;
using MessagingToolkit.Core;
using System.Collections;

namespace MessagingToolkit.UnitTests
{
    public class TestMessageIndicationHandlers
    {

        public void TestUssdHandler()
        {
            string UssdResponseIndication = "^\\+CUSD:\\s+([\\d-()]+)(?:,\\s*\"([^\"]*))?(?:\",\\s*(\\d+)\\s*)?\"?\r?$";
            string data1 = @"+CUSD: 0,""Hazineye akherin tamas 160 Rial ,va baghimande etebare asli 16764 Rial ast.Tarhe Ghermeze 1setare 1ruzeh: Mokaleme rayegan az 1ta7 sobh Ba *555*3*6*1# (500Toman)"",15";
            string data2 = @"+CUSD: 2";
            string data3 = @"+CUSD: 0,""H";
            string data4 = "+CUSD: (0-2)";
            Match match = new Regex(UssdResponseIndication).Match(data3);
            if (match.Success)
            {
                Console.WriteLine("success");
            }
            else
            {
                Console.WriteLine("failed");
            }

            UssdResponse ussdResponse = new UssdResponse(data3, "");
            Console.WriteLine(ussdResponse.Content);
        }

        [Fact]
        public void TestClccIndincation()
        {
            CallIndicationHandlers handlers = new CallIndicationHandlers();
            string data = "+CLCC: 1,0,3,0,0,\"08579300xxx\",129,\"\"";
            //string data = "+CLIP: \"08579300xxx\",129\\r\\n";
            if (handlers.IsUnsolicitedCall(data))
            {
                // Is unsolicited call, raise the call received event
                string description;
                string call = data;

                IIndicationObject callInformation = handlers.HandleUnsolicitedCall(ref call, out description);

                // Raise event
                if (callInformation != null && callInformation.GetType() == typeof(CurrentCallInformation))
                {
                    // Current call listing
                    Console.WriteLine(callInformation.ToString());
                }
                else
                {
                    Console.WriteLine(callInformation);
                }               
            }
        }

        public void TestGetCurrentCalls()
        {
            string response = "+CLCC: 1,0,6,0,0,\"085793001974\",129,\"Koh\"\r\n+CLCC: 1,0,6,0,1,\"085793001974\",129,\"\"\r\n";

            /// <summary>
            /// Expected response
            /// </summary>
            const string ExpectedResponse = "+CLCC: ";

            /// <summary>
            /// Call entry pattern
            /// </summary>
            const string CallPattern = "(\\d+),(\\d+),(\\d+),(\\d+),(\\d+),\"(.+)\",(\\d+),\"(.*)\".*\\r\\n";

            ArrayList list = new ArrayList();
            Regex regex = new Regex(Regex.Escape(ExpectedResponse) + CallPattern);
            for (Match match = regex.Match(response); match.Success; match = match.NextMatch())
            {
                int index = int.Parse(match.Groups[1].Value);
                string callType = match.Groups[2].Value;
                string state = match.Groups[3].Value;
                string mode = match.Groups[4].Value;
                string multiparty = match.Groups[5].Value;
                string phoneNumber = match.Groups[6].Value;
                string internationIndicator = match.Groups[7].Value;
                string contactName = match.Groups[8].Value;

                CallType callTypeEnum = (CallType)Enum.Parse(typeof(CallType), callType);
                CallState callStateEnum = CallState.Unknown;
                try
                {
                    callStateEnum = (CallState)Enum.Parse(typeof(CallState), state);
                }
                catch (Exception)
                {
                    // Unknown call state
                    callStateEnum = CallState.Unknown;
                }

                CallMode callModeEnum = CallMode.Unknown;
                try
                {
                    callModeEnum = (CallMode)Enum.Parse(typeof(CallMode), mode);
                }
                catch (Exception)
                {
                    // Unknown call mode
                    callModeEnum = CallMode.Unknown;
                }

                bool isMultiParty = false;
                if (StringEnum.GetStringValue(TrueFalseIndicator.True).Equals(multiparty))
                {
                    isMultiParty = true;
                }
                NumberType numberTypeEnum = (NumberType)Enum.Parse(typeof(NumberType), internationIndicator);
                list.Add(new CurrentCallInformation(index, callTypeEnum, callStateEnum, callModeEnum, isMultiParty, phoneNumber, numberTypeEnum, contactName));
            }
            CurrentCallInformation[] callInfos = new CurrentCallInformation[list.Count];
            list.CopyTo(callInfos, 0);

            //Console.WriteLine(callInfos[0].ToString());
        }

        public void TestResultParser()
        {
            string response = "CMS ERROR: 321 Invalid message index\r\n";
            // Check if there is any CMS error
            if (Regex.IsMatch(response, "[\\+]*CMS ERROR: (\\d+)[a-zA-Z\\s]*\r\n"))
            {
                Match match = Regex.Match(response, "[\\+]*CMS ERROR: (\\d+)[a-zA-Z\\s]*\r\n");
                Console.WriteLine(match.Groups[1].Value);

                string[] cols = response.Split(':');

                if (cols.Length >= 1)
                {
                    string errorCode = cols[1].Trim();
                    Console.WriteLine(errorCode);
                }
                else
                {
                    // Unknown CMS error, so just return it
                    Console.WriteLine(response);
                }
            }
        }

        public static string[] ParseResponse(string response)
        {
            List<string> filteredResult = new List<string>(2);

            if (string.IsNullOrEmpty(response)) return filteredResult.ToArray();

            string[] lines = response.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    filteredResult.Add(line);
                }
            }
            return filteredResult.ToArray();
        }
    }
}
