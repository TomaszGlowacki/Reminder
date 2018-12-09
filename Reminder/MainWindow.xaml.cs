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
        public string DaysTo { get; set; }
        public int HoursTo { get; set; }
        public string Description { get; set; }
    }

    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source=ReminderDatabase.db;Version=3";

        public MainWindow()
        {
            if (!System.IO.File.Exists("d:ReminderDatabase.db"))
            {
                SQLiteConnection.CreateFile("ReminderDatabase.db");
                SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);
                try
                {
                    DB_Connection.Open();

                    string sql = "CREATE TABLE events (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(20), date DATETIME, description VARCHAR(1000))";
                    SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                    command.ExecuteNonQuery();
                    sql = "CREATE TABLE category (id, name VARCHAR(20) )";
                    command = new SQLiteCommand(sql, DB_Connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    DB_Connection.Close();
                }
            }
            InitializeComponent();
            refreshList();
        }

        public void refreshList()
        {
            try
            {
                listView.Items.Clear();   //
            }
            catch (Exception e) { }

            listView.Items.Refresh();
            SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);
            try
            {
                DB_Connection.Open();

                string sql = "select * from events order by id desc";
                SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DateTime dt = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("Date")));
                    
                    double daysto = (dt - DateTime.Now).TotalDays;
                    double hoursto = (dt - DateTime.Now).TotalHours;
                    double resthours = 0;
                
                    resthours =  ((int)((dt - DateTime.Now).TotalHours % 24));
                    resthours = resthours < 0 ? -resthours : resthours;
                    string sign = "";
                    if (resthours * (int)hoursto < 0)
                    {
                        sign = "-";
                        daysto = -daysto;
                    }
                    string daysto_String = sign + (int)daysto + "d " + (int)resthours + "h";

                    EventItem item = new EventItem
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Date = reader.GetString(reader.GetOrdinal("Date")),   //reader.GetOrdinal("date").ToString(),  // why ?!
                        DaysTo = daysto_String,
                        HoursTo = (int)hoursto,
                        Description = reader["description"].ToString()
                    };
                    listView.Items.Add(item);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);

            DateTime date = new DateTime(datePicker.SelectedDate.Value.Year, datePicker.SelectedDate.Value.Month, datePicker.SelectedDate.Value.Day, Convert.ToInt32(timetextBoxh.Text), Convert.ToInt32(timetextboxmin.Text), 0);
            if (date != null)
            {
                string datestring = date.ToString();
                string timestring = timetextBoxh.Text + ":" + timetextboxmin.Text;

                string sql = "insert into events (name, date, description) values ('" + nametextBox.Text + "', '" + date + "', '" + DescriptiontextBox.Text + "')";
                //DescriptiontextBox.Text = sql;
                MessageBox.Show(sql);
                DB_Connection.Open();
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                finally
                {
                    DB_Connection.Close();
                }
            }
            refreshList();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventItem ei = (EventItem)listView.SelectedItem;
            nametextBox.Text = ei.Name;
            DescriptiontextBox.Text = ei.Description;

            DateTime dt; dt = Convert.ToDateTime(ei.Date);
            datePicker.Text = ei.Date;
            timetextBoxh.Text = dt.Hour.ToString();
            timetextboxmin.Text = dt.Minute.ToString();
            
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            EventItem ei = (EventItem)listView.SelectedItem;
            SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);

            DateTime date = new DateTime(datePicker.SelectedDate.Value.Year, datePicker.SelectedDate.Value.Month, datePicker.SelectedDate.Value.Day, Convert.ToInt32(timetextBoxh.Text), Convert.ToInt32(timetextboxmin.Text), 0);

            if (date != null)
            {
                MessageBox.Show(date.ToString());
                string sql = "UPDATE events SET name = '" + nametextBox.Text + "', date = '"+ date.ToString() + "', description = '"+ DescriptiontextBox.Text + "' WHERE id = "+ei.Id+"";
                //DescriptiontextBox.Text = sql; 
                DB_Connection.Open();
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                finally
                {
                    DB_Connection.Close();
                }
            }
            try
            {
                refreshList();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        { 
            EventItem ei = (EventItem)listView.SelectedItem;

            string sql = "DELETE FROM events WHERE id="+ ei.Id +"";

            SQLiteConnection DB_Connection = new SQLiteConnection(connectionString);
            DB_Connection.Open();
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, DB_Connection);
                command.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            finally
            {
                DB_Connection.Close();
            }
            try
            {
                refreshList();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
