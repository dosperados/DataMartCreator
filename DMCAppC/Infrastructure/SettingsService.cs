using Microsoft.Extensions.Configuration;

namespace DMCApp.Infrastructure;

public interface ISettingsService
{
    string GetConnectionString(string name);
    string SourceDB { get; }
    IConfigurationSection GetSection(string key);
    DataTypeMapping GetDataTypeMapping(string dataType);
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

    public DataTypeMapping GetDataTypeMapping(string dataType)
    {
        // Simple implementation reading from json
        var section = _configuration.GetSection("DataTypeMappings");
        var mapping = section.GetSection(dataType);
        if (mapping.Exists())
        {
            return new DataTypeMapping
            {
                MaxLength = mapping.GetValue<int?>("MaxLength"),
                Precision = mapping.GetValue<int?>("Precision")
            };
        }
        return null;
    }
}

public class DataTypeMapping
{
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
}
