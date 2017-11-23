using System.Collections.Generic;

namespace Scheduler.Data
{
    public interface IDataService
    {
        List<T> GetData<T>(string path);
        void UpdateData<T>(List<T> data, string path);
    }
}
