namespace EventService.Repository.Interface
{
	public interface IRepository<T> where T : class
	{
		Task<T> Save(T entity);
		Task<int> Delete(T entity);
		Task<int> SaveChanges();
	}
}