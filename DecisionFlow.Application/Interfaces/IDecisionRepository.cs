using DecisionFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Application.Interfaces
{
    public interface IDecisionRepository
    {
        Task<Decision> GetByIdAsync(Guid id);
        Task<List<Decision>> GetAllAsync();
        Task AddAsync(Decision decision);
        Task SaveChangesAsync();

    }
}
