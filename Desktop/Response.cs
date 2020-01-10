using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
//using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace Desktop
{
    static class Response<T>
    {
        public async static Task<T> GetResponse(HttpResponseMessage response)
        {
            if (response.Content == null)
                return default(T);
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (StreamReader reader = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }
    }
}
