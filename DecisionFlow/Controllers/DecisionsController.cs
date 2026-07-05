using DecisionFlow.Application.Commands;
using DecisionFlow.Application.Services;
using DecisionFlow.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DecisionFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecisionsController : ControllerBase
    {
        private readonly CreateDecisionService _createDecisionService;
        private readonly ApproveDecisionService _approveDecisionService;
        private readonly RejectDecisionService _rejectDecisionService;
        private readonly GetDecisionsService _getDecisionsService;

        public DecisionsController(
            CreateDecisionService createDecisionService,
            ApproveDecisionService approveDecisionService,
            RejectDecisionService rejectDecisionService,
            GetDecisionsService getDecisionsService)
        {
            _createDecisionService = createDecisionService;
            _approveDecisionService = approveDecisionService;
            _rejectDecisionService = rejectDecisionService;
            _getDecisionsService = getDecisionsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDecisionRequest request)
        {
            var id = await _createDecisionService.Handle(new CreateDecisionCommand(request.Title, request.Description));
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var decisions = await _getDecisionsService.Handle();
            return Ok(decisions);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id, ApproveDecisionRequest request)
        {
            await _approveDecisionService.Handle(id, request.ApprovedBy);
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, RejectDecisionRequest request)
        {
            await _rejectDecisionService.Handle(id, request.RejectedBy);
            return NoContent();
        }
    }
}
