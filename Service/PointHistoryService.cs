// Service/PointHistoryService.cs
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;

public class PointHistoryService : IPointHistoryService
{
    private readonly IPointHistoryRepository _repo;
    private readonly IAccountRepository _accountRepo;
    private readonly IIncidentReportRepository _reportRepo;
    private readonly IBlogRepository _blogRepo;

    private static readonly string[] ValidRanges = { "day", "week", "month", "year" };

    public PointHistoryService(IPointHistoryRepository repo,
                               IAccountRepository accountRepo,
                               IIncidentReportRepository reportRepo,
                               IBlogRepository blogRepo)
    {
        _repo = repo;
        _accountRepo = accountRepo;
        _reportRepo = reportRepo;
        _blogRepo = blogRepo;
    }

    public async Task<long> LogAsync(Guid userId, Guid? actorId, string sourceType, string? sourceId,
                                     string action, int pointsDelta, int reputationDelta, string? note = null)
    {
        var ph = new PointHistory
        {
            UserId = userId,
            ActorId = actorId,
            SourceType = sourceType,
            SourceId = sourceId,
            Action = action,
            PointsDelta = pointsDelta,
            ReputationDelta = reputationDelta,
            Note = note,
            CreatedAt = DateTime.UtcNow
        };
        await _repo.AddAsync(ph);
        return ph.Id;
    }


    public async Task<PointHistoryResponseDto> GetHistoryAsync(Guid userId, string? range, string? sourceType, bool desc = true)
    {
        var user = await _accountRepo.GetByIdAsync(userId)
                   ?? throw new KeyNotFoundException("Không tìm thấy user.");


        DateTime? fromUtc = null;
        if (!string.IsNullOrWhiteSpace(range))
        {
            var r = range.Trim().ToLowerInvariant();
            if (!ValidRanges.Contains(r))
                throw new ArgumentException($"range không hợp lệ. Hợp lệ: {string.Join(", ", ValidRanges)}");

            fromUtc = r switch
            {
                "day" => DateTime.UtcNow.AddDays(-1),
                "week" => DateTime.UtcNow.AddDays(-7),
                "month" => DateTime.UtcNow.AddMonths(-1),
                "year" => DateTime.UtcNow.AddYears(-1),
                _ => null
            };
        }


        var list = await _repo.GetByUserAsync(userId, fromUtc, sourceType, desc);

        var actorIds = list.Where(h => h.ActorId.HasValue).Select(h => h.ActorId!.Value).Distinct().ToList();
        var actors = await _accountRepo.GetByIdsAsync(actorIds);
        var actorMap = actors.ToDictionary(a => a.Id, a => a.FullName);

        var reportIds = list.Where(h => h.SourceType == "incident_report" && Guid.TryParse(h.SourceId, out _))
                            .Select(h => Guid.Parse(h.SourceId!)).Distinct().ToList();
        var blogIds = list.Where(h => h.SourceType == "blog" && int.TryParse(h.SourceId, out _))
                          .Select(h => int.Parse(h.SourceId!)).Distinct().ToList();

        var reports = await _reportRepo.GetByIdsAsync(reportIds);
        var blogs = await _blogRepo.GetByIdsAsync(blogIds);

        var reportMap = reports.ToDictionary(r => r.Id, r => new {
            id = r.Id,
            status = r.Status,
            occurredAt = DateTimeHelper.ToVietnamTime(r.OccurredAt),
            address = r.Address
        });
        var blogMap = blogs.ToDictionary(b => b.Id, b => new { id = b.Id, title = b.Title });

        var items = list.Select(h => new PointHistoryItemDto
        {
            Id = h.Id,
            CreatedAt = DateTimeHelper.ToVietnamTime(h.CreatedAt),
            PointsDelta = h.PointsDelta,
            ReputationDelta = h.ReputationDelta,
            SourceType = h.SourceType,
            Action = h.Action,
            Note = h.Note,
            ActorName = h.ActorId.HasValue && actorMap.TryGetValue(h.ActorId.Value, out var name) ? name : null,
            Source = h.SourceType switch
            {
                "incident_report" => (object?)(Guid.TryParse(h.SourceId, out var rid) && reportMap.ContainsKey(rid)
                                        ? reportMap[rid] : null),
                "blog" => (object?)(int.TryParse(h.SourceId, out var bid) && blogMap.ContainsKey(bid)
                                        ? blogMap[bid] : null),
                _ => null
            }
        }).ToList();

        return new PointHistoryResponseDto
        {
            UserId = userId,
            CurrentTotalPoint = user.TotalPoint,
            CurrentReputationPoint = user.ReputationPoint,
            Items = items
        };
    }
}
