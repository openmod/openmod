using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenMod.Unturned.PluginWizard
{
    /// <summary>
    /// Interaction logic for UserInputControl.xaml
    /// </summary>
    public partial class UserInputControl : UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                "Label",
                typeof(string),
                typeof(UserInputControl),
                new FrameworkPropertyMetadata("Unnamed Label"));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(UserInputControl),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private bool m_InputSpecified;
        private string m_DefaultText;

        public string GetInput() => m_InputSpecified ? Text : null!;

        public UserInputControl()
        {
            InitializeComponent();
            DataContext = this;

            m_InputSpecified = false;
            m_DefaultText = "";
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (m_InputSpecified || sender is not TextBox textBox) return;

            m_DefaultText = textBox.Text;

            textBox.Opacity = 1;
            textBox.Text = "";
        }

        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            if (textBox.Text != "")
            {
                m_InputSpecified = true;
            }
            else
            {
                m_InputSpecified = false;

                textBox.Opacity = 0.6;
                textBox.Text = m_DefaultText;
            }
        }
    }
}
