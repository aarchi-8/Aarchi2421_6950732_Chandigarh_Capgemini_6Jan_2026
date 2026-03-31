using System.ComponentModel.DataAnnotations;
namespace LearningPlatform.DTOs;
// ── Error ─────────────────────────────────────────────────────
public record ErrorResponse(string Error, int StatusCode, string? Details = null);