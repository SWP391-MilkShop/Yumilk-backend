using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;

namespace NET1814_MilkShop.API.CoreHelpers.Extensions
{
    public static class ResponseExtension
    {
        public static IActionResult Result(ResponseModel responseModel) =>
            responseModel.StatusCode switch
            {
                StatusCodes.Status200OK => new OkObjectResult(responseModel),
                StatusCodes.Status400BadRequest => new BadRequestObjectResult(responseModel),
                StatusCodes.Status404NotFound => new NotFoundObjectResult(responseModel),
                StatusCodes.Status500InternalServerError
                    => new ObjectResult(responseModel)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    },
                _ => new BadRequestObjectResult(responseModel)
            };
    }
}
