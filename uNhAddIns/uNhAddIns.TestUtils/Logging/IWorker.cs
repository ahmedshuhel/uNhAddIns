using System;

namespace uNhAddIns.TestUtils.Logging
{
	internal interface IWorker
	{
		void Enlist(Action work);
		void ExecuteEnlistments();
	}
}