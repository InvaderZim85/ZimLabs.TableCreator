using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ZimLabs.TableCreator;

namespace Demo;

[SimpleJob(RuntimeMoniker.Net70, baseline: true)]
[SimpleJob(RuntimeMoniker.Net80)]
[RPlotExporter]
public class BenchmarkTest
{
    private readonly string _testFile = Path.Combine(AppContext.BaseDirectory, "TestFile.txt");

    [Benchmark]
    #pragma warning disable CA1822 // Mark members as static
    public void CreateTableTest()
    {
        var dummyData = Helper.CreatePersonList();

        _ = dummyData.CreateTable();
    }
    #pragma warning restore CA1822 // Mark members as static

    [Benchmark]
    public void SaveTable()
    {
        var dummyData = Helper.CreatePersonList();

        dummyData.SaveTable(_testFile);
    }

    [Benchmark]
    public Task SaveTableTestAsync()
    {
        var dummyData = Helper.CreatePersonList();

        return dummyData.SaveTableAsync(_testFile);
    }
}