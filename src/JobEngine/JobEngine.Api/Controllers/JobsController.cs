using JobEngine.Core.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JobEngine.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobClient _jobClient;


    public JobsController(IJobClient jobClient)
    {
        ArgumentNullException.ThrowIfNull(jobClient);

        _jobClient = jobClient;
    }


    [HttpGet]
    public IActionResult Get()
    {
        var jobContainer = new JobContainer();

        try
        {
            _jobClient.Schedule(() => jobContainer.DelayedJob("Ali", 20), TimeSpan.FromMinutes(1));
        }
        catch (Exception ex)
        {

        }

        return Ok();
    }
}


public class JobContainer
{
    public void DelayedJob(string name, int age)
    {
        Console.WriteLine($"Person with name {name} and age {age} called Delayed Job");
    }
}
