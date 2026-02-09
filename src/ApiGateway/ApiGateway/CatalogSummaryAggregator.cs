using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace ApiGateway;

public sealed class CatalogSummaryAggregator : IDefinedAggregator
{
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        var productResponse = responses[0].Items.DownstreamResponse();
        var categoryResponse = responses[1].Items.DownstreamResponse();

        var products = await productResponse.Content.ReadAsStringAsync();
        var categories = await categoryResponse.Content.ReadAsStringAsync();

        var json = $"{{\"products\":{products},\"categories\":{categories}}}";
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return new DownstreamResponse(content, System.Net.HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");
    }
}
