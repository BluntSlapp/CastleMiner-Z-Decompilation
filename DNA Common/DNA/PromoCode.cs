using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using DNA.IO.Checksums;
using DNA.IO.Storage;
using DNA.Security;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA
{
	public class PromoCode
	{
		public class PromoCodeManager
		{
			private static string CodeFileName = "pdat.user";

			private SignedInGamer _gamer;

			private SaveDevice _saveDevice;

			public Dictionary<string, PromoCode> Codes = new Dictionary<string, PromoCode>();

			public PromoCodeManager(SignedInGamer gamer, SaveDevice saveDevice)
			{
				_saveDevice = saveDevice;
				_gamer = gamer;
			}

			public PromoCode RegisterCode(string systemName, string reward, object tag)
			{
				PromoCode promoCode = new PromoCode(this, systemName, reward, _gamer.Gamertag, tag);
				Codes[promoCode.SystemName] = promoCode;
				return promoCode;
			}

			public List<PromoCode> GetRedeemedCodes()
			{
				List<PromoCode> list = new List<PromoCode>();
				foreach (KeyValuePair<string, PromoCode> code in Codes)
				{
					if (code.Value.Redeemed)
					{
						list.Add(code.Value);
					}
				}
				return list;
			}

			public PromoCode GetDisplayCode(string name, string description, object tag)
			{
				return new PromoCode(null, name, description, _gamer.Gamertag, tag);
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
							uint num2 = binaryReader.ReadUInt32();
							PromoCode value;
							if (Codes.TryGetValue(key, out value) && (value.HashCode == num2 || value.AltHashCode == num2))
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
						List<PromoCode> redeemedCodes = GetRedeemedCodes();
						if (redeemedCodes.Count != 0)
						{
							BinaryWriter binaryWriter = new BinaryWriter(stream);
							binaryWriter.Write(redeemedCodes.Count);
							foreach (PromoCode item in redeemedCodes)
							{
								binaryWriter.Write(item.SystemName);
								binaryWriter.Write(item.HashCode);
							}
							binaryWriter.Flush();
						}
					});
				}
				catch
				{
				}
			}

			public void Redeem(PromoCode code)
			{
				code._redeemed = true;
				SaveCodes();
			}

			public PromoCode Redeem(string code, out string reason)
			{
				uint num = ParshHash(code);
				reason = "Invalid Code";
				foreach (KeyValuePair<string, PromoCode> code2 in Codes)
				{
					if (num == code2.Value.HashCode || num == code2.Value.AltHashCode)
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

		private static byte[] Key;

		private static byte[] Code;

		private static char[] FriendlyToHexLookup;

		private static char[] HexToFriendlyLookup;

		private string _systemName;

		private string _reward;

		private bool _redeemed;

		private uint _altHashCode;

		private uint _hashCode;

		private string _friendlyUserCode;

		private string _userCode;

		private string _altUserCode;

		public object Tag;

		private PromoCodeManager _manager;

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

		public uint AltHashCode
		{
			get
			{
				return _altHashCode;
			}
		}

		public uint HashCode
		{
			get
			{
				return _hashCode;
			}
		}

		public string UserCode
		{
			get
			{
				return _userCode;
			}
		}

		public string FriendlyUserCode
		{
			get
			{
				return _friendlyUserCode;
			}
		}

		public string AltUserCode
		{
			get
			{
				return _altUserCode;
			}
		}

		static PromoCode()
		{
			Key = new byte[32]
			{
				139, 82, 60, 111, 51, 59, 131, 183, 231, 245,
				94, 184, 156, 205, 144, 40, 162, 242, 237, 111,
				47, 165, 165, 151, 60, 233, 179, 58, 208, 152,
				219, 0
			};
			Code = new byte[16]
			{
				166, 148, 102, 129, 240, 5, 112, 15, 237, 81,
				251, 96, 55, 147, 255, 180
			};
			FriendlyToHexLookup = new char[256];
			HexToFriendlyLookup = new char[256];
			string text = "AEFHKMNPRTUVWXYZ";
			for (int i = 0; i < 256; i++)
			{
				FriendlyToHexLookup[i] = (char)i;
				HexToFriendlyLookup[i] = (char)i;
			}
			for (int j = 0; j < 16; j++)
			{
				char c = j.ToString("X1")[0];
				char c2 = text[j];
				FriendlyToHexLookup[(uint)c2] = c;
				HexToFriendlyLookup[(uint)c] = c2;
			}
		}

		private static string HexToFriendlyCode(string code)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < code.Length; i++)
			{
				char c = code[i];
				if (c < 'Ā')
				{
					c = HexToFriendlyLookup[(uint)c];
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		private static string FriendlyToHexCode(string code)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < code.Length; i++)
			{
				char c = code[i];
				if (c < 'Ā')
				{
					c = FriendlyToHexLookup[(uint)c];
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		private static uint ParshHash(string str)
		{
			str = str.ToUpper();
			str = str.Replace(" ", "");
			str = str.Replace("-", "");
			uint result;
			if (uint.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			str = FriendlyToHexCode(str);
			if (uint.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return 0u;
		}

		public PromoCode(PromoCodeManager manager, string systemName, string rewardDescription, string gamerTag, object tag)
		{
			_manager = manager;
			_reward = rewardDescription;
			_systemName = systemName;
			Tag = tag;
			string s = SecurityTools.DecryptString(Key, Code) + SystemName + gamerTag;
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			Crc32 crc = new Crc32();
			crc.Update(bytes);
			_altHashCode = crc.Value;
			_altUserCode = _altHashCode.ToString("X8");
			_altUserCode = _altUserCode.Substring(0, 4) + "-" + _altUserCode.Substring(4, 4);
			s = SecurityTools.DecryptString(Key, Code) + SystemName + gamerTag.ToLower();
			bytes = Encoding.UTF8.GetBytes(s);
			crc = new Crc32();
			crc.Update(bytes);
			_hashCode = crc.Value;
			_userCode = _hashCode.ToString("X8");
			_userCode = _userCode.Substring(0, 4) + "-" + _userCode.Substring(4, 4);
			_friendlyUserCode = HexToFriendlyCode(_userCode);
		}
	}
}
