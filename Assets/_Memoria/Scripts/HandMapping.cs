using Gamelogic;

namespace Memoria
{
	public enum HandMap
	{
		LeftIndex,
		RightIndex,
		LeftMiddle,
		RightMiddle,
		LeftThumb,
		RightThumb,
		LeftPinky,
		RightPinky,
		LeftRing,
		RightRing,
		LeftPalm,
		RightPalm
	}

	public class HandMapping : GLMonoBehaviour
	{
		public HandMap handMap;
	}
}
