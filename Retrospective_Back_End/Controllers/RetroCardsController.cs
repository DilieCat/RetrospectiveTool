using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Retrospective_Back_End.Realtime;
using Retrospective_Core.Models;
using Retrospective_Core.Services;

namespace Retrospective_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetroCardsController : ControllerBase
    {
        private readonly IRetroRespectiveRepository _context;
		private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;

		public RetroCardsController(IRetroRespectiveRepository context, IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            _context = context;
			_hubContext = hubContext;
		}

        /// <summary>
        /// Get all RetroCard
        /// </summary>
        // GET: api/RetroCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RetroCard>>> GetRetroCards()
        {
            return await _context.RetroCards.ToListAsync();
        }

        /// <summary>
        /// Get single RetroCard by id
        /// </summary>
        // GET: api/RetroCards/5
        [HttpGet("{id}")]
        public ActionResult<RetroCard> GetRetroCard(int id)
        {
            var retroCard = _context.RetroCards.FirstOrDefault(r => r.Id == id);

            if (retroCard == null)
            {
                return NotFound();
            }

            return retroCard;
        }


        /// <summary>
        /// Update a RetroCard
        /// </summary>
        // PUT: api/RetroCards
        [HttpPut]
        public ActionResult<RetroCard> UpdateRetroCard(RetroCard retroCard)
        {
            _context.SaveRetroCard(retroCard);

            RetroColumn retroColumn = _context.RetroColumns.Single(x => x.Id == retroCard.RetroColumnId);

            if (_hubContext != null)
            {
	            try
	            {
		            _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
	            }
	            catch
	            {
		            _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
	            }
            }

            return retroCard;
        }

        /// <summary>
        /// Create a new RetroCard
        /// </summary>
        // POST: api/RetroCards
        [HttpPost]
		public ActionResult<RetroCard> PostRetroCard(RetroCard retroCard)
		{
			_context.SaveRetroCard(retroCard);

            RetroColumn retroColumn = _context.RetroColumns.Single(x => x.Id == retroCard.RetroColumnId);

            if (_hubContext.Clients != null)
            {
	            try
	            {
		            _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
	            }
	            catch
	            {
		            _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
	            }
            }

            return CreatedAtAction("GetRetroCard", new { id = retroCard.Id }, retroCard);
		}

        /// <summary>
        /// Delete a RetroCard by id
        /// </summary>
        // DELETE: api/RetroCards/5
        [HttpDelete("{id}")]
        public ActionResult<RetroCard> DeleteRetroCard(int id)
        {
            RetroCard retroCard = _context.RetroCards.FirstOrDefault(r => r.Id == id);
            if (retroCard == null)
            {
                return NotFound();
            }

            _context.RemoveRetroCard(retroCard);

            RetroColumn retroColumn = _context.RetroColumns.Single(x => x.Id == retroCard.RetroColumnId);

            if (_hubContext != null)
            {
	            try
	            {
		            _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
	            }
	            catch
	            {
		            _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
	            }
            }

            return retroCard;
        }
    }
}
