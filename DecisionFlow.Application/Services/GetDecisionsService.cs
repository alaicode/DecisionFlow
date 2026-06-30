using DecisionFlow.Application.Interfaces;
using DecisionFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Application.Services
{
    public class GetDecisionsService
    {
        private readonly IDecisionRepository _repository;

        public GetDecisionsService(IDecisionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Decision>> Handle()
        {
            return await _repository.GetAllAsync();
        }
    }
}
