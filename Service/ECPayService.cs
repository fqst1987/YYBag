using System.Security.Cryptography;
using System.Text;
using System.Web;
using YYBagProgram.Models;
using YYBagProgram.Service.Interface;

namespace YYBagProgram.Service
{
    public class ECPayService
    {
        private readonly ECPaySettings _ecpaySettings;
        private readonly HttpClient _httpClient;
        private readonly CryptographyService _cryptoService;

        public ECPayService(ECPaySettings ecpaySettings, HttpClient httpClient, CryptographyService cryptographyService)
        {
            _ecpaySettings = ecpaySettings;
            _httpClient = httpClient;
            _cryptoService = cryptographyService;
        }

        public async Task<string> GenerateMapRequest(string MerchantID, string MerchantTradeNo, string LogisticsSubType, string IsCollection)
        {
            _cryptoService.HashPassword(MerchantID);

            var parameters = $"MerchantID={MerchantID}&MerchantTradeNo={MerchantTradeNo}&LogisticsSubType={LogisticsSubType}&IsCollection={IsCollection}";
            string checkMacValue = GenerateCheckMacValue(parameters);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("MerchantID", MerchantID),
                new KeyValuePair<string, string>("MerchantTradeNo", MerchantTradeNo),
                new KeyValuePair<string, string>("LogisticsSubType", LogisticsSubType),
                new KeyValuePair<string, string>("IsCollection", IsCollection),
                new KeyValuePair<string, string>("CheckMacValue", checkMacValue)
            });

            var response = await _httpClient.PostAsync(_ecpaySettings.MapUrl, content);
            return await response.Content.ReadAsStringAsync();
        }

        private string GenerateCheckMacValue(string parameters)
        {
            string raw = $"HashKey={_ecpaySettings.HashKey}&{parameters}&HashIV={_ecpaySettings.HashIV}";
            string urlEncoded = HttpUtility.UrlEncode(raw).ToLower();

            return _cryptoService.HashPassword(urlEncoded);
        }
    }
}
