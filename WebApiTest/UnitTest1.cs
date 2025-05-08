using ExcelFileToDB.Controllers;
using ExcelFileToDB.Services.DatabaseSaverServices;
using ExcelFileToDB.Services.ExcelReaderServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExcelFileToDB.Tests
{
    public class ProcessExcelControllerTests
    {
        private readonly Mock<IExcelReaderService> _mockExcelReader;
        private readonly Mock<IDatabaseSaverService> _mockDbSaver;
        private readonly ProcessExcelController _controller;

        public ProcessExcelControllerTests()
        {
            // Create mocks
            _mockExcelReader = new Mock<IExcelReaderService>();
            _mockDbSaver = new Mock<IDatabaseSaverService>();

            // Initialize the controller with mocked services
            _controller = new ProcessExcelController(
                new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build(),
                _mockExcelReader.Object,
                _mockDbSaver.Object);
        }

        [Fact]
        public async Task UploadExcel_ReturnsOkResult_WhenExcelIsProcessedSuccessfully()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var sheetName = "Sheet1";

            // Mock return data from ReadExcelAsync (List<Dictionary<string, object>>)
            var mockRows = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>
                {
                    { "Column1", "Value1" },
                    { "Column2", "Value2" }
                }
            };

            _mockExcelReader.Setup(x => x.ReadExcelAsync(mockFile.Object, sheetName))
                            .ReturnsAsync(mockRows);
            _mockDbSaver.Setup(x => x.SaveDataAsync(mockRows, "Customers"))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UploadExcel(mockFile.Object, sheetName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockRows, okResult.Value);
        }

        [Fact]
        public async Task UploadExcel_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var sheetName = "InvalidSheet";
            _mockExcelReader.Setup(x => x.ReadExcelAsync(mockFile.Object, sheetName))
                            .ThrowsAsync(new ArgumentException("Invalid sheet"));

            // Act
            var result = await _controller.UploadExcel(mockFile.Object, sheetName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid sheet", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadExcel_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var sheetName = "Sheet1";
            _mockExcelReader.Setup(x => x.ReadExcelAsync(mockFile.Object, sheetName))
                            .ThrowsAsync(new System.Exception("Internal error"));

            // Act
            var result = await _controller.UploadExcel(mockFile.Object, sheetName);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal error: Internal error", statusCodeResult.Value);
        }
    }
}
