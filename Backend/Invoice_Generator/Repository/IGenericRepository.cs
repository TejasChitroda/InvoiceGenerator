﻿using Invoice_Generator.Models;

namespace Invoice_Generator.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int? id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Query();
    }
}
