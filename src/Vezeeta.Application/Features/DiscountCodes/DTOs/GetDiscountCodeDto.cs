﻿

namespace Vezeeta.Application.Features.DiscountCodes.DTOs;
public class GetDiscountCodeDto
{
    public int Id { get; set; }
    public string Code { get; set; }

    public int DiscountType { get; set; }

    public int Value { get; set; }

    public bool? Activated { get; set; }

    public int RequestsNumber { get; set; }
}