DbCore是一个高性能线程安全的数据库持久层管理器，基于.Net Core。

设计DbCore的目的是回归简单，如果你习惯于编写SQL而不是ORM映射，DbCore是非常好的选择，
对于遵循微软的DbProviderFactory设计的驱动，DbCore可以管理各种类型的数据库，包括：
Access、SQLite、SqlServer、MySql、Oracle、Firebird、PostgreSQL、DB2、Sybase ...

DbCore并没有完全拒绝ORM，并支持Linq，其应用逻辑是：直接编写SQL语句从数据库查询数据，
缓存到本地以后作为内存数据源应用Linq进行查询。至于采用实体对表进行增删改操作目前不再考虑范围之内，
你仍然需要编写SQL语句对表进行增删改。

DbCore的使用及其简单，只需三步，使用步骤如下：

第一步：下载数据库驱动程序，将dll及其依赖项放在Providers文件夹中，下面是各种数据库驱动下载:

类型:        Access(等支持Odbc驱动的各种数据库)
提供程序名称： System.Data.Odbc
项目位置：    https://github.com/dotnet/corefx/
下载位置：    https://www.nuget.org/packages/System.Data.Odbc/

类型:        Access(等支持OleDb驱动的各种数据库)
提供程序名称： System.Data.OleDb
项目位置：    https://github.com/dotnet/corefx/
下载位置：    https://www.nuget.org/packages/System.Data.OleDb/

类型:        SQLite
提供程序名称： System.Data.SQLite
项目位置：    https://system.data.sqlite.org/
下载位置：    https://www.nuget.org/packages/System.Data.SQLite.Core/

类型:        SQLite(微软提供)
提供程序名称： Microsoft.Data.Sqlite
项目位置：    https://docs.microsoft.com/zh-cn/ef/core/
下载位置：    https://www.nuget.org/packages/Microsoft.Data.Sqlite/

类型:        SqlServer
提供程序名称： System.Data.SqlClient
项目位置：    https://docs.microsoft.com/zh-cn/ef/core/
下载位置：    https://www.nuget.org/packages/System.Data.SqlClient/

类型:        MySql
提供程序名称： MySql.Data.MySqlClient
项目位置：    https://dev.mysql.com/downloads/
下载位置：    https://www.nuget.org/packages/MySql.Data/

类型:        Oracle (Oracle client provider for .Net Core based on mono implementation)
提供程序名称： System.Data.OracleClient
项目位置：    https://github.com/tonyrapozo/System.Data.OracleClient
下载位置：    https://www.nuget.org/packages/System.Data.OracleClient/

类型:        Oracle (Oracle Data Provider for .NET (ODP.NET) Core)
提供程序名称： Oracle.ManagedDataAccess.Client
项目位置：
下载位置：    https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/

类型:        Firebird
提供程序名称： FirebirdSql.Data.FirebirdClient
项目位置：    http://www.firebirdsql.org/en/net-provider/
下载位置：    https://www.nuget.org/packages/FirebirdSql.Data.FirebirdClient/

类型:        PostgreSQL
提供程序名称： Npgsql
项目位置：    https://www.npgsql.org
下载位置：    https://www.nuget.org/packages/Npgsql/

类型:        DB2
提供程序名称： IBM.Data.DB2.Core
项目位置：    
下载位置：    https://www.nuget.org/packages/IBM.Data.DB2.Core/

类型:        Sybase
提供程序名称： AdoNetCore.AseClient
项目位置：    https://github.com/DataAction/AdoNetCore.AseClient/
下载位置：    https://www.nuget.org/packages/AdoNetCore.AseClient/

第二步：配置数据源，修改Config目录中的配置文件DataSource.config，示例如下:

<?xml version="1.0" encoding="utf-8"?>
<DataSources>
  <DataSource>
    <Name>001</Name>
    <Type>Access</Type>
    <Provider>System.Data.Odbc</Provider>
    <ConnectionString>Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\mydatabase.mdb;Uid=admin;Pwd=;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>002</Name>
    <Type>Access</Type>
    <Provider>System.Data.OleDb</Provider>
    <ConnectionString>Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\mydatabase.mdb;User Id=admin;Password=;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>003</Name>
    <Type>Excel</Type>
    <Provider>System.Data.OleDb</Provider>
    <ConnectionString>Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\MyExcel.xls;Extended Properties="Excel 8.0;HDR=Yes;IMEX=1";</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>004</Name>
    <Type>Excel</Type>
    <Provider>System.Data.OleDb</Provider>
    <ConnectionString>Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\myFolder\myExcel2007file.xlsx;Extended Properties="Excel 12.0 Xml;HDR=YES";</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>005</Name>
    <Type>SQLite</Type>
    <Provider>System.Data.SQLite</Provider>
    <ConnectionString>Data Source=c:\mydb.db;Version=3;Password=myPassword;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>006</Name>
    <Type>SQLServer</Type>
    <Provider>System.Data.SqlClient</Provider>
    <ConnectionString>Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>007</Name>
    <Type>Oracle</Type>
    <Provider>Oracle.ManagedDataAccess.Client</Provider>
    <ConnectionString>Data Source=127.0.0.1:1521/orcl;User ID=scott;Password=tiger;Persist Security Info=True;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>008</Name>
    <Type>MySql</Type>
    <Provider>MySql.Data.MySqlClient</Provider>
    <ConnectionString>Server=myServerAddress;Port=1234;Database=myDataBase;Uid=myUsername;Pwd=myPassword;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>009</Name>
    <Type>Firebird</Type>
    <Provider>FirebirdSql.Data.FirebirdClient</Provider>
    <ConnectionString>User=SYSDBA;Password=masterkey;Database=SampleDatabase.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>010</Name>
    <Type>PostgreSQL</Type>
    <Provider>Npgsql</Provider>
    <ConnectionString>User ID=root;Password=myPassword;Host=localhost;Port=5432;Database=myDataBase;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>011</Name>
    <Type>DB2</Type>
    <Provider>IBM.Data.DB2.Core</Provider>
    <ConnectionString>Server=myAddress:myPortNumber;Database=myDataBase;UID=myUsername;PWD=myPassword;</ConnectionString>
  </DataSource>
  <DataSource>
    <Name>012</Name>
    <Type>Sybase</Type>
    <Provider>AdoNetCore.AseClient</Provider>
    <ConnectionString>Data Source=myASEserver;Port=5000;Database=myDataBase;Uid=myUsername;Pwd=myPassword;</ConnectionString>
  </DataSource>
</DataSources>

说明：
1.更多数据库连接字符串(ConnectionString)请参考：https://www.connectionstrings.com/
2.对于文件数据库，支持占位符{AppPath}，例如：Provider=Microsoft.Jet.OLEDB.4.0;Data Source={AppPath}mydatabase.mdb;User Id=admin;Password=;

第三步：使用:

    using (ISession session = PersistentManager.Instance.OpenSession("007"))
    {
        // 返回DataTable
        var dt = session.ExecuteQuery("select * from movie");
        Console.WriteLine(dt?.Rows.Count);
        
        // 返回List<T>
        var list = session.ExecuteQuery<Movie>("select * from movie");
        Console.WriteLine(list.Count);
    }

性能测试（Oracle）：

    查询记录4100条, 返回DataTable, 消耗1238毫秒。Linq查询记录2000条, 消耗19毫秒。
    查询记录4100条, 返回List<T>, 消耗24毫秒。Linq查询记录2000条, 消耗0毫秒。

   （具体性能视配置而定）
     (你需要明白返回List<T>比DataTable快的原因，是因为采用DataReader从数据库读取数据，
      即采用仅向前游标查询数据，而不是写法有多牛逼，速度的快慢取决于数据库底层驱动)

最后，DbCore并没有什么花哨的东西，它只是合理管理和简单实用。
