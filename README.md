# CS455 Assigment-1 
This repository consists of the codebase for the Whack'Em All project, completed as the first Assignment for the course CS455 : Introduction to Software Engineering, under Prof. Sruti S Ragavan, department of CSE, IIT Kanpur

## Release Notes
**Version** : 2.0 

**Date** : 12 September 2024

**Notes** : Added Unit Test, Code Quailty Gates and Resolved Code Issues using Sonar Cloud

## Links

- The source code for the game is in the `src/` folder and can be accessed using [this](https://github.com/CS455-Project/CS455-Assignment-1/tree/main/src) link.
- The `TestProject/` folder contains the unit tests and coverage report of the source code and can be accessed using [this](https://github.com/CS455-Project/CS455-Assignment-1/tree/main/TestProject) link.
- The game is deployed on GitHub pages and can be accessed by following [this](https://cs455-project.github.io/CS455-Assignment-1/) link.
- For detailed code quality and coverage analysis, visit our [SonarCloud project page](https://sonarcloud.io/project/overview?id=CS455-Project_CS455-Assignment-1).

## Team 
1. Arush Upadhyaya | Roll No: 220213
2. Wattamwar Akanksha Balaji | Roll No: 221214


## Description 
1. The game involves a grid, which has, at any point of time, exactly of its cell occupied by the character. 
2. The goal is hit the character by clicking on it, which awards the player an increment in score by 1.
3. The player is supposed to achieve the maximum possible score within 1 minute.

## Requirements 
For local deployment, this app requires `.NET Core SDK 8.0.107` to be installed on the system.

## Instructions 
To run the application locally, follow the following steps &rarr;
1. In the directory of your choice, clone this repo 
```bash
git clone https://github.com/CS455-Project/CS455-Assignment-1
```
2. To run locally, make sure you are in the `src/` directory and run &rarr;
```bash
dotnet watch
```
3. Within a few seconds, this should direct you to your browser, with locally running version of the game.
## Testing
The project uses the following tools for testing and quality assurance:

### Tools for Unit Tests
- **xUnit**: This is the testing framework used for unit testing the application's code. It is known for its simplicity and ease of use.
- **bUnit**: This is used for testing Blazor components specifically. It allows for testing the UI components and their interactions in isolation.
- **NUnit**: A popular unit testing framework for .NET applications, known for its flexibility and rich set of attributes for organizing and running tests.
- **Moq**: A mocking library for .NET, used to create mock objects for testing dependencies, allowing for behavior verification and isolation of components.

### Tools for Code Quality
- **SonarCloud**: This tool has been integrated to enforce quality gates on the codebase. SonarCloud provides continuous inspection of code quality and helps identify bugs, code smells, and security vulnerabilities. Quality gates are used to ensure that the code meets predefined standards before it is merged or released.
  
- **SonarLint**: Along with SonarCloud, we used SonarLint explicitly as linters for our codebase.
  
- **dotCover**: This tool is used to create code coverage reports. It helps in identifying which parts of the code are covered by tests and which are not. Ensuring high code coverage is essential for maintaining code quality and reliability.

## Running Tests and Coverage Reports
To run tests, make sure you are in the `TestProject/` directory and run the following command &rarr;
```bash
dotnet test
```