using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ZimLabs.TableCreator;
using ZimLabs.TableCreator.DataObjects;

namespace Demo;

[SimpleJob(RuntimeMoniker.Net90)]
[RPlotExporter]
public class BenchmarkTest
{
    private readonly string _testFile = Path.Combine(AppContext.BaseDirectory, "TestFile.txt");

    [Benchmark]
    #pragma warning disable CA1822 // Mark members as static
    public void CreateTableTest()
    {
        var dummyData = Helper.CreatePersonList();

        _ = dummyData.CreateTable(new TableCreatorOptions());
    }
    #pragma warning restore CA1822 // Mark members as static

    [Benchmark]
    public void SaveTable()
    {
        var dummyData = Helper.CreatePersonList();

        dummyData.SaveTable(_testFile, new TableCreatorOptions());
    }

    [Benchmark]
    public Task SaveTableTestAsync()
    {
        var dummyData = Helper.CreatePersonList();

        return dummyData.SaveTableAsync(_testFile, new TableCreatorOptions());
    }
}