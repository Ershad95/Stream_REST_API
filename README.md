# Stream_REST_API
Make REST Api with Stream Data with IAsyncEnumerable

Server Side : 
<pre>
 [HttpGet]
  public async IAsyncEnumerable<Customer> Get(CancellationToken cancellationToken)
   {
      while (!cancellationToken.IsCancellationRequested && _customers.Any(_ => _.Key % 10 == 0))
      {
          var customer = _customers.First(_ => _.Key % 10 == 0);
           yield return new Customer(customer.Value.Name, customer.Key);
           _customers.Remove(customer);
            await Task.Delay(500,cancellationToken);
       }
   }
</pre>


Client Side : 

<pre>

const int TARGET = 80;
var _httpClient = new HttpClient();
var _cancelationTokenSource = new CancellationTokenSource();

using (var response = await _httpClient.GetAsync(
    "https://localhost:7284/customer",
     HttpCompletionOption.ResponseHeadersRead,
     _cancelationTokenSource.Token))
{
    var stream = await response.Content.ReadAsStreamAsync(_cancelationTokenSource.Token);

    var _jsonSerializerSettings = new JsonSerializerSettings();
    var _serializer = Newtonsoft.Json.JsonSerializer.Create(_jsonSerializerSettings);

    using TextReader textReader = new StreamReader(stream);
    using JsonReader jsonReader = new JsonTextReader(textReader);

    await using (stream.ConfigureAwait(false))
    {
        await jsonReader.ReadAsync(_cancelationTokenSource.Token).ConfigureAwait(false);
        while (await jsonReader.ReadAsync(_cancelationTokenSource.Token).ConfigureAwait(false) &&
               jsonReader.TokenType != JsonToken.EndArray)
        {
            Customer customer = _serializer!.Deserialize<Customer>(jsonReader);
            if (customer.Id == TARGET)
            {
                Console.WriteLine(customer.Id + " : " + customer.Name);
                _cancelationTokenSource.Cancel();
                break;
            }
        }
    }
}
</pre>
