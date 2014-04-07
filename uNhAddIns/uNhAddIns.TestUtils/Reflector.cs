using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace uNhAddIns.TestUtils
{
	public static class Reflector
	{
		public static MemberInfo MemberInfo<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			MemberExpression me=null;
			switch (expression.Body.NodeType)
			{
				case ExpressionType.MemberAccess:
					me= (MemberExpression) expression.Body;
					break;
				case ExpressionType.Convert:
					var unary = expression.Body as UnaryExpression;
					if (unary != null)
					{
						me = (MemberExpression) unary.Operand;
					}
					break;
			}
			return me != null ? me.Member : null;
		}

		public static MethodInfo MethodInfo<TEntity>(Expression<Action<TEntity>> expression)
		{
			return ExpressionType.Call.Equals(expression.Body.NodeType) ? ((MethodCallExpression) expression.Body).Method : null;
		}

		public static MethodInfo[] MethodsInfos<TEntity>(params Expression<Action<TEntity>>[] expressions)
		{
			var result = new List<MethodInfo>();
			foreach (var expression in expressions)
			{
				result.Add(MethodInfo(expression));
			}
			return result.ToArray();
		}

		public static MethodInfo PropertyGetter<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			var mi = MemberInfo(expression);
			if (mi != null && mi.MemberType == MemberTypes.Property)
			{
				var accessors = ((PropertyInfo) mi).GetAccessors();
				return accessors == null ? null : accessors.Where(x => x.Name.StartsWith("get_")).First();
			}
			else
			{
				throw new NotSupportedException("The expression is not a property.");
			}
		}

		public static MethodInfo PropertySetter<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			var mi = MemberInfo(expression);
			if (mi != null && mi.MemberType == MemberTypes.Property)
			{
				var accessors = ((PropertyInfo)mi).GetAccessors();
				return accessors == null ? null : accessors.Where(x => x.Name.StartsWith("set_")).First();
			}
			else
			{
				throw new NotSupportedException("The expression is not a property.");
			}
		}
	}
}