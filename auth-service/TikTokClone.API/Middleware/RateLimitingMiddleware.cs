using System.Collections.Concurrent;
using System.Net;

namespace TikTokClone.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, DateTime> _lastRequestTimes = new();
        private static readonly ConcurrentDictionary<string, int> _requestCounts = new();
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);
        private readonly int _maxRequests = 10;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.Request.Path.Value;

            if (IsAuthEndpoint(endpoint))
            {
                var clientId = GetClientIdentifier(context);

                if (!IsRequestAllowed(clientId))
                {
                    _logger.LogWarning("Rate limit exceeded for client: {ClientId} on endpoint: {Endpoint}",
                        clientId, endpoint);

                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                    return;
                }
            }

            await _next(context);
        }

        private bool IsAuthEndpoint(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            var authEndpoints = new[] { "/api/auth/login", "/api/auth/register", "/api/auth/reset-password" };
            return authEndpoints.Any(endpoint => path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase));
        }

        private string GetClientIdentifier(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private bool IsRequestAllowed(string clientId)
        {
            var now = DateTime.UtcNow;

            if (!_lastRequestTimes.TryGetValue(clientId, out var lastRequest) ||
                now - lastRequest > _timeWindow)
            {
                _lastRequestTimes[clientId] = now;
                _requestCounts[clientId] = 1;
                return true;
            }

            var count = _requestCounts.GetOrAdd(clientId, 0);
            if (count >= _maxRequests)
            {
                return false;
            }

            _requestCounts[clientId] = count + 1;
            return true;
        }
    }
}
