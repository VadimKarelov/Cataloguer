using System.Buffers;
using System.Text;
using System.Text.Json;
using Cataloguer.Common.Models;
using Cataloguer.Common.Models.SpecialModels.InputApiModels;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands;
using Serilog;

namespace Cataloguer.Server.ContextHandlers;

public static class ContextHandler
{
    public static string AddBrochure(HttpContext context, DataBaseConfiguration config)
    {
        try
        {
            var entireRequestBody = GetBody(context);

            var brochure = JsonSerializer.Deserialize<BrochureCreationModel>(entireRequestBody);

            if (brochure is null) throw new ArgumentNullException(nameof(brochure));

            var entity = new Brochure
            {
                Name = brochure.Name,
                Date = brochure.Date,
                Edition = brochure.Edition
            };

            var id = new AddOrUpdateCommand(config).AddOrUpdate(entity);

            new AddOrUpdateCommand(config).AddPositions(id, brochure.Positions);

            return id.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Обработка запроса на добавление нового каталога.");
            return "-1";
        }
    }

    public static string AddDistribution(HttpContext context, DataBaseConfiguration config)
    {
        try
        {
            var entireRequestBody = GetBody(context);

            var distribution = JsonSerializer.Deserialize<Distribution>(entireRequestBody);

            if (distribution is null) throw new ArgumentNullException(nameof(distribution));

            var id = new AddOrUpdateCommand(config).AddOrUpdate(distribution);

            return id.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Обработка запроса на добавление новой рассылки.");
            return "-1";
        }
    }

    public static string AddGoodsToBrochure(HttpContext context, DataBaseConfiguration config, int brochureId)
    {
        try
        {
            var entireRequestBody = GetBody(context);

            var goods = JsonSerializer.Deserialize<CreationPosition[]>(entireRequestBody);

            if (goods is null) throw new ArgumentNullException(nameof(goods));

            new AddOrUpdateCommand(config).AddPositions(brochureId, goods);

            return "OK";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Обработка запроса на добавление товаров в существующий каталог.");
            return "-1";
        }
    }

    public static string UpdateBrochure(HttpContext context, DataBaseConfiguration config, int brochureId)
    {
        try
        {
            var entireRequestBody = GetBody(context);

            var brochure = JsonSerializer.Deserialize<Brochure>(entireRequestBody);

            if (brochure is null) throw new ArgumentNullException(nameof(brochure));

            brochure.Id = brochureId;

            var id = new AddOrUpdateCommand(config).AddOrUpdate(brochure);

            return id.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Обработка запроса на обновление каталога.");
            return "-1";
        }
    }

    public static string UpdateDistribution(HttpContext context, DataBaseConfiguration config, int distributionId)
    {
        try
        {
            var entireRequestBody = GetBody(context);

            var distribution = JsonSerializer.Deserialize<Distribution>(entireRequestBody);

            if (distribution is null) throw new ArgumentNullException(nameof(distribution));

            distribution.Id = distributionId;

            var id = new AddOrUpdateCommand(config).AddOrUpdate(distribution);

            return id.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Обработка запроса на обновление рассылки.");
            return "-1";
        }
    }

    private static string GetBody(HttpContext context)
    {
        var requestBody = context.Request.Body;

        // Build up the request body in a string builder.
        var builder = new StringBuilder();

        // Rent a shared buffer to write the request body into.
        var buffer = ArrayPool<byte>.Shared.Rent(4096);

        while (true)
        {
            var bytesRemaining = requestBody.Read(buffer, 0, buffer.Length);
            if (bytesRemaining == 0) break;

            // Append the encoded string into the string builder.
            var encodedString = Encoding.UTF8.GetString(buffer, 0, bytesRemaining);
            builder.Append(encodedString);
        }

        ArrayPool<byte>.Shared.Return(buffer);

        return builder.ToString();
    }
}