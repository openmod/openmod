using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace OpenMod.Unturned.PluginWizard
{
    /// <summary>
    /// Interaction logic for UserInputWindow.xaml
    /// </summary>
    public partial class UserInputWindow : Window
    {
        public UserInputWindow(Dictionary<string, string> replacements)
        {
            DataContext = this;

            // A code reference to MahApps.Metro is necessary
            var ex = new MahApps.Metro.MahAppsException();

            InitializeComponent();

            InputDisplayName.Text = replacements["$projectname$"];
        }

        public bool MoreOptions => MoreOptionsPanel.Visibility == Visibility.Visible;

        public void ApplyInputs(Dictionary<string, string> replacements)
        {
            void AddOrReplace(string key, string value, string fallback = "")
            {
                if (string.IsNullOrWhiteSpace(value))
                    value = fallback;

                if (replacements.ContainsKey(key))
                    replacements[key] = value;
                else
                    replacements.Add(key, value);
            }

            AddOrReplace("$displayname$", InputDisplayName.Text, replacements["$projectname$"]);
            AddOrReplace("$author$", InputAuthor.Text);

            AddOrReplace("$description$", InputDescription.Text, MoreOptions ? "An OpenMod Unturned plugin." : "");
            AddOrReplace("$packageid$", InputPackageId.Text, MoreOptions ? replacements["$safeprojectname$"] : "");
        }

        private void Continue_Click(object sender, RoutedEventArgs e) => Close();

        private double m_NormalHeight = 0;

        public const double MoreOptionsWindowHeight = 400;

        private void MoreOptions_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.Source is not Hyperlink hyperlink) return;

            if (MoreOptions)
            {
                Height = m_NormalHeight;

                MoreOptionsPanel.Visibility = Visibility.Collapsed;
                hyperlink.Inlines.Clear();
                hyperlink.Inlines.Add("More options...");
            }
            else
            {
                m_NormalHeight = Height;
                Height = MoreOptionsWindowHeight;

                MoreOptionsPanel.Visibility = Visibility.Visible;
                hyperlink.Inlines.Clear();
                hyperlink.Inlines.Add("Less options...");
            }
        }
    }
}
