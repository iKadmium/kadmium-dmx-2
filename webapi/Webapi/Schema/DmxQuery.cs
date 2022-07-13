using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;

namespace Webapi.Schema
{
	public class DmxQuery : ObjectGraphType<object>
	{
		public DmxQuery()
		{
			Name = "Query";

			Field<StringGraphType>()
				.Name("hello")
				.Resolve(x => "World");
		}
	}
}