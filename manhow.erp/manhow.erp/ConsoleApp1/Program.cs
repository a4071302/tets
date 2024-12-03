using System;
using System.Data.SqlClient;
using Dapper;
using Interface.Repository.MsSql;
using Repository.MsSql.ErpDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Repository;


#region
//class Program
//{
//    private readonly IErpDbContext _erpDbContext;

//    public Program(IErpDbContext erpDbContext)
//    {
//        _erpDbContext = erpDbContext;
//    }

//    public static async Task Main()
//    {
//        var serviceProvider = new ServiceCollection()
//             .AddScoped<IErpDbContext>(provider =>
//             {
//                 // 直接在這裡設置連接字串
//                 string connectionString = "data source=220.132.33.142,9000;initial catalog=SunlitERP;integrated security=False;User ID=sunlitdev;password=admin@9000;TrustServerCertificate=True;";
//                 return new ErpDbContext(connectionString);
//             })
//             .AddScoped<Program>()
//             .BuildServiceProvider();

//        var program = serviceProvider.GetService<Program>();
//        Console.WriteLine("請按下某個按鍵來查詢資料...");
//        var key = Console.ReadKey(true).Key;

//        if (key == ConsoleKey.Enter)  // 當按下 Enter 鍵時
//        {
//            await program.GetDataFromDatabase1();  // 呼叫非靜態的 GetDataFromDatabase 方法
//        }
//    }

//    // 使用注入的 IErpDbContext 來查詢資料


//    public async Task GetDataFromDatabase1()
//    {
//        string query = "SELECT * FROM City;";
//        string sql = @"SELECT COUNT(*) FROM CarPallet;";

//        // 使用 await 來等待異步操作完成
//        //var tmp = await _erpDbContext.QueryAsync<int>(sql);
//        //var data = await _erpDbContext.Connection.QueryAsync(query);
//        var data = await _erpDbContext.QueryAsync<string>(query);


//        // 處理查詢結果
//        //int x = tmp.Any() ? tmp.FirstOrDefault() : 0;
//        //Console.WriteLine(x);

//        // 顯示 City 表中的資料
//        foreach (var item in data)
//        {
//            Console.WriteLine(item);
//        }
//    }

//}

#endregion


#region
//class Program
//{
//    private readonly IErpDbContext _erpDbContext;

//    public Program(IErpDbContext erpDbContext)
//    {
//        _erpDbContext = erpDbContext;
//    }

//    public static async Task Main()
//    {
//        var serviceProvider = new ServiceCollection()
//            .AddSqlServerContext()  // 使用 AddSqlServerContext 註冊資料庫上下文
//            .AddScoped<Program>()
//            .BuildServiceProvider();

//        var program = serviceProvider.GetService<Program>();
//        Console.WriteLine("請按下某個按鍵來查詢資料...");
//        var key = Console.ReadKey(true).Key;

//        if (key == ConsoleKey.Enter)  // 當按下 Enter 鍵時
//        {
//            await program.GetDataFromDatabase1();  // 呼叫非靜態的 GetDataFromDatabase 方法
//        }
//    }

//    // 使用注入的 IErpDbContext 來查詢資料
//    public async Task GetDataFromDatabase1()
//    {
//        string query = "SELECT * FROM City;";
//        string sql = @"SELECT COUNT(*) FROM CarPallet;";

//        // 使用 await 來等待異步操作完成
//        var data = await _erpDbContext.QueryAsync<string>(query);

//        // 顯示 City 表中的資料
//        foreach (var item in data)
//        {
//            Console.WriteLine(item);
//        }
//    }
//}

//// 在 AddSqlServerContext 中註冊資料庫上下文
//public static class DependencyInjection
//{
//    public static IServiceCollection AddSqlServerContext(this IServiceCollection services)
//    {
//        services.AddScoped<IErpDbContext>(provider =>
//        {
//            var configuration = provider.GetRequiredService<IConfiguration>();
//            var connectionString = configuration.GetSection("ConnectionStrings")["PointToPointDb"];

//            // 直接在這裡設置連接字串
//            //string connectionString = "data source=220.132.33.142,9000;initial catalog=SunlitERP;integrated security=False;User ID=sunlitdev;password=admin@9000;TrustServerCertificate=True;";
//            return new ErpDbContext(connectionString);
//        });

//        return services;
//    }
//}

#endregion

class Program
{
    private readonly IErpDbContext _erpDbContext;


    // 修改為靜態構造函數
    public Program(IErpDbContext erpDbContext)
    {
        _erpDbContext = erpDbContext;
    }

    public static async Task Main(string[] args)
    {
        // 建立 Host 並註冊服務
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSqlServerContext(); // 註冊 IErpDbContext
                services.AddScoped<Program>(); // 註冊 Program 類型
            })
            .Build();


        // 從 DI 容器中解析出 Program 實例
        var program = host.Services.GetRequiredService<Program>();

        Console.WriteLine("請按下某個按鍵來查詢資料...");
        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.Enter)  // 當按下 Enter 鍵時
        {
            await program.GetDataFromDatabase1();  // 呼叫非靜態的 GetDataFromDatabase 方法
        }
    }

    // 使用注入的 IErpDbContext 來查詢資料
    public async Task GetDataFromDatabase1()
    {
        string query = "SELECT * FROM City;";

        // 使用 await 來等待異步操作完成
        var data = await _erpDbContext.QueryAsync<string>(query);

        // 顯示 City 表中的資料
        foreach (var item in data)
        {
            Console.WriteLine(item);
        }
    }
}

// 在 AddSqlServerContext 中註冊資料庫上下文
public static class DependencyInjection
{
    public static IServiceCollection AddSqlServerContext(this IServiceCollection services)
    {
        services.AddScoped<IErpDbContext>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetSection("ConnectionStrings")["ErpDb"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException($"config需要設定ErpDb連線字串");

            var options = provider.GetService<ISqlServerOptions>();

            return options is null
                ? new ErpDbContext(connectionString)
                : new ErpDbContext(connectionString, options);
        });

        return services;
    }
}





