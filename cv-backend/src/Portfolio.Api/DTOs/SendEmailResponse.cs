namespace Portfolio.Api.DTOs;

public record SendEmailResponse(bool Success, string? Message = null);