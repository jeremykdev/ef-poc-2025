using DotNetEnv;
using EdiIngestConsole;
using System.Text;

namespace EdiIngestConsoleTests;

[TestFixture]
public class EdiParserTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        // parser depends on key in enviroment variable
        // set path to .env file containing license key
        Env.Load(@"..\..\..\.env");
    }

    [Test]
    public void Parse_Valid_Stream_Should_Return_Success_Is_True_With_EditItems()
    {
        // arrange
        EdiParser parser = new();
        using var stream = this.GetValid837Stream();

        // act
        var result = parser.ParseStream(stream);

        // assert        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.EdiItems, Is.Not.Empty);
        });
    }

    [Test]
    public void Parse_Empty_Stream_Should_Return_Empty_EditItems()
    {
        // arrange
        EdiParser parser = new();
        using var stream = this.GetEmptyStream();

        // act
        var result = parser.ParseStream(stream);

        // assert        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.EdiItems, Is.Empty);
        });
    }


    [Test]
    public void Parse_Malformed_Stream_Should_Return_Success_Is_False_With_Message()
    {
        // arrange
        EdiParser parser = new();
        using var stream = this.GetMalformed837Stream();

        // act
        var result = parser.ParseStream(stream);

        // assert        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.Not.Empty);
        });
    }


    /// <summary>
    /// Get an empty stream
    /// </summary>
    private MemoryStream GetEmptyStream() => new([]);
     

    /// <summary>
    /// Get stream containing a valid 837
    /// </summary>
    private MemoryStream GetValid837Stream()
    {
        string content = """"
ISA*00*          *00*          *ZZ*1234567        *ZZ*11111          *170508*1141*^*00501*000000101*1*P*:~
GS*HC*XXXXXXX*XXXXX*20170617*1741*101*X*005010X222A1~
ST*837*1239*005010X222A1~
BHT*0019*00*010*20170617*1741*CH~
NM1*41*2*SUBMITTER*****46*ABC123~
PER*IC*BOB SMITH*TE*4805551212~
NM1*40*2*RECEIVER*****46*44556~
HL*1**20*1~
NM1*85*2*BILLING PROVIDER*****XX*1122334455~
N3*1234 SOME ROAD~
N4*CHICAGO*IL*606739999~
REF*EI*999999999~
HL*2*1*22*0~
SBR*P*18*******12~
NM1*IL*1*BLOGGS*JOE****MI*1234567890~
N3*1 SOME BLVD~
N4*CHICAGO*IL*606129998~
DMG*D8*19570111*M~
NM1*PR*2*PAYER*****PI*12345~
N3*1 PAYER WAY~
N4*ST LOUIS*MO*212441850~
REF*2U*W1014~
CLM*1000A*140***19:B:1*Y*A*Y*Y~
HI*ABK:I10~
LX*1~
SV1*HC:99213*140*UN*1***1~
DTP*472*D8*20151124~
HL*3*1*22*0~
SBR*P*18*******12~
NM1*IL*1*BLOGGS*FRED****MI*9876543201~
N3*1 ANOTHER STR~
N4*CHICAGO*IL*606129998~
DMG*D8*19700601*M~
NM1*PR*2*PAYER*****PI*12345~
N3*1 PAYER WAY~
N4*ST LOUIS*MO*212441850~
REF*2U*W1014~
CLM*1001A*140***19:B:1*Y*A*Y*Y~
HI*ABK:I10~
LX*1~
SV1*HC:99213*140*UN*1***1~
DTP*472*D8*20151124~
SE*41*1239~
GE*1*101~
IEA*1*000000101~
"""";

        var bytes = Encoding.UTF8.GetBytes(content);
        return new(bytes);
    }



    private MemoryStream GetMalformed837Stream()
    {
        string content = """
MALFOMRED*****************ISA*00*          *00*          *ZZ*1234567        *ZZ*11111          *170508*1141*^*00501*000000101*1*P*:~
GS*HC*XXXXXXX*XXXXX*20170617*1741*101*X*005010X222A1~
ST*837*1239*005010X222A1~
BHT*0019*00*010*20170617*1741*CH~
NM1*41*2*SUBMITTER*****46*ABC123~
PER*IC*BOB SMITH*TE*4805551212~
NM1*40*2*RECEIVER*****46*44556~
HL*1**20*1~
NM1*85*2*BILLING PROVIDER*****XX*1122334455~
N3*1234 SOME ROAD~
N4*CHICAGO*IL*606739999~
REF*EI*999999999~
HL*2*1*22*0~
SBR*P*18*******12~
NM1*IL*1*BLOGGS*JOE****MI*1234567890~
N3*1 SOME BLVD~
N4*CHICAGO*IL*606129998~
DMG*D8*19570111*M~
NM1*PR*2*PAYER*****PI*12345~
N3*1 PAYER WAY~
N4*ST LOUIS*MO*212441850~
REF*2U*W1014~
CLM*1000A*140***19:B:1*Y*A*Y*Y~
HI*ABK:I10~
LX*1~
SV1*HC:99213*140*UN*1***1~
DTP*472*D8*20151124~
HL*3*1*22*0~
SBR*P*18*******12~
NM1*IL*1*BLOGGS*FRED****MI*9876543201~
N3*1 ANOTHER STR~
N4*CHICAGO*IL*606129998~
DMG*D8*19700601*M~
NM1*PR*2*PAYER*****PI*12345~
N3*1 PAYER WAY~
N4*ST LOUIS*MO*212441850~
REF*2U*W1014~
CLM*1001A*140***19:B:1*Y*A*Y*Y~
HI*ABK:I10~
LX*1~
SV1*HC:99213*140*UN*1***1~
DTP*472*D8*20151124~
SE*41*1239~
GE*1*101~
IEA*1*000000101~
******MALFORMED**************************************
""";

        var bytes = Encoding.UTF8.GetBytes(content);
        return new(bytes);
    }

}
