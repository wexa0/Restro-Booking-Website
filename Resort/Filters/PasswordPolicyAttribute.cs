using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Resort.Models.Auth;

namespace Resort.Filters;

public class PasswordPolicyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var model = context.ActionArguments.Values.OfType<RegisterModel>().FirstOrDefault();
        if (model is null) return;

        var password = model.Password ?? "";

        if (password.Length < 8)
        {
            ReturnWithError(context, model, "Password must be at least 8 characters.");
            return;
        }

        if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
        {
            ReturnWithError(context, model, "Password must contain letters and numbers.");
            return;
        }
    }

    private static void ReturnWithError(ActionExecutingContext context, RegisterModel model, string message)
    {
        if (context.Controller is Controller controller)
        {
            controller.ModelState.AddModelError(nameof(RegisterModel.Password), message);
            context.Result = controller.View("Register", model);
        }
    }
}
