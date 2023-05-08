using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DNA.IO.Storage
{
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[CompilerGenerated]
	internal class Strings
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = (resourceMan = new ResourceManager("DNA.IO.Storage.Strings", typeof(Strings).Assembly));
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string forceCanceledReselectionMessage
		{
			get
			{
				return ResourceManager.GetString("forceCanceledReselectionMessage", resourceCulture);
			}
		}

		internal static string forceDisconnectedReselectionMessage
		{
			get
			{
				return ResourceManager.GetString("forceDisconnectedReselectionMessage", resourceCulture);
			}
		}

		internal static string NeedGamerService
		{
			get
			{
				return ResourceManager.GetString("NeedGamerService", resourceCulture);
			}
		}

		internal static string No_Continue_without_device
		{
			get
			{
				return ResourceManager.GetString("No_Continue_without_device", resourceCulture);
			}
		}

		internal static string Ok
		{
			get
			{
				return ResourceManager.GetString("Ok", resourceCulture);
			}
		}

		internal static string promptForCancelledMessage
		{
			get
			{
				return ResourceManager.GetString("promptForCancelledMessage", resourceCulture);
			}
		}

		internal static string promptForDisconnectedMessage
		{
			get
			{
				return ResourceManager.GetString("promptForDisconnectedMessage", resourceCulture);
			}
		}

		internal static string Reselect_Storage_Device
		{
			get
			{
				return ResourceManager.GetString("Reselect_Storage_Device", resourceCulture);
			}
		}

		internal static string Storage_Device_Required
		{
			get
			{
				return ResourceManager.GetString("Storage_Device_Required", resourceCulture);
			}
		}

		internal static string StorageDevice_is_not_valid
		{
			get
			{
				return ResourceManager.GetString("StorageDevice_is_not_valid", resourceCulture);
			}
		}

		internal static string Yes_Select_new_device
		{
			get
			{
				return ResourceManager.GetString("Yes_Select_new_device", resourceCulture);
			}
		}

		internal Strings()
		{
		}
	}
}
