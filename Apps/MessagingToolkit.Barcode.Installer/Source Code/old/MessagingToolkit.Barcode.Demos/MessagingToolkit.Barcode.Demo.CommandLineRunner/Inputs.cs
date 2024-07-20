using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Demo.CommandLineRunner
{
    internal sealed class Inputs
    {
        private readonly List<String> inputs = new List<string>(10);
        private int position = 0;

        public void AddInput(string pathOrUrl)
        {
            lock (this)
            {
                inputs.Add(pathOrUrl);
            }
        }

        public String GetNextInput()
        {
            lock (this)
            {
                if (position < inputs.Count())
                {
                    String result = inputs[position];
                    position++;
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public int GetInputCount()
        {
            return inputs.Count();

        }
    }
}
