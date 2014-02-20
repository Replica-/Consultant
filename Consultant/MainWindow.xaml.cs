using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateDB();

            //FillData();
        }

        void CreateDB()
        {
            SQLiteConnection cnn;

            SQLiteConnection.CreateFile("consultant.sqlite");

            cnn = new SQLiteConnection("Data Source=consultant.sqlite;Version=3;");
            cnn.Open();

            string sql = "create table projects (name varchar(20),client varchar(20))";

            SQLiteCommand command = new SQLiteCommand(sql, cnn);
            command.ExecuteNonQuery();

            sql = "insert into projects (name,client) values ('Request For Quote', 'Sumo Visual Group')";

            command = new SQLiteCommand(sql, cnn);
            command.ExecuteNonQuery();

            cnn.Close();
        }

        void FillData()
        {

            SQLiteConnection cnn;
            var connectionString = GetConnectionString();
            cnn = new SQLiteConnection(connectionString);


            using (cnn)
            {
                cnn.Open();
                Console.WriteLine("ServerVersion: {0}", cnn.ServerVersion);
                Console.WriteLine("State: {0}", cnn.State);
            //Properties.Settings.Default.DataConnectionString))

                String SQL = "SELECT * FROM test";
                DataSet dataSet = new DataSet();
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(SQL, cnn);
                dataAdapter.Fill(dataSet);
                dataGrid.ItemsSource = dataSet.Tables["test"].Rows;
                
            }
        }

        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,  
            // you can retrieve it from a configuration file, using the  
            // System.Configuration.ConfigurationSettings.AppSettings property  
            return "Data Source=(local);database=database;"
                + "Integrated Security=SSPI;";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
        


        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
