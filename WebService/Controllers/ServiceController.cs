using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        [HttpGet("add")]
        public IActionResult Add(double a, double b) => Ok(a + b);

        [HttpGet("subtract")]
        public IActionResult Subtract(double a, double b) => Ok(a - b);

        [HttpGet("multiply")]
        public IActionResult Multiply(double a, double b) => Ok(a * b);

        [HttpGet("divide")]
        public IActionResult Divide(double a, double b)
        {
            if (b == 0)
                return BadRequest("Không thể chia cho 0.");
            return Ok(a / b);
        }

        [HttpGet("rectangle/area")]
        public IActionResult RectangleArea(double width, double height) => Ok(width * height);

        [HttpGet("developer")]
        public IActionResult Developer() => Ok("Xin chào, tôi là TUNA");
    }
}
