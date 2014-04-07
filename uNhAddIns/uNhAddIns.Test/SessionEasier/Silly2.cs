using System;

namespace uNhAddIns.Test.SessionEasier
{
	[Serializable]
	public class Silly2
	{
		public virtual long Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Other2 Other { get; set; }
	}
}