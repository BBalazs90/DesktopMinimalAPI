using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Models;

public interface IRoute
{
}

public record StringRoute(string Path) : IRoute
{
    public static explicit operator StringRoute(string path) => new(path);
}
