using System;
using System.Collections.Generic;
using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DNA.Drawing.Animation
{
	public class AnimationClip
	{
		public class Reader : ContentTypeReader<AnimationClip>
		{
			protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
			{
				AnimationClip animationClip = new AnimationClip();
				animationClip.Read((BinaryReader)(object)input);
				return animationClip;
			}
		}

		private int _animationFrameRate = 30;

		protected Vector3[][] _positions;

		protected Quaternion[][] _rotations;

		protected Vector3[][] _scales;

		private int _frameRate;

		public string Name { get; private set; }

		public TimeSpan Duration { get; private set; }

		public int BoneCount
		{
			get
			{
				return _positions.Length;
			}
		}

		public void Resample(int frameRate)
		{
			Matrix[] array = new Matrix[BoneCount];
			List<Vector3>[] array2 = new List<Vector3>[BoneCount];
			List<Quaternion>[] array3 = new List<Quaternion>[BoneCount];
			List<Vector3>[] array4 = new List<Vector3>[BoneCount];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new List<Vector3>();
				array3[i] = new List<Quaternion>();
				array4[i] = new List<Vector3>();
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(1.0 / (double)frameRate);
			for (TimeSpan zero = TimeSpan.Zero; zero <= Duration; zero += timeSpan)
			{
				float num = (float)((double)_animationFrameRate * zero.TotalSeconds);
				int num2 = (int)num;
				float amount = num - (float)num2;
				for (int j = 0; j < array.Length; j++)
				{
					Quaternion[] array5 = _rotations[j];
					Quaternion quaternion;
					if (num2 >= array5.Length)
					{
						quaternion = array5[array5.Length - 1];
					}
					else
					{
						quaternion = array5[num2];
						if (num2 < array5.Length - 2)
						{
							quaternion = Quaternion.Slerp(quaternion, array5[num2 + 1], amount);
						}
					}
					Vector3[] array6 = _positions[j];
					Vector3 vector;
					if (num2 >= array6.Length)
					{
						vector = array6[array6.Length - 1];
					}
					else
					{
						vector = array6[num2];
						if (num2 < array6.Length - 2)
						{
							vector = Vector3.Lerp(vector, array6[num2 + 1], amount);
						}
					}
					Vector3[] array7 = _scales[j];
					Vector3 vector2;
					if (num2 >= array7.Length)
					{
						vector2 = array7[array7.Length - 1];
					}
					else
					{
						vector2 = array7[num2];
						if (num2 < array7.Length - 2)
						{
							vector2 = Vector3.Lerp(vector2, array7[num2 + 1], amount);
						}
					}
					array4[j].Add(vector2);
					array2[j].Add(vector);
					array3[j].Add(quaternion);
				}
			}
			_frameRate = frameRate;
			_scales = new Vector3[BoneCount][];
			_positions = new Vector3[BoneCount][];
			_rotations = new Quaternion[BoneCount][];
			for (int k = 0; k < array.Length; k++)
			{
				_scales[k] = array4[k].ToArray();
				_positions[k] = array2[k].ToArray();
				_rotations[k] = array3[k].ToArray();
			}
			ReduceKeys();
		}

		public void ReduceKeys()
		{
			for (int i = 0; i < BoneCount; i++)
			{
				bool flag = true;
				for (int j = 1; j < _positions[i].Length; j++)
				{
					if (_positions[i][0] != _positions[i][j])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					Vector3 vector = _positions[i][0];
					_positions[i] = new Vector3[1];
					_positions[i][0] = vector;
				}
				flag = true;
				for (int k = 1; k < _scales[i].Length; k++)
				{
					if (_scales[i][0] != _scales[i][k])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					Vector3 vector2 = _scales[i][0];
					_scales[i] = new Vector3[1];
					_scales[i][0] = vector2;
				}
				flag = true;
				for (int l = 1; l < _rotations[i].Length; l++)
				{
					if (_rotations[i][0] != _rotations[i][l])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					Quaternion quaternion = _rotations[i][0];
					_rotations[i] = new Quaternion[1];
					_rotations[i][0] = quaternion;
				}
			}
		}

		public AnimationClip(string name, int frameRate, TimeSpan duration, Vector3[][] positions, Quaternion[][] rotations, Vector3[][] scales)
		{
			Name = name;
			_frameRate = frameRate;
			Duration = duration;
			if (positions.Length != rotations.Length)
			{
				throw new Exception("Bone Counts Must be the same");
			}
			_positions = positions;
			_rotations = rotations;
			_scales = scales;
		}

		public AnimationClip(string name, int frameRate, TimeSpan duration, IList<IList<Matrix>> keys)
		{
			Name = name;
			_frameRate = frameRate;
			Duration = duration;
			_scales = new Vector3[keys.Count][];
			_positions = new Vector3[keys.Count][];
			_rotations = new Quaternion[keys.Count][];
			for (int i = 0; i < keys.Count; i++)
			{
				int count = keys[i].Count;
				_positions[i] = new Vector3[count];
				_scales[i] = new Vector3[count];
				_rotations[i] = new Quaternion[count];
				for (int j = 0; j < count; j++)
				{
					keys[i][j].Decompose(out _scales[i][j], out _rotations[i][j], out _positions[i][j]);
				}
			}
			ReduceKeys();
		}

		protected AnimationClip()
		{
		}

		public static AnimationClip Load(BinaryReader reader)
		{
			AnimationClip animationClip = new AnimationClip();
			animationClip.Read(reader);
			return animationClip;
		}

		public void CopyTransforms(IList<Vector3> translations, IList<Quaternion> rotations, IList<Vector3> scales, TimeSpan position, bool[] influenceMap)
		{
			float num = (float)((double)_animationFrameRate * position.TotalSeconds);
			int num2 = (int)num;
			float amount = num - (float)num2;
			for (int i = 0; i < translations.Count; i++)
			{
				if (influenceMap != null && !influenceMap[i])
				{
					continue;
				}
				Quaternion[] array = _rotations[i];
				Quaternion quaternion;
				if (num2 >= array.Length)
				{
					quaternion = array[array.Length - 1];
				}
				else
				{
					quaternion = array[num2];
					if (num2 < array.Length - 2)
					{
						quaternion = Quaternion.Slerp(quaternion, array[num2 + 1], amount);
					}
				}
				Vector3[] array2 = _positions[i];
				Vector3 vector;
				if (num2 >= array2.Length)
				{
					vector = array2[array2.Length - 1];
				}
				else
				{
					vector = array2[num2];
					if (num2 < array2.Length - 2)
					{
						vector = Vector3.Lerp(vector, array2[num2 + 1], amount);
					}
				}
				Vector3[] array3 = _scales[i];
				Vector3 vector2;
				if (num2 >= array3.Length)
				{
					vector2 = array3[array3.Length - 1];
				}
				else
				{
					vector2 = array3[num2];
					if (num2 < array3.Length - 2)
					{
						vector2 = Vector3.Lerp(vector2, array3[num2 + 1], amount);
					}
				}
				translations[i] = vector;
				rotations[i] = quaternion;
				scales[i] = vector2;
			}
		}

		public void Read(BinaryReader reader)
		{
			Name = reader.ReadString();
			_animationFrameRate = reader.ReadInt32();
			Duration = TimeSpan.FromTicks(reader.ReadInt64());
			int num = reader.ReadInt32();
			_scales = new Vector3[num][];
			_positions = new Vector3[num][];
			_rotations = new Quaternion[num][];
			for (int i = 0; i < num; i++)
			{
				int num2 = reader.ReadInt32();
				_positions[i] = new Vector3[num2];
				for (int j = 0; j < num2; j++)
				{
					_positions[i][j] = reader.ReadVector3();
				}
				num2 = reader.ReadInt32();
				_rotations[i] = new Quaternion[num2];
				for (int k = 0; k < num2; k++)
				{
					_rotations[i][k] = reader.ReadQuaternion();
				}
				num2 = reader.ReadInt32();
				_scales[i] = new Vector3[num2];
				for (int l = 0; l < num2; l++)
				{
					_scales[i][l] = reader.ReadVector3();
				}
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Name);
			writer.Write(_animationFrameRate);
			writer.Write(Duration.Ticks);
			writer.Write(BoneCount);
			for (int i = 0; i < BoneCount; i++)
			{
				int num = _positions[i].Length;
				writer.Write(num);
				for (int j = 0; j < num; j++)
				{
					writer.Write(_positions[i][j]);
				}
				num = _rotations[i].Length;
				writer.Write(num);
				for (int k = 0; k < num; k++)
				{
					writer.Write(_rotations[i][k]);
				}
				num = _scales[i].Length;
				writer.Write(num);
				for (int l = 0; l < num; l++)
				{
					writer.Write(_scales[i][l]);
				}
			}
		}
	}
}
