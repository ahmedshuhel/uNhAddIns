namespace uNhAddIns.Adapters
{
	/// <summary>
	/// Contract for applicationi initializer.
	/// </summary>
	/// <remarks>
	/// http://en.wikipedia.org/wiki/Guy-wire
	/// A guy-wire or guy-rope is a tensioned cable designed to add stability to structures.
	/// One end of the cable is attached to the structure, and the other is anchored to the ground at a distance from the structure's base.
	/// </remarks>
	public interface IGuyWire
	{
		/// <summary>
		/// Application wire.
		/// </summary>
		/// <remarks>
		/// IoC container configuration (more probably conf. by code).
		/// </remarks>
		void Wire();

		/// <summary>
		/// Application dewire
		/// </summary>
		/// <remarks>
		/// IoC container dispose.
		/// </remarks>
		void Dewire();
	}
}