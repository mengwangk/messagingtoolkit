using System;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 18 2012
    /// </summary>
    internal sealed class ProductParsedResult : ParsedResult
	{
        private string productID;
        private string normalizedProductID;
		
		public string ProductID
		{
			get
			{
				return productID;
			}
			
		}
		public string NormalizedProductID
		{
			get
			{
				return normalizedProductID;
			}
			
		}
		override public string DisplayResult
		{
			get
			{
				return productID;
			}
			
		}
		
		
		internal ProductParsedResult(string productID):this(productID, productID)
		{
		}
		
		internal ProductParsedResult(string productID, string normalizedProductID):base(ParsedResultType.Product)
		{
			this.productID = productID;
			this.normalizedProductID = normalizedProductID;
		}
	}
}