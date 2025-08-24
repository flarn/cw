using CsvHelper;
using CsvHelper.Configuration;
using CW.Core.Events;
using CW.Core.interfaces;
using System.Globalization;

namespace CW.Worker.Shared
{
    public class CsvSystem : IExternalSystem
    {
        public async Task SyncOrder(OrderUpdatedEvent orderUpdatedEvent, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine("..", "externalSystem", "csv", "orders.csv");
            bool orderExists = false;

            if (!File.Exists(filePath))
            {
                await CreateOrder(orderUpdatedEvent, filePath);
                return;
            }

            orderExists = await ReadAndValidateOrderExists(orderUpdatedEvent, filePath);

            if (orderExists)
            {
                await UpdateOrder(orderUpdatedEvent, filePath);
            }
            else
            {
                //Append new order
                await AppendOrder(orderUpdatedEvent, filePath);
            }
        }

        private static async Task<bool> ReadAndValidateOrderExists(OrderUpdatedEvent orderUpdatedEvent, string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                await csv.ReadAsync();
                csv.ReadHeader();
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<OrderUpdatedEvent>();

                    if (record.Id == orderUpdatedEvent.Id)
                    {
                        if (record.UpdatedAt > orderUpdatedEvent.UpdatedAt)
                            throw new Exception("Row is modified later than event");

                        //order exists in system, dont appned it
                        return true;
                        break;
                    }
                }
            }

            return false;
        }

        private static async Task CreateOrder(OrderUpdatedEvent orderUpdatedEvent, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            await using var writer = new StreamWriter(filePath);
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteHeader<OrderUpdatedEvent>();
            await csv.NextRecordAsync();
            csv.WriteRecord(orderUpdatedEvent);
        }

        private static async Task AppendOrder(OrderUpdatedEvent orderUpdatedEvent, string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = false,
            };
            using (var stream = File.Open(filePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                await csv.NextRecordAsync();
                csv.WriteRecord(orderUpdatedEvent);
            }
        }

        private static async Task UpdateOrder(OrderUpdatedEvent orderUpdatedEvent, string filePath)
        {
            //Update order
            using var reader = new StreamReader(filePath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            await csvReader.ReadAsync();
            csvReader.ReadHeader();

            await using var stream = File.Open(filePath + ".new", FileMode.CreateNew);
            await using var writer = new StreamWriter(stream);
            await using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.WriteHeader<OrderUpdatedEvent>();

            while (await csvReader.ReadAsync())
            {
                await csvWriter.NextRecordAsync();
                
                var record = csvReader.GetRecord<OrderUpdatedEvent>();

                if (record.Id == orderUpdatedEvent.Id)
                {
                    csvWriter.WriteRecord(orderUpdatedEvent);
                }
                else
                {
                    csvWriter.WriteRecord(record);
                }
            }

            File.Delete(filePath);
            File.Move(filePath + ".new", filePath);
        }
    }
}