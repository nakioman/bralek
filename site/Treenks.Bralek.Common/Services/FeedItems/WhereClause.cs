namespace Treenks.Bralek.Common.Services.FeedItems
{
    internal class WhereClause
    {
        public PropertyName PropertyName { get; set; }

        public string Text { get; set; }

        public bool MustContainText { get; set; }
    }
}