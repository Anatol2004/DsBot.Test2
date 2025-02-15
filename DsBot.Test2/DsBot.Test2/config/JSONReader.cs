using System.IO;
using System.Threading.Tasks;

namespace DsBot.Test2.config
{
    public class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }

        public async Task ReadJson()
        {
            using (StreamReader sr = new StreamReader("Config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure jSONStructure = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONStructure>(json);

                token = jSONStructure.token;
                prefix = jSONStructure.prefix;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }
    }
}
