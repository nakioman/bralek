namespace Treenks.Bralek.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ReleaseOneDbModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        OrderByOldest = c.Boolean(nullable: false, defaultValue: false),
                        ShowAllItems = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.AlertInfos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false),
                        ViewedOnUTC = c.DateTime(nullable: false),
                        ViewedBy_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ViewedBy_Id)
                .Index(t => t.ViewedBy_Id);

            CreateTable(
                "dbo.UserSubscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UpdatedOnUTC = c.DateTime(nullable: false),
                        AddedOnUTC = c.DateTime(nullable: false),
                        Subscription_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subscriptions", t => t.Subscription_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Subscription_Id)
                .Index(t => t.User_Id);

            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUrl = c.String(),
                        FeedUrl = c.String(nullable: false),
                        Hash = c.Int(nullable: false),
                        AddedOnUTC = c.DateTime(nullable: false),
                        LastFeedUpdatesUTC = c.DateTime(storeType: "datetime2"),
                        IsWorking = c.Boolean(nullable: false),
                        LastError = c.String(),
                        Title = c.String(nullable: false),
                        AddedBy_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AddedBy_Id)
                .Index(t => t.AddedBy_Id);

            CreateTable(
                "dbo.SubscriptionPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        PublishDateUTC = c.DateTime(storeType: "datetime2"),
                        Authors = c.String(),
                        Categories = c.String(),
                        OriginalUrl = c.String(),
                        Content = c.String(),
                        Subscription_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subscriptions", t => t.Subscription_Id)
                .Index(t => t.Subscription_Id);

            CreateTable(
                "dbo.PostsRead",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReadOnUTC = c.DateTime(nullable: false),
                        SubscriptionPost_Id = c.Int(nullable: false),
                        UserSubscription_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubscriptionPosts", t => t.SubscriptionPost_Id)
                .ForeignKey("dbo.UserSubscriptions", t => t.UserSubscription_Id)
                .Index(t => t.SubscriptionPost_Id)
                .Index(t => t.UserSubscription_Id);

            CreateTable(
                "dbo.Bookmarks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOnUTC = c.DateTime(nullable: false),
                        SubscriptionPost_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubscriptionPosts", t => t.SubscriptionPost_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.SubscriptionPost_Id)
                .Index(t => t.User_Id);

        }

        public override void Down()
        {
            DropIndex("dbo.Bookmarks", new[] { "User_Id" });
            DropIndex("dbo.Bookmarks", new[] { "SubscriptionPost_Id" });
            DropIndex("dbo.PostsRead", new[] { "UserSubscription_Id" });
            DropIndex("dbo.PostsRead", new[] { "SubscriptionPost_Id" });
            DropIndex("dbo.SubscriptionPosts", new[] { "Subscription_Id" });
            DropIndex("dbo.Subscriptions", new[] { "AddedBy_Id" });
            DropIndex("dbo.UserSubscriptions", new[] { "User_Id" });
            DropIndex("dbo.UserSubscriptions", new[] { "Subscription_Id" });
            DropIndex("dbo.AlertInfos", new[] { "ViewedBy_Id" });
            DropForeignKey("dbo.Bookmarks", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Bookmarks", "SubscriptionPost_Id", "dbo.SubscriptionPosts");
            DropForeignKey("dbo.PostsRead", "UserSubscription_Id", "dbo.UserSubscriptions");
            DropForeignKey("dbo.PostsRead", "SubscriptionPost_Id", "dbo.SubscriptionPosts");
            DropForeignKey("dbo.SubscriptionPosts", "Subscription_Id", "dbo.Subscriptions");
            DropForeignKey("dbo.Subscriptions", "AddedBy_Id", "dbo.Users");
            DropForeignKey("dbo.UserSubscriptions", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserSubscriptions", "Subscription_Id", "dbo.Subscriptions");
            DropForeignKey("dbo.AlertInfos", "ViewedBy_Id", "dbo.Users");
            DropTable("dbo.Bookmarks");
            DropTable("dbo.PostsRead");
            DropTable("dbo.SubscriptionPosts");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.UserSubscriptions");
            DropTable("dbo.AlertInfos");
            DropTable("dbo.Users");
        }
    }
}
