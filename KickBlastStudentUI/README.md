# KickBlast Judo – Training Fee Management System

## Requirements
- Visual Studio 2022 (17.8 or newer)
- .NET 8 SDK with Desktop development workload
- Windows OS for WPF runtime

## How to open in Visual Studio
1. Open `KickBlastStudentUI.sln`.
2. Wait for NuGet restore.
3. Ensure startup project is `KickBlastStudentUI`.

## How to run
1. Choose **Debug** and **Any CPU**.
2. Press F5 / Start.
3. Login with:
   - Username: `rashmika`
   - Password: `123456`

## How to change prices
1. Go to **Settings** page in the app.
2. Update values and click **Save Pricing**.
3. Prices reload immediately in calculator and database plan fees.

## How to reset database
1. Close app.
2. Delete `KickBlastStudentUI/Data/kickblast_student.db`.
3. Run app again. It recreates DB with seed data.

## Notes
- SQLite is initialized with `EnsureCreated()` and sample data.
- Currency format is shown as `LKR 00,000.00`.
