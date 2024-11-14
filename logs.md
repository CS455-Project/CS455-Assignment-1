# Project Log: Whack'Em All

## Overview
**Project Start Date:** 6th August 2024  
**Project End Date (Expected):** TBD  
**Project Description:** This project seeks to implement the Whack'a'Mole Game using Blazor Web Assembly Application

---

## Log Entries

### 07-09-2024

**Task(s) Completed:**  
- Integrated Sonarlint and refactored the code.

**Challenges Faced:**  
- Many attributes were made public unnecessarily.

**Solutions or Workarounds:**  
- Read the documentation by SonarLint and made the required changes in the codebase.

---
### 09-09-2024

**Task(s) Completed:**  
- Created Test Project
- Organised project into src and test
- Modified the yaml file to run tests automatically and fail the deployment on test case failure
- Installed Coverlet and used it to generate test coverage report

**Challenges Faced:**  
- Figuring out adding a project reference to a test project took a while
- Ordering of the functionalities in the yaml file.
- glibc error with running `reportgenerator` in linux device, ran in the windows machine as resort

**Solutions or Workarounds:**  
- Used an online blog to understand setup --> https://blog.openreplay.com/how-to--unit-testing-blazor-apps/

---
### 10-09-2024

**Task(s) Completed:**  
- Added tests for start game and end game
- Added tests for SetNextAppearence
- Linked the Project with SonarCloud
- Added Code quality gates using SonarCloud
- Added cognitive complexity check
- Added cyclomatic complexity check
- Added numerous gates on old code
- Added tests for restart game and return to start menu 
- Added tests for testing sound

**Challenges Faced:**
- StateHasChanged had to be place outside SetNext to test it

---

### 11-09-2024

**Task(s) Completed:**  
- Integrated SonarCloud with github for test coverage with dotCover.
- Fixed the issues that were causing the quality gates to fail on SonarCloud.
- Added unit tests to check whether the javascript code is working properly on certain events.

**Challenges Faced:**
- SonarCloud was initially configuring source files as test files and thus we spent a long time to resolve this.

**Solutions or Workarounds:**  
- We could solve this issue with the help of community support on internet.
---

### 12-09-2024

**Task(s) Completed:**  
- Added the instructions to play the game on the home page.

---
### 30-09-2024

**Task(s) Completed:**  
- Added JavaScript server side code to run application on server

---

### 05-10-2024

**Task(s) Completed:**  
- Verified existance of minimum 50% coverage gate.
---

### 06-09-2024

**Task(s) Completed:**  
- Implemented leaderboard functionality 
- Implemented user name retrieval and sending score to the server
- Integerated server with MongoDB to maintain Name and Score

**Challenges Faced:**
- Facing conflicts because of different operating systems.

**Solutions or Workarounds:**  
- Added .gitignore to ignore all dependenices and build files

---
### 07-09-2024

**Task(s) Completed:**  
- Added Styles to homepage, leaderboard and endgame screen
- Fixed Bug where EndGame test was failing

---
### 08-09-2024

**Task(s) Completed:**  
- Used Mock Game to test EndGame and StartGame
- Import `PORT` from `.env`
- Formatted code using `dotnet format`
- Replaced local host address by remote host
- Disabled exit on quality gate failure for Test Period
- Renamed namespace to include all tests in one namespace
- Moved Variable declarations from `Game.razor` to `Game.cs`
- Added unit tests for leaderboard and name prompt
- Added Integeration tests

---
### 09-09-2024

**Task(s) Completed:**  
- Added integeration test for Leaderboard
- Added tests for server side
- Added coverage of server side tests
- Stopped tracking server-tests, to work on it later
- Extracted server url as configuration constant
- Turned public variables to properties as suggested by SonarCloud
- Solved Sonar Issue where non-nullable and nullable types were clashing

**Challenges Faced:**
- Server tests are run using jest, but current pipeline runs and collects dotnet test
- Currently sonar cannot track server test coverage

---
### 10-09-2024
**Task(s) Completed:**  
- Code cleaning, removed dead code
- Removed unnecessary constructors and logic from Pages, to make the code simpler.
- Added back server tests, in the `server.test.js` in the `server/` directory
- Added coverage for server side test and sending report to sonar in the pipeline.
- Added architecture diagram and Test Pyramid
- Re-Enabled exit on Quality Gate failure.

---
### 07-11-2024
**Task(s) Completed:**  
- Added python file `load-time.py` for testing the load
- Added requirements.txt for python requirements

---
### 09-11-2024
**Task(s) Completed:**  
- Used artillery for load testing game and leaderboard service
- Added load balancer that balances load between two servers

---
### 12-11-2024
**Task(s) Completed:**  
- Hosted the two servers on render and added links to load_balancer

---
### 14-11-2024
**Task(s) Completed:**  
- Documentation added to README
- Scheduled running of performance tests added

---
## Summary 

**Milestone Achieved:** 
1. Game Ready to Play Locally.
2. Game Deployed using GitHub Pages.
3. Automated Testing During Deployment.
4. Code Quality and Test Report.
5. Integerate Code Linters.
6. Unit Testing.
7. Choice of Code Quality Metrics.
8. Instructions page for the Game.
9. Server Side code for leaderboard
10. MongoDB database for keeping previous score
11. Integerations Tests for Game-Server and Server-Database endpoints
12. Tests for server side code
13. Server hosted on render.
14. Implemetned load balancing on two servers
15. Added Backup Server
16. Added load-time tests and load-tests

---

## Resources and References 

