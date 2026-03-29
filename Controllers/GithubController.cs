using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

[ApiController]
[Route("api/github")]
public class GitHubController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public GitHubController(IConfiguration config)
    {
        _httpClient = new HttpClient();

        _token = config["GitHub:Token"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("token", _token);

        _httpClient.DefaultRequestHeaders.Add("User-Agent", "GithubConnector");
    }

    [HttpGet("repos/{username}")]
    public async Task<IActionResult> GetRepos(string username)
    {
        var url = $"https://api.github.com/users/{username}/repos";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest("Error fetching repos");
        }

        var data = await response.Content.ReadAsStringAsync();

        return Ok(data);
    }
    [HttpPost("create-issue")]
    public async Task<IActionResult> CreateIssue(
    string owner,
    string repo,
    string title,
    string body)
    {
        var url = $"https://api.github.com/repos/{owner}/{repo}/issues";

        var json = $@"{{
        ""title"": ""{title}"",
        ""body"": ""{body}""
    }}";

        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest("Error creating issue");
        }

        var result = await response.Content.ReadAsStringAsync();

        return Content(result, "application/json");
    }
}