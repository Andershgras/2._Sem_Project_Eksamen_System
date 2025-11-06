using System;

namespace _2._Sem_Project_Eksamen_System;
public static class ClassNameNormalizer
{
    public static string Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input ?? string.Empty;

        var s = input.Trim();
        var parts = s.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5) return s;

        // Bykode -> UPPERCASE (2 bogstaver)
        parts[1] = parts[1].ToUpperInvariant();

        // F/V -> UPPERCASE
        parts[2] = parts[2].ToUpperInvariant();

        // Sæson+år+klassebogstav (fx v25b) -> UPPERCASE
        parts[3] = parts[3].ToUpperInvariant();

        // xsem -> behold tal, suffix normaliseres til “sem”
        if (parts[4].EndsWith("SEM", StringComparison.OrdinalIgnoreCase))
            parts[4] = parts[4][..^3] + "sem"; // bevar tallet foran

        return string.Join('-', parts);
    }
}
