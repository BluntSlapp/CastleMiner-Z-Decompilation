using System.Collections;
using System.Collections.Generic;

namespace DNA.Audio.SignalProcessing
{
	public abstract class SignalProcessorGroup<InputDataType, InternalDataType> : SignalProcessor<InputDataType>, IList<SignalProcessor<InternalDataType>>, ICollection<SignalProcessor<InternalDataType>>, IEnumerable<SignalProcessor<InternalDataType>>, IEnumerable
	{
		private List<SignalProcessor<InternalDataType>> Processors = new List<SignalProcessor<InternalDataType>>();

		public override int? SampleRate
		{
			get
			{
				for (int i = 0; i < Processors.Count; i++)
				{
					int? sampleRate = Processors[i].SampleRate;
					if (sampleRate.HasValue)
					{
						return sampleRate;
					}
				}
				return null;
			}
		}

		public override int? Channels
		{
			get
			{
				for (int i = 0; i < Processors.Count; i++)
				{
					int? channels = Processors[i].Channels;
					if (channels.HasValue)
					{
						return channels;
					}
				}
				return null;
			}
		}

		public SignalProcessor<InternalDataType> this[int index]
		{
			get
			{
				return Processors[index];
			}
			set
			{
				Processors[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return Processors.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override void OnStart()
		{
			for (int i = 0; i < Processors.Count; i++)
			{
				Processors[i].OnStart();
			}
			base.OnStart();
		}

		public override void OnStop()
		{
			for (int i = 0; i < Processors.Count; i++)
			{
				Processors[i].OnStop();
			}
			base.OnStop();
		}

		public int IndexOf(SignalProcessor<InternalDataType> item)
		{
			return Processors.IndexOf(item);
		}

		public void Insert(int index, SignalProcessor<InternalDataType> item)
		{
			Processors.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Processors.RemoveAt(index);
		}

		public void Add(SignalProcessor<InternalDataType> item)
		{
			Processors.Add(item);
		}

		public void Clear()
		{
			Processors.Clear();
		}

		public bool Contains(SignalProcessor<InternalDataType> item)
		{
			return Processors.Contains(item);
		}

		public void CopyTo(SignalProcessor<InternalDataType>[] array, int arrayIndex)
		{
			Processors.CopyTo(array, arrayIndex);
		}

		public bool Remove(SignalProcessor<InternalDataType> item)
		{
			return Processors.Remove(item);
		}

		public IEnumerator<SignalProcessor<InternalDataType>> GetEnumerator()
		{
			return Processors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Processors.GetEnumerator();
		}
	}
}
