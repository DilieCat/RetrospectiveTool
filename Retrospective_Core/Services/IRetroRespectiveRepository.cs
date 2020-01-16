using System.Linq;
using Retrospective_Core.Models;

namespace Retrospective_Core.Services
{
    public interface IRetroRespectiveRepository
    {

	    IQueryable<Retrospective> Retrospectives { get; }
	    IQueryable<RetroColumn> RetroColumns { get; }
	    IQueryable<RetroCard> RetroCards { get; }
		IQueryable<RetroFamily> RetroFamilies { get; }
	    void SaveRetroCard(RetroCard retroCard);
		void RemoveRetroCard(RetroCard retroCard);
		void SaveRetroFamily(RetroFamily retroFamily);
		void RemoveRetroFamily(RetroFamily retroFamily);
		void SaveRetroColumn(RetroColumn retroColumn);
	    void RemoveRetroColumn(RetroColumn retroColumn);
	    void SaveRetrospective(Retrospective retrospective);
	    void RemoveRetrospective(Retrospective retrospective);
		void CleanRetrospective(Retrospective retrospective);

		IQueryable<Retrospective> GetAll();
    }
}
