using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels;

namespace Uaeglp.Services
{
	public abstract class BaseService
	{
		public Result Result(bool success, string status = null, string Message = null)
		{
			return new Result
			{
				
			};
		}
	}
}
