using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public IQueryable<RetroFamily> RetroFamilies => _context.RetroFamilies;

        public IQueryable<Retrospective> getAll()
        {
            return _context.Retrospectives.Include(c => c.RetroColumns).ThenInclude(s => s.RetroCards);
        }

        public void RemoveRetroCard(RetroCard baseItem)
        {
            _context.RetroCards.Remove(baseItem);
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
                    dbEntry.Content = retroCard.Content;
                    dbEntry.Position = retroCard.Position;
                    dbEntry.RetroColumnId = retroCard.RetroColumnId;
                    dbEntry.RetroFamilyId = retroCard.RetroFamilyId;
                    dbEntry.DownVotes = retroCard.DownVotes;
                    dbEntry.UpVotes = retroCard.UpVotes;
                }
            }

            _context.SaveChanges();
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
                    dbEntry.RetroCards = retroColumn.RetroCards;
                    dbEntry.Title = retroColumn.Title;
                    dbEntry.RetrospectiveId = retroColumn.RetrospectiveId;

                    foreach(RetroFamily f in retroColumn.RetroFamilies)
                    {
                        this.SaveRetroFamily(f);
                    }
                }
            }

            _context.SaveChanges();
        }


        public void SaveRetroFamily(RetroFamily retroFamily)
        {
            if (retroFamily.Id == 0)
            {
                _context.RetroFamilies.Add(retroFamily);
            }
            else
            {
                RetroFamily dbEntry = _context.RetroFamilies
                    .FirstOrDefault(c => c.Id == retroFamily.Id);

                if (dbEntry != null)
                {
                    dbEntry.Position = retroFamily.Position;
                    dbEntry.RetroColumnId = retroFamily.RetroColumnId;
                    dbEntry.Content = retroFamily.Content;

                    foreach(RetroCard r in retroFamily.RetroCards)
                    {
                        this.SaveRetroCard(r);
                    }
                }
            }

            _context.SaveChanges();
        }

        public void RemoveRetroFamily(RetroFamily retroFamily)
        {
            IList<RetroCard> RetroCards = _context.RetroCards.Where(x => x.RetroFamilyId == retroFamily.Id).ToList();

            foreach(RetroCard r in RetroCards)
            {
                this.RemoveRetroCard(r);
            }

            _context.RetroFamilies.Remove(retroFamily);
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

        public void CleanRetrospective(Retrospective retrospective)
        {
            

            foreach (var rc in retrospective.RetroColumns)
            {
	            foreach (var rf in rc.RetroFamilies.ToList())
                {
                    RemoveRetroFamily(rf);
                }
                rc.RetroCards.Clear();
            }
            _context.SaveChanges();
        }
    }
}