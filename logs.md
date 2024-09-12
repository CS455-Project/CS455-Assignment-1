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
### 10-09-2024

**Task(s) Completed:**
- Added Test coverage to CI Pipeline

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

---

## Resources and References 

