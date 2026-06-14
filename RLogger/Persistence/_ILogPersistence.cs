namespace RLogger.Persistence
{
    public interface ILogPersistence
    {
        void Write(LogMessage message);
        void Acknowledge(Guid messageId);
        IEnumerable<LogMessage> GetPending();
    }

    internal class NoOpLogPersistence : ILogPersistence
    {
        public static readonly NoOpLogPersistence Instance = new NoOpLogPersistence();
        public void Write(LogMessage message)
        {
            // No-op
        }
        public void Acknowledge(Guid messageId)
        {
            // No-op
        }
        public IEnumerable<LogMessage> GetPending()
        {
            return [];
        }
    }
}