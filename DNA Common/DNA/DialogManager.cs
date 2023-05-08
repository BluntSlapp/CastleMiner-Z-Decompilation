using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace DNA
{
	public class DialogManager
	{
		public delegate void MessageCallback(int? result);

		public delegate void StorageCallback(StorageDevice device);

		private abstract class DialogBox
		{
			protected DNAGame Game;

			public DialogBox(DNAGame game)
			{
				Game = game;
			}

			public abstract void Show();
		}

		private class MessageBox : DialogBox
		{
			private PlayerIndex Player;

			private string Title;

			private string Text;

			private IEnumerable<string> Buttons;

			private int FocusButton;

			private MessageBoxIcon Icon;

			private MessageCallback Callback;

			private object State;

			public MessageBox(DNAGame game, PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, MessageCallback callback, object state)
				: base(game)
			{
				Player = player;
				Title = title;
				Text = text;
				Buttons = buttons;
				FocusButton = focusButton;
				Icon = icon;
				Callback = callback;
				State = state;
			}

			private void MessageCallback(IAsyncResult result)
			{
				int? result2 = Guide.EndShowMessageBox(result);
				if (Callback != null)
				{
					Callback(result2);
				}
			}

			public override void Show()
			{
				Guide.BeginShowMessageBox(Player, Title, Text, Buttons, FocusButton, Icon, (AsyncCallback)MessageCallback, State);
			}
		}

		private class SignIn : DialogBox
		{
			private bool OnlineOnly;

			public SignIn(DNAGame game, bool onlineOnly)
				: base(game)
			{
				OnlineOnly = onlineOnly;
			}

			public override void Show()
			{
				Guide.ShowSignIn(1, OnlineOnly);
			}
		}

		private class Marketplace : DialogBox
		{
			public PlayerIndex Player;

			public Marketplace(DNAGame game, PlayerIndex player)
				: base(game)
			{
				Player = player;
			}

			public override void Show()
			{
				SignedInGamer signedInGamer = Gamer.SignedInGamers[Player];
				if (signedInGamer != null && signedInGamer.Privileges.AllowPurchaseContent)
				{
					Guide.ShowMarketplace(Player);
				}
				else
				{
					Game.DialogManager.ShowSignIn(false);
				}
			}
		}

		private class Storage : DialogBox
		{
			public PlayerIndex Player;

			private StorageCallback Callback;

			private object State;

			public Storage(DNAGame game, PlayerIndex player, StorageCallback callback, object state)
				: base(game)
			{
				Player = player;
				Callback = callback;
				State = state;
			}

			private void EndStorage(IAsyncResult result)
			{
				if (Callback != null)
				{
					Callback(StorageDevice.EndShowSelector(result));
				}
			}

			public override void Show()
			{
				StorageDevice.BeginShowSelector(Player, (AsyncCallback)EndStorage, State);
			}
		}

		private Queue<DialogBox> _pendingMessages = new Queue<DialogBox>();

		private DNAGame _game;

		public DialogManager(DNAGame game)
		{
			_game = game;
		}

		public void ShowStorage(PlayerIndex player, StorageCallback callback, object state)
		{
			_pendingMessages.Enqueue(new Storage(_game, player, callback, state));
		}

		public void ShowSignIn(bool OnLineOnly)
		{
			_pendingMessages.Enqueue(new SignIn(_game, OnLineOnly));
		}

		public void ShowMarketPlace(PlayerIndex player)
		{
			_pendingMessages.Enqueue(new Marketplace(_game, player));
		}

		public void ShowMessageBox(PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon Icon, MessageCallback callback, object state)
		{
			_pendingMessages.Enqueue(new MessageBox(_game, player, title, text, buttons, focusButton, Icon, callback, state));
		}

		public void Update(GameTime time)
		{
			try
			{
				if (!Guide.IsVisible && _pendingMessages.Count > 0)
				{
					DialogBox dialogBox = _pendingMessages.Dequeue();
					dialogBox.Show();
				}
			}
			catch
			{
			}
		}
	}
}
