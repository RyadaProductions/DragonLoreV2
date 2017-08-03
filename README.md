# DragonLoreV2

Rewrite of the Dragon Lore bot that is used in the CSGO Hub NL Discord.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

.NET Core

```
This project runs on the .NET Core framework
```

### Installing

A step by step series of examples that tell you have to get a development env running

Clone the repo.

```
Clone the repo to a directory on your system.
```

Restore nuget packages

```
You can restore nuget packages by running 'Nuget Restore' 
```
Add environment variable for the token

```
Create an environment variable called 'token' on your system
```

## Deployment

Copy the output to a system that supports .NET Core and run 'Dotnet DragonLore.dll'

## Built With

* [Discord.NET](https://github.com/RogueException/Discord.Net) - The library used to run the discord integration
* [AngleSharp](https://github.com/AngleSharp/AngleSharp) - Used to read the upcomming matches from HLTV.org
* [CoreRCON](https://github.com/ScottKaye/CoreRCON) - Used to check if CSGO servers are online and get some information
* [CodeHollow.FeedReader](https://github.com/codehollow/FeedReader) - Used to read RSS feeds


## Authors

* **Ryada** - *Main dev* - [RyadaProductions](https://github.com/RyadaProductions)

See also the list of [contributors](https://github.com/RyadaProductions/DragonLoreV2/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Received a lot of help from multiple people in improving the codebase, Thank you everyone from the r/C# discord.
