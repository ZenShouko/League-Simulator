using LeagueClassLibrary.DataAccess;
using LeagueClassLibrary.Entities;
using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Match = LeagueClassLibrary.Entities.Match;

namespace LeagueSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string padNaarChampions = @"C:\\Users\\emre_\\Desktop\\League Simulator\\LeagueSimulator\\csv\\leagueOfLegendsChampions.csv";
        string padNaarAbilities = @"C:\Users\emre_\Desktop\League Simulator\LeagueSimulator\csv\leagueOfLegendsAbilities.csv";
        private Match currentMatch;
        public MainWindow()
        {
            InitializeComponent();

            ComboBoxPositions.Items.Add("Any");
            ComboBoxPositions.Items.Add("sup");
            ComboBoxPositions.Items.Add("mid");
            ComboBoxPositions.Items.Add("bot");
            ComboBoxPositions.Items.Add("jung");
            ComboBoxPositions.Items.Add("top");
            ComboBoxPositions.SelectedIndex = 0;
        }
        private void LaadChampionDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select a Champion data file";
            open.Filter = "CSV Files (*.csv)|*.csv";

            if (open.ShowDialog() == true)
            {
                string path = open.FileName;
                try
                {
                    ChampionData.LoadCsv(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Idiot!!");
                }

                CheckBoxLaadChampionData.IsChecked = true;
                MessageBox.Show($"{System.IO.Path.GetFileName(path)} succesfully loaded!", "OK");
                EnableTabsEnDataGridAlsDataGeladen();
            }
        }

        private void LaadAbilityDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select ability file";
            open.Filter = "CSV Files (*.csv)|*.csv";

            if (open.ShowDialog() == true)
            {
                try
                {
                    AbilityData.LoadCSV(open.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "IDIOT!!");
                }

                CheckBoxLaadAbilityData.IsChecked = true;
                MessageBox.Show($"{System.IO.Path.GetFileName(open.FileName)} loaded succesfully!", "OK");
                EnableTabsEnDataGridAlsDataGeladen();
            }
        }

        private void EnableTabsEnDataGridAlsDataGeladen()
        {
            //All files loaded?
            if (CheckBoxLaadAbilityData.IsChecked == false || CheckBoxLaadChampionData.IsChecked == false)
            { return; }

            //Initialize matchdata
            MatchData.InitializeDataTableMatches();

            //Enable tabs
            TabItemSimuleerMatch.IsEnabled = true;
            TabItemOverzichtMatches.IsEnabled = true;

            //Datagrid's itemsource
            DataGridChampions.ItemsSource = ChampionData.GetDataView();
        }


        private async void ComboBoxPositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(1);

            DataGridChampions.ItemsSource = ComboBoxPositions.SelectedIndex == 0 ? 
                ChampionData.GetDataView() : ChampionData.GetDataViewChampionsByPosition(ComboBoxPositions.Text);
        }

        private void BestToWorstButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridChampions.ItemsSource = ChampionData.GetDataViewChampionsBestToWorst();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridChampions.ItemsSource = ChampionData.GetDataView();
        }

        private void DataGridChampions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get selected row
            if (DataGridChampions.SelectedItem is DataRowView selectedRow)
            {
                string nameTitle = selectedRow["ChampionName"].ToString() + ", " + selectedRow["ChampionTitle"].ToString();
                string iconPath = selectedRow["ChampionIcon"].ToString();

                //Assign
                TextBlockChampionTitle.Text = nameTitle;
                ImageChampion.Source = new BitmapImage(new Uri(@"images/" + iconPath, UriKind.Relative));
            }
        }

        private void LaadChampion(int indexChampion, int team)
        {
            //Do not execute if teams don't exist
            if (currentMatch is null)
            {
                return;
            }

            //Return if index if bigger than team count
            if (indexChampion > currentMatch.Team1Champions.Count - 1 || 
                indexChampion > currentMatch.Team2Champions.Count - 1)
            {
                return;
            }

            if (team == 1)
            {
                //Banner
                ImageBanner.Source = new BitmapImage(new Uri("images/" + 
                    currentMatch.Team1Champions[indexChampion].BannerSource, UriKind.Relative));

                //Name & Title
                TextBlockChampion.Text = currentMatch.Team1Champions[indexChampion].Name + ", " +
                    currentMatch.Team1Champions[indexChampion].Title;

                //Class
                TextBlockClass.Text = currentMatch.Team1Champions[indexChampion].Class;

                //RP/IP
                TextBlockCost.Text = $"RP:{currentMatch.Team1Champions[indexChampion].CostRP} / " +
                    $"IP:{currentMatch.Team1Champions[indexChampion].CostIP}";

                //Abilities
                ListBoxChampionAbilities.Items.Clear();
                foreach (var ability in currentMatch.Team1Champions[indexChampion].Abilities)
                {
                    ListBoxChampionAbilities.Items.Add(ability);
                }
            }
            else
            {
                //Banner
                ImageBanner.Source = new BitmapImage(new Uri("images/" +
                    currentMatch.Team2Champions[indexChampion].BannerSource, UriKind.Relative));

                //Name & Title
                TextBlockChampion.Text = currentMatch.Team2Champions[indexChampion].Name + ", " +
                    currentMatch.Team2Champions[indexChampion].Title;

                //Class
                TextBlockClass.Text = currentMatch.Team1Champions[indexChampion].Class;

                //RP/IP
                TextBlockCost.Text = $"RP:{currentMatch.Team2Champions[indexChampion].CostRP} / " +
                    $"IP:{currentMatch.Team2Champions[indexChampion].CostIP}";

                //Abilities
                ListBoxChampionAbilities.Items.Clear();
                foreach (var ability in currentMatch.Team2Champions[indexChampion].Abilities)
                {
                    ListBoxChampionAbilities.Items.Add(ability);
                }
            }
        }

        #region imageMouseEnter
        private void ImageIconChampion1Team1_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(0, 1);
        }

        private void ImageIconChampion2Team1_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(1, 1);
        }

        private void ImageIconChampion3Team1_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(2, 1);
        }

        private void ImageIconChampion4Team1_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(3, 1);
        }

        private void ImageIconChampion5Team1_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(4, 1);
        }

        private void ImageIconChampion1Team2_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(0, 2);
        }

        private void ImageIconChampion2Team2_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(1, 2);
        }

        private void ImageIconChampion3Team2_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(2, 2);
        }

        private void ImageIconChampion4Team2_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(3, 2);
        }

        private void ImageIconChampion5Team2_MouseEnter(object sender, MouseEventArgs e)
        {
            LaadChampion(4, 2);
        }
        #endregion

        private void Genereer5v5Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "bruh"); 
                return;
            }

            currentMatch = new SummonersRift(PasswordBoxMatchCode.Password);
            ((SummonersRift)currentMatch).GenereerTeams();

            ClearChampionSpecificGUI();
            LoadChampionIcons();
        }

        private void Genereer3v3Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "bruh");
                return;
            }

            currentMatch = new TwistedTreeline(PasswordBoxMatchCode.Password);
            ((TwistedTreeline)currentMatch).GenereerTeams();

            ClearChampionSpecificGUI();
            LoadChampionIcons();
        }

        private void ClearChampionSpecificGUI()
        {
            ImageBanner.Source = null;
            TextBlockChampion.Text = null;
            TextBlockClass.Text = null;
            TextBlockCost.Text = null;
            ListBoxChampionAbilities.Items.Clear();
        }

        private void LoadChampionIcons()
        {
            //Team 1
            int index = 0;
            foreach (Image championImage in GridTeam1.Children)
            {
                //Default image if not enough champions
                if (index > currentMatch.Team1Champions.Count - 1)
                {
                    championImage.Source = new BitmapImage(new Uri(@"images/icons/empty_icon.png", UriKind.Relative));
                    continue;
                }

                championImage.Source = new BitmapImage(new Uri("images/" +
                    currentMatch.Team1Champions[index].IconSource, UriKind.Relative));
                index++;
            }

            //Team 2
            index = 0;
            foreach (Image championImage in GridTeam2.Children)
            {
                //Default image if not enough champions
                if (index > currentMatch.Team2Champions.Count - 1)
                {
                    championImage.Source = new BitmapImage(new Uri(@"images/icons/empty_icon.png", UriKind.Relative));
                    continue;
                }

                championImage.Source = new BitmapImage(new Uri("images/" +
                    currentMatch.Team2Champions[index].IconSource, UriKind.Relative));
                index++;
            }
        }

        private void CheckCode()
        {
            //Gooit een exception als code niet geldig is. Anders niks
            if (string.IsNullOrEmpty(PasswordBoxMatchCode.Password))
            {
                throw new Exception("Moet een code meegeven!!");
            }
            else if (!MatchData.IsUniqueCode(PasswordBoxMatchCode.Password))
            {
                throw new Exception("Code bestaat al lmao");   
            }
        }

        private void ExportToXMLButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Matches.xml";
            save.Filter = "XML Files (*.xml)|*.xml";

            if (save.ShowDialog() == true)
            {
                string folderPath = System.IO.Path.GetDirectoryName(save.FileName);
                string filePath = System.IO.Path.Combine(folderPath, "Matches.xml");
                try
                {
                    MatchData.ExportToXML(filePath);
                    MessageBox.Show("Ik heb goed nieuws voor je jongenman C:");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BeslisWinnaarButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentMatch is null) { return; }
            try
            {
                currentMatch.DecideWinner();
                MatchData.AddFinishedMatch(currentMatch);
                ClearSimulatieTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ClearSimulatieTab()
        {
            ClearChampionSpecificGUI();

            //Clear all images
            foreach (Image img in GridTeam1.Children)
            {
                img.Source = new BitmapImage(new Uri(@"images/icons/empty_icon.png", UriKind.Relative));
            }

            foreach (Image img in GridTeam2.Children)
            {
                img.Source = new BitmapImage(new Uri(@"images/icons/empty_icon.png", UriKind.Relative));
            }

            //Clear vars
            currentMatch = null;
        }

        private void TabItemOverzichtMatches_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataGridMatches.ItemsSource = MatchData.DataTableMatches is null ? null : MatchData.GetDataViewMatches();
        }
    }
}
