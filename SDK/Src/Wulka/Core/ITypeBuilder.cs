using System;

namespace Wulka.Core
{
	internal interface ITypeBuilder
	{
		Type GenerateType(string className);
	}
}