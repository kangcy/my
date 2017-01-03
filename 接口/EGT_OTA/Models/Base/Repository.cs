using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;
using SubSonic.Repository;
using SubSonic.DataProviders;
using System.Web;
using EGT_OTA.Models;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 在Global文件中注册时使用
    /// </summary>
    public class Repository
    {
        public static SimpleRepository GetRepo()
        {
            var item = HttpContext.Current.Items["DefaultConnection"] as SimpleRepository;
            if (item == null)
            {
                var newItem = EGT_OTA.Models.Repository.GetRepo("DefaultConnection");
                HttpContext.Current.Items["DefaultConnection"] = newItem;
                return newItem;
            }
            return item;
        }

        public static SimpleRepository GetRepo(string db)
        {
            return new SimpleRepository(db, SimpleRepositoryOptions.Default);
        }

        public static SimpleRepository GetRepoByConn(string conn)
        {
            var idp = ProviderFactory.GetProvider(conn, "MySql.Data.MySqlClient");
            //var idp = ProviderFactory.GetProvider(conn, "System.Data.SqlClient");

            return new SimpleRepository(idp);
        }

        public static IDataProvider GetProvider(string connection = "DefaultConnection")
        {
            string database = System.Configuration.ConfigurationManager.ConnectionStrings[connection].ToString();
            return ProviderFactory.GetProvider(database, "MySql.Data.MySqlClient");
        }

        public static void UpdateDB(string connection = "DefaultConnection")
        {
            var repo = new SimpleRepository(GetProvider(connection), SimpleRepositoryOptions.RunMigrations);

            repo.Single<User>(x => x.ID == 0);//用户
            repo.Single<Article>(x => x.ID == 0);//文章
            repo.Single<ArticlePart>(x => x.ID == 0);//文章部分
            repo.Single<Comment>(x => x.ID == 0);//评论
            repo.Single<Video>(x => x.ID == 0);//视频
            repo.Single<Fan>(x => x.ID == 0);//关注
            repo.Single<Keep>(x => x.ID == 0);//收藏
            repo.Single<Zan>(x => x.ID == 0);//点赞
            repo.Single<Pay>(x => x.ID == 0);//打赏
            repo.Single<FeedBack>(x => x.ID == 0);//意见反馈
            repo.Single<Help>(x => x.ID == 0);//帮助中心
            repo.Single<UserLogin>(x => x.ID == 0);//登录方式
            repo.Single<ShareLog>(x => x.ID == 0);//分享记录
            repo.Single<SendSMS>(x => x.ID == 0);//短信发送记录
            repo.Single<ApplyMoney>(x => x.ID == 0);//提现申请记录
            repo.Single<Report>(x => x.ID == 0);//举报记录
            repo.Single<ArticleRecommend>(x => x.ID == 0);//举报记录
        }
    }
}
