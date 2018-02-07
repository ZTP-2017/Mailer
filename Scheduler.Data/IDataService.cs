using System.Collections.Generic;

namespace Scheduler.Data
{
    public interface IDataService
    {
        List<T> GetAllMessages<T>(string path);
    }
}
