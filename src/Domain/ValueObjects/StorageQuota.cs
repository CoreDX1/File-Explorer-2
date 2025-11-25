using Domain.Monads.Result;

namespace Domain.ValueObjects;

public sealed record StorageQuota
{
    public long Bytes { get; }

    private StorageQuota(long bytes) => Bytes = bytes;

    public static StorageQuota Default => new(5368709120); // 5 GB
    public static StorageQuota OneGB => new(1073741824);
    public static StorageQuota TenGB => new(10737418240);
    public static StorageQuota OneTB => new(1099511627776);

    public static Result<StorageQuota> Create(long bytes)
    {
        if (bytes < 0)
            return Result.Failure<StorageQuota>("Storage quota cannot be negative");

        if (bytes > 1099511627776) // 1 TB
            return Result.Failure<StorageQuota>("Storage quota cannot exceed 1 TB");

        return Result.Success(new StorageQuota(bytes));
    }

    /// <summary>
    /// Verifica si se puede asignar la cantidad de bytes especificada
    /// </summary>
    public bool CanAllocate(long usedBytes, long requestedBytes)
    {
        if (requestedBytes < 0)
            return false;

        return usedBytes + requestedBytes <= Bytes;
    }

    /// <summary>
    /// Obtiene la cuota restante en bytes
    /// </summary>
    public long GetRemainingQuota(long usedBytes)
    {
        var remaining = Bytes - usedBytes;
        return remaining < 0 ? 0 : remaining;
    }

    /// <summary>
    /// Obtiene el porcentaje de uso
    /// </summary>
    public double GetUsagePercentage(long usedBytes)
    {
        if (Bytes == 0)
            return 0;

        var percentage = (double)usedBytes / Bytes * 100;
        return Math.Min(percentage, 100);
    }

    /// <summary>
    /// Formatea los bytes en una representación legible (KB, MB, GB, TB)
    /// </summary>
    public string ToHumanReadable()
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = Bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    public static implicit operator long(StorageQuota quota) => quota.Bytes;
}
