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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace WPF_TelephoneDiary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection DatabaseConnection;

        public MainWindow()
        {
            InitializeComponent();

            // This is our connection string to use for connecting to our Database. (Key)
            string connectionString = ConfigurationManager.ConnectionStrings["WPF_TelephoneDiary.Properties.Settings.ContactsDBConnectionString"].ConnectionString;

            // Creates a new connection to our Database using the connection string to access.
            DatabaseConnection = new SqlConnection(connectionString);
            ShowContacts();
        }




        // Method for showing table data in our DataGrid on the UI
        private void ShowContacts()
        {
            string query = "SELECT Id, FirstName, LastName, PhoneNumber FROM Contact";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, DatabaseConnection);


            using (dataAdapter)
            {
                // DataStorageTable serves as middle-man object that we populate with data retrieved through our dataAdapter object from the DataBase.
                DataTable DataStorageTable = new DataTable();
                dataAdapter.Fill(DataStorageTable);

                dataGridContactList.ItemsSource = DataStorageTable.DefaultView;
                dataAdapter.Update(DataStorageTable);
                

            }
            DatabaseConnection.Close();
        }

        // Insert Data into Database from UI
        private void EnterData()
        {
            
            // Pass the query to insert data into the DB
            string query = "INSERT into [Contact](FirstName, LastName, PhoneNumber)values('"+txtBoxFirstName.Text.ToString()+"'," +
                "'"+txtBoxLastName.Text.ToString()+"','"+txtBoxPhoneNumber.Text.ToString()+"')";
            DatabaseConnection.Open();

            SqlCommand cmd = new SqlCommand(query, DatabaseConnection);

            int a = cmd.ExecuteNonQuery();
            if (a == 1)
            {
                MessageBox.Show("Data Entry Successful!", "Success");
            }

            ClearFields();

            // Update the UI Grid with the new data inserted into DB
            ShowContacts();
        }

        // Update current Data in Datbase from UI
        private void UpdateData()
        {
            // Query for update
            string query = "UPDATE [Contact] SET FirstName = '" + txtBoxFirstName.Text.ToString() + "', LastName = '" + txtBoxLastName.Text.ToString() + "', PhoneNumber= '" + txtBoxPhoneNumber.Text.ToString() + "'" +
                "WHERE Id = '" + txtBoxID.Text.ToString() +"'";
            DatabaseConnection.Open();

            SqlCommand cmd = new SqlCommand(query, DatabaseConnection);

            int a = cmd.ExecuteNonQuery();
            if (a == 1)
            {
                MessageBox.Show("Data Update Successful!", "Success");
            }

            ClearFields();

            // Update UI Grid
            ShowContacts();
        }

        private void DeleteRecord()
        {
            // Query for deleting record
            string query = "DELETE FROM [Contact] WHERE Id = '"+txtBoxID.Text.ToString()+"'";
            DatabaseConnection.Open();

            SqlCommand cmd = new SqlCommand(query, DatabaseConnection);

            int a = cmd.ExecuteNonQuery();
            if (a == 1)
            {
                MessageBox.Show("Record successfully deleted from database!", "Success");
            }

            ClearFields();

            // Update UI Grid
            ShowContacts();
        }


        // Clear Method
        private void ClearFields()
        {
            txtBoxFirstName.Clear();
            txtBoxLastName.Clear();
            txtBoxPhoneNumber.Clear();
            txtBoxFirstName.Focus();
        }




        // FirstName txtBox
        private void txtBoxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        // LastName txtBox
        private void txtBoxLastName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        
        // PreviewTextInput Event - Validation for our FirstName and LastName txtBox
        private void IsLetterValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        // TextChanged Event for PhoneNumber txtBox
        private void txtBoxPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If the phone number has more than 16 digits, it is not allowed to be entered.
            if (txtBoxPhoneNumber.Text.Length > 16)
            {
                MessageBox.Show("Phone Number cannot have more than 16-digits.", "Invalid Phone Number", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtBoxPhoneNumber.Text = txtBoxPhoneNumber.Text.Substring(0, (txtBoxPhoneNumber.Text.Length - 1));
                txtBoxPhoneNumber.Select(txtBoxPhoneNumber.Text.Length, 0);
            }
        }

        // Validation for our PhoneNumber txtBox
        private void IsNumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        // OnClick Event for Enter Button
        private void btnAddButton_Click(object sender, RoutedEventArgs e)
        {
            // All fields must be filled and the PhoneNumber must be at least 10 digits (upperlimit: 16 digits)
            if ((txtBoxFirstName.Text == "") || (txtBoxLastName.Text == "") || (txtBoxPhoneNumber.Text == "") || txtBoxPhoneNumber.Text.Length < 10)
            {
                
                MessageBox.Show("Data Entry Failed! - ID field must be Empty upon new Entries", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtBoxFirstName.Focus();
                txtBoxFirstName.Select(txtBoxFirstName.Text.Length, 0);
                e.Handled = true;
            }
            else
            {
                EnterData();
            }
        }

        // OnClick Event for Update Button


        // OnClick Event for Reset Button
        private void btnResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all fields and set cursor in FirstName textbox
            ClearFields();
        }

        private void btnUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure conditions are aligned to allow this method to take place
            // We need an ID to update the correct entry, with all other txtboxes filled.
            if ((txtBoxID.Text == "")||(txtBoxFirstName.Text == "") || (txtBoxLastName.Text == "") || (txtBoxPhoneNumber.Text == "") || txtBoxPhoneNumber.Text.Length < 10)
            {
                MessageBox.Show("Data entry failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtBoxID.Focus();
                txtBoxID.Select(txtBoxID.Text.Length, 0);
            }
            else
            {
                // Method-call to update the records.
                UpdateData();
            }
        }

        private void btnDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxID.Text))
            {
                MessageBox.Show("Data entry failed! - Need an ID in order to delete record from Database", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtBoxID.Focus();
                txtBoxID.Select(txtBoxID.Text.Length, 0);
            }
            else
            {
                // Method-call to delete the record
                DeleteRecord();
            }
        }
    }
}