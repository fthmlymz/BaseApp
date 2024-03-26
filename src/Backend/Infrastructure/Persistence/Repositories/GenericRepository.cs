using Application.Interfaces.Repositories;
using Domain.Common;
using Persistence.Context;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseAuditableEntity
    {
        private readonly IMongoCollection<T> _collection;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            var database = dbContext._database;
            _collection = database.GetCollection<T>(typeof(T).Name);
        }

        public IQueryable<T> Entities => _collection.AsQueryable();

        public async Task<T> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, T entity) //sql için id alanı silinecek
        {
            /*var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);*/
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", new ObjectId(id)), entity);
        }

        public async Task DeleteAsync(string id) //T entity sql için olacak
        {
            /*var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await _collection.DeleteOneAsync(filter);*/
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", new ObjectId(id)));
        }

        public async Task<List<T>> GetAllAsync()
        {
            //return await _collection.Find(_ => true).ToListAsync();
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            /*var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();*/
            return await _collection.Find(Builders<T>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Where(expression);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _collection.Find(expression).AnyAsync();
        }
    }
}
