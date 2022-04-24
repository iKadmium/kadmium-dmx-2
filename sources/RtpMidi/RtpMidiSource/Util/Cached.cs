using System;

namespace RtpMidiSource.Util
{
	public class Cached<T>
	{
		private Func<Task<T>> Getter { get; }
		private Task<T>? LoadTask { get; set; }
		private T? Value { get; set; }
		public bool Loaded { get; private set; } = false;

		public Cached(Func<Task<T>> getter)
		{
			Getter = getter;
		}

		public async Task<T?> GetValue()
		{
			if (!Loaded)
			{
				if (LoadTask == null)
				{
					LoadTask = Getter();
				}
				Value = await LoadTask;
			}

			return Value;
		}
	}
}