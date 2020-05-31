using System.Threading.Tasks;
using LongRunningTasks.BackgroundWorkload;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace LongRunningTasks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public TaskController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int value)
        {
            var grain = _grainFactory.GetGrain<IBackgroundWorkload<int, string>>("Test");

            await grain.StartAsync(value);

            while (true)
            {
                var result = await grain.GetResultAsync();
                switch (result)
                {
                    case Started _:
                        await Task.Delay(100);
                        break;
                        
                    case Completed<string> completed:
                        return Ok($"Completed: {completed.Response}");
                    
                    case Failed failed:
                        return Ok($"Failed: {failed.Exception}");                    
                }
            }
        }
    }
}
