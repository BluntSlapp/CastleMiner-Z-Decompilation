namespace DNA.IO.Storage
{
	public interface IAsyncSaveDevice : ISaveDevice
	{
		bool IsBusy { get; }

		event SaveCompletedEventHandler SaveCompleted;

		event LoadCompletedEventHandler LoadCompleted;

		event DeleteCompletedEventHandler DeleteCompleted;

		event FileExistsCompletedEventHandler FileExistsCompleted;

		event GetFilesCompletedEventHandler GetFilesCompleted;

		void SaveAsync(string fileName, bool tamperProof, bool Compressed, FileAction saveAction);

		void SaveAsync(string fileName, bool tamperProof, bool Compressed, FileAction saveAction, object userState);

		void LoadAsync(string fileName, FileAction loadAction);

		void LoadAsync(string fileName, FileAction loadAction, object userState);

		void DeleteAsync(string fileName);

		void DeleteAsync(string fileName, object userState);

		void FileExistsAsync(string fileName);

		void FileExistsAsync(string fileName, object userState);

		void GetFilesAsync();

		void GetFilesAsync(object userState);

		void GetFilesAsync(string pattern);

		void GetFilesAsync(string pattern, object userState);

		void GetDirectoriesAsync(string path);

		void GetDirectoriesAsync(string path, string pattern);

		void CreateDirectoryAsync(string path);

		void DeleteDirectoryAsync(string path);

		void DirectoryExistsAsync(string path);
	}
}
