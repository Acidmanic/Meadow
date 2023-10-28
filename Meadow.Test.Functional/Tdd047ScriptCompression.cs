using System;
using System.IO;
using System.IO.Compression;
using Acidmanic.Utilities.Extensions;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional;

public class Tdd047ScriptCompression:MeadowFunctionalTest
{
    public override void Main()
    {
        var testData = File.ReadAllText("TestCaseData/0000-post-table-crud.sql");

        var compressed = testData.CompressAsync(Compressions.GZip,CompressionLevel.Optimal).Result;

        Console.WriteLine($"{testData.Length} bytes has been compressed to {compressed.Length} " +
                          $"(be come  %{Math.Round(compressed.Length*100.0/testData.Length)} of " +
                          $"original size)");
    }
}