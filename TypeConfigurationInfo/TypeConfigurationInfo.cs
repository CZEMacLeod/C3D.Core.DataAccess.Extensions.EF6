using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Text;

namespace C3D.Core.DataAccess.Extensions
{
	internal abstract class TypeConfigurationInfo : TypeInfo
	{
		protected ConfigurationRegistrar registrar;

		protected TypeConfigurationInfo(ConfigurationRegistrar registrar)
		{
			this.registrar = registrar;
		}
	}
}
