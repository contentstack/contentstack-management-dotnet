using System;
namespace contentstack.management.core.Http
{
	internal enum VersionStrategy
	{
		None,
		URLPath,
		Header,
		QueryParameter
	}
}

