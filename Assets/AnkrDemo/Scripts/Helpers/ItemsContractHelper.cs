namespace AnkrDemo.Helpers
{
	public static class ItemsContractHelper
	{
		public const string BlueHatAddress = "0x00010000000000000000000000000000000000000000000000000000000001";
		public const string RedHatAddress = "0x00010000000000000000000000000000000000000000000000000000000002";
		public const string BlueShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000001";
		public const string WhiteShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000003";
		public const string RedGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000002";
		public const string WhiteGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000003";
		
		public static bool TryConvertToHatColour(this string itemAddress, out HatColour hatColour)
		{
			switch (itemAddress)
			{
				case "0x10000000000000000000000000000000000000000000000000000000001":
					hatColour = HatColour.Blue;
					return true;
				case "0x10000000000000000000000000000000000000000000000000000000002":
					hatColour = HatColour.Red;
					return true;
			}

			hatColour = default;
			return false;
		}
	}
}