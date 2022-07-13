using System.Collections.Generic;
using System.Security.Claims;

namespace Webapi
{
	public class GraphQLUserContext : Dictionary<string, object>
	{
		public GraphQLUserContext(ClaimsPrincipal user)
		{
			User = user;
		}

		public ClaimsPrincipal User { get; set; }
	}
}