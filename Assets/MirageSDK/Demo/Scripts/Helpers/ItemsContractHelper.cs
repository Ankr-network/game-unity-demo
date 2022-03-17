namespace MirageSDK.Demo.Helpers
{
	public static class ItemsContractHelper
	{
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