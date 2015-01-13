using System;
using System.Threading;
using System.Collections.Concurrent;

namespace AsyncSocketLibrary.Pool
{
	/// <summary>
	/// 提供一个存储可复用对象集合的线程安全池
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Pool<T>
	{
		//PoolSlot是Pool的位置槽，其中安置着T对象实例，PoolSlot提供
		//该位置槽提供位置槽是否可用，以及提取和释放位置槽对象的功能
		private readonly ConcurrentStack<PoolSlot<T>> _storage;

		//仅控制_storage的push和pop的并发操作数量
		private readonly LockFreeSemaphore _allocSemaphore;//保证对象池线程安全的关键

		//当前对象池中的空闲可用对象数量
		//Push和Pop方法中控制其值
		private int _currentCount;

		protected Pool(int maxCapacity)
		{
			if (maxCapacity < 1)
				throw new ArgumentOutOfRangeException("maxCapacity", "Max capacity must be greater than 0");

			_storage = new ConcurrentStack<PoolSlot<T>>();
			_allocSemaphore = new LockFreeSemaphore(maxCapacity, maxCapacity);
		}

		//当前对象池中的空闲对象数量
		public int CurrentCount
		{
			get { return _currentCount; }
		}

		//当前可用的并发量
		public int TotalCount
		{
			get { return _allocSemaphore.MaxCount - _allocSemaphore.CurrentCount; }
		}

		#region 对外接口

		public void WaitAll()
		{
			//处于等待状态直到对象池中的空闲对象与可用并发量持平才退出
			//也就是说对象池中的对象是空闲待用的!
			while (_currentCount != TotalCount)
				Wait();
		}

		public PoolSlot<T> TakeSlot()
		{
			PoolSlot<T> slot;
			if (TryPop(out slot))
				return slot;

			if (TryAllocatePop(out slot))
				return slot;

			return WaitPop();
		}

		public void Release(PoolSlot<T> slot)
		{
			if (slot == null)
				throw new ArgumentNullException("slot");

			if (slot.GetStatus(this))
				throw new InvalidOperationException("Specified object is already in the pool");

			CleanUp(slot.Object);
			Push(slot);
		}

		//使当前可用并发量达到指定的targetTotalCount，同时意味着对象池中空闲待命
		//的对象也设置为使用状态，这个方法待后续澄清!
		public bool TryReduceTotal(int targetTotalCount)
		{
			if (targetTotalCount < 0)
				throw new ArgumentOutOfRangeException("targetTotalCount");

			var removingCount = TotalCount - targetTotalCount;
			for (var i = 0; i < removingCount; i++)
				if (!TryPopRemove())
					return TotalCount == targetTotalCount;
			return TotalCount == targetTotalCount;
		}

		public override string ToString()
		{
			//当前对象池中空闲对象数/可用的允许并发量/最大并发量
			return string.Format("{0}: {1}/{2}/{3}", GetType().Name, _currentCount,
				TotalCount, _allocSemaphore.MaxCount);
		}

		#endregion

		//继承类可修改delay的时间
		protected void Wait()
		{
			//如果当前线程进入不可执行状态，则休眠
			if (!Thread.Yield())
				Thread.Sleep(100);
		}        

		#region for overriding

		//提供T的构造方法
		protected abstract T ObjectConstructor();

		//可能的额外操作：位置槽和T对应关系的其他功能添加
		//用于实现IPoolSlotHolder的功能，即@object一般支持PoolSlot属性
		//通过该方法，将slot赋值到@object.PoolSlot
		protected virtual void HoldSlotInObject(T @object, PoolSlot<T> slot)
		{
		}

		//提供T的资源释放功能
		protected virtual void CleanUp(T @object)
		{
		}

		#endregion

		#region Pool Operation

		protected bool TryAllocatePush(int count)
		{
			for (var i = 0; i < count; i++)
				if (!TryAllocatePush())
					return false;
			return true;
		}

		protected bool TryAllocatePush()
		{
			if (_allocSemaphore.TryTake())
			{
				Push(Allocate());
				return true;
			}
			return false;
		}

		protected bool TryAllocatePop(out PoolSlot<T> slot)
		{
			if (_allocSemaphore.TryTake())
			{
				slot = Allocate();
				return true;
			}

			slot = null;
			return false;
		}

		//没有Push到_storage?还是留待HoldSlotInObject来做
		//精妙之处:在Release时，Push进入_storage
		private PoolSlot<T> Allocate()
		{
			var obj = ObjectConstructor();
			var slot = new PoolSlot<T>(obj, this);

			HoldSlotInObject(obj, slot);

			return slot;
		}

		protected PoolSlot<T> WaitPop()
		{
			PoolSlot<T> slot;
			while (!TryPop(out slot))
				Wait();

			return slot;
		}

		protected bool TryPopRemove()
		{
			PoolSlot<T> slot;
			if (TryPop(out slot))
			{
				//Release的蹊跷啊，这不是提高并发量了吗
				_allocSemaphore.Release();
				return true;
			}
			return false;
		}

		#endregion

		#region storage wrapper

		private void Push(PoolSlot<T> slot)
		{
			slot.SetStatus(true);
			_storage.Push(slot);

			Interlocked.Increment(ref _currentCount);
		}

		private bool TryPop(out PoolSlot<T> slot)
		{
			if (_storage.TryPop(out slot))
			{
				Interlocked.Decrement(ref _currentCount);
				slot.SetStatus(false);

				return true;
			}

			slot = null;
			return false;
		}

		#endregion
	}
}

