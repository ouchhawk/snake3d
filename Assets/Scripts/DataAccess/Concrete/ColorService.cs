using Assets.Scripts.DataAccess.Abstract;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class ColorService : IColorService
    {
        private static readonly HttpClient client = new HttpClient();

        public class PaletteResponse
        {
            public List<List<int>> result { get; set; }
        }

        public async Task<HttpResponseMessage> FetchColorPaletteAsync()
        {
            var url = "http://colormind.io/api/";
            var data = new
            {
                model = "default",
                input = new object[]
                {
                    new int[] { 44, 43, 44 },
                    new int[] { 90, 83, 82 },
                    "N", "N", "N"
                }
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var paletteResponse = JsonConvert.DeserializeObject<PaletteResponse>(responseContent);

                foreach (var color in paletteResponse.result)
                {
                    Debug.Log($"[{color[0]}, {color[1]}, {color[2]}]");
                }
            }
            else
            {
                Debug.LogError($"Request was not successful. Status code: {response.StatusCode}");
            }

            return response;
        }
    }
}
