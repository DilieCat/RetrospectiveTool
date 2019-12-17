using System.Collections.Generic;
using System.Linq;
using Retrospective_Core.Models;
using Retrospective_Core.Services;

namespace Retrospective_EFSQLRetrospectiveDbImpl
{
    public class EFRetrospectiveRepository : IRetroRespectiveRepository
    {

        private readonly RetroSpectiveDbContext _context;

        public EFRetrospectiveRepository(RetroSpectiveDbContext context)
        {
            this._context = context;
        }

        public IQueryable<Retrospective> Retrospectives => _context.Retrospectives;

        public IQueryable<RetroColumn> RetroColumns => _context.RetroColumns;

        public IQueryable<RetroCard> RetroCards => _context.RetroCards;

        public void RemoveRetroCard(RetroCard retroCard)
        {
            _context.RetroCards.Remove(retroCard);
            _context.SaveChanges();
        }

        public void RemoveRetroColumn(RetroColumn retroColumn)
        {
            _context.RetroColumns.Remove(retroColumn);
            _context.SaveChanges();
        }

        public void RemoveRetrospective(Retrospective retrospective)
        {
            _context.Retrospectives.Remove(retrospective);
            _context.SaveChanges();
        }

        public void SaveRetroCard(RetroCard retroCard)
        {
            if (retroCard.Id == 0)
            {
                _context.RetroCards.Add(retroCard);
            }
            else
            {
                RetroCard dbEntry = _context.RetroCards
                    .FirstOrDefault(c => c.Id == retroCard.Id);

                if (dbEntry != null)
                {
                    dbEntry.RetroColumnId = retroCard.RetroColumnId;
                    dbEntry.Id = retroCard.Id;
                    dbEntry.Content = retroCard.Content;
                    dbEntry.Position = retroCard.Position;
                }
            }
            _context.SaveChanges();
        }

        // TODO: add sort update of the positions of the cards
        public RetroCard UpdateRetroCard(RetroCard retroCard)
        {
            var dbe = _context.RetroCards.FirstOrDefault(rc => rc.Id == retroCard.Id);

            if (dbe != null && dbe != retroCard)
            {
                dbe.Content = retroCard.Content;
                dbe.RetroColumnId = retroCard.RetroColumnId;
                dbe.Position = retroCard.Position;

                RetroCard[] retroCards = _context.RetroCards
                    .Where(rc => rc.RetroColumnId == retroCard.RetroColumnId)
                    .ToArray();


                // Sort all cards positions
                var tempCard = retroCards[0];

                for (var i = 1; i < retroCards.Length - 1; i++)
                {
                    if (retroCards[i].Position == tempCard.Position &&
                        retroCards[i].Position > retroCards[i - 1].Position)
                    {
                        
                    }
                }

            }

            return dbe;
        }

        public void SaveRetroColumn(RetroColumn retroColumn)
        {
            if (retroColumn.Id == 0)
            {
                _context.RetroColumns.Add(retroColumn);
            }
            else
            {
                RetroColumn dbEntry = _context.RetroColumns
                    .FirstOrDefault(c => c.Id == retroColumn.Id);

                if (dbEntry != null)
                {
                    dbEntry.RetrospectiveId = retroColumn.RetrospectiveId;
                    dbEntry.Id = retroColumn.Id;
                    dbEntry.RetroCards = retroColumn.RetroCards;
                    dbEntry.Title = retroColumn.Title;
                }

            }
            _context.SaveChanges();
        }

        public void SaveRetrospective(Retrospective retrospective)
        {
            foreach (RetroColumn retroColumn in retrospective.RetroColumns)
            {
                foreach (RetroCard retroCard in retroColumn.RetroCards)
                {
                    _context.RetroCards.Add(retroCard);
                }
                _context.RetroColumns.Add(retroColumn);
            }

            _context.Retrospectives.Add(retrospective);
            _context.SaveChanges();
        }
    }
}
