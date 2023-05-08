using System;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class DoorEntity : Entity
	{
		public static Model _doorModel;

		private bool _xAxis;

		private ModelEntity _modelEnt;

		private bool _doorOpen;

		public bool DoorOpen
		{
			get
			{
				return _doorOpen;
			}
		}

		static DoorEntity()
		{
			_doorModel = CastleMinerZGame.Instance.Content.Load<Model>("WoodDoor");
		}

		public DoorEntity()
		{
			_modelEnt = new ModelEntity(_doorModel);
			base.Children.Add(_modelEnt);
			SetPosition(BlockTypeEnum.LowerDoorClosedX);
		}

		public void SetPosition(BlockTypeEnum doorType)
		{
			switch (doorType)
			{
			case BlockTypeEnum.LowerDoorClosedX:
				_xAxis = true;
				_doorOpen = false;
				break;
			case BlockTypeEnum.LowerDoorClosedZ:
				_xAxis = false;
				_doorOpen = false;
				break;
			case BlockTypeEnum.LowerDoorOpenX:
				_xAxis = true;
				_doorOpen = true;
				break;
			case BlockTypeEnum.LowerDoorOpenZ:
				_xAxis = false;
				_doorOpen = true;
				break;
			}
			if (_xAxis)
			{
				if (_doorOpen)
				{
					_modelEnt.LocalPosition = new Vector3(-0.5f, -0.5f, 0f);
					_modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 2f);
				}
				else
				{
					_modelEnt.LocalPosition = new Vector3(-0.5f, -0.5f, 0f);
					_modelEnt.LocalRotation = Quaternion.Identity;
				}
			}
			else if (_doorOpen)
			{
				_modelEnt.LocalPosition = new Vector3(0f, -0.5f, -0.5f);
				_modelEnt.LocalRotation = Quaternion.Identity;
			}
			else
			{
				_modelEnt.LocalPosition = new Vector3(0f, -0.5f, -0.5f);
				_modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -(float)Math.PI / 2f);
			}
		}
	}
}
