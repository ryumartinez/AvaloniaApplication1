using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace AvaloniaApplication1.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Product> _products;

    [ObservableProperty]
    private string _statusMessage;

    public MainWindowViewModel()
    {
        Products = new ObservableCollection<Product>();
        StatusMessage = "Ready to load data.";
        // Automatically load data on startup
        Task.Run(LoadDataAsync);
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        StatusMessage = "Loading data...";
        
        using (var context = new AppDbContext())
        {
            // Ensure DB is created and seeded
            await context.Database.EnsureCreatedAsync();
            
            var data = await context.Products.ToListAsync();
            Products = new ObservableCollection<Product>(data);
        }

        StatusMessage = $"Loaded {Products.Count} items.";
    }

    [RelayCommand]
    public async Task ExportToCsvAsync()
    {
        if (Products.Count == 0)
        {
            StatusMessage = "No data to export.";
            return;
        }

        try
        {
            var csvContent = new StringBuilder();
            
            // 1. Add Header
            csvContent.AppendLine("Id,Name,Price,Stock");

            // 2. Add Rows
            foreach (var p in Products)
            {
                csvContent.AppendLine($"{p.Id},{EscapeCsv(p.Name)},{p.Price},{p.Stock}");
            }

            // 3. Write to file (Saving to Desktop for simplicity)
            string fileName = $"Products_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            await File.WriteAllTextAsync(filePath, csvContent.ToString());

            StatusMessage = $"Exported to Desktop: {fileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    // Helper to handle commas in data
    private string EscapeCsv(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}