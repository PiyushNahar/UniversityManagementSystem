using Microsoft.Extensions.Configuration;

class Helper
{
    public static string ConnStr
    {
        get
        {
            var configure = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json",optional: false, reloadOnChange: true).Build();
            string connection = configure.GetConnectionString("Default");
            return connection;
        }
    }

    public static string ConnStrNoDB
    {
        get
        {
            var configure = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            string connection = configure.GetConnectionString("CreateDB");
            return connection;
        }
    }
}
