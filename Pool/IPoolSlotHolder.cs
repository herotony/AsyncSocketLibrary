using System;

namespace AsyncSocketLibrary.Pool
{
	public interface IPoolSlotHolder<T>
	{
		PoolSlot<T> PoolSlot { get; set; }
	}
}

