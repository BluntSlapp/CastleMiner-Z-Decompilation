using System;
using System.Collections.Generic;
using System.Threading;
using DNA.Audio;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class DialogScreen : Screen
	{
		private string Title;

		private string Description;

		private string[] Options;

		private Texture2D BgImage;

		private SpriteFont Font;

		public Vector2 TitlePadding = new Vector2(20f, 5f);

		public Vector2 DescriptionPadding = new Vector2(10f, 10f);

		public Vector2 OptionsPadding = new Vector2(10f, 10f);

		public Vector2 ButtonsPadding = new Vector2(10f, 10f);

		public Color TitleColor = Color.White;

		public Color DescriptionColor = Color.White;

		public Color OptionsColor = Color.White;

		public Color OptionsSelectedColor = Color.Red;

		public Color ButtonsColor = Color.White;

		public string ClickSound;

		public string OpenSound;

		private List<string> descriptionLinesToPrint = new List<string>();

		private List<string> optionLinesToPrint = new List<string>();

		private List<int> optionsStartLine = new List<int>();

		private bool _descriptionLinesCalculated;

		private bool _optionsLinesCalculated;

		private int _optionSelected = -1;

		private int optionCurrentlySelected;

		private OneShotTimer flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		private bool selectedDirection;

		private bool JoystickMoved;

		private bool printCancelButton;

		public ThreadStart Callback;

		private Texture2D DummyTexture;

		public int OptionSelected
		{
			get
			{
				return _optionSelected;
			}
		}

		public DialogScreen(string title, string description, string[] options, bool printCancel, Texture2D bgImage, SpriteFont font, bool drawBehind)
			: base(true, drawBehind)
		{
			Title = title;
			printCancelButton = printCancel;
			Description = description;
			if (options != null)
			{
				Options = new string[options.Length];
			}
			Options = options;
			BgImage = bgImage;
			Font = font;
			if (options == null)
			{
				optionCurrentlySelected = 0;
			}
		}

		public override void OnPushed()
		{
			if (OpenSound != null)
			{
				SoundManager.Instance.PlayInstance(OpenSound);
			}
			base.OnPushed();
		}

		private void GetDescriptionLines(float w)
		{
			if (_descriptionLinesCalculated)
			{
				return;
			}
			_descriptionLinesCalculated = true;
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < Description.Length; i++)
			{
				if (Description[i] == '\n')
				{
					if (Font.MeasureString(Description.Substring(num3, i - num3 + 1)).X > w - DescriptionPadding.X * 2f)
					{
						descriptionLinesToPrint.Add(Description.Substring(num3, num2 - num3));
						descriptionLinesToPrint.Add(Description.Substring(num2 + 1, i - num2));
					}
					else
					{
						descriptionLinesToPrint.Add(Description.Substring(num3, i - num3));
					}
					num3 = i + 1;
					num = 0f;
					num2 = i;
				}
				else if (Description[i] == ' ')
				{
					float x = Font.MeasureString(Description.Substring(num2, i - num2)).X;
					num += x;
					if (num > w - DescriptionPadding.X * 2f)
					{
						descriptionLinesToPrint.Add(Description.Substring(num3, num2 - num3 + 1));
						num3 = num2 + 1;
						num = x;
						num2 = i + 1;
					}
					else
					{
						num2 = i;
					}
				}
				if (i == Description.Length - 1)
				{
					if (Font.MeasureString(Description.Substring(num3, i - num3 + 1)).X > w - DescriptionPadding.X * 2f)
					{
						descriptionLinesToPrint.Add(Description.Substring(num3, num2 - num3 + 1));
						descriptionLinesToPrint.Add(Description.Substring(num2 + 1, i - num2));
					}
					else
					{
						descriptionLinesToPrint.Add(Description.Substring(num3, i - num3 + 1));
					}
				}
			}
		}

		private void GetOptionsLines(float w)
		{
			if (_optionsLinesCalculated || Options == null)
			{
				return;
			}
			_optionsLinesCalculated = true;
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < Options.Length; i++)
			{
				string text = Options[i];
				num = 0f;
				num2 = 0;
				num3 = 0;
				if (text == null)
				{
					continue;
				}
				for (int j = 0; j < text.Length; j++)
				{
					if (text[j] == '\n')
					{
						if (Font.MeasureString(text.Substring(num3, j - num3 + 1)).X > w - OptionsPadding.X * 2f)
						{
							optionLinesToPrint.Add(text.Substring(num3, num2 - num3));
							if (optionsStartLine.Count < i + 1)
							{
								optionsStartLine.Add(optionLinesToPrint.Count - 1);
							}
							optionLinesToPrint.Add(text.Substring(num2 + 1, j - num2));
						}
						else
						{
							optionLinesToPrint.Add(text.Substring(num3, j - num3));
							if (optionsStartLine.Count < i + 1)
							{
								optionsStartLine.Add(optionLinesToPrint.Count - 1);
							}
						}
						num3 = j + 1;
						num = 0f;
						num2 = j;
					}
					if (text[j] == ' ')
					{
						float x = Font.MeasureString(text.Substring(num2, j - num2)).X;
						num += x;
						if (num > w - OptionsPadding.X * 2f)
						{
							optionLinesToPrint.Add(text.Substring(num3, num2 - num3 + 1));
							if (optionsStartLine.Count < i + 1)
							{
								optionsStartLine.Add(optionLinesToPrint.Count - 1);
							}
							num3 = num2 + 1;
							num = x;
							num2 = j + 1;
						}
						else
						{
							num2 = j;
						}
					}
					if (j != text.Length - 1)
					{
						continue;
					}
					if (Font.MeasureString(text.Substring(num3, j - num3 + 1)).X > w - OptionsPadding.X * 2f)
					{
						optionLinesToPrint.Add(text.Substring(num3, num2 - num3 + 1));
						if (optionsStartLine.Count < i + 1)
						{
							optionsStartLine.Add(optionLinesToPrint.Count - 1);
						}
						optionLinesToPrint.Add(text.Substring(num2 + 1, j - num2));
					}
					else
					{
						optionLinesToPrint.Add(text.Substring(num3, j - num3 + 1));
						if (optionsStartLine.Count < i + 1)
						{
							optionsStartLine.Add(optionLinesToPrint.Count - 1);
						}
					}
				}
			}
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			float num = BgImage.Width;
			float num2 = BgImage.Height;
			Rectangle destinationRectangle = new Rectangle((int)((float)titleSafeArea.Center.X - num / 2f), (int)((float)titleSafeArea.Center.Y - num2 / 2f), (int)num, (int)num2);
			GetDescriptionLines(num);
			GetOptionsLines(num);
			if (DummyTexture == null)
			{
				DummyTexture = new Texture2D(device, 1, 1);
				DummyTexture.SetData<Color>(new Color[1] { Color.White });
			}
			spriteBatch.Begin();
			Rectangle destinationRectangle2 = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);
			spriteBatch.Draw(DummyTexture, destinationRectangle2, new Color(0f, 0f, 0f, 0.5f));
			spriteBatch.Draw(BgImage, destinationRectangle, Color.White);
			spriteBatch.DrawOutlinedText(Font, Title, new Vector2((float)destinationRectangle.X + TitlePadding.X, (float)destinationRectangle.Y + TitlePadding.Y), TitleColor, Color.Black, 1);
			float num3 = (float)destinationRectangle.Y + DescriptionPadding.Y;
			for (int i = 0; i < descriptionLinesToPrint.Count; i++)
			{
				num3 += Font.MeasureString(Title).Y;
				spriteBatch.DrawOutlinedText(Font, descriptionLinesToPrint[i], new Vector2((float)destinationRectangle.X + DescriptionPadding.X, num3), DescriptionColor, Color.Black, 1);
			}
			num3 = (float)(destinationRectangle.Y + destinationRectangle.Height) - OptionsPadding.Y - Font.MeasureString(Title).Y * (float)(optionLinesToPrint.Count + 2) - ButtonsPadding.Y;
			for (int j = 0; j < optionLinesToPrint.Count; j++)
			{
				if (j >= optionsStartLine[optionCurrentlySelected])
				{
					if (optionCurrentlySelected == Options.Length - 1 || j < optionsStartLine[optionCurrentlySelected + 1])
					{
						if (j == optionsStartLine[optionCurrentlySelected])
						{
							flashTimer.Update(gameTime.get_ElapsedGameTime());
							if (flashTimer.Expired)
							{
								flashTimer.Reset();
								selectedDirection = !selectedDirection;
							}
						}
						Color textColor = ((!selectedDirection) ? Color.Lerp(OptionsSelectedColor, OptionsColor, flashTimer.PercentComplete) : Color.Lerp(OptionsColor, OptionsSelectedColor, flashTimer.PercentComplete));
						num3 += Font.MeasureString(Title).Y;
						spriteBatch.DrawOutlinedText(Font, optionLinesToPrint[j], new Vector2((float)destinationRectangle.X + OptionsPadding.X, num3), textColor, Color.Black, 1);
					}
					else
					{
						num3 += Font.MeasureString(Title).Y;
						spriteBatch.DrawOutlinedText(Font, optionLinesToPrint[j], new Vector2((float)destinationRectangle.X + OptionsPadding.X, num3), OptionsColor, Color.Black, 1);
					}
				}
				else
				{
					num3 += Font.MeasureString(Title).Y;
					spriteBatch.DrawOutlinedText(Font, optionLinesToPrint[j], new Vector2((float)destinationRectangle.X + OptionsPadding.X, num3), OptionsColor, Color.Black, 1);
				}
			}
			Vector2 vector = Font.MeasureString(" OK");
			float num4 = vector.Y / (float)ControllerImages.A.Height;
			int num5 = (int)((float)ControllerImages.A.Width * num4);
			num3 = (float)(destinationRectangle.Y + destinationRectangle.Height) - ButtonsPadding.Y - Font.MeasureString(Title).Y;
			spriteBatch.Draw(ControllerImages.A, new Rectangle((int)((float)destinationRectangle.X + ButtonsPadding.X), (int)num3, num5, (int)vector.Y), Color.White);
			spriteBatch.DrawOutlinedText(Font, " OK", new Vector2((float)destinationRectangle.X + ButtonsPadding.X + (float)num5, num3), ButtonsColor, Color.Black, 1);
			if (printCancelButton)
			{
				vector = Font.MeasureString(" Cancel");
				num4 = vector.Y / (float)ControllerImages.B.Height;
				num5 = (int)((float)ControllerImages.B.Width * num4);
				spriteBatch.Draw(ControllerImages.B, new Rectangle((int)((float)destinationRectangle.X + ButtonsPadding.X + (float)num5 + Font.MeasureString(" OK").X + 10f), (int)num3, num5, (int)vector.Y), Color.White);
				spriteBatch.DrawOutlinedText(Font, " Cancel", new Vector2((float)destinationRectangle.X + ButtonsPadding.X + (float)(num5 * 2) + Font.MeasureString(" OK").X + 10f, num3), ButtonsColor, Color.Black, 1);
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			if (controller.PressedButtons.Start || controller.PressedButtons.A)
			{
				_optionSelected = optionCurrentlySelected;
				if (ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(ClickSound);
				}
				PopMe();
				if (Callback != null)
				{
					Callback();
				}
			}
			if (controller.PressedButtons.Back || controller.PressedButtons.B)
			{
				_optionSelected = -1;
				if (ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(ClickSound);
				}
				PopMe();
				if (Callback != null)
				{
					Callback();
				}
			}
			if (controller.CurrentState.ThumbSticks.Left.Y > 0f || controller.CurrentState.IsButtonDown(Buttons.DPadUp))
			{
				if (Options != null && !JoystickMoved)
				{
					JoystickMoved = true;
					if (optionCurrentlySelected > 0)
					{
						optionCurrentlySelected--;
						if (ClickSound != null)
						{
							SoundManager.Instance.PlayInstance(ClickSound);
						}
					}
				}
			}
			else if (controller.CurrentState.ThumbSticks.Left.Y < 0f || controller.CurrentState.IsButtonDown(Buttons.DPadDown))
			{
				if (Options != null && !JoystickMoved)
				{
					JoystickMoved = true;
					if (optionCurrentlySelected < Options.Length - 1)
					{
						optionCurrentlySelected++;
						if (ClickSound != null)
						{
							SoundManager.Instance.PlayInstance(ClickSound);
						}
					}
				}
			}
			else if (JoystickMoved)
			{
				JoystickMoved = false;
			}
			base.OnPlayerInput(controller, gameTime);
		}
	}
}
