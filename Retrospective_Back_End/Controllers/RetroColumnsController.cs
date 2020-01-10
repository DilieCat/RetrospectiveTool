﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public RetroColumnsController(IRetroRespectiveRepository context, IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/RetroColumns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RetroColumn>>> GetRetroColumns()
        {
            return await _context.RetroColumns.Include(c => c.RetroCards).ToListAsync();
        }

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

        // PUT: api/RetroColumns
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
	            catch (Exception e)
	            {
		            _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
	            }
            }

            return CreatedAtAction("GetRetroColumn", new { id = retroColumn.Id }, retroColumn);

        }

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
                catch (Exception e)
                {
                    _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
                }
            }


            return retroColumn;
        }

        private bool RetroColumnExists(int id)
        {
            return _context.RetroColumns.Any(e => e.Id == id);
        }
    }
}
