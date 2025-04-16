using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace Server.Handlers
{
    public static class GoldHandlers
    {
        private static readonly string goldApiKeyUrl = "https://api.vnappmob.com/api/request_api_key?scope=gold";
        //{"results":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3NDU2OTE3OTcsImlhdCI6MTc0NDM5NTc5Nywic2NvcGUiOiJnb2xkIiwicGVybWlzc2lvbiI6MH0.h9Of7FkeSA82kGcU4lbP_CHLkAPmIV1zXFVJGUneass"}
        private static readonly string goldApiUrlBase = "https://api.vnappmob.com/api/v2/gold/sjc?api_key=";
        //{"results":[{"buy_1c":"102200000.0","buy_1l":"102200000.0","buy_5c":"102200000.0","buy_nhan1c":"101100000.0","buy_nutrang_75":"74282793.2793","buy_nutrang_99":"99071287.1287","buy_nutrang_9999":"101100000.0","datetime":"1744358406","sell_1c":"105230000.0","sell_1l":"105200000.0","sell_5c":"105220000.0","sell_nhan1c":"104400000.0","sell_nutrang_75":"78082793.2793","sell_nutrang_99":"102871287.1287","sell_nutrang_9999":"103900000.0"}]}

        private static HttpClient _http = new();
        private static string _apiKey;

        public static async Task InitializeAsync()
        {
            try
            {
                var keyResponse = await _http.GetStringAsync(goldApiKeyUrl);
                var keyJson = JObject.Parse(keyResponse);
                _apiKey = keyJson["results"]?.ToString();

                Console.WriteLine(!string.IsNullOrWhiteSpace(_apiKey)
                    ? "🔑 API key lấy thành công."
                    : "❌ Không lấy được API key.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API KEY ERROR] {ex.Message}");
            }
        }

        public static async Task<string> GetGoldPriceJsonAsync()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                return JsonConvert.SerializeObject(new { error = "Server chưa có API key." });

            try
            {
                var goldResponse = await _http.GetStringAsync(goldApiUrlBase + _apiKey);
                var goldJson = JObject.Parse(goldResponse);
                var item = goldJson["results"]?.FirstOrDefault();
                if (item == null)
                    return JsonConvert.SerializeObject(new { error = "Không có dữ liệu giá vàng." });

                var vn = CultureInfo.GetCultureInfo("vi-VN");

                string Format(string field) =>
                    string.Format(vn, "{0:#,##0}", decimal.Parse(item[field]?.ToString() ?? "0", CultureInfo.InvariantCulture));

                var data = new[]
                {
            new { type = "Vàng SJC 1L, 10L, 1KG", buy = Format("buy_1l"), sell = Format("sell_1l") },
            new { type = "Vàng SJC 5 chỉ", buy = Format("buy_5c"), sell = Format("sell_5c") },
            new { type = "Vàng SJC 0.5, 1, 2 chỉ", buy = Format("buy_1c"), sell = Format("sell_1c") },
            new { type = "Vàng nhẫn 99.99% (1-5 chỉ)", buy = Format("buy_nhan1c"), sell = Format("sell_nhan1c") },
            new { type = "Nữ trang 99.99%", buy = Format("buy_nutrang_9999"), sell = Format("sell_nutrang_9999") },
            new { type = "Nữ trang 99%", buy = Format("buy_nutrang_99"), sell = Format("sell_nutrang_99") },
            new { type = "Nữ trang 75%", buy = Format("buy_nutrang_75"), sell = Format("sell_nutrang_75") },
        };

                return JsonConvert.SerializeObject(data, Formatting.None);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = "Lỗi xử lý: " + ex.Message });
            }
        }

    }
}
