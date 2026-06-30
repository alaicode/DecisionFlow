using DecisionFlow.Application.Interfaces;
using DecisionFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Infrastructure.Repositories
{
    public class DecisionFlowRepository<T> : IDecisionRepository<T> where T : class
    {
        protected readonly DecisionFlowDbContext _context;
        public DecisionFlowRepository(DecisionFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
