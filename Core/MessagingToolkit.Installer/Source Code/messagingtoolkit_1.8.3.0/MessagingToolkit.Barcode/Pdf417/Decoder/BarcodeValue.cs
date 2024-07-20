using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class BarcodeValue
    {
        private readonly IDictionary<int?, int?> values = new Dictionary<int?, int?>();

        /// <summary>
        /// Add an occurrence of a value </summary>
        /// <param name="value"> </param>
        internal void SetValue(int value)
        {
            int? confidence = null;
            if (values.ContainsKey(value))
                confidence = values[value];
            if (confidence == null)
            {
                confidence = 0;
            }
            confidence++;
            if (values.ContainsKey(value))
                values[value] = confidence;
            else
                values.Add(value, confidence);

        }

        /// <summary>
        /// Determines the maximum occurrence of a set value and returns all values which were set with this occurrence.
        /// Return an array of int, containing the values with the highest occurrence, or null, if no value was set
        /// </summary>
        internal int[] GetValue()
        {
            int maxConfidence = -1;
            IList<int?> result = new List<int?>();
            foreach (KeyValuePair<int?, int?> entry in values)
            {
                if (entry.Value > maxConfidence)
                {
                    maxConfidence = entry.Value.Value;
                    result.Clear();
                    result.Add(entry.Key);
                }
                else if (entry.Value == maxConfidence)
                {
                    result.Add(entry.Key);
                }
            }
            return Pdf417Common.ToIntArray(result);

        }


        public int? GetConfidence(int value)
        {
            return values[value];
        }

    }
}
