using DNA.Drawing;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA.Avatars.Actions
{
	public class SetExpressionAction : State
	{
		private AvatarExpression _expression;

		public SetExpressionAction(AvatarExpression expression)
		{
			_expression = expression;
		}

		protected override void OnStart(Entity entity)
		{
			Avatar avatar = (Avatar)entity;
			avatar.Expression = _expression;
			base.OnStart(entity);
		}
	}
}
