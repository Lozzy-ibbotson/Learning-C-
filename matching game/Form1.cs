using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace matching_game
{
    public partial class Form1 : Form
    {       
        // Use this Random object to choose random icons for the squares
        Random random = new Random();

        // will be passed to form3 to display the leaderboard with potntially inserted new user value
        string ForOutput = "";

        //value will be imported from form2 input box
        private string userName = "";
        public string UserName
        {
            get { return userName; }
            set
            { 
                userName = value;
                this.Text = this.Text + " " + userName;
            }
        }


        // Each of these letters is an interesting icon
        // in the Webdings font,
        // and each icon appears twice in this list
        List<string> icons = new List<string>()
        {
           "s", "s", "j", "j", "$", "$", "o", "o",
           "T", "T", "*", "*", "`", "`", "%", "%"
        };

        // firstClicked points to the first Label control 
        // that the player clicks, but it will be null 
        // if the player hasn't clicked a label yet
        Label firstClicked = null;

        // secondClicked points to the second Label control 
        // that the player clicks
        Label secondClicked = null;

        public Form1()
        {
            InitializeComponent();
            AssignIconsToSquares();
        }

        //get the start time of the game
        DateTime StartTimer = DateTime.Now;

        /// <summary>
        /// Assign each icon from the list of icons to a random square
        /// </summary>
        private void AssignIconsToSquares()
        {
            // The TableLayoutPanel has 16 labels,
            // and the icon list has 16 icons,
            // so an icon is pulled at random from the list
            // and added to each label
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    int randomNumber = random.Next(icons.Count);
                    iconLabel.Text = icons[randomNumber];
                    iconLabel.ForeColor = iconLabel.BackColor;
                    icons.RemoveAt(randomNumber);
                }
            }
        }

        /// <summary>
        /// Every label's Click event is handled by this event handler
        /// </summary>
        /// <param name="sender">The label that was clicked</param>
        /// <param name="e"></param>
        private void label_Click(object sender, EventArgs e)
        {
            // The timer is only on after two non-matching 
            // icons have been shown to the player, 
            // so ignore any clicks if the timer is running
            if (timer1.Enabled == true)
                return;

            Label clickedLabel = sender as Label;

            // If secondClicked is not null, the player has already 
            // clicked twice and the game has not yet reset --
            // ignore the click
            if (secondClicked != null)
                return;

            if (clickedLabel != null)
            {
                // If the clicked label is black, the player clicked
                // an icon that's already been revealed --
                // ignore the click
                if (clickedLabel.ForeColor == Color.Black)
                    return;

                // If firstClicked is null, this is the first icon
                // in the pair that the player clicked, 
                // so set firstClicked to the label that the player 
                // clicked, change its color to black, and return
                if (firstClicked == null)
                {
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.Black;
                    return;
                }

                // If the player gets this far, the timer isn't
                // running and firstClicked isn't null,
                // so this must be the second icon the player clicked
                // Set its color to black
                secondClicked = clickedLabel;
                secondClicked.ForeColor = Color.Black;

                // Check to see if the player won
                CheckForWinner();

                // If the player clicked two matching icons, keep them 
                // black and reset firstClicked and secondClicked 
                // so the player can click another icon
                if (firstClicked.Text == secondClicked.Text)
                {
                    firstClicked = null;
                    secondClicked = null;
                    return;
                }

                // If the player gets this far, the player 
                // clicked two different icons, so start the 
                // timer (which will wait three quarters of 
                // a second, and then hide the icons)
                timer1.Start();
            }
        }

        /// <summary>
        /// Check every icon to see if it is matched, by 
        /// comparing its foreground color to its background color. 
        /// If all of the icons are matched, the player wins
        /// </summary>
        private void CheckForWinner()
        {
            // Go through all of the labels in the TableLayoutPanel, 
            // checking each one to see if its icon is matched
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;

                if (iconLabel != null)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                        return;
                }
            }

            // If the loop didn't return, it didn't find
            // any unmatched icons
            // That means the user won. Show a message and close the form
            DateTime endTimer = DateTime.Now;
            TimeSpan elapsed = endTimer - StartTimer;

            //format into a comparable float
            int mins = (elapsed.Minutes);
            //convert into seconds
            mins = mins * 60;
            float decimals = (elapsed.Milliseconds);
            decimals = decimals/ 1000;
            float UserTime = mins + elapsed.Seconds + decimals;

            MessageBox.Show($"Congratulations {UserName}, You matched all the icons in {UserTime} seconds! Congratulations");
            CheckLeaderBoard(UserName, UserTime);
            Close();
        }

        private void CheckLeaderBoard(string Username, float UserTime)
        //read text from leaderboard
        {
            try
            { 
                //define the Leaderboard nested list
                List<List<string>> LeaderBoard = new List<List<string>>();
                //import the existing list from the csv file
                List<string> leaderboardFile = new List<string>(File.ReadAllLines("leaderboard.csv"));
                //remove all white space
                for (int i = 0; i < leaderboardFile.Count -1; i++)
                {
                    leaderboardFile[i].Trim();
                }
                foreach (string line in leaderboardFile)
                {
                    if (line != "")
                    {
                        string[] words = line.Split(',');
                        //add the player name and score as an element of the leaderboard
                        List<string> element = new List<string>();
                        element.Add(words[0]);
                        element.Add(words[1]);
                        LeaderBoard.Add(element);
                    }
                }

                //work out whether or not the new time is a new leaderboard score
                //count will be used to index the insert for position in Leaderboard
                int count = 0;
                //loop exits if position found and inserted, or if count exceeds leaderboard positions
                while (count < 10)
                {
                    //compare the leaderboard time to user time
                    string element = LeaderBoard[count][1];
                    float existingTime = float.Parse(element);
                    if (existingTime > UserTime)
                    {
                        //the users time needs adding to the leaderboard in the correct position
                        //create an element based on the user's new score to be inserted into LeaderBoard
                        List<string> UserLeaderBoard = new List<string>();
                        UserLeaderBoard.Add(Username);
                        UserLeaderBoard.Add(UserTime.ToString());

                        // we need to insert UserLeaderBoard into LeaderBoard
                        // then shuffle all items on and delete any that exceed index 9
                        LeaderBoard.Insert(count, UserLeaderBoard);
                        //remove the final item which will exceed the top ten leaderboard (item #11)
                        LeaderBoard.RemoveAt(10);
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }

                //write the new leaderboard back to the file, with user inserted if needed
                string ToWrite = "";
                using (StreamWriter file = new StreamWriter("leaderboard.csv"))
                {
                    int counter = 1;
                    foreach (List<string> element in LeaderBoard)
                    {
                        ForOutput += $"position {counter}:{element[0]},{element[1]}\r";
                        ToWrite += $"{element[0]},{element[1]}\r";
                        counter++;
                    }
                    file.WriteLine(ToWrite);
                }

                //display the new leaderboard to a message box
                // {Environment.NewLine}
                MessageBox.Show($"leaderboard:\r{ForOutput}");
            }
            catch (IOException e)
            {
                MessageBox.Show("The file could not be read", e.Message);
            }
        }
    
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            // Stop the timer
            timer1.Stop();

            // Hide both icons
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            // Reset firstClicked and secondClicked 
            // so the next time a label is
            // clicked, the program knows it's the first click
            firstClicked = null;
            secondClicked = null;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Owner = this;
            f.ShowDialog();
        }
    }
}