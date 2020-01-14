using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Retrospective_Core.Services;
using Retrospective_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System;
using Retrospective_Back_End.Utils;

namespace Retrospective_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetrospectivesController : ControllerBase
    {
        private readonly IRetroRespectiveRepository _context;

        public RetrospectivesController(IRetroRespectiveRepository context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all Retrospectives
        /// </summary>
        // GET: api/Retrospectives
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Retrospective>>> GetRetrospectives()
        {
            return await Task.FromResult(_context.getAll().ToList());
        }

        /// <summary>
        /// Get single Retrospective by id
        /// </summary>
        // GET: api/Retrospectives/5
        [HttpGet("{id}")]
        public ActionResult<Retrospective> GetRetrospective(int id)
        {
            var retrospective = _context.Retrospectives.Include(c => c.RetroColumns).ThenInclude(s => s.RetroCards)
                .Include(c => c.RetroColumns).ThenInclude(s => s.RetroFamilies).ThenInclude(c => c.RetroCards)
                .FirstOrDefault(r => r.Id == id);

            ICollection<RetroCard> removedRetroCards = new List<RetroCard>();

            if (retrospective != null)
            {
                foreach (RetroColumn r in retrospective.RetroColumns)
                {
                    foreach (RetroCard i in r.RetroCards)
                    {
                        RetroCard c = (RetroCard)i;
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


            if (retrospective == null)
            {
                return NotFound();
            }

            return retrospective;
        }

        /// <summary>
        /// Update a Retrospective by id
        /// </summary>
        // PUT: api/Retrospectives/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult PutRetrospective(int id, Retrospective retrospective)
        {
            if (id != retrospective.Id)
            {
                return BadRequest();
            }

            try
            {
                _context.SaveRetrospective(retrospective);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RetrospectiveExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new Retrospective
        /// </summary>
        // POST: api/Retrospectives
        [HttpPost]
        public ActionResult<Retrospective> PostRetrospective(Retrospective retrospective)
        {
            var id = Decoder.DecodeToken(Request.Headers["token"]);

            if (id != null)
            {
                retrospective.RetroUserId = int.Parse(id);

                retrospective = ThreeColumnTemplate(retrospective);

                _context.SaveRetrospective(retrospective);
            } else
            {
                return Unauthorized();
            }

            return CreatedAtAction("GetRetrospective", new { id = retrospective.Id }, retrospective);
        }

        /// <summary>
        /// Delete a Retrospective by id
        /// </summary>
        // DELETE: api/Retrospectives/
        [HttpDelete("{id}")]
        public ActionResult<Retrospective> DeleteRetrospective(int id)
        {
            var retrospective = _context.Retrospectives.First(r => r.Id == id);
            if (retrospective == null)
            {
                return NotFound();
            }

            _context.RemoveRetrospective(retrospective);

            return retrospective;
        }

        private bool RetrospectiveExists(int id)
        {
            return _context.Retrospectives.Any(e => e.Id == id);
        }

        /// <summary>
        /// Delete all RetroCards and RetroItems from a Retrospective by id
        /// </summary>
        // DELETE: api/Retrospectives/{id}/RetroCards
        [HttpDelete("{id}/RetroCards")]
        public ActionResult<Retrospective> CleanRetrospective(int id)
        {
            var retrospective = _context.Retrospectives
                .Include(c => c.RetroColumns)
                .ThenInclude(s => s.RetroCards)
                .Include(c => c.RetroColumns)
                .ThenInclude(s => s.RetroFamilies)
                .ThenInclude(x => x.RetroCards)
                .FirstOrDefault(r => r.Id == id);

            if (retrospective == null)
            {
                return NotFound();
            }

            _context.CleanRetrospective(retrospective);

            return retrospective;
        }



        private Retrospective ThreeColumnTemplate(Retrospective retrospective)
        {
            var columns = new List<RetroColumn>
            {
                new RetroColumn
                {
                    Title = "To do"
                },
                new RetroColumn
                {
                    Title = "Doing"
                },

                new RetroColumn
                {
                    Title = "Done"
                }
            };

            foreach (RetroColumn r in columns)
            {
                retrospective.RetroColumns.Add(r);
            }

            return retrospective;
        }
    }
}
