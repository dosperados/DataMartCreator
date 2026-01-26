using DMCApp;
using DMCApp.Models;
using DMCApp.Models.EntityConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DMCApp.Models.Entities;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace DMCApp.Data
{
    public class ConfigBuilderExtension
    {
#warning Сюда надо перенести логику выбора строки подключения из appsettings.json


        string connectionKey = "DestDbWindowsAuth";
        public void SwitchConnection(string connectionKey)
        {
            var newConn = App.Configuration.GetConnectionString(connectionKey);
            if (!string.IsNullOrEmpty(newConn))
            {
                App.CurrentConnectionString = newConn;
            }
            else
            {
                MessageBox.Show($"Строка подключения \"{connectionKey}\" не найдена в appsettings.json");
            }
        }
    }
}
