using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[Route("api/predict")]
[ApiController]
public class PredictController : ControllerBase
{
    private readonly HttpClient _httpClient;

    private const string FASTAPI_URL =
        "https://fast-api-production-58c1.up.railway.app/predict";

    public PredictController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost]
    public async Task<IActionResult> Predict()
    {
        try
        {
            // 🔥 اقرأ body كـ string مباشرة (أكثر أمانًا)
            using var reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(bodyString))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Empty body"
                });
            }

            var content = new StringContent(bodyString, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(FASTAPI_URL, content);

            var resultText = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                success = true,
                data = resultText
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

}