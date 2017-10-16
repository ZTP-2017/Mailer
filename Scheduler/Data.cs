using CsvHelper;
using Scheduler.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scheduler
{
    public class Data
    {
        public static List<Message> data;

        public static void LoadDataFromFile()
        {
            try
            {
                using (var sr = new StreamReader(Settings.EmailFilePath))
                {
                    var csvReader = new CsvReader(sr);
                    data = csvReader.GetRecords<Message>().ToList();
                    Log.Information("Loaded data from file");
                }
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex, "File not found");
                data = new List<Message>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Load data from file error");
                data = new List<Message>();
            }
        }

        public static void UpdateFile(List<Message> data)
        {
            try
            {
                using (var sw = new StreamWriter(Settings.EmailFilePath))
                {
                    var csvWriter = new CsvWriter(sw);
                    csvWriter.WriteRecords(data);
                    Log.Information("Updated data in file");
                }
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex, "File not found");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Update data in file error");
            }
        }
    }
}
