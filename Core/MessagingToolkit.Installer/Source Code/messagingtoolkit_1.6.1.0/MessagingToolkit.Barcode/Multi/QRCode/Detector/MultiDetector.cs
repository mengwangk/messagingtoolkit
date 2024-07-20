using System.Collections.Generic;
using DetectorResult = MessagingToolkit.Barcode.Common.DetectorResult;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;
using FinderPatternInfo = MessagingToolkit.Barcode.QRCode.Detector.FinderPatternInfo;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.Multi.QRCode.Detector
{

    /// <summary>
    /// Encapsulates logic that can detect one or more QR Codes in an image, even if the QR Code
    /// is rotated or skewed, or partially obscured.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public sealed class MultiDetector : MessagingToolkit.Barcode.QRCode.Detector.Detector
    {
        private static readonly DetectorResult[] EMPTY_DETECTOR_RESULTS = new DetectorResult[0];

        public MultiDetector(BitMatrix image)
            : base(image)
        {
        }

        public DetectorResult[] DetectMulti(Dictionary<DecodeOptions, object> decodingOptions)
        {
            BitMatrix image = this.Image;
            ResultPointCallback resultPointCallback = (decodingOptions == null) ? null
                        : (ResultPointCallback)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.NeedResultPointCallback);
            MultiFinderPatternFinder finder = new MultiFinderPatternFinder(image, resultPointCallback);
            FinderPatternInfo[] infos = finder.FindMulti(decodingOptions);

            if (infos.Length == 0)
            {
                throw NotFoundException.Instance;
            }

            IList<DetectorResult> result = new List<DetectorResult>();
            /* foreach */
            foreach (FinderPatternInfo info in infos)
            {
                try
                {
                    result.Add(ProcessFinderPatternInfo(info));
                }
                catch (BarcodeDecoderException e)
                {
                }
            }
            if ((result.Count == 0))
            {
                return EMPTY_DETECTOR_RESULTS;
            }
            else
            {
                DetectorResult[] resultArray = new DetectorResult[result.Count];
                for (int i = 0; i < result.Count; i++)
                {
                    resultArray[i] = (DetectorResult)result[i];
                }
                return resultArray;
            }
        }
    }
}
