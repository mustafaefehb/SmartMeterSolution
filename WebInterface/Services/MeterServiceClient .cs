using Microsoft.Extensions.Configuration;
using Shared.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebInterface.Models;

namespace WebInterface.Services
{
    public class MeterServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _meterServiceUrl;

        public MeterServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _meterServiceUrl = configuration["Services:MeterService"];
        }

        public async Task<PagedResult<MeterData>> GetMeterDataAsync(int pageNumber, int pageSize, string meterSerialNumber = null)
        {
            var filter = !string.IsNullOrEmpty(meterSerialNumber) ? $"&meterSerialNumber={meterSerialNumber}" : string.Empty;
            var response = await _httpClient.GetAsync($"{_meterServiceUrl}/api/meter?pageNumber={pageNumber}&pageSize={pageSize}{filter}");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PagedResult<MeterData>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<MeterData> GetMeterDataByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_meterServiceUrl}/api/meter/{id}");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MeterData>(responseBody);
        }

        public async Task CreateMeterDataAsync(MeterData meterData)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(meterData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_meterServiceUrl}/api/meter", jsonContent);
            response.EnsureSuccessStatusCode();
        }
    }
}