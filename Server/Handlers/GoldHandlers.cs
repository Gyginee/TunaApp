
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization; 
using Newtonsoft.Json;
using System.Collections.Generic; 
using System.Linq; 

namespace Server.Handlers
{
    public class GoldData
    {
        [JsonProperty("type")] 
        public string Type { get; set; }

        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("buy")]
        public decimal Buy { get; set; } 

        [JsonProperty("sell")]
        public decimal Sell { get; set; }
    }

    public static class GoldHandlers
    {
        private static readonly string sjcApiUrl = "https://sjc.com.vn/GoldPrice/Services/PriceService.ashx";
        private static readonly HttpClient _http = new HttpClient();
        private static readonly CultureInfo VietnameseCulture = CultureInfo.GetCultureInfo("vi-VN");

        public static Task InitializeAsync() => Task.CompletedTask;

        public static async Task<string> GetGoldPriceJsonAsync()
        {
            try
            {
                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(15));
                HttpResponseMessage response = await _http.GetAsync(sjcApiUrl, cts.Token);
                response.EnsureSuccessStatusCode(); 

                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                var list = json["data"] as JArray;
                if (list == null || !list.HasValues)
                {
                    return JsonConvert.SerializeObject(new { message = "Không tìm thấy dữ liệu giá vàng từ SJC." });
                }

                var goldDataList = new List<GoldData>();

                foreach (var item in list)
                {
                    var type = item["TypeName"]?.ToString() ?? string.Empty;
                    var branch = item["BranchName"]?.ToString() ?? string.Empty;
                    var buyStr = item["Buy"]?.ToString();
                    var sellStr = item["Sell"]?.ToString();

                    decimal buyValue = 0;
                    decimal sellValue = 0;

                    if (!string.IsNullOrWhiteSpace(buyStr))
                    {
                        decimal.TryParse(buyStr, NumberStyles.Number, VietnameseCulture, out buyValue);
                    }
                    if (!string.IsNullOrWhiteSpace(sellStr))
                    {
                        decimal.TryParse(sellStr, NumberStyles.Number, VietnameseCulture, out sellValue);
                    }

                    goldDataList.Add(new GoldData
                    {
                        Type = type,
                        Branch = branch,
                        Buy = buyValue, 
                        Sell = sellValue 
                    });
                }

                return JsonConvert.SerializeObject(goldDataList, Formatting.None);
            }
            catch (HttpRequestException httpEx)
            {
                //Console.WriteLine($"[GOLD HANDLER ERROR] HTTP request failed: {httpEx.Message}");
                return JsonConvert.SerializeObject(new { error = $"Lỗi kết nối đến SJC: {httpEx.Message}" });
            }
            catch (JsonException jsonEx)
            {
                //Console.WriteLine($"[GOLD HANDLER ERROR] JSON parsing failed: {jsonEx.Message}");
                return JsonConvert.SerializeObject(new { error = $"Lỗi xử lý dữ liệu từ SJC: {jsonEx.Message}" });
            }
            catch (TaskCanceledException timeoutEx)
            {
                //Console.WriteLine($"[GOLD HANDLER ERROR] Request timed out: {timeoutEx.Message}");
                return JsonConvert.SerializeObject(new { error = "Yêu cầu đến SJC bị timeout." });
            }
            catch (Exception ex) 
            {
                //Console.WriteLine($"[GOLD HANDLER ERROR] Unexpected error: {ex.Message}\n{ex.StackTrace}");
                return JsonConvert.SerializeObject(new { error = $"Lỗi không xác định khi lấy giá vàng: {ex.Message}" });
            }
        }
    }
}