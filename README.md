# Online Comic & Novel Reading Platform (OnComics)

OnComics is a modern, scalable backend platform that provides APIs for reading comics and novels online.  
The system supports browsing, reading, interactions, favorites, comments, ratings, and more — all powered by a clean relational database design.

---

## Key Goals
- Deliver a smooth and reliable API backend for comic & novel content.
- Support structured chapter management with multiple content sources.
- Provide complete interaction features: comments, reactions, favorites, ratings.
- Ensure scalable data integrity with a well-defined relational schema.
- Enable admin/owner to manage content and maintain quality.

---

## Highlight Features  
- **Authentication**: Register, Login, Reset password  
- **Comic & Chapter Management**: Comic metadata, categories, chapter listing, multi-source chapter images  
- **Reading Experience**: Reading progress (History), continue-reading, favorites, search & filtering  
- **Interactions**: Reactions, ratings, comments, comment attachments
- **User Experience**: Personalized favorites, category/tag filtering, full-text search
- **Platform Management**: Manage comics, categories, chapters, comments, and interactions 
- **Media Handling**: Optimized chapter images, thumbnails, and file storage

---

## Technologies
- **Backend**: ASP.NET Core 8 Web API + EF Core  
- **Database**: MySql 8, Redis
- **Integrations**: [Google OAuth](https://developers.google.com/identity/protocols/oauth2), [Appwrite](https://appwrite.io/), [ImageSharp](https://sixlabors.com/products/imagesharp/)

---

## Installation (For Local Development)

### 1. Setup Database

**1.1 Install MySQL**<br>
[Instructor](https://dev.mysql.com/doc/mysql-installation-excerpt/5.7/en/)

**1.2 Create thhe database (run schema script)**

In the `OnComics.Scripts` folder run the `OnComics-DB-Script.sql` script to create the database schema used by the project.

**1.3 (Optional) Import sample data**

If you want sample/test data for API testing, run the `OnComics-Sample-Data-Script.sql` script.

### 2. Setup Project

**2.1 Install Visual Studio 2022**<br>
[Instructor](https://learn.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2022)

**2.2 Install Git**<br>
[Instructor](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)

**2.3 Clone the project to your computer**

**2.4 Open and run the porject solution file**

In the `OnComics.BE`, open the `OnComcics.BE.sln` file with Visual Studio, the project solution.

**2.5 Configure application settings**

Open `appsettings.json` and set the external integrations and environment-specific values.

***2.5.1 Configure the JWT setting***

Find the Authentication section then Jwt section and fill required elements bellow.
<!--
![Jwt Section](https://github.com/user-attachments/assets/202b6b27-c6f5-4f4c-8c73-916c541df4ba)
-->
***2.5.2 Configure the Database Connection String setting***

Find the DefaultConnection in the Database ConnectionStrings section and fill required elements bellow.
<!--
![Database Section](https://github.com/user-attachments/assets/00845524-0df5-4016-9463-973ef5212ff4)
-->
***2.5.3 Configure the Email SMTP Server setting***

Find the EmailSettings section and fill required elements bellow.
<!--
![Email Section](https://github.com/user-attachments/assets/b13befaf-6095-4a70-b53c-171f2538ebab)
-->
***2.5.4 Configure the Google Oauth Authentication setting***

Find the Authentication section the Google section and fill required elements bellow.
<!--
![Google Auth Section](https://github.com/user-attachments/assets/f3060f28-3220-4efc-82c6-7febe186f2ec)
-->
***2.5.5 Configure the Redis Cloud setting***

Find the Redis section and fill required elements bellow.
<!--
![Redis Section](https://github.com/user-attachments/assets/45bcae0a-61b8-4598-872f-b845e447cb0a)
-->
***2.5.6 Configure the Appwrite API Key setting***

Find the Appwrite section and fill required elements bellow.
<!--
![Appwrite Section](https://github.com/user-attachments/assets/e7df77ea-5d81-49a9-b7aa-5911dcb8cb3e)
-->
**2.6 Build and run the project**

Build the solution (Build → Build Solution).

Run (F5) or launch without debugging (Ctrl+F5).

---
