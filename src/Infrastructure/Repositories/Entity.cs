using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace Infrastructure.Repositories;

public abstract class Entity : ITrackable
{
    [NotMapped]
    public TrackingState TrackingState { get; set; }

    [NotMapped]
    public ICollection<string> ModifiedProperties { get; set; }
}
