using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using MessagingToolkit.Core.Mobile;
using System.Text.RegularExpressions;
using MessagingToolkit.Core;

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
    }
}
