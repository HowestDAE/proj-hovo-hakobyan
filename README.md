# Tool Development using C#

For this project, I used Riot API to build a WPF application that provides information about League of Legends players. The app consists of different pages with unique functionality. I will talk about each page, but first, let's talk API keys.

## Riot API Keys
Riot API works with 2 types of API keys.

### Production Key 
This key is used for "official" software, for which you need approval from Riot games. Once Riot approves of your application, you get a permanent API key. This is too much for a school project, so I used a Development key.

### Dev key
This key can be generated by everyone using their Riot account. It is meant for development purposes. The only downside is the fact that you need to manually regenerate the key every 24 hours. While this seemed like an issue for this project, I got "creative" and solved most of the manual work inside the Validation page.

## Validation page
This is the first page in the software. Here I check if the API key that I currently use (from a txt file) is still valid. Doing a simple API call tells me if I can still use this key or not. If the key is still valid, great the software goes to the Menu page.

If the key is not valid, then I use a bot that automatically takes the user up to the point where they need to click on "Generate API key" button in Riot Dev Portal. One click, and the new API key is inside the local txt file and the software can be used again.

**Note:** After generating a new key, wait a couple of minutes until the key is active.

## Menu page
This page gives you the option to go to Leaderboards page or Search page

## Leaderboard page
Here I show the top 15 League of Legends players in Europe West. I try to do this using the API first, but since there are limits, doing too many calls can cause issues. To solve this I fallback on a local version of the data if i get a **BadResponse** from the API. If there are no issues, then I overwrite the local version so that it's up-to-date.

Clicking on a player will take you to the Details page.

## Details page
This page provides more information about the chosen player. Summoner name, level, icon, top 5 masteries etc.

## Search page
This page provides the functionality to look for players by their usernames in Europe West server. When the player is found, the details page will provide more information about this player. I also save the last 5 searches to show them as search history.
