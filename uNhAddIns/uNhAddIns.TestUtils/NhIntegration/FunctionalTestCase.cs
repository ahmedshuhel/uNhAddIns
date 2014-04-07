using NUnit.Framework;

namespace uNhAddIns.TestUtils.NhIntegration
{
	/// <summary>
	/// The <see cref="FunctionalTestCase"/> should be the base class of your tests, 
	/// where you need manage the nh's session out-side any kind of pattern.
	/// </summary>
	public class FunctionalTestCase : FunctionalTestCaseTemplate
	{
		public FunctionalTestCase()
		{
			// Convention: mappings are in the same namespace of the test
			var ml = new NamespaceMappingsLoader(GetType().Assembly, GetType().Namespace);
			var s = new DefaultFunctionalTestSettings(ml);
			Settings = s;
		}

		public FunctionalTestCase(string baseName, string[] mappings)
		{
			var ml = new ResourceWithRelativeNameMappingsLoader(GetType().Assembly, baseName, mappings);
			var s = new DefaultFunctionalTestSettings(ml);
			Settings = s;
		}

		#region Overrides of AbstractFunctionalTestCase

		protected override IFunctionalTestSettings Settings { get; set; }

		#endregion

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			SetUpNhibernate();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			ShutdownNhibernate();
		}

		[TearDown]
		public void TearDown()
		{
			OnTearDown();
			AssertAllDataRemovedIfNeeded();
		}

		protected virtual void OnTearDown()
		{

		}
	}
}