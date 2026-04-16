# EggBoard

Leaderboard and data management application for Egg, Inc. With complementing open user webpage.

## Tech-stack

Typescript + React
C Sharp
[...]

## Architecture

Frontend with user statistics and more. (Login with EID)

Backend with data parsing, leaderboard construction and a custom cloudflare worker for interactions with the Egg, Inc. API

Database storing user data pseudonymously
[...]

## Feature plan

[...]

### Week 5
*Kick-off week - no features to be planned here*

### Week 6
**Feature 1:** Basic Login System

**Feature 2:** EggApi Data Fetch raw

### Week 7
*Winter vacation - nothing planned.*

### Week 8
**Feature 1:** Account Linking

**Feature 2:** Stats Display

### Week 9
**Feature 1:** Leaderboard page part 1

**Feature 2:** Account Level Calculation*

### Week 10
**Feature 1:** Create User

**Feature 2:** Update User

### Week 11
**Feature 1:** Delete User

**Feature 2:** Leaderboard page part 2

### Week 12
**Feature 1:** EggApi Data Fetch final

**Feature 2:** Themes*

### Week 13
**Feature 1:** Leaderboard page part 3

**Feature 2:** Account linking with Alt Account Support

### Week 14
*Easter vacation - nothing planned.*

### Week 15
**Feature 1:** Toggle feature*

**Feature 2:** Toggle feature*

## Feature Flags

The application uses feature flags to enable or disable specific functionality at runtime. Configuration is located in `appsettings.json` under the `FeatureFlags` section.

| Flag | Default | Description |
| --- | --- | --- |
| `Leaderboards` | `true` | Enables or disables leaderboard functionality. |
| `Admin` | `true` | Enables or disables administrative API endpoints (e.g., user management). |

## DevOps & Automation

This project utilizes a modern DevOps stack to ensure code quality and automated delivery.

### CI/CD Pipeline
- **DevOps Pipeline**: Automatically triggered on pushes to `main`, `Dev`, `Dev2`, `StrykerTest`, and `FlyTestStaging`.
  - **Build & Test**: Compiles the solution and runs unit tests.
  - **Code Coverage**: Generates reports for test coverage.
  - **Mutation Testing**: Employs [Stryker.NET](https://stryker-mutator.io/) to evaluate test suite effectiveness by injecting faults (mutants) into the code. Reports are available as build artifacts.

### Deployment (Fly.io)
We use [Fly.io](https://fly.io/) for hosting with a multi-environment strategy:

- **Staging Environment**:
  - Automatically deployed on pushes to development branches.
  - Client: [devopsclient.fly.dev](https://devopsclient.fly.dev)
  - Server: [server-spring-cloud-8981.fly.dev](https://server-spring-cloud-8981.fly.dev)
- **Production Environment**:
  - Deployed from the `FlyTestStaging` branch after successful staging verification.
  - Client: [eggincproductionclient.fly.dev](https://eggincproductionclient.fly.dev)
  - Server: [eggincproductionserver.fly.dev](https://eggincproductionserver.fly.dev)

### Agentic Workflows
The repository includes automated AI-driven workflows located in `.github/workflows/`:
- **Code Simplifier**: Analyzes daily changes and proposes refinements for better readability and maintainability.
- **Daily Documentation Updater**: Ensures this documentation reflects the latest code changes.
- **Daily Repo Status**: Provides daily summaries of repository activity and project progress.

