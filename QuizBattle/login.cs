using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace QuizBattle
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUser.Text.Equals("") || txtPassword.Text.Equals(""))
            {
                if (txtUser.Text.Equals(""))
                {
                    txtUser.BackColor = Color.Red;
                }
                if (txtPassword.Text.Equals(""))
                {
                    txtPassword.BackColor = Color.Red;
                }
                MessageBox.Show("Bitte die rot markierten Felder ausfüllen!", "Fehler");
            }
            else
            {
                string username = txtUser.Text;
                string password = txtPassword.Text;
                string connStr = @"server=localhost;userid=root;password=root;database=quizbattle";
                MySqlConnection conn = null;
                try
                {
                    conn = new MySqlConnection(connStr);
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "SELECT * FROM user WHERE name=@";
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Connection = conn;
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if(rdr.Read())
                    {
                        string passHash = rdr["password"].ToString();
                        string salt = rdr["salt"].ToString();
                        string passHashAndSalt = passHash + salt;
                        bool matches = BCrypt.Net.BCrypt.Verify(password, passHashAndSalt);

                        if (!matches)
                        {
                            MessageBox.Show("Falsches Passwort. Bitte korrigiere deine Eingabe!", "Fehler");
                        }
                        else
                        {
                            if (rdr["active"].ToString().Equals('1'))
                            {
                                lblSuccess.Visible = true;
                                lblSuccess.Text = "Erfolgreich angemeldet. Bitte warten...";

                                string id = rdr["id"].ToString();
                                /// 
                                this.Visible = false;
                            }
                            else
                            {
                                MessageBox.Show("Dein Account wurde nicht aktivert!");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ungültiger Benutzername oder ungültiges Passwort!");
                    }
                    rdr.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString(), "Fehler");
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
                conn.Close();
            }
        }
    }
}
