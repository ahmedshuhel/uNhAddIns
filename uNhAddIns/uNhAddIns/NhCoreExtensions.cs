using System.Reflection;
using NHibernate.Engine;

namespace uNhAddIns
{
	public static class NhCoreExtensions
	{
		private static readonly FieldInfo EntityEntryStatusFieldInfo =
	                typeof(EntityEntry).GetField("status", BindingFlags.NonPublic | BindingFlags.Instance);

		public static void BackSetStatus(this EntityEntry entry, Status status)
		{
			EntityEntryStatusFieldInfo.SetValue(entry, status);
		}
	}
}