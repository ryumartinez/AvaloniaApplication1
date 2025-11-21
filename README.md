# AvaloniaApplication1
This is a comprehensive documentation for the **SQLite Data Visualizer**, a desktop application built with **Avalonia UI** and **Entity Framework Core**.

## 1\. Project Overview

This application serves as a dashboard to visualize product inventory. It connects to a local SQLite database, displays data in a grid, and provides functionality to export that data to a CSV file.

**Key Features:**

* **MVVM Architecture:** Strict separation of logic (ViewModel) and UI (View).
* **Database:** Local SQLite database accessed via EF Core.
* **Data Binding:** Real-time UI updates using `ObservableCollection`.
* **Export:** CSV file generation saved to the User's Desktop.

-----

## 2\. Architecture & Data Flow

The application follows the Model-View-ViewModel (MVVM) pattern.

[Image of MVVM pattern architecture diagram]

1.  **Model (`Product.cs`):** Defines the data structure.
2.  **Data Layer (`AppDbContext`):** Fetches data from SQLite.
3.  **ViewModel (`MainWindowViewModel`):** Calls the Data Layer, holds the state, and defines Commands.
4.  **View (`MainWindow.axaml`):** Binds to the ViewModel to display data.

-----

## 3\. Directory Structure

Here is the breakdown of the project organization:

| Folder / File | Description |
| :--- | :--- |
| **`Data/`** | Contains Database Logic. |
| `└── AppDbContext.cs` | The EF Core context. Configures SQLite and seeds initial data. |
| **`Models/`** | Contains Plain Old CLR Objects (POCOs). |
| `└── Product.cs` | Represents a single row in the database (Id, Name, Price, Stock). |
| **`ViewModels/`** | Contains Application Logic. |
| `├── MainWindowViewModel.cs` | The "Brain" of the main screen. Handles loading and exporting. |
| `└── ViewModelBase.cs` | Base class inheriting from `ObservableObject` (CommunityToolkit). |
| **`Views/`** | Contains UI Definitions (XAML). |
| `├── MainWindow.axaml` | The visual layout (Grid, Buttons, DataGrid). |
| `└── MainWindow.axaml.cs` | Code-behind (mostly empty, follows MVVM best practices). |
| **`App.axaml`** | Application-level styles and theme definitions. |
| **`Program.cs`** | The entry point (`main`) of the application. |

-----

## 4\. Key Components Explained

### A. The Data Layer (`Data/AppDbContext.cs`)

Uses Entity Framework Core to manage the database.

* **Connection:** Uses `optionsBuilder.UseSqlite("Data Source=app.db")`. This creates a file named `app.db` in the build directory.
* **Seeding:** The `OnModelCreating` method automatically populates the database with 4 sample products (Laptop, Mouse, etc.) if it is empty.

### B. The Logic Layer (`ViewModels/MainWindowViewModel.cs`)

Uses `CommunityToolkit.Mvvm` to reduce boilerplate code.

* **`[ObservableProperty]`:** automatically generates `public Product Products { get; set; }` and handles `INotifyPropertyChanged` events.
    * `_products`: A collection that notifies the UI when items are added/removed.
    * `_statusMessage`: Updates the text at the bottom of the window (e.g., "Loading...", "Exported").
* **`[RelayCommand]`:** Turns methods into `ICommand` objects bindable in XAML.
    * `LoadDataAsync()`: Initializes the DB and fetches the list.
    * `ExportToCsvAsync()`: Iterates through `Products`, builds a CSV string, and saves it to `Environment.SpecialFolder.Desktop`.

### C. The UI Layer (`Views/MainWindow.axaml`)

Defines the visual structure using a Grid layout.

* **DataGrid:**
  ```xml
  <DataGrid ItemsSource="{Binding Products}" AutoGenerateColumns="True" ... />
  ```
  It automatically creates columns based on the properties of the `Product` class.
* **Command Binding:**
  Buttons are linked to methods in the ViewModel:
  ```xml
  <Button Command="{Binding LoadDataCommand}" ... />
  ```

### D. Critical Styling (`App.axaml`)

**Important Note:** The `DataGrid` control is not part of the core Avalonia library; it is a separate package. For it to render correctly, its theme must be explicitly included in `App.axaml`:

```xml
<StyleInclude Source="avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml"/>
```

*Without this line, the DataGrid would appear invisible or broken.*

-----

## 5\. Usage & Dependencies

### Nuget Packages Required

* `Avalonia`
* `Avalonia.Controls.DataGrid` (Crucial for the table view)
* `CommunityToolkit.Mvvm` (For ObservableProperty/RelayCommand)
* `Microsoft.EntityFrameworkCore.Sqlite`

### How to Run

1.  **Build** the solution.
2.  **Run** the application.
3.  On the first run, `EnsureCreatedAsync()` will trigger, creating `app.db` and seeding it.
4.  The list will automatically load (via the Constructor `Task.Run`).
5.  Click **"Export to CSV"** to generate a file on your Desktop.

-----

## 6\. Known Logic Details

* **CSV Escaping:** The `EscapeCsv` helper method handles edge cases where product names might contain commas or quotes (e.g., `24" Monitor` becomes `"24"" Monitor"`), ensuring the CSV format remains valid.
* **Validation Disabling:** In `App.axaml.cs`, `DisableAvaloniaDataAnnotationValidation()` is called to prevent conflicts between Avalonia's built-in validation and the CommunityToolkit's validation logic.