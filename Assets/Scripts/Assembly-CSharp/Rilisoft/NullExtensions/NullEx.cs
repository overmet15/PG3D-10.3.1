using System;

namespace Rilisoft.NullExtensions
{
	public static class NullEx
	{
		public static TResult Map<TInput, TResult>(this TInput o, Func<TInput, TResult> selector) where TInput : class
		{
			if (o == null)
			{
				return default(TResult);
			}
			return selector(o);
		}

		public static TResult Map<TInput, TResult>(this TInput o, Func<TInput, TResult> selector, TResult defaultValue) where TInput : class
		{
			if (o == null)
			{
				return defaultValue;
			}
			return selector(o);
		}

		public static TResult Catch<TInput, TResult>(this TInput o, Func<TInput, TResult> selector) where TInput : class
		{
			if (o == null)
			{
				return default(TResult);
			}
			try
			{
				return selector(o);
			}
			catch (NullReferenceException)
			{
				return default(TResult);
			}
		}

		public static TResult CatchAll<TInput, TResult>(this TInput o, Func<TInput, TResult> selector) where TInput : class
		{
			if (o == null)
			{
				return default(TResult);
			}
			try
			{
				return selector(o);
			}
			catch
			{
				return default(TResult);
			}
		}

		public static TResult Catch<TInput, TResult>(this TInput o, Func<TInput, TResult> selector, TResult defaultValue) where TInput : class
		{
			if (o == null)
			{
				return defaultValue;
			}
			try
			{
				return selector(o);
			}
			catch (NullReferenceException)
			{
				return defaultValue;
			}
		}

		public static TInput Filter<TInput>(this TInput o, Func<TInput, bool> pred) where TInput : class
		{
			if (o == null)
			{
				return (TInput)null;
			}
			return (!pred(o)) ? ((TInput)null) : o;
		}

		public static TInput Do<TInput>(this TInput o, Action<TInput> action) where TInput : class
		{
			if (o == null)
			{
				return (TInput)null;
			}
			action(o);
			return o;
		}

		public static TInput Do<TInput>(this TInput o, Action<TInput> action, Action defaultAction) where TInput : class
		{
			if (o == null)
			{
				defaultAction();
				return (TInput)null;
			}
			action(o);
			return o;
		}
	}
}
