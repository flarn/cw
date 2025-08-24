using CsvHelper;
using CsvHelper.Configuration;
using CW.Core.Events;
using CW.Core.interfaces;
using System.Globalization;

namespace cw.worker.shared
{
    public class CsvSystem : IExternalSystem
    {
        public async Task SyncOrder(OrderUpdatedEvent orderUpdatedEvent, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine("..", "externalSystem", "csv", "orders.csv");
            bool orderExists = false;

            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using var writer = new StreamWriter(filePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteHeader<OrderUpdatedEvent>();
                csv.NextRecord();
                csv.WriteRecord(orderUpdatedEvent);

                return;
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = csv.GetRecord<OrderUpdatedEvent>();

                    if (record.Id == orderUpdatedEvent.Id)
                    {
                        if (record.UpdatedAt > orderUpdatedEvent.UpdatedAt)
                            throw new Exception("Row is modified later than event");

                        //order exists in system, dont appned it
                        orderExists = true;

                        break;
                    }
                }
            }

            if (orderExists)
            {
                //Update order

            }
            else
            {
                //Append new order
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Don't write the header again.
                    HasHeaderRecord = false,
                };
                using (var stream = File.Open(filePath, FileMode.Append))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.NextRecord();
                    csv.WriteRecord(orderUpdatedEvent);
                }
            }
        }
    }
}
