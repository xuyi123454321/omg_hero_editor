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
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace OMGHero
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        // List<AbilityData> AbilityDataList = new List<AbilityData>();
        public static Dictionary<string, AbilityData> AbilityDataDict = 
            JsonConvert.DeserializeObject<Dictionary<string, AbilityData>>(File.ReadAllText("HeroAbilities.json"));

        List<HeroData> HeroDataList = new List<HeroData>();
        public string DotaPath;
        public static bool PathSet { get; private set; }

        DotaNpcHeroes omg;

        public MainWindow()
        {
            InitializeComponent();

            string[] HeroNames = (string[])FindResource("HeroNames");
            string[] HeroThumbs = (string[])FindResource("HeroThumbs");

            for (int i = 0; i < HeroNames.Length; i++)
            {
                HeroDataList.Add(new HeroData { ID = 1, Name = HeroNames[i], Thumb = HeroThumbs[i] });
            }

            comboBox.ItemsSource = HeroDataList;
            comboBox2.ItemsSource = HeroDataList;
            comboBox3.ItemsSource = HeroDataList;
            comboBox4.ItemsSource = HeroDataList;
            comboBox5.ItemsSource = HeroDataList;

            if (!(bool)checkBox2.IsChecked)
            {
                comboBox9.IsEnabled = false;
            }


            PathSet = false;
            // Read Dota Path from Registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\OMG", false);
            if (key != null)
            {
                DotaPath = key.GetValue("DotaPath").ToString();
                textBox.Text = DotaPath;
                if (File.Exists(DotaPath + @"\game\dota\scripts\npc\npc_heroes.txt"))
                {
                    PathSet = true;
                }
                else
                {
                    MessageBox.Show(@"未在当前目录找到game\dota\scripts\npc文件");
                }
                key.Close();
            }

            omg = new DotaNpcHeroes(DotaPath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!PathSet)
            {
                MessageBox.Show("请先设置文件位置");
                return;
            }
            else if (omg.IfRunning == false)
            {
                omg.StartOMG(DotaPath);
            }

            // check comboBox data selected
            if (comboBox.SelectedIndex > -1 &&
                comboBox2.SelectedIndex > -1 &&
                comboBox3.SelectedIndex > -1 &&
                comboBox4.SelectedIndex > -1 &&
                comboBox5.SelectedIndex > -1 &&
                comboBox6.SelectedIndex > -1 &&
                comboBox7.SelectedIndex > -1 &&
                comboBox8.SelectedIndex > -1 &&
                comboBox9.SelectedIndex > -1)
            {
                // check ultimate skill number
                int n = 0;
                List<string[]> Abilities = new List<string[]> ();

                if (comboBox6.Text == "大招")
                {
                    n++;
                }
                if (comboBox7.Text == "大招")
                {
                    n++;
                }
                if (comboBox8.Text == "大招")
                {
                    n++;
                }
                if (comboBox9.Text == "大招")
                {
                    n++;
                }

                if (n == 0 && (!(bool)checkBox2.IsChecked))
                {
                    MessageBox.Show("请选择一个大招");
                    return;
                }
                else if (n > 1 && (!(bool)checkBox2.IsChecked))
                {
                    MessageBox.Show("只能选择一个大招");
                    return;
                }

                Dictionary<string, string> cbtemp = new Dictionary<string, string> {
                    { "1技能", "1" },
                    { "2技能", "2" },
                    { "3技能", "3" } };

                if (comboBox6.Text != "大招")
                {
                    HeroData item = (HeroData)comboBox2.SelectedItem;
                    Abilities.Add(new string[] { item.Name, cbtemp[comboBox6.Text] });
                }
                if (comboBox7.Text != "大招")
                {
                    HeroData item = (HeroData)comboBox3.SelectedItem;
                    Abilities.Add(new string[] { item.Name, cbtemp[comboBox7.Text] });
                }
                if (comboBox8.Text != "大招")
                {
                    HeroData item = (HeroData)comboBox4.SelectedItem;
                    Abilities.Add(new string[] { item.Name, cbtemp[comboBox8.Text] });
                }
                if (comboBox9.Text != "大招")
                {
                    HeroData item = (HeroData)comboBox5.SelectedItem;
                    Abilities.Add(new string[] { item.Name, cbtemp[comboBox9.Text] });
                }


                if (comboBox6.Text == "大招")
                {
                    HeroData item = (HeroData)comboBox2.SelectedItem;
                    Abilities.Add(new string[] { item.Name, "6" });
                }
                if (comboBox7.Text == "大招")
                {
                    HeroData item = (HeroData)comboBox3.SelectedItem;
                    Abilities.Add(new string[] { item.Name, "6" });
                }
                if (comboBox8.Text == "大招")
                {
                    HeroData item = (HeroData)comboBox4.SelectedItem;
                    Abilities.Add(new string[] { item.Name, "6" });
                }
                if (comboBox9.Text == "大招")
                {
                    HeroData item = (HeroData)comboBox5.SelectedItem;
                    Abilities.Add(new string[] { item.Name, "6" });
                }


                // check ability repeart
                for (int i = 0; i < 4; i++)
                {
                    for (int j = i + 1; j < 4; j++)
                    {
                        if (String.Equals(Abilities[i][0], Abilities[j][0]) &&
                            String.Equals(Abilities[i][1], Abilities[j][1]))
                        {
                            MessageBox.Show("技能重复");
                            return;
                        }
                    }
                }

                HeroData modelitem = (HeroData)comboBox.SelectedItem;
                var HeroName = modelitem.Name;

                
                if ((bool)checkBox.IsChecked)
                {
                    omg.AddNewHero(HeroName, Abilities, true);
                }
                else
                {
                    omg.AddNewHero(HeroName, Abilities);
                }

            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if ( result == System.Windows.Forms.DialogResult.OK )
            {
                var pathname = dialog.SelectedPath;
                textBox.Text = pathname;

                if (File.Exists(pathname + @"\game\dota\scripts\npc\npc_heroes.txt") 
                    && !String.Equals(DotaPath, pathname))
                {
                    DotaPath = pathname;

                    if (omg.IfRunning) // restart OMG to a different path
                    {
                        omg.CleanOMG();
                        omg.WriteConfig();
                        omg.StartOMG(DotaPath);
                    }

                    PathSet = true;

                    // Edit Registry to Store Path
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\OMG");
                    if (key != null)
                    {
                        key.SetValue("DotaPath", pathname);
                        key.Close();
                    }
                    else
                    {
                        key = Registry.CurrentUser.CreateSubKey(@"Software\OMG");
                        key.SetValue("DotaPath", pathname);
                        key.Close();
                    }

                }
                else
                {
                    MessageBox.Show(@"未在当前目录找到game\dota\scripts\npc文件");
                }

            }
        }

        private void CheckBox2_Checked(object sender, RoutedEventArgs e)
        {
            comboBox9.IsEnabled = true;
            comboBox6.ItemsSource = (string[])FindResource("HeroAbilities");
            comboBox7.ItemsSource = (string[])FindResource("HeroAbilities");
            comboBox8.ItemsSource = (string[])FindResource("HeroAbilities");
        }

        private void CheckBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox9.SelectedIndex = 3;
            comboBox9.IsEnabled = false;

            comboBox6.ItemsSource = (string[])FindResource("HeroNormalAbilities");
            comboBox7.ItemsSource = (string[])FindResource("HeroNormalAbilities");
            comboBox8.ItemsSource = (string[])FindResource("HeroNormalAbilities");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            comboBox.SelectedIndex = rand.Next(0, HeroDataList.Count);
            comboBox2.SelectedIndex = rand.Next(0, HeroDataList.Count);
            comboBox3.SelectedIndex = rand.Next(0, HeroDataList.Count);
            comboBox4.SelectedIndex = rand.Next(0, HeroDataList.Count);
            comboBox5.SelectedIndex = rand.Next(0, HeroDataList.Count);


            if ((bool)checkBox2.IsChecked)
            {
                comboBox9.SelectedIndex = rand.Next(0, 4);
                comboBox6.SelectedIndex = rand.Next(0, 4);
                comboBox7.SelectedIndex = rand.Next(0, 4);
                comboBox8.SelectedIndex = rand.Next(0, 4);
            }
            else
            {
                comboBox6.SelectedIndex = rand.Next(0, 3);
                comboBox7.SelectedIndex = rand.Next(0, 3);
                comboBox8.SelectedIndex = rand.Next(0, 3);
            }
        }


    public class HeroData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Thumb { get; set; }
    }

    public class AbilityData
    {
        public string tag { get; set; }
        public Dictionary<string, List<string>> abilities { get; set; }
    }

    public class DotaNpcHeroes
    {
        private string HeroConfigPath;
        // private FileStream HeroConfigFS;
        private List<string> HeroConfigLines = new List<string>();

        private Regex HeroTagRx = new Regex(@"^\t""(?<tag>npc_dota_hero_\w+)""");

        private List<string> RemovedHero = new List<string>();

        public bool IfRunning { get; private set; }

        public DotaNpcHeroes(string DotaPath)
        {
            HeroConfigPath = DotaPath + @"\game\dota\scripts\npc\npc_heroes.txt";
            IfRunning = false;
            if (MainWindow.PathSet)
            {
                StartOMG(DotaPath);
            }
        }

        public void StartOMG(string DotaPath)
        {
            // HeroConfigLines = new List<string>(File.ReadAllLines(HeroConfigPath));
            HeroConfigPath = DotaPath + @"\game\dota\scripts\npc\npc_heroes.txt";
            HeroConfigLines = new List<string>(File.ReadAllLines(HeroConfigPath));
            // add OMG flag for cleanup later
            for (int i = HeroConfigLines.Count - 1; i > 0; i--)
            {
                if (String.Equals(HeroConfigLines[i], "}"))
                {
                    HeroConfigLines.Insert(i, "// End OMG");
                    HeroConfigLines.Insert(i, "// Begin OMG");
                }

            }

            IfRunning = true;
        }

        
        ~DotaNpcHeroes()
        {
            if (MainWindow.PathSet)
            {
                foreach (var Hero in RemovedHero)
                {
                    RestoreHero(Hero);
                }

                CleanOMG();

                WriteConfig();
            }
        }
        

        public void CleanOMG()
        {
            for (int i = HeroConfigLines.Count - 1; i >= 0; i--)
            {
                if (String.Equals(HeroConfigLines[i], "// Begin OMG"))
                {
                    while (!String.Equals(HeroConfigLines[i], "// End OMG"))
                    {
                        HeroConfigLines.RemoveAt(i);
                    }
                    HeroConfigLines.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveHero(string HeroName)
        {
            string tag;
            if (HeroName == "OMG")
            {
                tag = "npc_dota_hero_omg";
            }
            else
            {
                tag = MainWindow.AbilityDataDict[HeroName].tag;
            }

            Regex HeroTagRx = new Regex(String.Format(@"^\t""(?<tag>{0})""", tag));

            var flag = false;
            var omghero = false;
            for(int i = 0; i < HeroConfigLines.Count; i ++)
            {
                var line = HeroConfigLines[i];
                if (String.Equals(line, "// Begin OMG")) // hero in OMG region
                {
                    omghero = true;
                }

                if (HeroTagRx.IsMatch(line))
                {
                    flag = true;
                }

                if (flag && omghero)
                {
                    HeroConfigLines.RemoveAt(i);
                    i--;
                }
                else if(flag)
                {
                    HeroConfigLines[i] = "//" + line;
                }

                if (flag && String.Equals(line, "\t}"))
                {
                    break;
                }
            }

        }

        public void RestoreHero(string HeroName)
        {
            string tag;
            tag = MainWindow.AbilityDataDict[HeroName].tag;

            Regex RemovedHeroTagRx = new Regex(String.Format(@"^//\t""(?<tag>{0})""", tag));

            var flag = false;
            for (int i = 0; i < HeroConfigLines.Count; i++)
            {
                var line = HeroConfigLines[i];
                if (RemovedHeroTagRx.IsMatch(line))
                {
                    flag = true;
                }

                if (flag)
                {
                    HeroConfigLines[i] = line.Substring(2);
                }

                if (flag && String.Equals(line, "//\t}"))
                {
                    break;
                }
            }

        }

        private string[] FindHeroConfig(string HeroName)
        {
            string tag = MainWindow.AbilityDataDict[HeroName].tag;
            Regex HeroTagRx = new Regex(String.Format(@"^\t""(?<tag>{0})""", tag));
            // Regex HeroTagRx = new Regex(String.Format("{0}", tag));
            List<string> HeroConfig = new List<string>();

            var flag = false;
            foreach (var line in HeroConfigLines)
            {
                if (HeroTagRx.IsMatch(line))
                {
                    flag = true;
                }

                if (flag)
                {
                    HeroConfig.Add(line);
                }

                if (flag && String.Equals(line, "\t}"))
                {
                    break;
                }
            }

            return HeroConfig.ToArray();
        }

        private string[] ModifyAbility(string[] HeroConfig, List<string[]> NewAbilities)
        {
            string HeroTag = HeroTagRx.Match(HeroConfig[0]).Groups["tag"].Value;
            Dictionary<string, string> AbilitySlot = new Dictionary<string, string>();
            List<string> MainSlot = new List<string> { "1", "2", "3", "6" }; // reserved for new abilities
            List<string> NewConfig = new List<string>();

            Regex rx = new Regex(@"^\t\t""Ability(?<slot>\d+)""\t+""(?<content>.+)""", RegexOptions.Compiled);
            // Regex rx2 = new Regex("generic_hidden", RegexOptions.Compiled);
            foreach (var line in HeroConfig)
            {
                if (rx.IsMatch(line))
                {
                    Match m = rx.Match(line);
                    var slot = m.Groups["slot"].Value; // ability slot
                    var ability = m.Groups["content"].Value; // ability content
                    if ((!MainSlot.Contains(slot)) && (!String.Equals(ability, "generic_hidden")))
                    {
                        AbilitySlot.Add(slot, ability); // add model ability including talents
                    }
                }
                else if(!String.Equals(line, "\t}"))
                {
                    NewConfig.Add(line);
                }
            }

            // add new ability
            var slotn = 1;
            foreach (string[] pair in NewAbilities)
            {
                AbilityData data = MainWindow.AbilityDataDict[pair[0]];
                var abilities = data.abilities[pair[1]];




                if (!String.Equals(HeroTag, data.tag)) // not this hero's own ability
                {
                    for (int i = 1; i < abilities.Count; i++)
                    {
                        for (int j = 4; j < 25; j++) // invoker has 24 ability slots
                        {
                            if (j == 6 || AbilitySlot.ContainsKey(j.ToString()))
                            {
                                continue;
                            }

                            AbilitySlot.Add(j.ToString(), abilities[i]);
                            break;
                        }

                        if (!AbilitySlot.ContainsValue(abilities[i]))
                        {
                            MessageBox.Show("出错了！技能槽已满");
                            break;
                        }
                    }
                }

                // add "generic hidden" to slot 4 and 5 before ulti
                if (slotn == 6)
                {
                    if (!AbilitySlot.ContainsKey("4"))
                    {
                        AbilitySlot.Add("4", "generic_hiddern");
                    }
                    if (!AbilitySlot.ContainsKey("5"))
                    {
                        AbilitySlot.Add("5", "generic_hiddern");
                    }
                }

                AbilitySlot.Add(slotn.ToString(), abilities[0]); // add main ability last after "generic hidder"

                slotn++;
                if(slotn == 4)
                {
                    slotn = 6; // move to ulti
                }
            }

            // add "generic hidden" to slot 4 and 5
            if (!AbilitySlot.ContainsKey("4"))
            {
                AbilitySlot.Add("4", "generic_hiddern");
            }
            if (!AbilitySlot.ContainsKey("5"))
            {
                AbilitySlot.Add("5", "generic_hiddern");
            }

            foreach (KeyValuePair<string, string> pair in AbilitySlot)
            {
                NewConfig.Add(String.Format("\t\t\"Ability{0}\"\t\t\"{1}\"",
                                            pair.Key, pair.Value));
            }
            NewConfig.Add("\t}");

            // TODO: exceptions: morphling nevermore troll invoker ...

            return NewConfig.ToArray();
        }

        public void AddNewHero(string HeroName, List<string[]> Abilities, bool replace = false)
        {
            if (RemovedHero.Contains(HeroName)) // modify a hero again
            {
                RemoveHero(HeroName); // remove omg hero
                RestoreHero(HeroName); // restore original hero
            }

            string[] HeroConfig = FindHeroConfig(HeroName);
            string[] NewConfig = ModifyAbility(HeroConfig, Abilities);

            if (replace)
            {
                RemoveHero(HeroName);
                RemovedHero.Add(HeroName);
            }
            else
            {
                RemoveHero("OMG");
                NewConfig[0] = "\t\"npc_dota_hero_omg\"";
            }

            for (int i = HeroConfigLines.Count - 1; i > 0; i --)
            {
                if (String.Equals(HeroConfigLines[i], "// Begin OMG"))
                {
                    for (int j = NewConfig.Length - 1; j >= 0; j --)
                    {
                        HeroConfigLines.Insert(i + 1, NewConfig[j]);
                    }
                }
            }

            WriteConfig();
        }

        public void WriteConfig()
        {
            File.WriteAllLines(HeroConfigPath, HeroConfigLines);
        }
    }
}
