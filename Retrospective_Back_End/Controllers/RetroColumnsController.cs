using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Retrospective_Back_End.Realtime;
using Retrospective_Back_End.Utils;
using Retrospective_Core.Models;
using Retrospective_Core.Services;

namespace Retrospective_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetroColumnsController : ControllerBase
    {
        private readonly IRetroRespectiveRepository _context;
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        private IDecoder decoder;

        public RetroColumnsController(IRetroRespectiveRepository context, IHubContext<NotifyHub, ITypedHubClient> hubContext, IDecoder decoder)
        {
            _context = context;
            _hubContext = hubContext;
            this.decoder = decoder;
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

        //TODO: FIGURE OUT HOW TO AUTHORIZE THIS ONE!
        [HttpPut]
        public ActionResult<RetroColumn> PutRetroColumn(RetroColumn retroColumn)
        {
            _context.SaveRetroColumn(retroColumn);

            try
            {
                _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
            }
            catch (Exception e)
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
            Retrospective retrospective = _context.Retrospectives.First(x => x.Id == retroColumn.RetrospectiveId);

            var decodedId = decoder.DecodeToken(Request != null ? Request.Headers["token"].ToString() : null);

            if (retrospective == null && retroColumn != null)
                return NotFound();

            if (decodedId == null || retrospective.RetroUserId != int.Parse(decodedId))
                return Unauthorized();

            _context.SaveRetroColumn(retroColumn);

            if (_hubContext.Clients != null)
            {
                try
                {
                    _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
                }
                catch (Exception e)
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

            Retrospective retrospective = _context.Retrospectives.First(x => x.Id == retroColumn.RetrospectiveId);

            var decodedId = decoder.DecodeToken(Request != null ? Request.Headers["token"].ToString() : null);

            if (retrospective == null && retroColumn != null)
                return NotFound();

            if (decodedId == null || retrospective.RetroUserId != int.Parse(decodedId))
                return Unauthorized();

            _context.RemoveRetroColumn(retroColumn);

            if (_hubContext.Clients != null)
            {
                try
                {
                    _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
                }
                catch (Exception e)
                {
                    _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
                }
            }


            return retroColumn;
        }
    }
}
