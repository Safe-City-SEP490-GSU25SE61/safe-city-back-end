using BusinessObject.DTOs.Enums;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.Text.Json;

namespace Service
{
    public class BlogModerationService
    {
        private readonly ChatClient _chat;
        private readonly ILogger<BlogModerationService> _logger;

        public BlogModerationService(string apiKey, ILogger<BlogModerationService> logger)
        {
            _chat = new ChatClient(model: "gpt-4o", apiKey: apiKey);
            _logger = logger;
        }

        public async Task<BlogModerationResult> ModerateBlogAsync(string blogContent, BlogType blogType)
        {
            try
            {
                var typeExpectations = blogType switch
                {
                    BlogType.Alert => """
                            - **Alert (Cảnh báo)**: Content must be **factual**, **urgent**, and **direct**. It should warn people clearly and avoid causing panic or spreading rumors.
                            """,
                    BlogType.Tip => """
                            - **Tip (Mẹo)**: Content must be **helpful**, **practical**, and **clear**. Avoid vague, misleading, or unverified suggestions.
                            """,
                    BlogType.Event => """
                            - **Event (Sự kiện)**: Must clearly state time, place, and purpose. Avoid exaggeration or unconfirmed event details.
                            """,
                    BlogType.News => """
                            - **News (Tin tức)**: Should reflect verified, neutral information with a clear source. Avoid bias or sensationalism.
                            """,
                    _ => ""
                };

                var prompt = $$"""
                        You are a strict Vietnamese content moderator and also a community officer (công an). Analyze the blog content below based on **four main criteria**:

                        1. **Politeness (Lịch sự)**: Respectful and non-offensive language. Mark false if any part is disrespectful or uses bad language.

                        2. **No anti-state content (Không chống phá nhà nước)**: Do not allow any content that criticizes or compares negatively against the Vietnamese government or policies.

                        3. **Positive meaning (Có ý nghĩa tích cực)**: The blog must offer clear, motivational, or socially beneficial ideas. No confusion or negativity.

                        4. **Type-Specific Requirement**:
                        {{typeExpectations}}

                        Return ONLY strict JSON (no code blocks or markdown):

                        {
                          "is_approved": true/false,
                          "criteria": {
                            "politeness": true/false,
                            "no_anti_state": true/false,
                            "positive_meaning": true/false,
                            "type_requirement": true/false
                          },
                          "reasoning": "Brief explanation in Vietnamese"
                        }

                        Here is the blog content to analyze:
                        {{blogContent}}
                        """;


                var response = await _chat.CompleteChatAsync(prompt);

                string rawResponse = response.Value.Content[0].Text;
                _logger.LogInformation("Raw AI Response: {Response}", rawResponse);

                string json = CleanJsonResponse(rawResponse);
                _logger.LogInformation("Cleaned JSON: {Json}", json);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                return new BlogModerationResult
                {
                    IsApproved = root.GetProperty("is_approved").GetBoolean(),
                    Politeness = root.GetProperty("criteria").GetProperty("politeness").GetBoolean(),
                    NoAntiState = root.GetProperty("criteria").GetProperty("no_anti_state").GetBoolean(),
                    PositiveMeaning = root.GetProperty("criteria").GetProperty("positive_meaning").GetBoolean(),
                    TypeRequirement = root.GetProperty("criteria").GetProperty("type_requirement").GetBoolean(),
                    Reasoning = root.GetProperty("reasoning").GetString() ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Moderation failed.");
                return new BlogModerationResult
                {
                    IsApproved = false,
                    Reasoning = $"Error: {ex.Message}"
                };
            }
        }

        private string CleanJsonResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return "{}";

            response = response.Trim();

            if (response.StartsWith("```json"))
            {
                response = response.Substring(7);
            }
            if (response.StartsWith("```"))
            {
                response = response.Substring(3);
            }
            if (response.EndsWith("```"))
            {
                response = response.Substring(0, response.Length - 3);
            }

            int startIndex = response.IndexOf('{');
            int lastIndex = response.LastIndexOf('}');

            if (startIndex >= 0 && lastIndex > startIndex)
            {
                response = response.Substring(startIndex, lastIndex - startIndex + 1);
            }

            return response.Trim();
        }
    }
}