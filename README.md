# FoodyWebAppTestProject

FoodyWebAppTestProject is an automated test suite written in C# using NUnit and Selenium WebDriver. It is designed to test the functionality of the Foody Web Application, ensuring that key features like adding, editing, searching, and deleting food items work as expected.

## Technologies Used
- **C#**
- **NUnit** - Test framework
- **Selenium WebDriver** - Browser automation
- **ChromeDriver** - For running tests on Google Chrome

## Setup Instructions

### Prerequisites
Ensure you have the following installed:
- .NET SDK
- Chrome browser
- ChromeDriver (matching your Chrome version)
- NUnit & NUnit Console Runner
- Selenium WebDriver NuGet package

### Installation
1. Clone the repository:
   ```sh
   git clone <repository_url>
   cd FoodyWebAppTestProject
   ```
2. Restore NuGet packages:
   ```sh
   dotnet restore
   ```
3. Run the tests:
   ```sh
   dotnet test
   ```

## Test Scenarios
The test suite includes the following scenarios:

1. **Add Food with Invalid Data** - Ensures validation messages appear when submitting empty fields.
2. **Add Random Food Item** - Adds a food item with a randomly generated name and description.
3. **Edit Last Added Food** - Attempts to modify the last added food item.
4. **Search for Food Item** - Verifies that searching for a specific food title returns correct results.
5. **Delete Last Added Food** - Ensures a food item can be successfully deleted.
6. **Search for Deleted Food** - Confirms that deleted items no longer appear in search results.

## Test Execution
- The tests are executed in Google Chrome using Selenium WebDriver.
- A test user logs in automatically before running tests.
- Web elements are identified using XPath and CSS selectors.
- Assertions validate expected behaviors and page navigations.

## Notes
- The test suite navigates to the application's login page and signs in before execution.
- Ensure the base URL in `BaseUrl` is correctly configured before running tests.
- The "Edit Food" functionality has a known issue where food titles cannot be changed.

## Contribution
If you would like to contribute:
1. Fork the repository.
2. Create a new branch: `git checkout -b feature-branch`
3. Make changes and commit: `git commit -m "Add new test case"`
4. Push to the branch: `git push origin feature-branch`
5. Open a pull request.
