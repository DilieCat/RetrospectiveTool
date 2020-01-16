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
            IList<Retrospective> retrospectives =  _context.Retrospectives.Include(c => c.RetroColumns).ThenInclude(s => s.RetroCards).Include(x => x.RetroColumns).ThenInclude(x => x.RetroFamilies).ThenInclude(x => x.RetroCards).ToList();
            ICollection<RetroCard> removedRetroCards = new List<RetroCard>();

            foreach (Retrospective retrospective in retrospectives)
            {
                foreach (RetroColumn r in retrospective.RetroColumns)
                {
                    foreach (RetroCard i in r.RetroCards)
                    {
                        RetroCard c = (RetroCard) i;
                        if (c.RetroFamily != null)
                        {
                            removedRetroCards.Add(i);
                        }
                    }

                    foreach (RetroCard i in removedRetroCards)
                    {
                        r.RetroCards.Remove(i);
                    }
                }
            }

            return retrospectives.AsQueryable();
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
            if (retrospective.Id == 0)
            {
                _context.Retrospectives.Add(retrospective);
            }
            else
            {
                Retrospective dbEntry = _context.Retrospectives
                    .FirstOrDefault(c => c.Id == retrospective.Id);

                if (dbEntry != null) {
                    dbEntry.Title = retrospective.Title;
                    dbEntry.CreatedDate = retrospective.CreatedDate;
                    dbEntry.Description = retrospective.Description;
                    dbEntry.RetroUserId = retrospective.RetroUserId;

                    foreach (RetroColumn r in retrospective.RetroColumns) {
                        this.SaveRetroColumn(r);
                    }
                }
            }

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