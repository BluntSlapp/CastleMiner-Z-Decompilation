using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DNA.IO;
using DNA.IO.Checksums;
using DNA.Reflection;
using Microsoft.Xna.Framework.Net;

namespace DNA.Net
{
	public abstract class Message
	{
		private static Message[] _sendInstances;

		private static Message[] _receiveInstance;

		private static Type[] _messageTypes;

		private static Dictionary<Type, byte> _messageIDs;

		private static ChecksumStream<byte> _writeBufferStream;

		private static BinaryWriter _writer;

		private static ChecksumStream<byte> _readBufferStream;

		private static BinaryReader _reader;

		protected NetworkGamer _sender;

		private static byte[] messageBuffer;

		public virtual bool Echo
		{
			get
			{
				return true;
			}
		}

		public NetworkGamer Sender
		{
			get
			{
				return _sender;
			}
		}

		public byte MessageID
		{
			get
			{
				return _messageIDs[GetType()];
			}
		}

		protected abstract SendDataOptions SendDataOptions { get; }

		static Message()
		{
			messageBuffer = new byte[4096];
			_writeBufferStream = new ChecksumStream<byte>(new MemoryStream(4096), new XOR8Checksum());
			_writer = new BinaryWriter(_writeBufferStream);
			_readBufferStream = new ChecksumStream<byte>(new MemoryStream(4096), new XOR8Checksum());
			_reader = new BinaryReader(_readBufferStream);
			PopulateMessageTypes();
		}

		protected abstract void RecieveData(BinaryReader reader);

		protected abstract void SendData(BinaryWriter writer);

		protected static T GetSendInstance<T>() where T : Message
		{
			Type typeFromHandle = typeof(T);
			return (T)_sendInstances[_messageIDs[typeFromHandle]];
		}

		private void DoSendInternal(NetworkGamer recipiant)
		{
			lock (_writer)
			{
				MemoryStream memoryStream = (MemoryStream)_writeBufferStream.BaseStream;
				memoryStream.Position = 0L;
				_writeBufferStream.Reset();
				_writer.Write(MessageID);
				SendData(_writer);
				_writer.Flush();
				byte checksumValue = _writeBufferStream.ChecksumValue;
				_writer.Write(checksumValue);
				_writer.Flush();
				if (!_sender.HasLeftSession)
				{
					if (recipiant != null)
					{
						((LocalNetworkGamer)_sender).SendData(memoryStream.GetBuffer(), 0, (int)memoryStream.Position, SendDataOptions, recipiant);
					}
					else
					{
						((LocalNetworkGamer)_sender).SendData(memoryStream.GetBuffer(), 0, (int)memoryStream.Position, SendDataOptions);
					}
				}
			}
		}

		protected void DoSend(LocalNetworkGamer sender)
		{
			_sender = sender;
			DoSendInternal(null);
		}

		protected void DoSend(LocalNetworkGamer sender, NetworkGamer recipiant)
		{
			_sender = sender;
			DoSendInternal(recipiant);
		}

		private static bool TypeFilter(Type type)
		{
			if (type.IsSubclassOf(typeof(Message)))
			{
				return !type.IsAbstract;
			}
			return false;
		}

		private static void PopulateMessageTypes()
		{
			_messageTypes = ReflectionTools.GetTypes(TypeFilter);
			_messageIDs = new Dictionary<Type, byte>();
			for (byte b = 0; b < _messageTypes.Length; b = (byte)(b + 1))
			{
				_messageIDs[_messageTypes[b]] = b;
			}
			_receiveInstance = new Message[_messageTypes.Length];
			_sendInstances = new Message[_messageTypes.Length];
			for (int i = 0; i < _messageTypes.Length; i++)
			{
				ConstructorInfo constructor = _messageTypes[i].GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
				if (constructor == null)
				{
					throw new Exception(_messageTypes[i].Name + " Needs a private parameterless constructor");
				}
				_receiveInstance[i] = (Message)constructor.Invoke(new object[0]);
				_sendInstances[i] = (Message)constructor.Invoke(new object[0]);
			}
		}

		private static Message ReadMessage(NetworkGamer sender)
		{
			_readBufferStream.Reset();
			byte b = _reader.ReadByte();
			Message message = _receiveInstance[b];
			message._sender = sender;
			message.RecieveData(_reader);
			byte checksumValue = _readBufferStream.ChecksumValue;
			byte b2 = _reader.ReadByte();
			if (checksumValue != b2)
			{
				throw new Exception("CheckSum Error");
			}
			return message;
		}

		public static Message GetMessage(LocalNetworkGamer localGamer)
		{
			lock (_reader)
			{
				MemoryStream memoryStream = (MemoryStream)_readBufferStream.BaseStream;
				int num = 0;
				NetworkGamer sender;
				while (true)
				{
					try
					{
						num = localGamer.ReceiveData(messageBuffer, out sender);
					}
					catch (ArgumentException)
					{
						messageBuffer = new byte[messageBuffer.Length * 2];
						continue;
					}
					break;
				}
				memoryStream.Position = 0L;
				memoryStream.Write(messageBuffer, 0, num);
				memoryStream.Position = 0L;
				if (localGamer == sender)
				{
					return ReadMessage(sender);
				}
				try
				{
					return ReadMessage(sender);
				}
				catch (Exception innerException)
				{
					throw new InvalidMessageException(sender, innerException);
				}
			}
		}
	}
}
