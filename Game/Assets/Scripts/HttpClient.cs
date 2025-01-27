using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Scripts
{
    public static class MyHttpClient
    {

        public static async Task<T> Get<T>(string endpoint,bool auth = false,string token = "")
        {
            var getRequest = CreateRequest(endpoint);
            if (auth)
                AttachHeader(getRequest, "Authorization", token);
            getRequest.SendWebRequest();

            while (!getRequest.isDone) await Task.Delay(10);
            return JsonConvert.DeserializeObject<T>(getRequest.downloadHandler.text);
        }

        public static async Task<T> Post<T>(string endpoint,object payload,bool auth = false,string token = "")
        {
            var postRequest = CreateRequest(endpoint,RequestType.POST,payload);
            if (auth)
                AttachHeader(postRequest, "Authorization", token);
            postRequest.SendWebRequest();

            while(!postRequest.isDone) await Task.Delay(10);
            return JsonConvert.DeserializeObject<T>(postRequest.downloadHandler.text);
        }

        private static UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null)
        {
            var request = new UnityWebRequest(path, type.ToString());

            if (data != null)
            {
                var bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-type", "application/json");

            return request;
        }

        private static void AttachHeader(UnityWebRequest request,string key,string value)
        {
            request.SetRequestHeader(key,value);
        }

        public enum RequestType
        {
            GET = 0,
            POST = 1,
            PUT = 2
        }
    }

}