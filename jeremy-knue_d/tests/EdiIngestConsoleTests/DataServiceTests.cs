using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiIngestConsole;
using Moq;

namespace EdiIngestConsoleTests;

[TestFixture]
public class DataServiceTests
{
    private Mock<EdiDbContext> _dbContextMock;

    [OneTimeSetUp]
    public void Setup()
    {
        this._dbContextMock = new Mock<EdiDbContext>();
    }


    [Test]
    public async Task SaveItems_With_Input_Should_Call_SaveChangesAsync()
    {
        // arrange
        List<IEdiItem> items = [];

        GS gs = new()
        {
            ReceiverIDCode_3 = "TEST",
            Date_4 = "TEST",
            GroupControlNumber_6 = "TEST",
            SenderIDCode_2 = "TEST",
            CodeIdentifyingInformationType_1 = "TEST"
        };

        items.Add(gs);

        ParseResult parseResult = new()
        {
            Success = true,
            Message = String.Empty,
            EdiItems = items,
            SourceFileName = "Test.txt",
            SourceHash = String.Empty // empty hash bypasses duplicate check 
        };

        DataService dataService = new(this._dbContextMock.Object);

        // act
        var result = await dataService.SaveItems(parseResult);

        // assert
        this._dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
     
}