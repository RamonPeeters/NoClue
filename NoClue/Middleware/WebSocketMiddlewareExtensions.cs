using Microsoft.AspNetCore.Builder;

namespace NoClue.Middleware {
    public static class WebSocketMiddlewareExtensions {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder builder) {
            return builder.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
