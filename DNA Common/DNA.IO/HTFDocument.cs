using System.Collections.Generic;
using System.IO;

namespace DNA.IO
{
	public class HTFDocument
	{
		private HTFElement _root;

		public HTFElement Root
		{
			get
			{
				return _root;
			}
			set
			{
				_root = value;
			}
		}

		public List<HTFElement> Children
		{
			get
			{
				return _root.Children;
			}
		}

		public HTFDocument()
		{
			_root = new HTFElement();
		}

		public HTFDocument(string fileName)
		{
			using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Load(stream);
			}
		}

		public HTFDocument(Stream stream)
		{
			Load(stream);
		}

		public void Load(Stream stream)
		{
			_root = new HTFElement(stream);
		}

		public void Load(string path)
		{
			using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				_root = new HTFElement(stream);
			}
		}

		public void Save(string filename)
		{
			using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				Save(stream);
			}
		}

		public void Save(Stream stream)
		{
			StreamWriter writer = new StreamWriter(stream);
			Save(writer);
		}

		public void Save(StreamWriter writer)
		{
			_root.Save(writer);
			writer.Flush();
		}

		public override string ToString()
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter writer = new StreamWriter(memoryStream);
			Save(writer);
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			return streamReader.ReadToEnd();
		}

		public void LoadFromString(string data)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(data);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			Load(memoryStream);
		}
	}
}
