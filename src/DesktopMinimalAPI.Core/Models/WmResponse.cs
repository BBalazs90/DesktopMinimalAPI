﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Models;

public record WmResponse(string RequestId, int Status, string Data);