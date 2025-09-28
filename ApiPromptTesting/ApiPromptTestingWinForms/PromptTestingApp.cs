using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApiPromptTestingWinForms;

public partial class PromptTestingApp : Form
{
    // API endpoint and key are retrieved from User Secrets or appsettings.json
    private readonly string _apiKey;
    private readonly string _apiUrl;

    private readonly Button _saveButton = new();
    private readonly Button _sendButton = new();
    private readonly TextBox _responseTextBox = new();
    private readonly TextBox _systemPromptTextBox = new();
    private readonly TextBox _userPromptTextBox = new();
    
    public PromptTestingApp()
    {
        InitializeComponent();

        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .Build();

            _apiKey = config["Perplexity:ApiKey"] ?? throw new InvalidOperationException("Perplexity API key is not set.");
            _apiUrl = config["Perplexity:ApiUrl"] ?? throw new InvalidOperationException("API URL not found in config.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }

    private async Task CallApiAsync()
    {
        _responseTextBox.Text = "Calling API...";

        try
        {
            var requestBody = new
            {
                model = "sonar",
                messages = new[]
                {
                    new { role = "system", content = _systemPromptTextBox.Text },
                    new { role = "user", content = _userPromptTextBox.Text }
                },
                web_search_options = new { search_context_size = "low" },
                max_tokens = 1800,
                temperature = 0.3 // Perplexity recommends not changing this from 0.3 
            };

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);
            var responseBody = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();

            _responseTextBox.Text = responseBody;
        }
        catch (Exception ex)
        {
            _responseTextBox.Text = $"Error: {ex.Message}";
        }
    }

    private void SaveResponse(object? sender, EventArgs e)
    {
        try
        {
            SaveFileDialog saveDialog = new()
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = $"api_response_{DateTime.Now:yyyyMMddHHmmss}.txt"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveDialog.FileName, _responseTextBox.Text);
                MessageBox.Show("Response saved!");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Form
        this.Icon = Resources.AppIcon;
        this.Size = new Size(800, 600);
        this.Text = "Sonar API Prompt Tester";
        
        // System Prompt
        _systemPromptTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _systemPromptTextBox.Bounds = new Rectangle(10, 35, 760, 100);
        _systemPromptTextBox.Multiline = true;
        _systemPromptTextBox.ScrollBars = ScrollBars.Vertical;

        // User Prompt
        _userPromptTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _userPromptTextBox.Bounds = new Rectangle(10, 175, 760, 150);
        _userPromptTextBox.Multiline = true;
        _userPromptTextBox.ScrollBars = ScrollBars.Vertical;

        // Response
        _responseTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _responseTextBox.Bounds = new Rectangle(10, 365, 760, 150);
        _responseTextBox.Multiline = true;
        _responseTextBox.ReadOnly = true;
        _responseTextBox.ScrollBars = ScrollBars.Vertical;

        // Save Button
        _saveButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        _saveButton.Bounds = new Rectangle(140, 525, 120, 30);
        _saveButton.Text = "Save Response";
        _saveButton.Click += SaveResponse;

        // Send Button
        _sendButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        _sendButton.Bounds = new Rectangle(10, 525, 120, 30);
        _sendButton.Text = "Send to API";
        _sendButton.Click += async (sender, args) => await CallApiAsync();

        // Add controls to the form
        this.Controls.AddRange([
            new Label { Text = "System Prompt:", Bounds = new(x: 10, y: 10, width: 100, height: 20) },
            new Label { Text = "User Prompt:", Bounds = new(x: 10, y: 150, width: 100, height: 20) },
            new Label { Text = "API Response:", Bounds = new(x: 10, y: 340, width: 100, height: 20) },
            _responseTextBox, _systemPromptTextBox, _userPromptTextBox, 
            _saveButton, _sendButton
        ]);

        ResumeLayout(false);
        PerformLayout();
    }
}
