using System;

namespace AsyncSocketLibrary.Pool
{
	//该类的唯一扩展就是@object对象必须支持了PoolSlot属性，以支持Release方法
	public abstract class PoolEx<T> : Pool<T> where T : IPoolSlotHolder<T>
	{
		protected PoolEx(int maxCapacity) : base(maxCapacity) { }

		public T TakeObject() { return TakeSlot().Object; }

		public void Release(T item) {

			if (item == null)
				throw new ArgumentNullException("item");

			Release(item.PoolSlot);
		}
	}
}

