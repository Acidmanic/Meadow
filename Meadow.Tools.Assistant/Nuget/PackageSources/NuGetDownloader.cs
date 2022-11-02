using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Tools.Assistant.Nuget.PackageSources
{
    public class NuGetDownloader : IPackageSource
    {
        //private const string DownloadApiBase = "https://www.nuget.org/api/v2/package/";
        private const string DownloadApiBase = "https://globalcdn.nuget.org/packages/";
        private const string NuspecApiBase = "https://api.nuget.org/v3-flatcontainer/";


        public string Proxy { get; set; } = null;

        public NuGetDownloader FuckSsl()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };

            Logger.LogWarning("SSL issues are being ignored. ,,|,,");

            return this;
        }

        public ILogger Logger { get; set; } = new LoggerAdapter(s => { });

        public Result<byte[]> ProvidePackage(PackageId packageId)
        {
            var link = GetDownloadLink(packageId.Id, packageId.Version);

            return DownloadFile(link).Result;
        }

        public string GetNuspec(PackageId packageId)
        {
            var loweredId = packageId.Id.ToLower();
            var loweredVersion = packageId.Version.ToLower();

            var url = NuspecApiBase + $"{loweredId}/{loweredVersion}/{loweredId}.nuspec";

            var downloadResult = DownloadFile(url).Result;

            if (downloadResult.Success)
            {
                var bytes = downloadResult.Value;

                var xmlString = Encoding.ASCII.GetString(bytes);

                return xmlString;
            }

            return null;
        }

        private string GetDownloadLink(string packageName, string packageVersion)
        {
            var parameters = packageName;

            if (!string.IsNullOrEmpty(packageVersion))
            {
                parameters += "/" + packageVersion;
            }

            return DownloadApiBase + packageName.ToLower() + "." + packageVersion.ToLower() + ".nupkg";
        }


        private async Task<Result<byte[]>> DownloadFile(string url)
        {
           

            // using (var client = new HttpClient())
            // {
            //     using (var result = await client.GetAsync(url))
            //     {
            //         if (result.IsSuccessStatusCode)
            //         {
            //             var bytes = await result.Content.ReadAsByteArrayAsync();
            //
            //             Logger.LogInformation("{Url} Has been downloaded Successfully.", url);
            //
            //             return new Result<byte[]>().Succeed(bytes);
            //         }
            //         else
            //         {
            //             Logger.LogError("Unable to download {Url}, Status:{Status} - {Code}"
            //                 , url, result.ReasonPhrase, result.StatusCode);
            //         }
            //     }
            // }
            using (var net = new WebClient())
            {
                
                if (Proxy != null)
                {
                    Logger.LogDebug("Using Proxy: {Proxy}", Proxy);

                    net.Proxy = new WebProxy(Proxy);
                }
                
                net.Headers.Add("User-Agent",
                    " Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:105.0) Gecko/20100101 Firefox/105.0");
                
                try
                {
                    Logger.LogDebug("Downloading {Url} ...", url);

                    var data = await net.DownloadDataTaskAsync(url);

                    Logger.LogInformation("{Url} Has been downloaded Successfully.", url);

                    return new Result<byte[]>().Succeed(data);
                }
                catch (Exception e)
                {
                    Logger.LogError("Unable to download {Url}, Exception: {Exception}", url, e);
                }
            }
            return new Result<byte[]>().FailAndDefaultValue();
        }
    }
}