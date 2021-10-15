using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NoClue.Core.WebSockets;

namespace NoClue.Middleware {
    public static class WebSocketMiddlewareExtensions {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder builder) {
            return builder.UseMiddleware<WebSocketMiddleware>();
        }

        public static IServiceCollection AddWebSocketCollection(this IServiceCollection services) {
            return services.AddSingleton<WebSocketCollection>();
        }
    }
}
