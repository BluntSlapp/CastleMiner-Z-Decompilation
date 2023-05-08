using System;
using System.IO;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class PlayerUpdateMessage : CastleMinerZMessage
	{
		public Vector3 LocalPosition;

		public Quaternion LocalRotation;

		public Vector3 WorldVelocity;

		public Vector2 Movement;

		public Angle TorsoPitch;

		public bool Using;

		public bool Dead;

		public bool Shouldering;

		public bool Reloading;

		public PlayerMode PlayerMode;

		public override bool Echo
		{
			get
			{
				return false;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				if (Using || Reloading)
				{
					return SendDataOptions.ReliableInOrder;
				}
				return SendDataOptions.InOrder;
			}
		}

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.PlayerUpdate;
			}
		}

		private PlayerUpdateMessage()
		{
		}

		public void Apply(Player player)
		{
			player.LocalPosition = LocalPosition;
			player.PlayerPhysics.WorldVelocity = WorldVelocity;
			player.LocalRotation = LocalRotation;
			if (!player.IsLocal)
			{
				player.Shouldering = Shouldering;
				player.Reloading = Reloading;
				player.UsingTool = Using;
				player.Dead = Dead;
				player._playerMode = PlayerMode;
			}
			if (player.Dead)
			{
				player.UpdateAnimation(0f, 0f, Angle.Zero, PlayerMode, Using);
			}
			else
			{
				player.UpdateAnimation(Movement.Y, Movement.X, TorsoPitch, PlayerMode, Using);
			}
		}

		public static void Send(LocalNetworkGamer from, Player player, CastleMinerZControllerMapping input)
		{
			PlayerUpdateMessage sendInstance = Message.GetSendInstance<PlayerUpdateMessage>();
			sendInstance.LocalPosition = player.LocalPosition;
			sendInstance.WorldVelocity = player.PlayerPhysics.WorldVelocity;
			sendInstance.LocalRotation = player.LocalRotation;
			sendInstance.Movement = input.Movement;
			sendInstance.TorsoPitch = player.TorsoPitch;
			sendInstance.Using = player.UsingTool;
			sendInstance.Shouldering = player.Shouldering;
			sendInstance.Reloading = player.Reloading;
			sendInstance.PlayerMode = player._playerMode;
			sendInstance.Dead = player.Dead;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			LocalPosition = reader.ReadVector3();
			byte b = reader.ReadByte();
			byte b2 = (byte)(b & 0xFu);
			byte b3 = (byte)(b >> 4);
			LocalRotation = Quaternion.CreateFromYawPitchRoll(Angle.FromRevolutions((float)(int)b2 / 15f).Radians, 0f, 0f);
			TorsoPitch = Angle.FromRevolutions((float)(int)b3 / 30f) - Angle.FromDegrees(90f);
			byte b4 = reader.ReadByte();
			PlayerMode = (PlayerMode)reader.ReadByte();
			Using = (b4 & 1) != 0;
			Dead = (b4 & 2) != 0;
			Shouldering = (b4 & 4) != 0;
			Reloading = (b4 & 8) != 0;
			byte b5 = reader.ReadByte();
			byte b6 = (byte)(b5 & 0xFu);
			byte b7 = (byte)(b5 >> 4);
			Movement.X = (float)(int)b6 / 14f * 2f - 1f;
			Movement.Y = (float)(int)b7 / 14f * 2f - 1f;
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = reader.ReadUInt16();
			WorldVelocity.X = halfSingle.ToSingle();
			halfSingle.PackedValue = reader.ReadUInt16();
			WorldVelocity.Y = halfSingle.ToSingle();
			halfSingle.PackedValue = reader.ReadUInt16();
			WorldVelocity.Z = halfSingle.ToSingle();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(LocalPosition);
			float revolutions = new EulerAngle(LocalRotation).Yaw.Revolutions;
			revolutions -= (float)Math.Floor(revolutions);
			byte b = (byte)Math.Round(15f * revolutions);
			float num = (TorsoPitch + Angle.FromDegrees(90f)).Degrees / 180f;
			num -= (float)Math.Floor(revolutions);
			byte b2 = (byte)Math.Round(15f * num);
			byte b3 = 0;
			if (Using)
			{
				b3 = (byte)(b3 | 1u);
			}
			if (Dead)
			{
				b3 = (byte)(b3 | 2u);
			}
			if (Shouldering)
			{
				b3 = (byte)(b3 | 4u);
			}
			if (Reloading)
			{
				b3 = (byte)(b3 | 8u);
			}
			writer.Write((byte)((b2 << 4) | b));
			writer.Write(b3);
			writer.Write((byte)PlayerMode);
			byte b4 = (byte)((Movement.X + 1f) / 2f * 14f);
			byte b5 = (byte)((Movement.Y + 1f) / 2f * 14f);
			byte value = (byte)((b5 << 4) | b4);
			writer.Write(value);
			writer.Write(new HalfSingle(WorldVelocity.X).PackedValue);
			writer.Write(new HalfSingle(WorldVelocity.Y).PackedValue);
			writer.Write(new HalfSingle(WorldVelocity.Z).PackedValue);
		}
	}
}
