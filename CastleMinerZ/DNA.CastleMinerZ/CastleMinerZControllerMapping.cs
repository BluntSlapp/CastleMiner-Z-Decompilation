using DNA.Input;
using Microsoft.Xna.Framework.Input;

namespace DNA.CastleMinerZ
{
	public class CastleMinerZControllerMapping : FPSControllerMapping
	{
		public Trigger Shoulder;

		public Trigger Use;

		public Trigger Reload;

		public Trigger NextItem;

		public Trigger PrevoiusItem;

		public Trigger BlockUI;

		public Trigger PlayersScreen;

		public Trigger NoFallMode;

		public Trigger FlyMode;

		public Trigger Activate;

		public override void ProcessInput(GameController controller)
		{
			PrevoiusItem.Pressed = controller.PressedButtons.LeftShoulder || controller.PressedDPad.Left;
			PrevoiusItem.Released = controller.ReleasedButtons.LeftShoulder || controller.ReleasedDPad.Left;
			PrevoiusItem.Held = controller.CurrentState.IsButtonDown(Buttons.LeftShoulder) || controller.CurrentState.DPad.Left == ButtonState.Pressed;
			NextItem.Pressed = controller.PressedButtons.RightShoulder || controller.PressedDPad.Right;
			NextItem.Released = controller.ReleasedButtons.RightShoulder || controller.ReleasedDPad.Right;
			NextItem.Held = controller.CurrentState.IsButtonDown(Buttons.RightShoulder) || controller.CurrentState.DPad.Right == ButtonState.Pressed;
			BlockUI.Pressed = controller.PressedButtons.Y;
			BlockUI.Released = controller.PressedButtons.Y;
			BlockUI.Held = controller.CurrentState.IsButtonDown(Buttons.Y);
			Activate.Pressed = controller.PressedButtons.B;
			Activate.Released = controller.PressedButtons.B;
			Activate.Held = controller.CurrentState.IsButtonDown(Buttons.B);
			Shoulder.Pressed = controller.PressedButtons.LeftTrigger;
			Shoulder.Released = controller.ReleasedButtons.LeftTrigger;
			Shoulder.Held = controller.CurrentState.IsButtonDown(Buttons.LeftTrigger);
			Use.Pressed = controller.PressedButtons.RightTrigger;
			Use.Released = controller.ReleasedButtons.RightTrigger;
			Use.Held = controller.CurrentState.IsButtonDown(Buttons.RightTrigger);
			Reload.Pressed = controller.PressedButtons.X;
			Reload.Released = controller.ReleasedButtons.X;
			Reload.Held = controller.CurrentState.IsButtonDown(Buttons.X);
			PlayersScreen.Pressed = controller.PressedButtons.Back;
			PlayersScreen.Released = controller.ReleasedButtons.Back;
			PlayersScreen.Held = controller.CurrentState.IsButtonDown(Buttons.Back);
			NoFallMode.Pressed = controller.PressedButtons.LeftStick;
			NoFallMode.Released = controller.ReleasedButtons.LeftStick;
			NoFallMode.Held = controller.CurrentState.IsButtonDown(Buttons.LeftStick);
			FlyMode.Pressed = controller.PressedButtons.RightStick;
			FlyMode.Released = controller.ReleasedButtons.RightStick;
			FlyMode.Held = controller.CurrentState.IsButtonDown(Buttons.RightStick);
			base.ProcessInput(controller);
		}
	}
}
