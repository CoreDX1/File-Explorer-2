using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace Domain.Entities;

public abstract class Entity : ITrackable
{
    [NotMapped]
    public TrackingState TrackingState { get; set; }

    [NotMapped]
    public ICollection<string> ModifiedProperties { get; set; }
}
