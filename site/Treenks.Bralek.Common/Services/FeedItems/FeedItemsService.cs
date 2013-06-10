using System;
using System.Collections.Generic;
using System.Linq;
using Treenks.Bralek.Common.Model;

namespace Treenks.Bralek.Common.Services.FeedItems
{
    public class FeedItemsService : IFeedItemsService
    {
        private readonly BralekDbContext _dbContext;

        public FeedItemsService(BralekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<SubscriptionPost> Search(string query, string userEmail)
        {
            var subscriptionPosts = _dbContext.SubscriptionPosts
                .Where(x => x.Subscription.UserSubscriptions.Any(y => y.User.Email == userEmail));

            subscriptionPosts = AddNeededWhereClauses(query, subscriptionPosts);

            return subscriptionPosts;
        }

        private IQueryable<SubscriptionPost> AddNeededWhereClauses(string query,
            IQueryable<SubscriptionPost> subscriptionPosts)
        {
            foreach (var whereClause in GetWhereClausesFromQuery(query))
            {
                switch (whereClause.PropertyName)
                {
                    case PropertyName.Author:
                        subscriptionPosts = subscriptionPosts
                            .Where(x => x.Authors.Contains(whereClause.Text) == whereClause.MustContainText);
                        break;
                    case PropertyName.Content:
                        subscriptionPosts = subscriptionPosts
                            .Where(x => x.Content.Contains(whereClause.Text) == whereClause.MustContainText);
                        break;
                    case PropertyName.Categories:
                        subscriptionPosts = subscriptionPosts
                            .Where(x => x.Categories.Contains(whereClause.Text) == whereClause.MustContainText);
                        break;
                    case PropertyName.Feed:
                        subscriptionPosts = subscriptionPosts
                            .Where(x => x.Subscription.Title.Contains(whereClause.Text) == whereClause.MustContainText);
                        break;
                    case PropertyName.Title:
                        subscriptionPosts = subscriptionPosts
                            .Where(x => x.Title.Contains(whereClause.Text) == whereClause.MustContainText);
                        break;
                }
            }
            return subscriptionPosts;
        }

        private IEnumerable<WhereClause> GetWhereClausesFromQuery(string query)
        {
            var startIndex = 0;
            var whereClauses = new List<WhereClause>();
            while (startIndex < query.Length && startIndex != -1)
            {
                var colonIndex = query.IndexOf(':', startIndex);
                if (colonIndex == -1 && startIndex == 0)
                {
                    return new List<WhereClause> { new WhereClause { MustContainText = true, PropertyName = PropertyName.Content, Text = query } };
                }

                var searchTerm = GetSearchTerm(query, startIndex);
                var mustContainText = !searchTerm.Contains("NOT");
                var propertyName = string.Empty;
                if (!mustContainText)
                {
                    searchTerm = searchTerm.Substring(3).Trim();

                }
                propertyName = searchTerm.Remove(searchTerm.IndexOf(':'));
                var text = searchTerm.Replace(String.Format("{0}:", propertyName), "").Trim();
                var whereClause = new WhereClause
                                      {
                                          MustContainText = mustContainText,
                                          PropertyName = (PropertyName)Enum.Parse(typeof(PropertyName), propertyName, true),
                                          Text = text
                                      };
                whereClauses.Add(whereClause);
                var nextAnd = query.IndexOf("AND", colonIndex + 1, StringComparison.Ordinal);
                startIndex = nextAnd == -1 ? -1 : nextAnd + 3;
            }
            return whereClauses;
        }

        private static string GetSearchTerm(string query, int startIndex)
        {
            query = query.Substring(startIndex);
            var nextAnd = query.IndexOf("AND", StringComparison.Ordinal);
            return nextAnd == -1 ? query.Trim() : query.Remove(nextAnd).Trim();
        }
    }
}