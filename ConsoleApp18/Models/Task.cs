using System;
using System.Collections.Generic;

namespace ConsoleApp18.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int Userid { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Time { get; set; }

    public virtual User User { get; set; } = null!;
}
