using ConfigurationManagement.Application.Common.Base;
using ConfigurationManagement.Application.Configurations.Dto;

namespace ConfigurationManagement.Application.Configurations.Queries.GetConfigurationVersions;

/// <summary>
/// Получить версии конфигурации с пагинацией и фильтрацией.
/// </summary>
public record GetConfigurationVersionsQuery : Query<ConfigurationVersionsDto>
{
    /// <summary>
    /// Configuration ID to get versions for
    /// </summary>
    public Guid ConfigurationId { get; init; }

    /// <summary>
    /// Filter by date range (start)
    /// </summary>
    public DateTime? CreatedFrom { get; init; }

    /// <summary>
    /// Filter by date range (end)
    /// </summary>
    public DateTime? CreatedTo { get; init; }
}