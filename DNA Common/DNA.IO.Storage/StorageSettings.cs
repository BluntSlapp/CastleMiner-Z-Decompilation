using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DNA.IO.Storage
{
	public static class StorageSettings
	{
		private static readonly Dictionary<string, Language> languageMap = new Dictionary<string, Language>
		{
			{
				"de",
				Language.German
			},
			{
				"es",
				Language.Spanish
			},
			{
				"fr",
				Language.French
			},
			{
				"it",
				Language.Italian
			},
			{
				"ja",
				Language.Japanese
			},
			{
				"en",
				Language.English
			}
		};

		private static readonly Dictionary<Language, string> cultureMap = new Dictionary<Language, string>
		{
			{
				Language.German,
				"de-DE"
			},
			{
				Language.Spanish,
				"es-ES"
			},
			{
				Language.French,
				"fr-FR"
			},
			{
				Language.Italian,
				"it-IT"
			},
			{
				Language.Japanese,
				"ja-JP"
			},
			{
				Language.English,
				"en-US"
			}
		};

		public static void SetSupportedLanguages(params Language[] supportedLanguages)
		{
			if (supportedLanguages == null)
			{
				throw new ArgumentNullException("supportedLanguages");
			}
			if (supportedLanguages.Length == 0)
			{
				throw new ArgumentException("supportedLanguages");
			}
			foreach (Language language in supportedLanguages)
			{
				if (language < Language.German || language > Language.English)
				{
					throw new ArgumentException("supportedLanguages");
				}
			}
			bool flag = false;
			Language value;
			if (!languageMap.TryGetValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower(), out value) || !Enumerable.Contains<Language>((IEnumerable<Language>)supportedLanguages, value))
			{
				Strings.Culture = new CultureInfo(cultureMap[supportedLanguages[0]]);
				ResetSaveDeviceStrings();
			}
		}

		public static void ResetSaveDeviceStrings()
		{
			SaveDevice.OkOption = Strings.Ok;
			SaveDevice.YesOption = Strings.Yes_Select_new_device;
			SaveDevice.NoOption = Strings.No_Continue_without_device;
			SaveDevice.DeviceOptionalTitle = Strings.Reselect_Storage_Device;
			SaveDevice.DeviceRequiredTitle = Strings.Storage_Device_Required;
			SaveDevice.ForceDisconnectedReselectionMessage = Strings.forceDisconnectedReselectionMessage;
			SaveDevice.PromptForDisconnectedMessage = Strings.promptForDisconnectedMessage;
			SaveDevice.ForceCancelledReselectionMessage = Strings.forceCanceledReselectionMessage;
			SaveDevice.PromptForCancelledMessage = Strings.promptForCancelledMessage;
		}
	}
}
