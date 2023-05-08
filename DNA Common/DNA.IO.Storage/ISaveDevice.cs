namespace DNA.IO.Storage
{
	public interface ISaveDevice
	{
		bool IsReady { get; }

		void Save(string fileName, bool tamperProof, bool Compressed, FileAction saveAction);

		void Load(string fileName, FileAction loadAction);

		void Delete(string fileName);

		bool FileExists(string fileName);

		string[] GetFiles();

		string[] GetFiles(string pattern);

		string[] GetDirectories(string path);

		string[] GetDirectories(string path, string pattern);

		void CreateDirectory(string path);

		void DeleteDirectory(string path);

		bool DirectoryExists(string path);
	}
}
