using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineRA
{
    public class ProdutosDao
    {
        public async Task<List<Produtos>> getListAsync()
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            List<Produtos> produtos = new List<Produtos>();
            var headers = httpClient.DefaultRequestHeaders;
            Uri requestUri = new Uri("http://julianoblanco-001-site3.ctempurl.com/WebService/ProductList");
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                List<Produtos> list = JsonConvert.DeserializeObject<List<Produtos>>(httpResponseBody);
                foreach (Produtos p in list)
                {
                    if (p.AR == true)
                        produtos.Add(p);
                }
                return produtos;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                return null;
            }
        }
    }
}
