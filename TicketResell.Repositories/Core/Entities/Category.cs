﻿namespace TicketResell.Repository.Core.Entities;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}