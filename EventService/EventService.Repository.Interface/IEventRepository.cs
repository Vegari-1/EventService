using EventService.Model;

namespace EventService.Repository.Interface
{
	public interface IEventRepository : IRepository<Event>
	{
		Task<IEnumerable<Event>> GetAllSortByTimestamp();
	}
}