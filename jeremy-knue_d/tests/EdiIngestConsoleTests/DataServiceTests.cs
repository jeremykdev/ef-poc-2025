using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiIngestConsole;
using Moq;


namespace EdiIngest.Tests;

[TestFixture]
public class DataServiceTests
{
    private Mock<EdiDbContext> _dbContext;

    [SetUp]
    public void Setup()
    {
        this._dbContext = new Mock<EdiDbContext>();
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

        DataService dataService = new(this._dbContext.Object);

        // act
        var result = await dataService.SaveItems(items);

        // assert
        this._dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SaveItems_With_Null_Input_Should_Not_Call_SaveChangesAsync()
    {
        // arrange
        List<IEdiItem>? items = null;       

        DataService dataService = new(this._dbContext.Object);

        // act
        var result = await dataService.SaveItems(items);

        // assert
        this._dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }


    [Test]
    public async Task SaveItems_With_Empy_Input_Should_Not_Call_SaveChangesAsync()
    {
        // arrange
        List<IEdiItem> items = [];

        DataService dataService = new(this._dbContext.Object);

        // act
        var result = await dataService.SaveItems(items);

        // assert
        this._dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

