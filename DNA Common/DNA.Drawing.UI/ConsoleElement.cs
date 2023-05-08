using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ConsoleElement : UIElement
	{
		private class ConsoleTextWriter : TextWriter
		{
			private ConsoleElement _owner;

			private ConsoleElement Owner
			{
				get
				{
					return _owner;
				}
			}

			public override Encoding Encoding
			{
				get
				{
					return Encoding.UTF8;
				}
			}

			public override void WriteLine(string value)
			{
				_owner.WriteLine(value);
			}

			public override void Write(char value)
			{
				_owner.Write(value);
			}

			public ConsoleTextWriter(ConsoleElement control)
			{
				NewLine = "\n";
				_owner = control;
			}
		}

		private class Message
		{
			public string Text;

			public Color Color;

			private OneShotTimer lifeTimer = new OneShotTimer(TimeSpan.FromSeconds(10.0));

			private OneShotTimer fadeTimer = new OneShotTimer(TimeSpan.FromSeconds(3.0));

			public float Visibility
			{
				get
				{
					if (!lifeTimer.Expired)
					{
						return 1f;
					}
					return 1f - fadeTimer.PercentComplete;
				}
			}

			public Message()
			{
			}

			public Message(string text)
			{
				Text = text;
			}

			public Message(string text, Color color)
			{
				Text = text;
				Color = color;
			}

			public override string ToString()
			{
				return Text;
			}

			public void Append(string str)
			{
				Text += str;
			}

			public void Update(GameTime gameTime)
			{
				lifeTimer.Update(gameTime.get_ElapsedGameTime());
				if (lifeTimer.Expired)
				{
					fadeTimer.Update(gameTime.get_ElapsedGameTime());
				}
			}
		}

		private ConsoleTextWriter _textWriter;

		private SpriteFont _font;

		private Message _currentMessage = new Message("");

		private Queue<Message> _messages = new Queue<Message>();

		private Vector2 _size;

		private Message[] messages = new Message[0];

		private int LinesSupported
		{
			get
			{
				return 100;
			}
		}

		public override Vector2 Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		public ConsoleElement(SpriteFont font)
		{
			_font = font;
			_textWriter = new ConsoleTextWriter(this);
		}

		public void GrabConsole()
		{
			GameConsole.SetControl(this);
			Console.SetOut(_textWriter);
		}

		public void Write(char value)
		{
			if (value == '\n')
			{
				WriteLine();
			}
			else
			{
				_currentMessage.Append(value.ToString());
			}
		}

		public void Write(string value)
		{
			string[] array = value.Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				_currentMessage.Append(array[i]);
				if (i < array.Length - 1)
				{
					WriteLine();
				}
			}
		}

		public void WriteLine(string value)
		{
			Write(value);
			WriteLine();
		}

		public void WriteLine()
		{
			lock (_messages)
			{
				_messages.Enqueue(_currentMessage);
				_currentMessage = new Message("");
				while (_messages.Count > LinesSupported)
				{
					_messages.Dequeue();
				}
			}
		}

		public void Clear()
		{
			lock (_messages)
			{
				_messages.Clear();
				_currentMessage = new Message("");
			}
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, bool selected)
		{
			lock (_messages)
			{
				if (messages.Length != _messages.Count)
				{
					messages = new Message[_messages.Count];
				}
				_messages.CopyTo(messages, 0);
			}
			int lineSpacing = _font.LineSpacing;
			Vector2 location = new Vector2(base.Location.X, base.Location.Y + Size.Y - (float)lineSpacing);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
			for (int num = messages.Length - 1; num >= 0; num--)
			{
				Message message = messages[num];
				message.Update(gameTime);
				if (message.Visibility > 0f)
				{
					spriteBatch.DrawOutlinedText(_font, message.Text, location, Color.Lerp(Color.Transparent, base.Color, message.Visibility), Color.Lerp(Color.Transparent, Color.Black, message.Visibility), 1);
				}
				location.Y -= lineSpacing;
				if (location.Y < base.Location.Y)
				{
					break;
				}
			}
			spriteBatch.End();
		}
	}
}
