using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SurgicalInstrumentsApi.Db;
using SurgicalInstrumentsApi.Db.Exceptions;
using SurgicalInstrumentsApi.Dto;
using SurgicalInstrumentsApi.Web.Controllers;

namespace SurgicalInstrumentsApi.Web.Test
{
    public class TestInstrumentController
    {
        [Test]
        public void Get_NonExistnentInstrument_ReturnsNotFound()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.GetById(It.IsAny<int>()))
                      .Returns((Instrument)null);
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Get(0);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Get_ExistnentInstrument_ReturnsOk()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.GetById(It.IsAny<int>()))
                      .Returns(new Instrument { InstrumentID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Get(0);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void Post_DatabaseError_ReturnsStatusCode500()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.CreateInstrument(It.IsAny<InstrumentCreate>()))
                      .Throws<DatabaseOperationException>();
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Post(new InstrumentCreate { SubcategoryID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(((StatusCodeResult)result).StatusCode, 500);
        }

        [Test]
        public void Post_CreateSuccessful_ReturnsCreated()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.CreateInstrument(It.IsAny<InstrumentCreate>()))
                      .Returns(new Instrument { InstrumentID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Post(new InstrumentCreate { SubcategoryID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            Assert.IsInstanceOf<CreatedResult>(result);
        }

        [Test]
        public void Put_DatabaseError_ReturnsStatusCode500()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.UpdateInstrument(It.IsAny<InstrumentUpdate>()))
                      .Throws<DatabaseOperationException>();
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Put(new InstrumentUpdate { InstrumentID=0, SubcategoryID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(((StatusCodeResult)result).StatusCode, 500);
        }

        [Test]
        public void Put_UpdateSuccessful_ReturnsOk()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.UpdateInstrument(It.IsAny<InstrumentUpdate>()))
                      .Returns(new Instrument { InstrumentID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Put(new InstrumentUpdate { InstrumentID = 0, SubcategoryID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void Put_NonExistantInstrument_ReturnsNotFound()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.UpdateInstrument(It.IsAny<InstrumentUpdate>()))
                      .Throws<InstrumentNotFoundException>();
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Put(new InstrumentUpdate { InstrumentID = 0, SubcategoryID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void Put_InvalidSubcategory_ReturnsBadRequest()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.UpdateInstrument(It.IsAny<InstrumentUpdate>()))
                      .Throws<SubcategoryNotFoundException>();
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Put(new InstrumentUpdate { InstrumentID = 0, SubcategoryID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void Delete_DatabaseError_ReturnsStatusCode500()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.DeleteInstrument(It.IsAny<int>()))
                      .Throws<DatabaseOperationException>();
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Delete(0);
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(((StatusCodeResult)result).StatusCode, 500);
        }

        [Test]
        public void Delete_DeleteSuccessful_ReturnsOk()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.DeleteInstrument(It.IsAny<int>()))
                      .Returns(new Instrument { InstrumentID = 0, Name = "Test", ImageUrl = null, Sku = "TestSku" });
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Delete(0);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void Delete_NonExistantInstrument_ReturnsNotFound()
        {
            var logger = Mock.Of<ILogger<InstrumentController>>();
            var repository = new Mock<IInstrumentRepository>(MockBehavior.Strict);
            repository.Setup(item => item.DeleteInstrument(It.IsAny<int>()))
                      .Throws<InstrumentNotFoundException>();
            var controller = new InstrumentController(logger, repository.Object);
            var result = controller.Delete(0);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}