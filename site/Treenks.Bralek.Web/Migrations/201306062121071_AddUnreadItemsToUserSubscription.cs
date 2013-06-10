namespace Treenks.Bralek.Web.Migrations
{
    using System.Data.Entity.Migrations;
    public partial class AddUnreadItemsToUserSubscription : DbMigration
    {
        public override void Up()
        {            
            AddColumn("dbo.UserSubscriptions", "UnreadItems", c => c.Int(nullable: false, defaultValue: 0));
            Sql(@"UPDATE [dbo].[UserSubscriptions]  SET UnreadItems = i.itemsNotRead 
                FROM (SELECT COUNT(*) AS itemsNotRead,us.Id FROM [dbo].[SubscriptionPosts] sp 
                    INNER JOIN [dbo].[Subscriptions] s ON sp.Subscription_Id = s.Id
                    INNER JOIN [dbo].UserSubscriptions us on us.Subscription_Id = s.Id
                    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[PostsRead] pr
	                                  WHERE pr.SubscriptionPost_Id = sp.Id and pr.UserSubscription_Id = us.Id)
                    GROUP BY us.Id) i
                WHERE i.Id = [dbo].[UserSubscriptions].Id");
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserSubscriptions", "UnreadItems");
        }
    }
}
