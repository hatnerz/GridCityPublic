namespace GridCityServer.Dtos;

public record AuthResultDto(
    bool IsSuccess,
    string? Token,
    string? Error);
