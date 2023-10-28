using Cataloguer.Database.Base;
using Cataloguer.Database.Commands;
using Cataloguer.Database.Models;
using Cataloguer.Database.Models.SpecialModels;
using Serilog;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace Cataloguer.Server.ContextHandlers
{
    public static class ContextHandler
    {
        public static string AddBrochure(HttpContext context, DataBaseConfiguration config)
        {
            try
            {
                var requestBody = context.Request.Body;

                // Build up the request body in a string builder.
                StringBuilder builder = new StringBuilder();

                // Rent a shared buffer to write the request body into.
                byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);

                while (true)
                {
                    var bytesRemaining = requestBody.Read(buffer, offset: 0, buffer.Length);
                    if (bytesRemaining == 0)
                    {
                        break;
                    }

                    // Append the encoded string into the string builder.
                    var encodedString = Encoding.UTF8.GetString(buffer, 0, bytesRemaining);
                    builder.Append(encodedString);
                }

                ArrayPool<byte>.Shared.Return(buffer);

                var entireRequestBody = builder.ToString();

                var brochure = JsonSerializer.Deserialize<BrochureCreationModel>(entireRequestBody);

                if (brochure is null)
                {
                    throw new ArgumentNullException(nameof(brochure));
                }

                Brochure entity = new Brochure()
                {
                    Name = brochure.Name,
                    Date = brochure.Date,
                    Edition = brochure.Edition,
                };

                int id = new AddOrUpdateCommand(config).AddOrUpdate(entity);

                new AddOrUpdateCommand(config).AddPositions(id, brochure.Positions);

                return id.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Обработка запроса на добавление нового каталога.");
                return "-1";
            }
        }
    }
}
