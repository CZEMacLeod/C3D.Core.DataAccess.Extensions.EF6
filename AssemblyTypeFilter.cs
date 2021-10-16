namespace System.Data.Entity.ModelConfiguration
{
	[Flags]
	public enum AssemblyTypeFilter
	{
		None = 0,
		/// <summary>
		/// Filter for Model types only
		/// </summary>
		Model = 1,
		/// <summary>
		/// Filter for EntityTypeConfiguration types only
		/// </summary>
		EntityTypeConfiguration = 2,
		/// <summary>
		/// Filter for ComplexTypeConfiguration types only
		/// </summary>
		ComplexTypeConfiguration = 4,
		/// <summary>
		/// Filter for both EntityTypeConfiguration and ComplexTypeConfiguration types
		/// </summary>
		TypeConfigurations = 6,
		/// <summary>
		/// Filter for both all types
		/// </summary>
		All = 7

	}
}
