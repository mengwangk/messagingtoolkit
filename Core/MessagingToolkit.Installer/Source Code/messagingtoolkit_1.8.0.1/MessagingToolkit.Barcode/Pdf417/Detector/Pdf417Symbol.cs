using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    public sealed class Pdf417Symbol
    {
        public int K;
        public int codeword;

        public Pdf417Symbol()
        {
            this.K = -1;
            this.codeword = -1;
        }

        public Pdf417Symbol(int k, int cw)
        {
            this.K = k;
            this.codeword = cw;
        }

        public static bool GetPatternEdges(int[] pattern, int[] edges)
        {
            int p = 0;
            for (int i = 0; i < 8; i++)
            {
                p += pattern[i];
            }

            int E1 = Convert.ToInt32(Math.Round((pattern[0] + pattern[1]) * (17 / (float)p)));
            int E2 = Convert.ToInt32(Math.Round((pattern[1] + pattern[2]) * (17 / (float)p)));
            int E3 = Convert.ToInt32(Math.Round((pattern[2] + pattern[3]) * (17 / (float)p)));
            int E4 = Convert.ToInt32(Math.Round((pattern[3] + pattern[4]) * (17 / (float)p)));
            int E5 = Convert.ToInt32(Math.Round((pattern[4] + pattern[5]) * (17 / (float)p)));
            int E6 = Convert.ToInt32(Math.Round((pattern[5] + pattern[6]) * (17 / (float)p)));

            if (E1 < 2 || E1 > 9 || E2 < 2 || E2 > 9 || E3 < 2 || E3 > 9 || E4 < 2 || E4 > 9 || E5 < 2 || E5 > 9 || E6 < 2 || E6 > 9)
            {
                return false;
            }

            edges[0] = E1; edges[1] = E2; edges[2] = E3; edges[3] = E4; edges[4] = E5; edges[5] = E6;
            return true;
        }

        public static bool CheckPatternIsStartLocator(int[] pattern)
        {
            int[] edges = new int[6];
            if (GetPatternEdges(pattern, edges))
            {
                return (edges[0] == 9 && edges[1] == 2 && edges[2] == 2 && edges[3] == 2 && edges[4] == 2 && edges[5] == 2);
            }
            return false;
        }

        public static bool CheckPatternIsEndLocator(int[] pattern)
        {
            int[] edges = new int[6];
            if (GetPatternEdges(pattern, edges))
            {
                return (edges[0] == 8 && edges[1] == 2 && edges[2] == 4 && edges[3] == 4 && edges[4] == 2 && edges[5] == 2);
            }
            return false;
        }

        public  static int GetPatternSizeInPixels(int[] pattern)
        {
            int length = 0;
            for (int i = 0; i < 8; i++)
            {
                length += pattern[i];
            }
            return length;
        }

        public static bool CheckPatternBlockLengths(int[] pattern)
        {
            for (int i = 0; i < 8; i++)
            {
                if (pattern[i] <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static int GetCluster(int[] edges)
        {
            int K = (edges[0] - edges[1] + edges[4] - edges[5] + 9) % 9;
            if (!(K == 3 || K == 0 || K == 6))
            {
                K = -1;
            }
            return K;
        }
    }
}
