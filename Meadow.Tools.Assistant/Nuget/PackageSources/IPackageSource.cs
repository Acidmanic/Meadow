namespace Meadow.Tools.Assistant.Nuget.PackageSources
{
    public interface IPackageSource
    {

        Result<byte[]> ProvidePackage(PackageId packageId);

        string GetNuspec(PackageId packageId);
    }
}