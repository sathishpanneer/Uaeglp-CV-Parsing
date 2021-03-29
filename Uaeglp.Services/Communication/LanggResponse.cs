using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
	public class LanggResponse : Response<ViewModels.PageNew>
	{
		private LanggResponse(bool success, string message, ViewModels.PageNew Label) : base(success, message)
		{
			Resource = Label;
		}

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="user">user view model.</param>
		/// <returns>Response.</returns>
		public LanggResponse(ViewModels.PageNew Label) : this(true, string.Empty, Label)
		{ }

		/// <summary>
		/// Creates am error response.
		/// </summary>
		/// <param name="Message">Error Message.</param>
		/// <returns>Response.</returns>
		//public LanggResponse(ViewModels.PageNew Label, RespCode HTTPStatusCode) : this(false, HTTPStatusCode, Label)
		//{ }
	}
}
