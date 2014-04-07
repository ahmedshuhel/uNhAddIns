using System;
using NHibernate;

namespace uNhAddIns.Extensions
{
	public static class ReflectionExtensions
	{
		public static TResult Instantiate<TResult>(this Type type) where TResult: class
		{
			try
			{
				return (TResult) Activator.CreateInstance(type);
			}
			catch (MissingMethodException ex)
			{
				throw new InstantiationException("Public constructor was not found for " + type, ex, type);
			}
			catch (InvalidCastException ex)
			{
				throw new InstantiationException(type + "Type does not implement "+ typeof(TResult)  , ex, type);
			}
			catch (Exception ex)
			{
				throw new InstantiationException("Unable to instantiate: " + type, ex, type);
			}
		}

		public static TResult Instantiate<TResult>(this Type type, params object[] args) where TResult : class
		{
			try
			{
				return (TResult)Activator.CreateInstance(type, args);
			}
			catch (MissingMethodException ex)
			{
				throw new InstantiationException("Public constructor was not found for " + type, ex, type);
			}
			catch (InvalidCastException ex)
			{
				throw new InstantiationException(type + "Type does not implement " + typeof(TResult), ex, type);
			}
			catch (Exception ex)
			{
				throw new InstantiationException("Unable to instantiate: " + type, ex, type);
			}
		}
	}
}