using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.Services.Nlog
{
	public interface ILog
	{
		void Information(string message);
		void Warning(string message);
		void Debug(string message);
		void Error(string message);
	}
}
