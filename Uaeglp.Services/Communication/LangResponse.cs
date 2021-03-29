using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class LangResponse : Response<List<ViewModels.Page>>
    {
        private LangResponse(bool success, string message, List<ViewModels.Page> Label) : base(success, message)
        {
            Resource = Label;
        }

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="user">user view model.</param>
		/// <returns>Response.</returns>
		public LangResponse(List<ViewModels.Page> Label) : this(true, string.Empty, Label)
		{ }

		/// <summary>
		/// Creates am error response.
		/// </summary>
		/// <param name="Message">Error Message.</param>
		/// <returns>Response.</returns>
		//public LangResponse(List<ViewModels.Page> Label, RespCode HTTPStatusCode) : this(false, HTTPStatusCode, Label)
		//{ }
	}
	public class LangResponseUI : Response<List<ViewModels.LabelNames>>
	{
		private LangResponseUI(bool success, string message, List<ViewModels.LabelNames> Label) : base(success, message)
		{
			Resource = Label;
		}

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="user">user view model.</param>
		/// <returns>Response.</returns>
		public LangResponseUI(List<ViewModels.LabelNames> Label) : this(true, string.Empty, Label)
		{ }

		/// <summary>
		/// Creates am error response.
		/// </summary>
		/// <param name="Message">Error Message.</param>
		/// <returns>Response.</returns>
		//public LangResponse(List<ViewModels.Page> Label, RespCode HTTPStatusCode) : this(false, HTTPStatusCode, Label)
		//{ }
	}
	public class PageResponseUI : Response<List<ViewModels.PageNames>>
	{
		private PageResponseUI(bool success, string message, List<ViewModels.PageNames> Label) : base(success, message)
		{
			Resource = Label;
		}

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="user">user view model.</param>
		/// <returns>Response.</returns>
		public PageResponseUI(List<ViewModels.PageNames> Label) : this(true, string.Empty, Label)
		{ }

		/// <summary>
		/// Creates am error response.
		/// </summary>
		/// <param name="Message">Error Message.</param>
		/// <returns>Response.</returns>
		//public LangResponse(List<ViewModels.Page> Label, RespCode HTTPStatusCode) : this(false, HTTPStatusCode, Label)
		//{ }
	}
}
