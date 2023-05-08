using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ
{
	internal static class CMZColors
	{
		public static readonly Color Wood = Color.SaddleBrown;

		public static readonly Color Copper = new Color(184, 115, 51);

		public static readonly Color Coal = Color.Black;

		public static readonly Color Stone = Color.DarkGray;

		public static readonly Color Brass = new Color(234, 227, 150);

		public static readonly Color Iron = new Color(128, 128, 128);

		public static readonly Color Gold = new Color(255, 215, 0);

		public static readonly Color Diamond = Color.Cyan;

		public static readonly Color BloodStone = Color.DarkRed;

		public static readonly Color CopperOre = new Color(99, 146, 131);

		public static readonly Color IronOre = new Color(183, 65, 14);

		public static Color GetMaterialcColor(ToolMaterialTypes mat)
		{
			switch (mat)
			{
			case ToolMaterialTypes.Wood:
				return Wood;
			case ToolMaterialTypes.Stone:
				return Stone;
			case ToolMaterialTypes.Copper:
				return Copper;
			case ToolMaterialTypes.Iron:
				return Iron;
			case ToolMaterialTypes.Gold:
				return Gold;
			case ToolMaterialTypes.Diamond:
				return Diamond;
			case ToolMaterialTypes.BloodStone:
				return BloodStone;
			default:
				return Color.White;
			}
		}
	}
}
