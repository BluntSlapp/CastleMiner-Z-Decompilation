using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNA.Avatars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class Bone
	{
		[ContentSerializer]
		private Vector3 _translation;

		[ContentSerializer]
		private Vector3 _scale;

		[ContentSerializer]
		private Quaternion _rotation;

		private bool _dirty = true;

		private Matrix _transform;

		[ContentSerializer]
		public ReadOnlyCollection<Bone> Children { get; private set; }

		[ContentSerializer]
		public int Index { get; private set; }

		[ContentSerializer]
		public string Name { get; private set; }

		[ContentSerializer]
		public Bone Parent { get; private set; }

		public Vector3 Translation
		{
			get
			{
				return _translation;
			}
			set
			{
				_translation = value;
				_dirty = true;
			}
		}

		public Vector3 Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
				_dirty = true;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				_rotation = value;
				_dirty = true;
			}
		}

		public Matrix Transform
		{
			get
			{
				if (_dirty)
				{
					ComposeTransform();
				}
				return _transform;
			}
		}

		public void Reset()
		{
			_rotation = Quaternion.Identity;
			_translation = Vector3.Zero;
			_scale = Vector3.One;
			_transform = Matrix.Identity;
			_dirty = false;
		}

		private void ComposeTransform()
		{
			_transform = Matrix.CreateScale(_scale) * Matrix.CreateFromQuaternion(_rotation);
			_transform.Translation = _translation;
			_dirty = false;
		}

		public void SetTransform(Matrix xform)
		{
			_transform = xform;
			_transform.Decompose(out _scale, out _rotation, out _translation);
			_dirty = false;
		}

		public void SetTransform(Vector3 trans, Quaternion rot, Vector3 scale)
		{
			_translation = trans;
			_scale = scale;
			_rotation = rot;
			ComposeTransform();
			_dirty = false;
		}

		public Bone(int index, string name, Bone parent, Vector3 translation, Quaternion rotation, Vector3 scale, IList<Bone> children)
		{
			Index = index;
			Name = name;
			SetTransform(translation, rotation, scale);
			Children = new ReadOnlyCollection<Bone>(children);
		}

		public Bone(int index, string name, Bone parent, Matrix xform, IList<Bone> children)
		{
			Index = index;
			Name = name;
			SetTransform(xform);
			Children = new ReadOnlyCollection<Bone>(children);
		}

		private Bone()
		{
		}

		public static Skeleton BuildSkeleton(AvatarRenderer avatarRenderer)
		{
			return BuildSkeleton(avatarRenderer.get_BindPose(), avatarRenderer.get_ParentBones(), Avatar.BoneNames);
		}

		public static Skeleton BuildSkeleton(Model model)
		{
			int count = ((ReadOnlyCollection<ModelBone>)(object)model.Bones).Count;
			Matrix[] array = new Matrix[count];
			string[] array2 = new string[count];
			int[] array3 = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ((ReadOnlyCollection<ModelBone>)(object)model.Bones)[i].Transform;
				array2[i] = ((ReadOnlyCollection<ModelBone>)(object)model.Bones)[i].Name;
				array3[i] = ((((ReadOnlyCollection<ModelBone>)(object)model.Bones)[i].Parent == null) ? (-1) : ((ReadOnlyCollection<ModelBone>)(object)model.Bones)[i].Parent.Index);
			}
			return BuildSkeleton(array, array3, array2);
		}

		public static Skeleton BuildSkeleton(IList<Matrix> transforms, IList<int> heirachy, IList<string> names)
		{
			Bone[] array = new Bone[transforms.Count];
			for (int i = 0; i < transforms.Count; i++)
			{
				if (heirachy[i] < 0)
				{
					Bone bone = new Bone();
					bone.Index = i;
					bone.Name = names[i];
					bone.SetTransform(transforms[i]);
					array[bone.Index] = bone;
					bone.BuildSubSkeleton(array, transforms, heirachy, names);
				}
			}
			return new Skeleton(array);
		}

		private void BuildSubSkeleton(Bone[] bones, IList<Matrix> transforms, IList<int> heirachy, IList<string> names)
		{
			List<Bone> list = new List<Bone>();
			for (int i = 0; i < transforms.Count; i++)
			{
				if (heirachy[i] == Index)
				{
					Bone bone = new Bone();
					bone.Index = i;
					bone.Name = names[i];
					bone.SetTransform(transforms[i]);
					bone.Parent = this;
					bones[bone.Index] = bone;
					bone.BuildSubSkeleton(bones, transforms, heirachy, names);
					list.Add(bone);
				}
			}
			Children = new ReadOnlyCollection<Bone>(list);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
