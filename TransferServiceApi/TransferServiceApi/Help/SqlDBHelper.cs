using SqlSugar;
using System;
using System.Linq;

namespace TransferServiceApi.Help
{
    public class SqlDBHelper
    {
        /// <summary>
        /// 创建SqlSugarClient
        /// </summary>
        /// <param name="ConnectionString">数据库地址连接字符串</param>
        /// <param name="DbType">连接数据库类型：0-MySql 1-SqlServer 3-Oracle</param>
        /// <returns></returns>
        public static SqlSugarClient GetInstance(string ConnectionString, int DbType = 1)
        {
            DbType dbType = (DbType)DbType;
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = dbType,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
#if DEBUG
            //添加Sql打印事件，开发中可以删掉这个代码
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                string value = sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));
                Console.WriteLine(value);
            };
#endif
            return db;
        }
    }
}
