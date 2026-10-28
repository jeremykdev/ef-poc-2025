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


    [Test]
    public void ShouldSaveEdiItems_Should_Return_False_When_isDuplicateFile_Is_True()
    {
        // arrange
        bool isDuplicate = true;
        ParseResult parseResult = new();

        // act
        bool result = DataService.ShouldSaveEdiItems(parseResult, isDuplicate);

        // assert
        Assert.That(result, Is.False);
    }


    [Test]
    public void ShouldSaveEdiItems_Should_Return_False_When_ParseResult_Success_IsFalse()
    {
        // arrange
        bool isDuplicate = false;
        var parseResult = CreateValidParseResult();
        parseResult.Success = false;

        // act
        bool result = DataService.ShouldSaveEdiItems(parseResult, isDuplicate);

        // assert
        Assert.That(result, Is.False);
    }


    [Test]
    public void ShouldSaveEdiItems_Should_Return_False_When_ParseResult_EdiItems_Is_Null()
    {
        // arrange
        bool isDuplicate = false;
        var parseResult = CreateValidParseResult();
        parseResult.EdiItems = null;

        // act
        bool result = DataService.ShouldSaveEdiItems(parseResult, isDuplicate);

        // assert
        Assert.That(result, Is.False);
    }


    [Test]
    public void ShouldSaveEdiItems_Should_Return_False_When_ParseResult_EdiItems_Is_Empty()
    {
        // arrange
        bool isDuplicate = false;
        var parseResult = CreateValidParseResult();
        parseResult.EdiItems = [];

        // act
        bool result = DataService.ShouldSaveEdiItems(parseResult, isDuplicate);

        // assert
        Assert.That(result, Is.False);
    }



    [Test]
    public void ShouldSaveEdiItems_Should_Return_False_When_ParseResult_ValidationResults_Contains_Errors()
    {
        // arrange
        bool isDuplicate = false;

        var validationResult = new EdiIngestConsole.Validations.ValidationResult
        {
            ErrorMessages = ["Error 1", "Error 2"]
        };

        var parseResult = CreateValidParseResult();
        parseResult.ValidationResults.Add(validationResult);

        // act
        bool result = DataService.ShouldSaveEdiItems(parseResult, isDuplicate);

        // assert
        Assert.That(result, Is.False);
    }


    [Test]
    public void ShouldSaveEdiItems_Should_Return_True_When_All_Criteria_Met()
    {
        // arrange
        bool isDuplicate = false;
        var parseResult = CreateValidParseResult();
        

        // act
        bool result = DataService.ShouldSaveEdiItems(parseResult, isDuplicate);

        // assert
        Assert.That(result, Is.True);
    }


    protected static ParseResult CreateValidParseResult()
    {
        var validationResult = new EdiIngestConsole.Validations.ValidationResult();

        ParseResult parseResult = new()
        {
            Success = true,
            EdiItems = [new ISA(), new GS()],
            ValidationResults = [validationResult]
        };

        return parseResult;
    }
}