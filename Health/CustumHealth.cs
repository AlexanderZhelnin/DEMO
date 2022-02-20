using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Health;

/// <summary>
/// Кастомная провека работоспособности
/// </summary>
public class CustumHealthCheck : IHealthCheck
{
    /// <summary>
    /// Фукнция проверки
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {

        return Task.FromResult(HealthCheckResult.Healthy("Моя проверка работает"));

        // return Task.FromResult(HealthCheckResult.Unhealthy("Моя проверка не работает"));
    }
}
