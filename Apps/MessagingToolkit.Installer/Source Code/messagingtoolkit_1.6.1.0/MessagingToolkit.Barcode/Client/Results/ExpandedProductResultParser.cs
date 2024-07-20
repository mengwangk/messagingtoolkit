using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Parses strings of digits that represent a RSS Extended code.
    /// 
    /// Modified: May 18 2012
    /// </summary>
    internal sealed class ExpandedProductResultParser : ResultParser
    {
        public override ParsedResult Parse(Result result)
        {
            BarcodeFormat format = result.BarcodeFormat;
            if (format != BarcodeFormat.RSSExpanded)
            {
                // ExtendedProductParsedResult NOT created. Not a RSS Expanded barcode
                return null;
            }
            // Really neither of these should happen:
            String rawText = GetMassagedText(result);
            if (rawText == null)
            {
                // ExtendedProductParsedResult NOT created. Input text is NULL
                return null;
            }

            String productID = null;
            String sscc = null;
            String lotNumber = null;
            String productionDate = null;
            String packagingDate = null;
            String bestBeforeDate = null;
            String expirationDate = null;
            String weight = null;
            String weightType = null;
            String weightIncrement = null;
            String price = null;
            String priceIncrement = null;
            String priceCurrency = null;
            Dictionary<String, String> uncommonAIs = new Dictionary<String, String>();

            int i = 0;

            while (i < rawText.Length)
            {
                String ai = FindAIvalue(i, rawText);
                if (ai == null)
                {
                    // Error. Code doesn't match with RSS expanded pattern
                    // ExtendedProductParsedResult NOT created. Not match with RSS Expanded pattern
                    return null;
                }
                i += ai.Length + 2;
                String val = FindValue(i, rawText);
                i += val.Length;

                if ("00".Equals(ai))
                {
                    sscc = val;
                }
                else if ("01".Equals(ai))
                {
                    productID = val;
                }
                else if ("10".Equals(ai))
                {
                    lotNumber = val;
                }
                else if ("11".Equals(ai))
                {
                    productionDate = val;
                }
                else if ("13".Equals(ai))
                {
                    packagingDate = val;
                }
                else if ("15".Equals(ai))
                {
                    bestBeforeDate = val;
                }
                else if ("17".Equals(ai))
                {
                    expirationDate = val;
                }
                else if ("3100".Equals(ai) || "3101".Equals(ai) || "3102".Equals(ai) || "3103".Equals(ai) || "3104".Equals(ai) || "3105".Equals(ai) || "3106".Equals(ai) || "3107".Equals(ai)
                      || "3108".Equals(ai) || "3109".Equals(ai))
                {
                    weight = val;
                    weightType = ExpandedProductParsedResult.KILOGRAM;
                    weightIncrement = ai.Substring(3);
                }
                else if ("3200".Equals(ai) || "3201".Equals(ai) || "3202".Equals(ai) || "3203".Equals(ai) || "3204".Equals(ai) || "3205".Equals(ai) || "3206".Equals(ai) || "3207".Equals(ai)
                      || "3208".Equals(ai) || "3209".Equals(ai))
                {
                    weight = val;
                    weightType = ExpandedProductParsedResult.POUND;
                    weightIncrement = ai.Substring(3);
                }
                else if ("3920".Equals(ai) || "3921".Equals(ai) || "3922".Equals(ai) || "3923".Equals(ai))
                {
                    price = val;
                    priceIncrement = ai.Substring(3);
                }
                else if ("3930".Equals(ai) || "3931".Equals(ai) || "3932".Equals(ai) || "3933".Equals(ai))
                {
                    if (val.Length < 4)
                    {
                        // The value must have more of 3 symbols (3 for currency and
                        // 1 at least for the price)
                        // ExtendedProductParsedResult NOT created. Not match with RSS Expanded pattern
                        return null;
                    }
                    price = val.Substring(3);
                    priceCurrency = val.Substring(0, (3) - (0));
                    priceIncrement = ai.Substring(3);
                }
                else
                {
                    // No match with common AIs
                    uncommonAIs.Add(ai, val);
                }
            }

            return new ExpandedProductParsedResult(productID, sscc, lotNumber, productionDate, packagingDate, bestBeforeDate, expirationDate, weight, weightType, weightIncrement, price, priceIncrement,
                    priceCurrency, uncommonAIs);
        }

        private static String FindAIvalue(int i, String rawText)
        {
            StringBuilder buf = new StringBuilder();
            char c = rawText[i];
            // First character must be a open parenthesis.If not, ERROR
            if (c != '(')
            {
                return null;
            }

            String rawTextAux = rawText.Substring(i + 1);

            for (int index = 0; index < rawTextAux.Length; index++)
            {
                char currentChar = rawTextAux[index];
                if (currentChar == ')')
                {
                    return buf.ToString();
                }
                else if (currentChar >= '0' && currentChar <= '9')
                {
                    buf.Append(currentChar);
                }
                else
                {
                    return null;
                }
            }
            return buf.ToString();
        }

        private static String FindValue(int i, String rawText)
        {
            StringBuilder buf = new StringBuilder();
            String rawTextAux = rawText.Substring(i);

            for (int index = 0; index < rawTextAux.Length; index++)
            {
                char c = rawTextAux[index];
                if (c == '(')
                {
                    // We look for a new AI. If it doesn't exist (ERROR), we coninue
                    // with the iteration
                    if (FindAIvalue(index, rawTextAux) == null)
                    {
                        buf.Append('(');
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    buf.Append(c);
                }
            }
            return buf.ToString();
        }
    }
}
