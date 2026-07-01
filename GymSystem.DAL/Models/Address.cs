using Microsoft.EntityFrameworkCore;

namespace GymSystem.DAL.Models;

[Owned]
public class Address
{
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
    public int BuildingNumber { get; set; }
}
