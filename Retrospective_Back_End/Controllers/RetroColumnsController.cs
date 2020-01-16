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
    public class RetroColumnsController : ControllerBase
    {
        private readonly IRetroRespectiveRepository _context;
        private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public RetroColumnsController(IRetroRespectiveRepository context, IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get all RetroColumns
        /// </summary>
        // GET: api/RetroColumns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RetroColumn>>> GetRetroColumns()
        {
            return await _context.RetroColumns.Include(c => c.RetroCards).ToListAsync();
        }

        /// <summary>
        /// Get single RetroColumn by id
        /// </summary>
        // GET: api/RetroColumns/5
        [HttpGet("{id}")]
        public ActionResult<RetroColumn> GetRetroColumn(int id)
        {
            var retroColumn = _context.RetroColumns.Include(c => c.RetroCards).FirstOrDefault(r => r.Id == id);

            if (retroColumn == null)
            {
                return NotFound();
            }

            return retroColumn;
        }

        /// <summary>
        /// Update a RetroColumn
        /// </summary>
        // PUT: api/RetroColumns
        [HttpPut]
        public ActionResult<RetroColumn> PutRetroColumn(RetroColumn retroColumn)
        {
            _context.SaveRetroColumn(retroColumn);

            try
            {
                _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
            }
            catch
            {
                _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
            }

            return retroColumn;
        }

        /// <summary>
        /// Create a new RetroColumn
        /// </summary>
        // POST: api/RetroColumns
        [HttpPost]
        public ActionResult<RetroColumn> PostRetroColumn(RetroColumn retroColumn)
        {
            _context.SaveRetroColumn(retroColumn);

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

            return CreatedAtAction("GetRetroColumn", new { id = retroColumn.Id }, retroColumn);

        }

        /// <summary>
        /// Delete a RetroColumn by id
        /// </summary>
        // DELETE: api/RetroColumns/5
        [HttpDelete("{id}")]
        public ActionResult<RetroColumn> DeleteRetroColumn(int id)
        {
            RetroColumn retroColumn = _context.RetroColumns.FirstOrDefault(c => c.Id == id);
            if (retroColumn == null)
            {
                return NotFound();
            }

            _context.RemoveRetroColumn(retroColumn);

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


            return retroColumn;
        }
    }
}
