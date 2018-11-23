using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Globalization;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class EventItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
    }

    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source=ReminderDatabase.db;Version=3";

        public MainWindow()
        {
            if (!System.IO.File.Exists("d:ReminderDatabase.db"))
            {
                try
                {
                    SQLiteConnection.CreateFile("ReminderDatabase.db");
                    SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);
                    DB_Connection.Open();

                        string sql = "CREATE TABLE events (id INT AUTO_INCREMENT, name VARCHAR(20), date DATETIME, description VARCHAR(1000))";
                        SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                        command.ExecuteNonQuery();
                        sql = "CREATE TABLE category (id, name VARCHAR(20) )";
                        command = new SQLiteCommand(sql, DB_Connection);
                        command.ExecuteNonQuery();

                    DB_Connection.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            InitializeComponent();
            refreshList();
        }

        public void refreshList()
        {
            listView.Items.Clear();
            try
            {
            SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);
            DB_Connection.Open();

                string sql = "select * from events order by id desc";
                SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                SQLiteDataReader reader = command.ExecuteReader();
            
                while (reader.Read())
                {
                    EventItem item = new EventItem
                    {
                        Id = reader.GetOrdinal("id"),
                        Name = reader["name"].ToString(),
                        Date = reader.GetString(reader.GetOrdinal("Date")),   //reader.GetOrdinal("date").ToString(),  // why ?!
                        Description = reader["description"].ToString()
                    };
                    listView.Items.Add(item);
                }
            DB_Connection.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);
            
            DateTime date = datePicker.SelectedDate ?? DateTime.Now ;
            if (date != null)
            {
                string datestring = date.ToString();
                string timestring = timetextBoxh.Text + ":" + timetextboxmin.Text;

                date = date.AddHours(Convert.ToDouble(timetextBoxh.Text));
                date = date.AddMinutes(Convert.ToDouble(timetextboxmin.Text));

                //DescriptiontextBox.Text = date.ToString();

                string sql = "insert into events (name, date, description) values ('" + nametextBox.Text + "', '" + date + "', '" + DescriptiontextBox.Text + "')";
                //DescriptiontextBox.Text = sql;

                DB_Connection.Open();
                try
                { 
                    SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                    command.ExecuteNonQuery();
                    DB_Connection.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            refreshList();
        }
    }
}
