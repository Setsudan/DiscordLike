using System.Windows;

internal class InputDialog : Window {
    private string v;
    public string ResponseText {
        get; private set;
    }

    public InputDialog(string v) {
        this.v = v;
        // Initialize components and set up the dialog UI here
    }

    public bool? ShowDialog() {
     
        this.ShowDialog();
        return true; // or false based on user input
    }
}
