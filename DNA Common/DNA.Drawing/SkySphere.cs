using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class SkySphere : Entity
	{
		private Effect _effect;

		private VertexBuffer _vertexBuffer;

		private IndexBuffer _indexBuffer;

		private Texture _texture;

		private void CreateBall(GraphicsDevice device, float radius, int segments, Vector3 center, bool insideOut)
		{
			int num = segments / 2;
			VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[segments * (num - 1) + 2];
			int num2 = array.Length - 1;
			array[0].Position = new Vector3(0f, radius, 0f);
			array[0].Normal = new Vector3(0f, 1f, 0f);
			array[0].TextureCoordinate = new Vector2(0f, 0f);
			array[num2].Position = new Vector3(0f, 0f - radius, 0f);
			array[num2].Normal = new Vector3(0f, -1f, 0f);
			array[num2].TextureCoordinate = new Vector2(0f, 1f);
			int num3 = 1;
			for (int i = 1; i < num; i++)
			{
				for (int j = 0; j < segments; j++)
				{
					Angle angle = Angle.Lerp(Angle.FromDegrees(0f), Angle.FromDegrees(180f), (float)i / (float)num);
					Angle angle2 = Angle.Lerp(Angle.FromDegrees(0f), Angle.FromDegrees(360f), (float)j / (float)segments);
					array[num3].Normal = new Vector3((float)(angle2.Sin * angle.Sin), (float)angle.Cos, (float)(angle2.Cos * angle.Sin));
					array[num3].Position = center + array[num3].Normal * radius;
					array[num3].TextureCoordinate = new Vector2((float)j / (float)segments, (0f - ((float)angle.Cos - 1f)) / 2f);
					num3++;
				}
			}
			short[] array2 = new short[segments * 6 * num];
			int num4 = 0;
			for (int k = 0; k < segments; k++)
			{
				int num5 = 1;
				int num6 = (k + 1) % segments;
				array2[num4++] = 0;
				array2[num4++] = (short)(num6 + num5);
				array2[num4++] = (short)(k + num5);
				num5 = num2 - segments;
				array2[num4++] = (short)num2;
				array2[num4++] = (short)(num5 + k);
				array2[num4++] = (short)(num5 + num6);
			}
			for (int l = 0; l < num - 2; l++)
			{
				int num7 = l * segments + 1;
				for (int m = 0; m < segments; m++)
				{
					int num8 = (m + 1) % segments;
					array2[num4++] = (short)(num7 + m);
					array2[num4++] = (short)(num7 + num8 + segments);
					array2[num4++] = (short)(num7 + m + segments);
					array2[num4++] = (short)(num7 + m);
					array2[num4++] = (short)(num7 + num8);
					array2[num4++] = (short)(num7 + num8 + segments);
				}
			}
			if (insideOut)
			{
				for (int n = 0; n < array.Length; n++)
				{
					array[n].Normal = -array[n].Normal;
				}
				for (int num9 = 0; num9 < array2.Length; num9 += 3)
				{
					short num10 = array2[num9 + 1];
					array2[num9 + 1] = array2[num9 + 2];
					array2[num9 + 2] = num10;
				}
			}
			_vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), array.Length, BufferUsage.WriteOnly);
			_vertexBuffer.SetData<VertexPositionNormalTexture>(array);
			_indexBuffer = new IndexBuffer(device, typeof(short), array2.Length, BufferUsage.WriteOnly);
			_indexBuffer.SetData(array2);
		}

		public SkySphere(GraphicsDevice device, float radius, Vector3 center, int segments, Effect effect, Texture texture)
		{
			CreateBall(device, radius, segments, center, true);
			_effect = effect;
			_texture = texture;
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			view.Translation = Vector3.Zero;
			if (_effect.Parameters["World"] != null)
			{
				_effect.Parameters["World"].SetValue(base.LocalToWorld);
			}
			if (_effect.Parameters["View"] != null)
			{
				_effect.Parameters["View"].SetValue(view);
			}
			if (_effect.Parameters["Projection"] != null)
			{
				_effect.Parameters["Projection"].SetValue(projection);
			}
			if (_effect.Parameters["Texture"] != null)
			{
				_effect.Parameters["Texture"].SetValue(_texture);
			}
			if (_effect.Parameters["WorldViewProj"] != null)
			{
				_effect.Parameters["WorldViewProj"].SetValue(base.LocalToWorld * view * projection);
			}
			if (_effect.Parameters["WorldView"] != null)
			{
				_effect.Parameters["WorldView"].SetValue(base.LocalToWorld * view);
			}
			device.SetVertexBuffer(_vertexBuffer);
			device.Indices = _indexBuffer;
			for (int i = 0; i < _effect.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass effectPass = _effect.CurrentTechnique.Passes[i];
				effectPass.Apply();
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertexBuffer.VertexCount, 0, _indexBuffer.IndexCount / 3);
			}
			base.Draw(device, gameTime, view, projection);
		}
	}
}
