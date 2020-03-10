using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using DbCore;

namespace ConsoleApp
{
    public class Movie
    {
        public long ID;
        public string Title;
        public string ReleaseDate;
        public string Genre;
        public double Price;
    }
    class Program
    {
        static void Main(string[] args)
        {

            // 打印可用的数据库提供程序
            Console.WriteLine("-------------数据库提供程序-------------\r\n");
            foreach (var provider in PersistentManager.Instance.Providers)
            {
                Console.WriteLine($"提供程序名称: {provider.Name}");
                Console.WriteLine($"提供程序全称: {provider.FullName}");
                Console.WriteLine($"提供程序文件: {provider.Assembly}");
                Console.WriteLine("");
            }

            // 打印可用的数据源
            Console.WriteLine("-------------数据源-------------\r\n");
            foreach (var ds in PersistentManager.Instance.DataSources)
            {
                Console.WriteLine($"数据源名称: {ds.Name}");
                Console.WriteLine($"数据源类型: {ds.Type}");
                Console.WriteLine($"提供程序名: {ds.Provider}");
                Console.WriteLine($"连接字符串: {ds.ConnectionString}");
                Console.WriteLine("");
            }

            // 操作
            Console.WriteLine("-------------Oracle数据库测试-------------\r\n");
            using (ISession session = PersistentManager.Instance.OpenSession("007"))
            {
                Stopwatch t = new Stopwatch();

                t.Start();
                var dt = session.ExecuteQuery("select * from movie");
                Console.Write($"查询记录{dt.Rows.Count}条, 返回DataTable, 消耗{t.ElapsedMilliseconds}毫秒。");

                // 返回DataTable以后就可以使用Linq进行各种操作了，例如分页：
                t.Restart();
                var rows = from DataRow dr in dt.Rows.AsParallel() select dr;
                int pageIndex = 1; // 取第几页
                int pageSize = 2000; // 每页的数量
                pageIndex = pageIndex <= 0 ? 1 : pageIndex;
                int count = pageSize * (pageIndex - 1); // 跳过的元素数量
                var pageList1 = rows.Skip(count).Take(pageSize).ToList();
                Console.Write($"Linq查询记录{pageSize}条, 消耗{t.ElapsedMilliseconds}毫秒");
                Console.WriteLine("");

                t.Restart();
                var list = session.ExecuteQuery<Movie>("select * from movie");
                Console.Write($"查询记录{list.Count}条, 返回List<T>, 消耗{t.ElapsedMilliseconds}毫秒。");

                // 返回实体以后就可以使用Linq进行各种操作了，例如分页：
                t.Restart();
                pageIndex = 1; // 取第几页
                pageSize = 2000; // 每页的数量
                pageIndex = pageIndex <= 0 ? 1 : pageIndex;
                count = pageSize * (pageIndex - 1); // 跳过的元素数量
                var pageList2 = list.Skip(count).Take(pageSize).ToList();
                Console.Write($"Linq查询记录{pageSize}条, 消耗{t.ElapsedMilliseconds}毫秒");
                Console.WriteLine("");

                t.Stop();
            }

            Console.ReadLine();
        }
    }
}
