# MAL_Score_Apps
## Description
It is a pet project made for MyAnimeList user score analysis.

Consists of two separate parts:
  - Analyser, which fetches data from MAL API, processes it, and stores it into database;
  - API, which provides data from database for further use;

## Setup
Database connection string should be provided at ./Shared/sharedsettings.json , as it is shown in ./Shared/sharedsettings.json.template .
MAL API authorisation header and base Uri for building links to heatmaps should be provided at ./MAL_Score_Analyzer/settings.json , as it is shown in ./MAL_Score_Analyzer/settings.json.template .
