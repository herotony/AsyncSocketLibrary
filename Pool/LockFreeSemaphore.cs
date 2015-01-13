using System;
using System.Threading;

namespace AsyncSocketLibrary.Pool
{
	/// <summary>
	/// 并发控制信号类
	/// seal:不允许继承
	/// 
	/// 应用场景：TryTake成功，开启一个并发；Release一个或多个并发
	/// 
	/// </summary>
	public sealed class LockFreeSemaphore
	{
		//最大并发量
		private readonly int _maxCount;

		//当前并发量
		private int _currentCount;

		public LockFreeSemaphore(int initialCount)
			: this(initialCount, int.MaxValue)
		{ 
		}

		public LockFreeSemaphore(int initialCount, int maxCount) {

			if (initialCount < 0 || maxCount < initialCount)
				throw new ArgumentOutOfRangeException("initialCount");

			if (maxCount <= 0)
				throw new ArgumentOutOfRangeException("maxCount");

			_currentCount = initialCount;
			_maxCount = maxCount;
		}

		public int MaxCount
		{
			get { return _maxCount; }
		}

		public int CurrentCount
		{
			get { return _currentCount; }
		}

		//true，允许新开一个并发线程；false，不允许
		public bool TryTake()
		{
			int oldValue, newValue;
			do
			{
				oldValue = _currentCount;
				newValue = oldValue - 1;

				if (newValue < 0) return false;
			} while (Interlocked.CompareExchange(ref _currentCount, newValue, oldValue) != oldValue);
			//oldValue如果与_currentCount相等，则用newValue替代_currentCount，而该方法固定返回_currentCount的原始值(如果newValue替换，则是替换前的值)
			//多线程情况下，不断修正currentCount，确保currentCount被正确修正!即确保_currentCount此间没被其他线程修改，若修改这重新进行一次修正
			//currentCount减一操作得以正确赋值，表示被占用了一个名额

			return true;
		}

		public void Release()
		{
			int oldValue, newValue;
			do
			{
				oldValue = _currentCount;
				newValue = oldValue + 1;
				if (newValue > _maxCount)
					throw new SemaphoreFullException();
			} while (Interlocked.CompareExchange(ref _currentCount, newValue, oldValue) != oldValue);
			//同样是确保多线程下，currentCount被正确修正!
			//currentCoun加一操作得以正确赋值，表示一个占用名额得以释放

		}

		//一次释放releaseCount个占用名额
		public void Release(int releaseCount)
		{
			if (releaseCount < 1)
				throw new ArgumentOutOfRangeException("releaseCount", "Release сount is less than 1");

			int oldValue, newValue;
			do
			{
				oldValue = _currentCount;
				newValue = oldValue + releaseCount;
				if (newValue > _maxCount)
					throw new SemaphoreFullException();
			} while (Interlocked.CompareExchange(ref _currentCount, newValue, oldValue) != oldValue);
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}/{2}", typeof(LockFreeSemaphore).Name, _currentCount, _maxCount);
		}
	}
}

