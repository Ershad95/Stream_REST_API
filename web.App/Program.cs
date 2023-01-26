using Newtonsoft.Json;

const int StartUpTime = 6000;
const int TARGET = 80;
var _httpClient = new HttpClient();
var _cancelationTokenSource = new CancellationTokenSource();

await Task.Delay(StartUpTime);
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


public class Customer
{
    public int Id { get; private set; }

    public string Name { get; private set; }
    public Customer(string name, int id)
    {
        Name = name;
        Id = id;
    }
}

