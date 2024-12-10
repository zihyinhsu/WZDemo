using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WZDemo.API.CustomActionFilter
{
	// 模型驗證
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			if (context.ModelState.IsValid == false)
			{
				context.Result = new BadRequestResult();
			}
		}
	}
}
