using System.IO;

namespace Treenks.Bralek.Common.Services.ImportFeeds
{
    public interface IImportFeedService
    {
        void ImportOpml(Stream opmlFile, string userEmail);
    }
}