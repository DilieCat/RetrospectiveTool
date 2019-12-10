﻿using RetroSpective.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RetroSpective.Core.Services {

    public interface IRetroColumnRepository {

        IQueryable<RetroColumn> RetroColumns { get; }

    }
}
