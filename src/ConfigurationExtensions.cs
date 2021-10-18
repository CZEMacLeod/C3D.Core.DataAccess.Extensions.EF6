using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace System.Data.Entity.ModelConfiguration
{
	public static class ConfigurationExtensions
	{
		public static ConventionPrimitivePropertyConfiguration HasIndex(this ConventionPrimitivePropertyConfiguration configuration,
																		IndexAttribute index = null) =>
			configuration.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(index ?? new IndexAttribute()));

		public static PrimitivePropertyConfiguration HasIndex(this PrimitivePropertyConfiguration configuration,
															  IndexAttribute index = null) =>
			configuration.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(index ?? new IndexAttribute()));

		public static ConventionPrimitivePropertyConfiguration HasIndexes(this ConventionPrimitivePropertyConfiguration configuration,
																		  params IndexAttribute[] indexes) =>
			configuration.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(indexes));

		public static PrimitivePropertyConfiguration HasIndexes(this PrimitivePropertyConfiguration configuration,
																params IndexAttribute[] indexes) =>
			configuration.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(indexes));
	}
}
