namespace Domain.Monads;

public sealed class Unit : IEquatable<Unit>
{
    public static readonly Unit Instance = new();

    public Unit() { }

    public override bool Equals(Object? obj) => obj is Unit;

    public bool Equals(Unit? other) => other is not null;

    public override int GetHashCode() => 0;

    public static bool operator ==(Unit? left, Unit? right) => true;

    public static bool operator !=(Unit? left, Unit? right) => false;

    public override string ToString() => nameof(Unit);
}
