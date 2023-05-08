namespace DNA.IO.Storage
{
	internal enum SaveDevicePromptState
	{
		None,
		ShowSelector,
		PromptForCanceled,
		ForceCanceledReselection,
		PromptForDisconnected,
		ForceDisconnectedReselection
	}
}
