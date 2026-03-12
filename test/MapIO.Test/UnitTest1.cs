using AutoMapper;
using MapIO.TSK;
using Newtonsoft.Json;

namespace MapIO.Test;

[TestClass]
public class UnitTest1
{
    static TskData ReadTsk(string path)
    {
        using var fStream = new FileStream(path, FileMode.Open);
        MemoryStream mStream = new();
        fStream.CopyTo(mStream);
        return new TskData(mStream);
    }

    [TestMethod]
    public void TestTskHeadInfo()
    {
        //var path = @"D:\Desktop\Wafer\NKE178-23-B1.txt";
        using var tskData = ReadTsk(@"D:\Desktop\Wafer\NLK097-10-H2");
        var accessor = tskData.Accessor;
        var headInfo = tskData.HeaderInformationSection;

        Console.WriteLine(JsonConvert.SerializeObject(headInfo));

        var config = new MapperConfiguration(cfg => { });
        var mapper = config.CreateMapper();
        mapper.Map(headInfo, headInfo);
    }

    [TestMethod]
    public void TestTskTypeCasting()
    {
        using var tskData = ReadTsk(@"D:\Desktop\Wafer\NLK097-10-H2");
        tskData.Accessor.ColCount = 10;
        Console.WriteLine(tskData.Accessor.ColCount);
    }
}