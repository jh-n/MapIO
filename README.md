# MapIO
Read and write wafer map data

## How to use

### MapIO.TSK

Made for convenient reading from or writing to TSK file via building ORM to binary data.

```c#
// Read TSK Map File
var mStream = new MemoryStream();
using (var fStream = new FileStream("filepath", FileMode.Open))
{
    fStream.CopyTo(mStream);
}
using var data = new TskData(mStream); // MemoryStream disposed when TskData is disposed
var accessor = data.Accessor; // Quickly get common parameters
var headInfo = data.HeaderInformationSection;
```

### MapIO.SINF

```c#
var data = new SinfData();
using (var reader = new StreamReader("filepath"))
{
    sinfData = new SinfData(reader);
}
var accessor = sinfData.Accessor;
```