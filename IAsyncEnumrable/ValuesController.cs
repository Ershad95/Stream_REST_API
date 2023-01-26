using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAsyncEnumrable
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [Route("/ok")]
        public async IAsyncEnumerable<int> OK(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested)
            {
                Task.Delay(1000).Wait();
                yield return 2;
                Task.Delay(4500).Wait();
                yield return 100;
                Task.Delay(4500).Wait();
                yield return 300;
            }
        }
    }
}
