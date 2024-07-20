using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Numerics
{
    internal static class BigIntegerExtensions
    {
        internal static BigInteger Parse(string str)
        {
            if (String.IsNullOrEmpty(str))
                return BigInteger.Zero;

            var result = new BigInteger();
            var idx = 0;
            var sign = false;
            if (str[0] == '-')
            {
                idx++;
                sign = true;
            }
            if (str[0] == '+')
            {
                idx++;
            }

            int num = str.Length - idx;
            result = 0;
            while (--num >= 0)
            {
                result *= 10;
                if (str[idx] != '\0')
                {
                    result += (str[idx++] - '0');
                }
            }

            if (sign)
            {
                result = -result;
            }

            return result;
        }
    }
}

