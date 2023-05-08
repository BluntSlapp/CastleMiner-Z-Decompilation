using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public class FPSControllerMapping : ControllerMapping
	{
		public bool InvertY;

		public float Sensitivity = 1f;

		public Vector2 Movement;

		public Vector2 Aiming;

		public Trigger Fire;

		public Trigger Jump;

		public override void ProcessInput(GameController controller)
		{
			Movement = controller.CurrentState.ThumbSticks.Left;
			Aiming = controller.CurrentState.ThumbSticks.Right;
			if (InvertY)
			{
				Aiming.Y = 0f - Aiming.Y;
			}
			Aiming *= Sensitivity;
			Jump.Pressed = controller.PressedButtons.A;
			Jump.Released = controller.ReleasedButtons.A;
			Jump.Held = controller.CurrentState.Buttons.A == ButtonState.Pressed;
			Fire.Pressed = controller.CurrentState.Triggers.Right > 0.5f && controller.LastState.Triggers.Right <= 0.5f;
			Fire.Released = controller.CurrentState.Triggers.Right < 0.5f && controller.LastState.Triggers.Right >= 0.5f;
			Fire.Held = controller.CurrentState.Triggers.Right > 0.5f;
		}
	}
}
