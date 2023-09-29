# Bet Data Provider

This application requests bet data from an XML feed every 60 seconds. The XML data is then parsed, the new entities are added, the existing entities are updated if a change is present. If any match / bet / odd dissapears from the feed, its status is updated in the database, if any inactive match / bet / odd reappears in the feed its status is once again updated accordingly. Additionally any relevant change triggers a change message to be inserted in the database. The application exposes 2 endpoints which serve to retrieve specific match data from the database. This application was a task from UltraPlay.

## Libraries and technologies
- Entity Framework Core
- Microsoft SQL Server
- AutoMapper
- Swagger
