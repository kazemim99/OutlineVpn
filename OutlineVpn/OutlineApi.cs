using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OutlineVpn;

public class OutlineApi
{
    private WebClient _webClient = new();
    public string ApiUrl;
    public OutlineApi(string apiUrl)
    {
        ApiUrl = apiUrl;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    }

    private bool CallRequest(string url, string method, NameValueCollection args, out string? content)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{ApiUrl}/{url}");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = method;
            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            return true;
        }
        catch
        {
            content = null;
        }
        
        return false;
    }

    public object? Capacity(string mobile)
    {
        object? capacity = null;
        double? bytes = 0;
        var data2 = GetKeys(); // Get all transferred data
        var user = data2.FirstOrDefault(a => a.Name.Contains(mobile));
        if (user != null)
            bytes = GetTransferredData().FirstOrDefault(a => a.Id == user.Id)?.UsedBytes;

        if (bytes != null || bytes > 0)
            capacity = string.Format("{0:N2}", (bytes / Math.Pow(1024, 3)));

        return capacity;
    }

    private bool CallRequest(string url, string method, JObject args, out string? content)
    {
        try
        {
            _webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

            content = _webClient.UploadString($"{ApiUrl}/{url}", method, args.ToString());
            return true;
        }
        catch
        {
            content = null;
        }
        
        return false;
    }
    
    public List<OutlineKey> GetKeys()
    {
        CallRequest("access-keys", "GET", new NameValueCollection(), out string? content);
        return (JObject.Parse(content)["accessKeys"] as JArray).ToObject<List<OutlineKey>>();
    }

    public OutlineKey CreateKey()
    {
        var cal = new NameValueCollection();
        CallRequest("access-keys", "POST", cal, out string? content);
        return (JObject.Parse(content)).ToObject<OutlineKey>();
    }

    public bool DeleteKey(int id)
        => CallRequest($"access-keys/{id}", "DELETE", new NameValueCollection(), out _);

    public bool RenameKey(int id, string name)
        => CallRequest($"access-keys/{id}/name", "PUT", new JObject
        {
            {"name", name}
        }, out _);

    public bool AddDataLimit(int id, long limitBytes)
        => CallRequest($"access-keys/{id}/data-limit", "PUT", new JObject
        {
            {
                "limit", new JObject
                {
                    {"bytes", limitBytes}
                }
            }
        }, out _);

    public bool DeleteDataLimit(int id)
        => CallRequest($"access-keys/{id}/data-limit", "DELETE", new NameValueCollection(), out _);

    public List<OutlineKey> GetTransferredData()
    {
        List<OutlineKey> outline = new List<OutlineKey>();
        CallRequest("metrics/transfer", "GET", new NameValueCollection(), out string content);
        var response = JObject.Parse(content)["bytesTransferredByUserId"] as JObject;
        foreach (var x in response)
        {
            OutlineKey outl = new OutlineKey()
            {
                Id = int.Parse(x.Key),
                UsedBytes = (long)x.Value
            };
            outline.Add(outl);
        }
        return outline;
    }

    public string GetAccessUrl(string mobile)
    {
        var key =GetKeys().FirstOrDefault(a => a.Name.Contains(mobile));
        if (key == null)
            throw new Exception("access key is null");

            return key.AccessUrl;

       
    }
}