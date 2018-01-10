namespace Scheduler.Interfaces
{
    public interface ISender
    {
        void SendEmails();
        void SetSkipValue(int value);
    }
}
