namespace Scheduler
{
    public interface ISender
    {
        void SendEmails();
        void SetSkipValue(int value);
        void LoadAllMessagesFromFile(string path);
    }
}
