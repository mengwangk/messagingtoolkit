using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 18 2012
    /// </summary>
    public class ExpandedProductParsedResult : ParsedResult
    {
        public const String KILOGRAM = "KG";
        public const String POUND = "LB";

        private readonly String productID;
        private readonly String sscc;
        private readonly String lotNumber;
        private readonly String productionDate;
        private readonly String packagingDate;
        private readonly String bestBeforeDate;
        private readonly String expirationDate;
        private readonly String weight;
        private readonly String weightType;
        private readonly String weightIncrement;
        private readonly String price;
        private readonly String priceIncrement;
        private readonly String priceCurrency;

        // For AIS that not exist in this object
        private readonly Dictionary<String, String> uncommonAIs;

        public ExpandedProductParsedResult(String productID, String sscc, String lotNumber, String productionDate, String packagingDate, String bestBeforeDate, 
                String expirationDate, String weight,
                String weightType, String weightIncrement, String price, String priceIncrement, String priceCurrency, Dictionary<String, String> uncommonAIs)
            : base(ParsedResultType.Product)
        {
            this.productID = productID;
            this.sscc = sscc;
            this.lotNumber = lotNumber;
            this.productionDate = productionDate;
            this.packagingDate = packagingDate;
            this.bestBeforeDate = bestBeforeDate;
            this.expirationDate = expirationDate;
            this.weight = weight;
            this.weightType = weightType;
            this.weightIncrement = weightIncrement;
            this.price = price;
            this.priceIncrement = priceIncrement;
            this.priceCurrency = priceCurrency;
            this.uncommonAIs = uncommonAIs;
        }

        public override bool Equals(Object o)
        {
            if (!(o is ExpandedProductParsedResult))
            {
                return false;
            }

            ExpandedProductParsedResult other = (ExpandedProductParsedResult)o;

            return EqualsOrNull(productID, other.productID) && EqualsOrNull(sscc, other.sscc) && EqualsOrNull(lotNumber, other.lotNumber) && EqualsOrNull(productionDate, other.productionDate)
                    && EqualsOrNull(bestBeforeDate, other.bestBeforeDate) && EqualsOrNull(expirationDate, other.expirationDate) && EqualsOrNull(weight, other.weight)
                    && EqualsOrNull(weightType, other.weightType) && EqualsOrNull(weightIncrement, other.weightIncrement) && EqualsOrNull(price, other.price)
                    && EqualsOrNull(priceIncrement, other.priceIncrement) && EqualsOrNull(priceCurrency, other.priceCurrency) && EqualsOrNull(uncommonAIs, other.uncommonAIs);
        }

        private static bool EqualsOrNull(Object o1, Object o2)
        {
            return (o1 == null) ? o2 == null : o1.Equals(o2);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= HashNotNull(productID);
            hash ^= HashNotNull(sscc);
            hash ^= HashNotNull(lotNumber);
            hash ^= HashNotNull(productionDate);
            hash ^= HashNotNull(bestBeforeDate);
            hash ^= HashNotNull(expirationDate);
            hash ^= HashNotNull(weight);
            hash ^= HashNotNull(weightType);
            hash ^= HashNotNull(weightIncrement);
            hash ^= HashNotNull(price);
            hash ^= HashNotNull(priceIncrement);
            hash ^= HashNotNull(priceCurrency);
            hash ^= HashNotNull(uncommonAIs);
            return hash;
        }

        private static int HashNotNull(Object o)
        {
            return (o == null) ? 0 : o.GetHashCode();
        }

        public String GetProductID()
        {
            return productID;
        }

        public String GetSscc()
        {
            return sscc;
        }

        public String GetLotNumber()
        {
            return lotNumber;
        }

        public String GetProductionDate()
        {
            return productionDate;
        }

        public String GetPackagingDate()
        {
            return packagingDate;
        }

        public String GetBestBeforeDate()
        {
            return bestBeforeDate;
        }

        public String GetExpirationDate()
        {
            return expirationDate;
        }

        public String GetWeight()
        {
            return weight;
        }

        public String GetWeightType()
        {
            return weightType;
        }

        public String GetWeightIncrement()
        {
            return weightIncrement;
        }

        public String GetPrice()
        {
            return price;
        }

        public String GetPriceIncrement()
        {
            return priceIncrement;
        }

        public String GetPriceCurrency()
        {
            return priceCurrency;
        }

        public Dictionary<String, String> GetUncommonAIs()
        {
            return uncommonAIs;
        }

        public override String DisplayResult
        {
            get
            {
                return productID.ToString();
            }
        }
    }
}
