﻿using System;
using System.Collections.Generic;

namespace Api.Core.Entities;

public partial class OrderDetail
{
    public string OrderDetailId { get; set; } = null!;

    public string? OrderId { get; set; }

    public string? TicketId { get; set; }

    public double? Price { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
