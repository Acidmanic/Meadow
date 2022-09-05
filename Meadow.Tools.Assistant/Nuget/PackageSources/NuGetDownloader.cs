using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;

namespace Meadow.Tools.Assistant.Nuget.PackageSources
{
    public class NuGetDownloader : IPackageSource
    {
        private const string DownloadApiBase = "https://www.nuget.org/api/v2/package/";
        private const string NuspecApiBase = "https://api.nuget.org/v3-flatcontainer/";
        
        
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
            
            var downloadResult =   DownloadFile(url).Result;
            
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

            return DownloadApiBase + parameters;
        }


        private async Task<Result<byte[]>> DownloadFile(string url)
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        var bytes = await result.Content.ReadAsByteArrayAsync();

                        return new Result<byte[]>().Succeed(bytes);
                    }
                }
            }

            return new Result<byte[]>().FailAndDefaultValue();
        }
    }
}