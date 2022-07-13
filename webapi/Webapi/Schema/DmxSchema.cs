using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using GraphQL.Instrumentation;

namespace Webapi.Schema
{
	public class DmxSchema : GraphQL.Types.Schema
	{
		public DmxSchema(IServiceProvider provider) : base(provider)
		{
			Query = provider.GetRequiredService<DmxQuery>();

			FieldMiddleware.Use(new InstrumentFieldsMiddleware());
		}
	}
}