using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Uaeglp.Web.Filters
{
	public class FileUploadOperation : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.OperationId == null) return;
			if (operation.OperationId.ToLower() == "uploadfile")
			{
				operation.Parameters.Clear();
				operation.RequestBody = new OpenApiRequestBody
				{
					Content = new Dictionary<string, OpenApiMediaType>()
				};
				operation.RequestBody.Content.Add("application/octet-stream", new OpenApiMediaType
				{
					Schema = new OpenApiSchema
					{
						Type = "string",
						Format = "binary"
					}
				});
			}
		}
	}
}
