using System;

namespace AsyncSocketLibrary.Pool
{
	/// <summary>
	/// PoolSlot是Pool的位置槽，其中安置着T对象实例，PoolSlot提供
	/// 该位置槽提供查询位置槽是否可用，以及提取和释放位置槽中对象的功能
	/// PoolSlot只供Pool使用，固很多方法都是internal
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PoolSlot<T> : IDisposable
	{
		private readonly T _object;
		private readonly Pool<T> _pool;

		//true：当前位置槽可用，亦即当前位置槽中的对象是空闲可用的!，false不可用
		private bool _inPool;

		//只限于当前项目可使用
		//T和对应的pool必须同时进行配置，才能确保Pool->PoolSlot->T instance的对应关系
		//且为了维护这个对应关系，必须采用readonly模式和internal模式
		internal PoolSlot(T @object, Pool<T> pool)
		{
			_object = @object;
			_pool = pool;
		}

		internal bool GetStatus(Pool<T> pool)
		{
			if (_pool != pool)
				throw new ArgumentException("This slot not for specified pool", "pool");
			return _inPool;
		}

		internal void SetStatus(bool inPool)
		{
			_inPool = inPool;
		}

		//这个必须不能是internal了!
		public T Object
		{
			get { return _object; }
		}

		public void Dispose() {

			//释放当前位置槽，亦即回归对象池待命
			_pool.Release(this);
		}
	}
}

