using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Scheduler.Data
{
    public class DataService : IDataService
    {
        public List<T> GetData<T>(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var csvReader = new CsvReader(sr);
                var result = csvReader.GetRecords<T>().ToList();

                return result;
            }
        }

        public void UpdateData<T>(List<T> data, string path)
        {
            using (var sw = new StreamWriter(path))
            {
                var csvWriter = new CsvWriter(sw);
                csvWriter.WriteRecords(data);
            }
        }
    }
}
