using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using SurgicalInstrumentsApi.Db.Exceptions;
using SurgicalInstrumentsApi.Db.Sql.Model;

namespace SurgicalInstrumentsApi.Db.Sql.Test
{
    public class TestSurgicalInstrumentSqlRepository
    {
        private Mock<DbSet<Instrument>> _dbSetMock;
        private Mock<DbSet<Subcategory>> _dbSetSubcategoryMock;
        private Mock<SurgicalInstrumentsDbContext> _dbContextMock;
        private ILogger<SurgicalInstrumentSqlRepository> _loggerMock;

        [SetUp]
        public void Init()
        {
            var _loggerMock = Mock.Of<ILogger<SurgicalInstrumentSqlRepository>>();
            var instrumentList = new List<Instrument>() {
                new Instrument { InstrumentID=1, Name="Instrument1", ImageUrl = null, Sku="TestSku1", Subcategory = new Subcategory { SubcategoryID= 2} },
                new Instrument { InstrumentID=2, Name="Instrument2", ImageUrl = null, Sku="TestSku2", Subcategory = new Subcategory { SubcategoryID= 1} }, };
            _dbSetMock = new Mock<DbSet<Instrument>>();
            _dbSetMock.As<IQueryable<Instrument>>().Setup(x => x.Provider).Returns(instrumentList.AsQueryable().Provider);
            _dbSetMock.As<IQueryable<Instrument>>().Setup(x => x.Expression).Returns(instrumentList.AsQueryable().Expression);
            _dbSetMock.As<IQueryable<Instrument>>().Setup(x => x.ElementType).Returns(instrumentList.AsQueryable().ElementType);
            _dbSetMock.As<IQueryable<Instrument>>().Setup(x => x.GetEnumerator()).Returns(instrumentList.AsQueryable().GetEnumerator());

            var subcategoryList = new List<Subcategory>() {
                new Subcategory { SubcategoryID= 1, Name="Subcategory1", ImageUrl = null, Category = new Category{CategoryID=1}},
                new Subcategory { SubcategoryID= 2, Name="Subcategory2", ImageUrl = null, Category = new Category{CategoryID=1}}
            };

            _dbSetSubcategoryMock = new Mock<DbSet<Subcategory>>();
            _dbSetSubcategoryMock.As<IQueryable<Subcategory>>().Setup(x => x.Provider).Returns(subcategoryList.AsQueryable().Provider);
            _dbSetSubcategoryMock.As<IQueryable<Subcategory>>().Setup(x => x.Expression).Returns(subcategoryList.AsQueryable().Expression);
            _dbSetSubcategoryMock.As<IQueryable<Subcategory>>().Setup(x => x.ElementType).Returns(subcategoryList.AsQueryable().ElementType);
            _dbSetSubcategoryMock.As<IQueryable<Subcategory>>().Setup(x => x.GetEnumerator()).Returns(subcategoryList.AsQueryable().GetEnumerator());

            _dbContextMock = new Mock<SurgicalInstrumentsDbContext>();
            _dbContextMock.Setup(item => item.Instruments).Returns(_dbSetMock.Object);
            _dbContextMock.Setup(item => item.Subcategories).Returns(_dbSetSubcategoryMock.Object);
        }

        [Test]
        public void GetById_InstrumentFound_ReturnsInstrument()
        {
            var repository = new SurgicalInstrumentSqlRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.GetById(2);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Dto.Instrument>(result);
            Assert.AreEqual(2, result.InstrumentID);
        }

        [Test]
        public void GetById_NonExistantInstrument_ReturnsNull()
        {
            var repository = new SurgicalInstrumentSqlRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.GetById(3);
            Assert.IsNull(result);
        }

        [Test]
        public void FindByName_QueryShorterThan3Chars_ReturnsEmpty()
        {
            var repository = new SurgicalInstrumentSqlRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.FindByName("I");
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void FindByName_QueryLongerThan3Chars_ReturnsResults()
        {
            var repository = new SurgicalInstrumentSqlRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.FindByName("Ins");
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void CreateInstrument_NonExistantSubcategory_ThrowsException()
        {
            var repository = new SurgicalInstrumentSqlRepository(_loggerMock, _dbContextMock.Object);
            Assert.Throws<SubcategoryNotFoundException>(() => repository.CreateInstrument(new Dto.InstrumentCreate { Name = "Instrument3", ImageUrl = "", Sku = "Sku3", SubcategoryID = 3 }));
        }

        [Test]
        public void CreateInstrument_ValidSubcategory_ReturnsCreatedInstrument()
        {
            var repository = new SurgicalInstrumentSqlRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.CreateInstrument(new Dto.InstrumentCreate { Name = "Instrument3", ImageUrl = "", Sku = "Sku3", SubcategoryID = 2 });
            Assert.IsNotNull(result);
            Assert.AreEqual("Instrument3", result.Name);
        }
    }
}