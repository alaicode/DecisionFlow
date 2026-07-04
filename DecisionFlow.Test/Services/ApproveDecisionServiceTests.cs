using DecisionFlow.Application.Interfaces;
using DecisionFlow.Application.Services;
using DecisionFlow.Domain.Entities;
using Moq;

namespace DecisionFlow.UnitTest
{
    public class ApproveDecisionServiceTests
    {
        private readonly Mock<IDecisionRepository<Decision>> _repositoryMock;
        private readonly ApproveDecisionService _service;

        public ApproveDecisionServiceTests()
        {
            _repositoryMock = new Mock<IDecisionRepository<Decision>>();
            _service = new ApproveDecisionService(_repositoryMock.Object);
        }

        [Fact]
        public void Constructor_ValidRepository_CreatesInstance()
        {
            // Arrange
            var repositoryMock = new Mock<IDecisionRepository<Decision>>();

            // Act
            var service = new ApproveDecisionService(repositoryMock.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_NullRepository_DoesNotThrow()
        {
            // Arrange & Act
            var exception = Record.Exception(() => new ApproveDecisionService(null!));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Handle_PendingDecision_SetsStatusToApproved()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.Handle(decisionId, "john.doe");

            // Assert
            Assert.Equal(DecisionStatus.Approved, decision.Status);
        }

        [Fact]
        public async Task Handle_PendingDecision_AddsApprovalWithProvidedUser()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.Handle(decisionId, "john.doe");

            // Assert
            var approval = Assert.Single(decision.Approvals);
            Assert.Equal("john.doe", approval.ApprovedBy);
            Assert.True(approval.IsApproved);
        }

        [Fact]
        public async Task Handle_PendingDecision_CallsGetByIdAsyncWithProvidedId()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.Handle(decisionId, "john.doe");

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(decisionId), Times.Once);
        }

        [Fact]
        public async Task Handle_PendingDecision_CallsSaveChangesAsyncOnce()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description");
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.Handle(decisionId, "john.doe");

            // Assert
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_AlreadyApprovedDecision_ThrowsInvalidOperationException()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description") { Status = DecisionStatus.Approved };
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "john.doe"));
        }

        [Fact]
        public async Task Handle_AlreadyApprovedDecision_DoesNotCallSaveChangesAsync()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description") { Status = DecisionStatus.Approved };
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "john.doe"));

            // Assert
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_RejectedDecision_ThrowsInvalidOperationException()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description") { Status = DecisionStatus.Rejected };
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "john.doe"));
        }

        [Fact]
        public async Task Handle_RejectedDecision_DoesNotCallSaveChangesAsync()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            var decision = new Decision("Title", "Description") { Status = DecisionStatus.Rejected };
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync(decision);

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(decisionId, "john.doe"));

            // Assert
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryReturnsNullDecision_ThrowsNullReferenceException()
        {
            // Arrange
            var decisionId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(decisionId)).ReturnsAsync((Decision)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.Handle(decisionId, "john.doe"));
        }
    }
}
