namespace Subless.PayoutCalculator.Scheduler
{
    public interface IReminderEmailJob
    {
        void QueueIdleEmailsForThisMonth();
    }
}