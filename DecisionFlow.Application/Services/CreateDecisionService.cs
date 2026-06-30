using DecisionFlow.Application.Commands;
using DecisionFlow.Application.Interfaces;
using DecisionFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Application.Services
{
    public class CreateDecisionService
    {
        private readonly IDecisionRepository _repository;

        public CreateDecisionService(IDecisionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateDecisionCommand command)
        {
            var decision = new Decision(command.Title, command.Description);
            await _repository.AddAsync(decision);
            await _repository.SaveChangesAsync();
            return decision.Id;
        }
    }
}
