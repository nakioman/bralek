namespace Treenks.Bralek.Worker.Jobs
{
    public interface IJob
    {
        void Start();
        void Stop();
    }
}