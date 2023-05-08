using System.Collections.Generic;
using System.IO;
using DNA.IO.Storage;

namespace DNA
{
	public class CheatCode
	{
		public class CheatCodeManager
		{
			private static string CodeFileName = "cdat.user";

			private SaveDevice _saveDevice;

			public Dictionary<string, CheatCode> Codes = new Dictionary<string, CheatCode>();

			public CheatCodeManager(SaveDevice saveDevice)
			{
				_saveDevice = saveDevice;
			}

			public CheatCode RegisterCode(string systemName, string reward, string unlockCode, object tag)
			{
				CheatCode cheatCode = new CheatCode(this, systemName, reward, unlockCode, tag);
				Codes[cheatCode.SystemName] = cheatCode;
				return cheatCode;
			}

			public List<CheatCode> GetRedeemedCodes()
			{
				List<CheatCode> list = new List<CheatCode>();
				foreach (KeyValuePair<string, CheatCode> code in Codes)
				{
					if (code.Value.Redeemed)
					{
						list.Add(code.Value);
					}
				}
				return list;
			}

			public CheatCode GetDisplayCode(string name, string description, string unlockCode, object tag)
			{
				return new CheatCode(null, name, description, unlockCode, tag);
			}

			public void LoadCodes()
			{
				try
				{
					_saveDevice.Load(CodeFileName, delegate(Stream stream)
					{
						BinaryReader binaryReader = new BinaryReader(stream);
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							string key = binaryReader.ReadString();
							CheatCode value;
							if (Codes.TryGetValue(key, out value))
							{
								value._redeemed = true;
							}
						}
					});
				}
				catch
				{
				}
			}

			private void SaveCodes()
			{
				try
				{
					_saveDevice.Save(CodeFileName, true, true, delegate(Stream stream)
					{
						List<CheatCode> redeemedCodes = GetRedeemedCodes();
						if (redeemedCodes.Count != 0)
						{
							BinaryWriter binaryWriter = new BinaryWriter(stream);
							binaryWriter.Write(redeemedCodes.Count);
							foreach (CheatCode item in redeemedCodes)
							{
								binaryWriter.Write(item.SystemName);
							}
							binaryWriter.Flush();
						}
					});
				}
				catch
				{
				}
			}

			public void Redeem(CheatCode code)
			{
				code._redeemed = true;
				SaveCodes();
			}

			public CheatCode Redeem(string code, out string reason)
			{
				string text = code.ToUpper();
				reason = "Invalid Code";
				foreach (KeyValuePair<string, CheatCode> code2 in Codes)
				{
					if (text == code2.Value.Cheatcode)
					{
						if (!code2.Value.Redeemed)
						{
							code2.Value._redeemed = true;
							reason = "Success";
							SaveCodes();
							return code2.Value;
						}
						reason = "Code Already Redeemed";
					}
				}
				return null;
			}
		}

		private string _systemName;

		private string _reward;

		private bool _redeemed;

		private string _cheatCode;

		public object Tag;

		private CheatCodeManager _manager;

		public string Reward
		{
			get
			{
				return _reward;
			}
		}

		public string SystemName
		{
			get
			{
				return _systemName;
			}
		}

		public bool Redeemed
		{
			get
			{
				return _redeemed;
			}
		}

		public string Cheatcode
		{
			get
			{
				return _cheatCode;
			}
		}

		public CheatCode(CheatCodeManager manager, string systemName, string rewardDescription, string code, object tag)
		{
			_manager = manager;
			_reward = rewardDescription;
			_systemName = systemName;
			Tag = tag;
			_cheatCode = code.ToUpper();
		}
	}
}
