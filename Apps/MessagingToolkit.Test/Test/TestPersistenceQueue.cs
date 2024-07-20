using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Test.Test
{
    [TestFixture]
    public class TestPersistenceQueue
    {
       

        [Test]
        public void TestPersistence()
        {
            Sms sms = Sms.NewInstance();
            sms.Content = "testing content";
            sms.Identifier = "12121";
            SerializeMessage(sms);

            Sms sms1 = DeserializeMessage(@"c:\temp\12121") as Sms;

            Console.WriteLine(sms1.Content);

            Mms mms = Mms.NewInstance("test mms", "012121212121");
        }

        public static void SerializeMessage(IMessage sms)
        {
            Stream stream = File.Open(Path.Combine(@"c:\temp\", sms.Identifier), FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();

            bformatter.Serialize(stream, sms);
            stream.Close();
        }

        public static IMessage DeserializeMessage(string fileName)
        {           
            Stream stream = File.Open(fileName, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();        
            Sms sms = (Sms)bformatter.Deserialize(stream);
            stream.Close();
            return sms;
        }
    }
}
