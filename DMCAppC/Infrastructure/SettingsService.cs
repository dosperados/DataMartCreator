using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DMCApp.Infrastructure;

public interface ISettingsService
{
    string GetConnectionString(string name);
    string SourceDB { get; }
    IConfigurationSection GetSection(string key);
    DataTypeMapping? GetDataTypeMapping(string sourceDataType, string sourceName, int? sourcePrecision);
    DataTypeDefault? GetDataTypeDefault(string dataType);
}

public class SettingsService : ISettingsService
{
    private readonly IConfiguration _configuration;

    public SettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name) ?? string.Empty;
    }

    public string SourceDB => _configuration["AppSettings:SourceDB"] ?? "TADWH";

    public IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }

    public DataTypeMapping? GetDataTypeMapping(string sourceDataType, string sourceName, int? sourcePrecision)
    {
        var section = _configuration.GetSection("DataTypeMappings");
        var mappings = section.Get<List<DataTypeMapping>>();

        if (mappings == null) return null;

        foreach (var m in mappings)
        {
            bool match = true;
            if (!string.IsNullOrEmpty(m.SourceType) && m.SourceType != sourceDataType) match = false;
            if (m.SourcePrecision.HasValue && m.SourcePrecision != sourcePrecision) match = false;
            if (!string.IsNullOrEmpty(m.SourceNamePattern) && !System.Text.RegularExpressions.Regex.IsMatch(sourceName, m.SourceNamePattern)) match = false;

            if (match) return m;
        }

        return null;
    }

    public DataTypeDefault? GetDataTypeDefault(string dataType)
    {
        var section = _configuration.GetSection("DataTypeDefaults");
        var defaults = section.Get<List<DataTypeDefault>>();

        if (defaults == null) return null;

        foreach (var d in defaults)
        {
            if (d.Types != null && d.Types.Contains(dataType, System.StringComparer.OrdinalIgnoreCase))
                return d;
        }

        return null;
    }
}

public class DataTypeMapping
{
    public string? SourceType { get; set; }
    public int? SourcePrecision { get; set; }
    public string? SourceNamePattern { get; set; }

    public string? TargetType { get; set; }
    public int? TargetPrecision { get; set; }
    public int? TargetScale { get; set; }
    public int? TargetMaxLength { get; set; }
}

public class DataTypeDefault
{
    public List<string>? Types { get; set; }
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public string? Collation { get; set; }
}
