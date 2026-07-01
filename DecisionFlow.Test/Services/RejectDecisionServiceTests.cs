using DecisionFlow.Application.Interfaces;
using DecisionFlow.Application.Services;
using DecisionFlow.Domain.Entities;
using Moq;

namespace DecisionFlow.UnitTest
{
    public class RejectDecisionServiceTests
    {
        private readonly Mock<IDecisionRepository<Decision>> _repositoryMock;
        private readonly RejectDecisionService _service;

        public RejectDecisionServiceTests()
        {
            _repositoryMock = new Mock<IDecisionRepository<Decision>>();
            _service = new RejectDecisionService(_repositoryMock.Object);
        }

        [Fact]
        public void Constructor_ValidRepository_CreatesInstanceSuccessfully()
        {
            // Arrange
            var repositoryMock = new Mock<IDecisionRepository<Decision>>();

            // Act
            var exception = Record.Exception(() => new RejectDecisionService(repositoryMock.Object));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Handle_PendingDecision_RejectsDecisionAndSavesChanges()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            await _service.Handle(decisionId, "alice");

            // Assert
            Assert.Equal(DecisionStatus.Rejected, decision.Status);
            var approval = Assert.Single(decision.Approvals);
            Assert.Equal("alice", approval.ApprovedBy);
            Assert.False(approval.IsApproved);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_PendingDecision_CallsGetByIdAsyncWithProvidedDecisionId()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            await _service.Handle(decisionId, "alice");

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(decisionId), Times.Once);
        }

        [Fact]
        public async Task Handle_AlreadyApprovedDecision_ThrowsInvalidOperationExceptionAndDoesNotSaveChanges()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            decision.Approve("bob");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "alice"));

            // Assert
            Assert.Equal("Cannot reject an already approved decision.", exception.Message);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_AlreadyRejectedDecision_ThrowsInvalidOperationExceptionAndDoesNotSaveChanges()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            decision.Reject("bob");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "alice"));

            // Assert
            Assert.Equal("Cannot reject an already rejected decision.", exception.Message);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryReturnsNullDecision_ThrowsNullReferenceException()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync((Decision)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.Handle(decisionId, "alice"));
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_GetByIdAsyncThrows_PropagatesExceptionAndDoesNotSaveChanges()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ThrowsAsync(new InvalidOperationException("DB failure"));

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "alice"));

            // Assert
            Assert.Equal("DB failure", exception.Message);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_SaveChangesAsyncThrows_RejectsDecisionButPropagatesException()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("Save failure"));

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "alice"));

            // Assert
            Assert.Equal("Save failure", exception.Message);
            Assert.Equal(DecisionStatus.Rejected, decision.Status);
        }

        [Theory]
        [InlineData("alice")]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Handle_VariousUserValues_RecordsApprovalWithProvidedUser(string user)
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            await _service.Handle(decisionId, user);

            // Assert
            var approval = Assert.Single(decision.Approvals);
            Assert.Equal(user, approval.ApprovedBy);
        }

        [Fact]
        public async Task Handle_NullUser_RecordsApprovalWithNullApprovedBy()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            await _service.Handle(decisionId, null!);

            // Assert
            var approval = Assert.Single(decision.Approvals);
            Assert.Null(approval.ApprovedBy);
        }
    }
}
