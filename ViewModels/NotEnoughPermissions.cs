using Microsoft.AspNetCore.Mvc;

public class NotEnoughPermissions : ActionResult
{
    public override void ExecuteResult(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.StatusCode = 403; // Forbidden
        response.ContentType = "application/json";
        response.WriteAsync("{\"message\":\"Não tens permissões para tal\"}");
    }
}
