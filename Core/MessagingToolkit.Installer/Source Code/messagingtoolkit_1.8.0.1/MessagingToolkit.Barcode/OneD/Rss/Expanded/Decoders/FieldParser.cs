using System;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class FieldParser
    {

        private static readonly Object VariableLength = new Object();

        private static readonly Object[][] TwoDigitDataLength = { new Object[] { "00", ((int)(18)) },
				new Object[] { "01", ((int)(14)) },
				new Object[] { "02", ((int)(14)) },
				new Object[] { "10", VariableLength, ((int)(20)) },
				new Object[] { "11", ((int)(6)) },
				new Object[] { "12", ((int)(6)) },
				new Object[] { "13", ((int)(6)) },
				new Object[] { "15", ((int)(6)) },
				new Object[] { "17", ((int)(6)) },
				new Object[] { "20", ((int)(2)) },
				new Object[] { "21", VariableLength, ((int)(20)) },
				new Object[] { "22", VariableLength, ((int)(29)) },
				new Object[] { "30", VariableLength, ((int)(8)) },
				new Object[] { "37", VariableLength, ((int)(8)) },
				new Object[] { "90", VariableLength, ((int)(30)) },
				new Object[] { "91", VariableLength, ((int)(30)) },
				new Object[] { "92", VariableLength, ((int)(30)) },
				new Object[] { "93", VariableLength, ((int)(30)) },
				new Object[] { "94", VariableLength, ((int)(30)) },
				new Object[] { "95", VariableLength, ((int)(30)) },
				new Object[] { "96", VariableLength, ((int)(30)) },
				new Object[] { "97", VariableLength, ((int)(30)) },
				new Object[] { "98", VariableLength, ((int)(30)) },
				new Object[] { "99", VariableLength, ((int)(30)) } };

        private static readonly Object[][] ThreeDigitDataLength = { new Object[] { "240", VariableLength, ((int)(30)) },
				new Object[] { "241", VariableLength, ((int)(30)) },
				new Object[] { "242", VariableLength, ((int)(6)) },
				new Object[] { "250", VariableLength, ((int)(30)) },
				new Object[] { "251", VariableLength, ((int)(30)) },
				new Object[] { "253", VariableLength, ((int)(17)) },
				new Object[] { "254", VariableLength, ((int)(20)) },
				new Object[] { "400", VariableLength, ((int)(30)) },
				new Object[] { "401", VariableLength, ((int)(30)) },
				new Object[] { "402", ((int)(17)) },
				new Object[] { "403", VariableLength, ((int)(30)) },
				new Object[] { "410", ((int)(13)) },
				new Object[] { "411", ((int)(13)) },
				new Object[] { "412", ((int)(13)) },
				new Object[] { "413", ((int)(13)) },
				new Object[] { "414", ((int)(13)) },
				new Object[] { "420", VariableLength, ((int)(20)) },
				new Object[] { "421", VariableLength, ((int)(15)) },
				new Object[] { "422", ((int)(3)) },
				new Object[] { "423", VariableLength, ((int)(15)) },
				new Object[] { "424", ((int)(3)) },
				new Object[] { "425", ((int)(3)) },
				new Object[] { "426", ((int)(3)) } };

        private static readonly Object[][] ThreeDigitPlusDigitDataLength = { new Object[] { "310", ((int)(6)) },
				new Object[] { "311", ((int)(6)) },
				new Object[] { "312", ((int)(6)) },
				new Object[] { "313", ((int)(6)) },
				new Object[] { "314", ((int)(6)) },
				new Object[] { "315", ((int)(6)) },
				new Object[] { "316", ((int)(6)) },
				new Object[] { "320", ((int)(6)) },
				new Object[] { "321", ((int)(6)) },
				new Object[] { "322", ((int)(6)) },
				new Object[] { "323", ((int)(6)) },
				new Object[] { "324", ((int)(6)) },
				new Object[] { "325", ((int)(6)) },
				new Object[] { "326", ((int)(6)) },
				new Object[] { "327", ((int)(6)) },
				new Object[] { "328", ((int)(6)) },
				new Object[] { "329", ((int)(6)) },
				new Object[] { "330", ((int)(6)) },
				new Object[] { "331", ((int)(6)) },
				new Object[] { "332", ((int)(6)) },
				new Object[] { "333", ((int)(6)) },
				new Object[] { "334", ((int)(6)) },
				new Object[] { "335", ((int)(6)) },
				new Object[] { "336", ((int)(6)) },
				new Object[] { "340", ((int)(6)) },
				new Object[] { "341", ((int)(6)) },
				new Object[] { "342", ((int)(6)) },
				new Object[] { "343", ((int)(6)) },
				new Object[] { "344", ((int)(6)) },
				new Object[] { "345", ((int)(6)) },
				new Object[] { "346", ((int)(6)) },
				new Object[] { "347", ((int)(6)) },
				new Object[] { "348", ((int)(6)) },
				new Object[] { "349", ((int)(6)) },
				new Object[] { "350", ((int)(6)) },
				new Object[] { "351", ((int)(6)) },
				new Object[] { "352", ((int)(6)) },
				new Object[] { "353", ((int)(6)) },
				new Object[] { "354", ((int)(6)) },
				new Object[] { "355", ((int)(6)) },
				new Object[] { "356", ((int)(6)) },
				new Object[] { "357", ((int)(6)) },
				new Object[] { "360", ((int)(6)) },
				new Object[] { "361", ((int)(6)) },
				new Object[] { "362", ((int)(6)) },
				new Object[] { "363", ((int)(6)) },
				new Object[] { "364", ((int)(6)) },
				new Object[] { "365", ((int)(6)) },
				new Object[] { "366", ((int)(6)) },
				new Object[] { "367", ((int)(6)) },
				new Object[] { "368", ((int)(6)) },
				new Object[] { "369", ((int)(6)) },
				new Object[] { "390", VariableLength, ((int)(15)) },
				new Object[] { "391", VariableLength, ((int)(18)) },
				new Object[] { "392", VariableLength, ((int)(15)) },
				new Object[] { "393", VariableLength, ((int)(18)) },
				new Object[] { "703", VariableLength, ((int)(30)) } };

        private static readonly Object[][] FourDigitDataLength = { new Object[] { "7001", ((int)(13)) },
				new Object[] { "7002", VariableLength, ((int)(30)) },
				new Object[] { "7003", ((int)(10)) },
				new Object[] { "8001", ((int)(14)) },
				new Object[] { "8002", VariableLength, ((int)(20)) },
				new Object[] { "8003", VariableLength, ((int)(30)) },
				new Object[] { "8004", VariableLength, ((int)(30)) },
				new Object[] { "8005", ((int)(6)) },
				new Object[] { "8006", ((int)(18)) },
				new Object[] { "8007", VariableLength, ((int)(30)) },
				new Object[] { "8008", VariableLength, ((int)(12)) },
				new Object[] { "8018", ((int)(18)) },
				new Object[] { "8020", VariableLength, ((int)(25)) },
				new Object[] { "8100", ((int)(6)) },
				new Object[] { "8101", ((int)(10)) },
				new Object[] { "8102", ((int)(2)) },
				new Object[] { "8110", VariableLength, ((int)(70)) },
				new Object[] { "8200", VariableLength, ((int)(70)) } 
              };

        private FieldParser()
        {
        }

        static internal String ParseFieldsInGeneralPurpose(String rawInformation)
        {
            if (string.IsNullOrEmpty(rawInformation))
            {
                return null;
            }

            // Processing 2-digit AIs

            if (rawInformation.Length < 2)
            {
                throw NotFoundException.Instance;
            }

            String firstTwoDigits = rawInformation.Substring(0, (2) - (0));

            for (int i = 0; i < TwoDigitDataLength.Length; ++i)
            {
                if (TwoDigitDataLength[i][0].Equals(firstTwoDigits))
                {
                    if (TwoDigitDataLength[i][1] == VariableLength)
                    {
                        return ProcessVariableAI(2,
                                ((Int32)TwoDigitDataLength[i][2]),
                                rawInformation);
                    }
                    return ProcessFixedAI(2,
                            ((Int32)TwoDigitDataLength[i][1]),
                            rawInformation);
                }
            }

            if (rawInformation.Length < 3)
            {
                throw NotFoundException.Instance;
            }

            String firstThreeDigits = rawInformation.Substring(0, (3) - (0));

            for (int i_0 = 0; i_0 < ThreeDigitDataLength.Length; ++i_0)
            {
                if (ThreeDigitDataLength[i_0][0].Equals(firstThreeDigits))
                {
                    if (ThreeDigitDataLength[i_0][1] == VariableLength)
                    {
                        return ProcessVariableAI(3,
                                ((Int32)ThreeDigitDataLength[i_0][2]), rawInformation);
                    }
                    return ProcessFixedAI(3,
                            ((Int32)ThreeDigitDataLength[i_0][1]),
                            rawInformation);
                }
            }

            for (int i_1 = 0; i_1 < ThreeDigitPlusDigitDataLength.Length; ++i_1)
            {
                if (ThreeDigitPlusDigitDataLength[i_1][0]
                        .Equals(firstThreeDigits))
                {
                    if (ThreeDigitPlusDigitDataLength[i_1][1] == VariableLength)
                    {
                        return ProcessVariableAI(
                                4,
                                ((Int32)ThreeDigitPlusDigitDataLength[i_1][2]), rawInformation);
                    }
                    return ProcessFixedAI(4,
                            ((Int32)ThreeDigitPlusDigitDataLength[i_1][1]), rawInformation);
                }
            }

            if (rawInformation.Length < 4)
            {
                throw NotFoundException.Instance;
            }

            String firstFourDigits = rawInformation.Substring(0, (4) - (0));

            for (int i_2 = 0; i_2 < FourDigitDataLength.Length; ++i_2)
            {
                if (FourDigitDataLength[i_2][0].Equals(firstFourDigits))
                {
                    if (FourDigitDataLength[i_2][1] == VariableLength)
                    {
                        return ProcessVariableAI(
                                4,
                                ((Int32)FourDigitDataLength[i_2][2]),
                                rawInformation);
                    }
                    return ProcessFixedAI(4,
                            ((Int32)FourDigitDataLength[i_2][1]),
                            rawInformation);
                }
            }

            throw NotFoundException.Instance;
        }

        private static String ProcessFixedAI(int aiSize, int fieldSize, String rawInformation)
        {
            if (rawInformation.Length < aiSize)
            {
                throw NotFoundException.Instance;
            }

            String ai = rawInformation.Substring(0, (aiSize) - (0));

            if (rawInformation.Length < aiSize + fieldSize)
            {
                throw NotFoundException.Instance;
            }

            String field = rawInformation.Substring(aiSize, (aiSize + fieldSize) - (aiSize));
            String remaining = rawInformation.Substring(aiSize + fieldSize);
            return '(' + ai + ')' + field + ParseFieldsInGeneralPurpose(remaining);
        }

        private static String ProcessVariableAI(int aiSize, int variableFieldSize, String rawInformation)
        {
            String ai = rawInformation.Substring(0, (aiSize) - (0));
            int maxSize;
            if (rawInformation.Length < aiSize + variableFieldSize)
            {
                maxSize = rawInformation.Length;
            }
            else
            {
                maxSize = aiSize + variableFieldSize;
            }
            String field = rawInformation.Substring(aiSize, (maxSize) - (aiSize));
            String remaining = rawInformation.Substring(maxSize);
            return '(' + ai + ')' + field + ParseFieldsInGeneralPurpose(remaining);
        }
    }
}
