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
using Xceed.Wpf.Toolkit;

using System.Data;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteConnection cnn;

        private void Calendar_DisplayModeChanged(object sender,
                                         CalendarModeChangedEventArgs e)
        {
            Calendar calObj = sender as Calendar;




            calObj.DisplayMode = CalendarMode.Year;
              FillData();
        }

        public MainWindow()
        {
            InitializeComponent();
        
            /*
            calendar1.Format = DateTimePickerFormat.Custom;
            calendar1.CustomFormat = "MM yyyy";
            calendar1.ShowUpDown = true; // to prevent the calendar from being displayed
            */
            //CreateDB();
            cnn = new SQLiteConnection("Data Source=consultant.sqlite;Version=3;");
            cnn.Open();
            
            FillProjects();
           
        }

        void CreateDB()
        {
            SQLiteConnection cnn;

            SQLiteConnection.CreateFile("consultant.sqlite");

            cnn = new SQLiteConnection("Data Source=consultant.sqlite;Version=3;");
            cnn.Open();
            /*
            string sql = "drop table projects";
            SQLiteCommand command = new SQLiteCommand(sql, cnn);
            command.ExecuteNonQuery();

            sql = "drop table times";
            command = new SQLiteCommand(sql, cnn);
            command.ExecuteNonQuery();
            */
            string sql = "create table times (id INTEGER PRIMARY KEY AUTOINCREMENT,project_id INTEGER, start_date DATETIME,end_date DATETIME,description TEXT)";
            SQLiteCommand command = new SQLiteCommand(sql, cnn);
            command.ExecuteNonQuery();

            sql = "create table projects (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(20),client VARCHAR(20))";
            command = new SQLiteCommand(sql, cnn);
            command.ExecuteNonQuery();

        



        /*    sql = "insert into projects (name,client) values ('Request For Quote', 'Sumo Visual Group')";

            command = new SQLiteCommand(sql, cnn);
            command.ExecuteNonQuery();
            */
            cnn.Close();
        }

        void FillProjects()
        {


            String SQL = "SELECT * FROM projects";
            DataSet ds = new DataSet();
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(SQL, cnn))
            {
                dataAdapter.Fill(ds);
            }

            DataView dv = ds.Tables[0].DefaultView;

            dataGrid_projects.ItemsSource = dv;
        }

        void FillData()
        {

            DateTime theMonthToFilter = DateTime.Today;
            
            if (calendar1.DisplayDate != null)
            {
                theMonthToFilter = (DateTime)calendar1.DisplayDate;
            }

            var startOfMonth = new DateTime(theMonthToFilter.Year, theMonthToFilter.Month, 1);
            var endOfMonth = new DateTime(theMonthToFilter.Year, theMonthToFilter.Month, 1).AddMonths(1).AddDays(-1); ;

            //calendar1.SelectedDate;
                DataRowView dr = ((DataRowView)dataGrid_projects.SelectedItem);

            if (dr==null) return;

                String SQL = "SELECT * FROM times WHERE project_id = '" + dr.Row[0] + "' AND julianday(start_date) > julianday('" + DateTimeSQLite(startOfMonth) + "') AND julianday(end_date) < julianday('" + DateTimeSQLite(endOfMonth) + "')";
                DataSet ds = new DataSet();
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(SQL, cnn))
                {
                    dataAdapter.Fill(ds);
                }

       
                DataView dv = ds.Tables[0].DefaultView;

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Link", typeof(DataRow)));
                dt.Columns.Add(new DataColumn("Readable", typeof(string)));
                dt.Columns.Add(new DataColumn("Day", typeof(int)));
                dt.Columns.Add(new DataColumn("Times", typeof(string)));
                dt.Columns.Add(new DataColumn("Duration", typeof(string)));
                dt.Columns.Add(new DataColumn("Description", typeof(string)));

                
                
               // dt.Columns[0].Visible = false;
           


                OrderedDictionary dtIndex = new OrderedDictionary();
                dtIndex.Clear();
              
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    dtIndex.Add(ds.Tables[0].Columns[i].ColumnName, i); 
                }

                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    var id = r[(int)dtIndex["id"]];
                    var project_id = r[(int)dtIndex["project_id"]];
                    DateTime start_date = (DateTime)r[(int)dtIndex["start_date"]];
                    DateTime end_date =(DateTime)r[(int)dtIndex["end_date"]];
                    var desc = r[(int)dtIndex["description"]];

                    TimeSpan diff = end_date-start_date;
                    String diffTime = diff.Hours.ToString() + " Hrs " + diff.Minutes.ToString() + "Mins";
                    String readable = Ordinal(Convert.ToInt32(start_date.ToString("dd"))) + " " + start_date.ToString("dddd");

                    dt.Rows.Add(r,readable,start_date.ToString("dd"), start_date.ToString("h:mm tt") + " - " + end_date.ToString("h:mm tt"), diffTime, desc);
                }
                             

                DataView dvdt = dt.DefaultView;
                dvdt.Sort = "Day ASC";
                
                ICollectionView iG = CollectionViewSource.GetDefaultView(dvdt);
                iG.GroupDescriptions.Add(new PropertyGroupDescription("Day"));
                dataGrid.ItemsSource = iG;

        }

        public static string Ordinal(int number)
        {
            string suffix = String.Empty;

            int ones = number % 10;
            int tens = (int)Math.Floor(number / 10M) % 10;

            if (tens == 1)
            {
                suffix = "th";
            }
            else
            {
                switch (ones)
                {
                    case 1:
                        suffix = "st";
                        break;

                    case 2:
                        suffix = "nd";
                        break;

                    case 3:
                        suffix = "rd";
                        break;

                    default:
                        suffix = "th";
                        break;
                }
            }
            return String.Format("{0}{1}", number, suffix);
        }

        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,  
            // you can retrieve it from a configuration file, using the  
            // System.Configuration.ConfigurationSettings.AppSettings property  
            return "Data Source=consultant.sqlite;Version=3;";
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

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void calendar1_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            FillData();
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private string DateTimeSQLite(DateTime datetime)
        {
            //string dateTimeFormat = ;
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dr = ((DataRowView)dataGrid_projects.SelectedItem);

            DateTime day = ((DateTime)edit_datePicker_start.SelectedDate).Date;
            DateTime start_day = ((DateTime)edit_datePicker_start.SelectedDate).Date;
            DateTime end_day = ((DateTime)edit_datePicker_start.SelectedDate).Date;

            TimeSpan start_time = ((DateTime)edit_startTime.Value).TimeOfDay;
            TimeSpan end_time = ((DateTime)edit_endTime.Value).TimeOfDay;

            start_day = start_day + start_time;
            end_day = end_day + end_time;

            //  WorkPeriod newWorkPeriod = new WorkPeriod(-1, (DateTime)dr.Row[0], start_day, end_day, description.Text);

            SQLiteCommand insertSQL = new SQLiteCommand("UPDATE times SET project_id=@param1,start_date=@param2,end_date=@param3,description=@param4 WHERE ID = @param5", cnn);
            insertSQL.CommandType = CommandType.Text;

            insertSQL.Parameters.Add(new SQLiteParameter("@param1", dr.Row[0]));
            insertSQL.Parameters.Add(new SQLiteParameter("@param2", DateTimeSQLite(start_day)));
            insertSQL.Parameters.Add(new SQLiteParameter("@param3", DateTimeSQLite(end_day)));
            insertSQL.Parameters.Add(new SQLiteParameter("@param4", description.Text));
            insertSQL.Parameters.Add(new SQLiteParameter("@param5", ID.Content));

            try
            {
                insertSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            FillData();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

            

            DataRowView dr = ((DataRowView)dataGrid_projects.SelectedItem);

            DateTime day = ((DateTime)datePicker_start.SelectedDate).Date;
            DateTime start_day = ((DateTime)datePicker_start.SelectedDate).Date;
            DateTime end_day = ((DateTime)datePicker_start.SelectedDate).Date;
            
            TimeSpan start_time = ((DateTime)startTime.Value).TimeOfDay;
            TimeSpan end_time = ((DateTime)endTime.Value).TimeOfDay;

            start_day = start_day + start_time;
            end_day = end_day + end_time;

          //  WorkPeriod newWorkPeriod = new WorkPeriod(-1, (DateTime)dr.Row[0], start_day, end_day, description.Text);

            SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO times (project_id,start_date,end_date,description) VALUES (@param1,@param2,@param3,@param4)", cnn);
            insertSQL.CommandType = CommandType.Text;

            insertSQL.Parameters.Add(new SQLiteParameter("@param1", dr.Row[0]));
            insertSQL.Parameters.Add(new SQLiteParameter("@param2", DateTimeSQLite(start_day)));
            insertSQL.Parameters.Add(new SQLiteParameter("@param3", DateTimeSQLite(end_day)));
            insertSQL.Parameters.Add(new SQLiteParameter("@param4",edit_description.Text));
          
            try
            {
                insertSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            FillData();

        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid_projects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillData();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //int rowIndex = e.RowIndex;
            System.Data.DataRowView row = (System.Data.DataRowView)dataGrid.CurrentItem;
           
            FillEdit();
            tabControl.SelectedItem = edit_tab;
        }

        private void FillEdit()
        {


            if (dataGrid.SelectedItem == null)
                return;
            DataRowView dr = ((DataRowView)dataGrid.SelectedItem);
            DataRow drow = (DataRow)dr.Row[0];

            DateTime startDate = (DateTime)drow["start_date"];
            DateTime endDate = (DateTime)drow["end_date"];
            String des = (String)drow["description"];

            //Extract timespan
            TimeSpan sstartTime = startDate.TimeOfDay;
            TimeSpan eendTime = endDate.TimeOfDay;

            edit_datePicker_start.SelectedDate = startDate;
            edit_startTime.Value = startDate;
            edit_endTime.Value = endDate;
            edit_description.Text = des;

            ID.Content = drow["id"];

        }

        private void edit_button3_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
