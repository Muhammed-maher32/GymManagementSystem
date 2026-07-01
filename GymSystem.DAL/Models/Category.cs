namespace GymSystem.DAL.Models;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;

    public IEnumerable<Session> Sessions { get; set; }

}
