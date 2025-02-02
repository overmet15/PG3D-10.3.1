using com.amazon.device.iap.cpt;

namespace Rilisoft
{
	internal sealed class AmazonMarketProduct : IMarketProduct
	{
		private readonly ProductData _marketProduct;

		public string Id
		{
			get
			{
				return _marketProduct.Sku;
			}
		}

		public string Title
		{
			get
			{
				return _marketProduct.Title;
			}
		}

		public string Description
		{
			get
			{
				return _marketProduct.Description;
			}
		}

		public string Price
		{
			get
			{
				return _marketProduct.Price;
			}
		}

		public AmazonMarketProduct(ProductData amazonItem)
		{
			_marketProduct = amazonItem;
		}

		public override string ToString()
		{
			return _marketProduct.ToString();
		}

		public override bool Equals(object obj)
		{
			AmazonMarketProduct amazonMarketProduct = obj as AmazonMarketProduct;
			if (amazonMarketProduct == null)
			{
				return false;
			}
			ProductData marketProduct = amazonMarketProduct._marketProduct;
			return _marketProduct.Equals(marketProduct);
		}

		public override int GetHashCode()
		{
			return _marketProduct.GetHashCode();
		}
	}
}
