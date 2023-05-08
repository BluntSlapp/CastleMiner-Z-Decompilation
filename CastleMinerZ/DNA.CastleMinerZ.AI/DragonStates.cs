using System.Runtime.InteropServices;

namespace DNA.CastleMinerZ.AI
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DragonStates
	{
		public static DragonBaseState Default = new DragonDefaultState();

		public static DragonBaseState Loiter = new DragonLoiterState();

		public static DragonBaseState LoiterLeft = new DragonLoiterLeftState();

		public static DragonBaseState LoiterRight = new DragonLoiterRightState();

		public static DragonBaseState Hover = new DragonHoverState();

		public static DragonBaseState HoverAttack = new DragonHoverAttackState();

		public static DragonBaseState StrafeAttack = new DragonStrafeAttackState();
	}
}
