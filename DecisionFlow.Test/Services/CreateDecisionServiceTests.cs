using DecisionFlow.Application.Commands;
using DecisionFlow.Application.Interfaces;
using DecisionFlow.Application.Services;
using DecisionFlow.Domain.Entities;
using Moq;

namespace DecisionFlow.UnitTest.Services
{
    public class CreateDecisionServiceTests
    {
        [Fact]
        public void Constructor_ValidRepository_DoesNotThrow()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();

            // Act
            var exception = Record.Exception(() => new CreateDecisionService(mockRepository.Object));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Constructor_ValidRepository_StoresRepositoryUsedByHandle()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Decision>())).Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act
            await service.Handle(command);

            // Assert
            mockRepository.Verify(r => r.AddAsync(It.IsAny<Decision>()), Times.Once);
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsNonEmptyGuid()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Decision>())).Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Approve Budget", "Quarterly budget approval");

            // Act
            var result = await service.Handle(command);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsAddAsyncExactlyOnce()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Decision>())).Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act
            await service.Handle(command);

            // Assert
            mockRepository.Verify(r => r.AddAsync(It.IsAny<Decision>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsSaveChangesAsyncExactlyOnce()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Decision>())).Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act
            await service.Handle(command);

            // Assert
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_PassesDecisionWithMatchingTitleAndDescriptionToRepository()
        {
            // Arrange
            Decision? capturedDecision = null;
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback<Decision>(d => capturedDecision = d)
                .Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Server Migration", "Migrate prod servers to new datacenter");

            // Act
            await service.Handle(command);

            // Assert
            Assert.NotNull(capturedDecision);
            Assert.Equal(command.Title, capturedDecision!.Title);
            Assert.Equal(command.Description, capturedDecision.Description);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesDecisionWithPendingStatus()
        {
            // Arrange
            Decision? capturedDecision = null;
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback<Decision>(d => capturedDecision = d)
                .Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act
            await service.Handle(command);

            // Assert
            Assert.NotNull(capturedDecision);
            Assert.Equal(DecisionStatus.Pending, capturedDecision!.Status);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesDecisionWithEmptyApprovalsList()
        {
            // Arrange
            Decision? capturedDecision = null;
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback<Decision>(d => capturedDecision = d)
                .Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act
            await service.Handle(command);

            // Assert
            Assert.NotNull(capturedDecision);
            Assert.Empty(capturedDecision!.Approvals);
        }

        [Fact]
        public async Task Handle_ValidCommand_SetsCreatedAtCloseToUtcNow()
        {
            // Arrange
            Decision? capturedDecision = null;
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback<Decision>(d => capturedDecision = d)
                .Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");
            var beforeCall = DateTime.UtcNow.AddSeconds(-5);

            // Act
            await service.Handle(command);
            var afterCall = DateTime.UtcNow.AddSeconds(5);

            // Assert
            Assert.NotNull(capturedDecision);
            Assert.InRange(capturedDecision!.CreatedAt, beforeCall, afterCall);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsIdMatchingPersistedDecision()
        {
            // Arrange
            Decision? capturedDecision = null;
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback<Decision>(d => capturedDecision = d)
                .Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act
            var result = await service.Handle(command);

            // Assert
            Assert.NotNull(capturedDecision);
            Assert.Equal(capturedDecision!.Id, result);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsAddAsyncBeforeSaveChangesAsync()
        {
            // Arrange
            var callOrder = new List<string>();
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback(() => callOrder.Add("AddAsync"))
                .Returns(Task.CompletedTask);
            mockRepository
                .Setup(r => r.SaveChangesAsync())
                .Callback(() => callOrder.Add("SaveChangesAsync"))
                .Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act
            await service.Handle(command);

            // Assert
            Assert.Equal(new[] { "AddAsync", "SaveChangesAsync" }, callOrder);
        }

        [Fact]
        public async Task Handle_NullCommand_ThrowsNullReferenceException()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            var service = new CreateDecisionService(mockRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.Handle(null!));
        }

        [Fact]
        public async Task Handle_AddAsyncThrows_PropagatesExceptionAndDoesNotCallSaveChanges()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .ThrowsAsync(new InvalidOperationException("Database unavailable"));
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Handle(command));
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_SaveChangesAsyncThrows_PropagatesException()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Decision>())).Returns(Task.CompletedTask);
            mockRepository
                .Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new InvalidOperationException("Commit failed"));
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand("Title", "Description");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Handle(command));
            mockRepository.Verify(r => r.AddAsync(It.IsAny<Decision>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyTitleAndDescription_CreatesDecisionWithEmptyStrings()
        {
            // Arrange
            Decision? capturedDecision = null;
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback<Decision>(d => capturedDecision = d)
                .Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand(string.Empty, string.Empty);

            // Act
            await service.Handle(command);

            // Assert
            Assert.NotNull(capturedDecision);
            Assert.Equal(string.Empty, capturedDecision!.Title);
            Assert.Equal(string.Empty, capturedDecision.Description);
        }

        [Fact]
        public async Task Handle_NullTitleAndDescriptionInCommand_PassesNullValuesToDecision()
        {
            // Arrange
            Decision? capturedDecision = null;
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Decision>()))
                .Callback<Decision>(d => capturedDecision = d)
                .Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var command = new CreateDecisionCommand(null!, null!);

            // Act
            await service.Handle(command);

            // Assert
            Assert.NotNull(capturedDecision);
            Assert.Null(capturedDecision!.Title);
            Assert.Null(capturedDecision.Description);
        }

        [Fact]
        public async Task Handle_CalledTwiceWithDifferentCommands_ReturnsDistinctGuidsAndCallsRepositoryTwice()
        {
            // Arrange
            var mockRepository = new Mock<IDecisionRepository<Decision>>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Decision>())).Returns(Task.CompletedTask);
            mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new CreateDecisionService(mockRepository.Object);
            var firstCommand = new CreateDecisionCommand("First", "First description");
            var secondCommand = new CreateDecisionCommand("Second", "Second description");

            // Act
            var firstResult = await service.Handle(firstCommand);
            var secondResult = await service.Handle(secondCommand);

            // Assert
            Assert.NotEqual(firstResult, secondResult);
            mockRepository.Verify(r => r.AddAsync(It.IsAny<Decision>()), Times.Exactly(2));
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
        }
    }
}
