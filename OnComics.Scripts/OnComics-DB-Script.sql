DROP DATABASE IF EXISTS OnComics_Database;

CREATE DATABASE IF NOT EXISTS OnComics_Database;

USE OnComics_Database;

DROP TABLE IF EXISTS Interaction,
                     LeaderBoard,
                     History,
                     Favortite, 
                     Comment, 
                     ComicRating,
                     ComicCategory,
                     ChapterSource,
                     Chapter,
                     InteractionType,
                     LeaderBoardType,
                     Category,
                     Comic,
                     Account;

CREATE TABLE Account (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Fullname NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash TEXT NOT NULL,
    Dob DATE NOT NULL,
    Gender VARCHAR(10) NOT NULL,
    ImgUrl TEXT NULL,
    IsGoogle BOOL CHECK(IsGoogle IN (0,1)) NOT NULL,
    IsVerified BOOL CHECK(IsVerified IN (0,1)) NOT NULL,
    RefreshToken TEXT NULL,
    RefreshExpireTime DATETIME NULL,
    FCMToken TEXT NULL,
    Role VARCHAR(10) NOT NULL,
    Status VARCHAR(10) NOT NULL
);

CREATE TABLE Comic (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    Author NVARCHAR(100) NOT NULL,
    ReleaseDate DATE NOT NULL,
    UpdateTime DATETIME NOT NULL,
    ThumbnailUrl TEXT NOT NULL,
    Rating DECIMAL(3 , 1 ) NOT NULL,
    RateNum INT NOT NULL,
    ChapNum INT NOT NULL,
    FavoriteNum INT NOT NULL,
    DayReadNum INT NOT NULL,
    WeekReadNum INT NOT NULL,
    MonthReadNum INT NOT NULL,
    TotalReadNum INT NOT NULL,
    IsNovel BOOL CHECK(IsNovel IN (0,1)) NOT NULL,
    Status VARCHAR(10) NOT NULL
);

CREATE TABLE Category (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Description TEXT NULL,
    Status VARCHAR(10) NOT NULL
);

CREATE TABLE LeaderboardType (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Description TEXT NULL,
    Status VARCHAR(10) NOT NULL
);

CREATE TABLE InteractionType (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name NVARCHAR(100) UNIQUE NOT NULL,
    ImgUrl TEXT NULL,
    Status VARCHAR(10) NOT NULL
);

CREATE TABLE Chapter (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ComicId INT NOT NULL,
    ChapNo INT NOT NULL,
    Name NVARCHAR(100) NULL,
    ReadNum INT NOT NULL,
    ReleaseTime DATETIME NOT NULL,
    Status VARCHAR(10) NOT NULL,
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id)
);

CREATE TABLE ChapterSource (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ComicId INT NOT NULL,
    SrcUrl TEXT NOT NULL,
    Arrangement INT NOT NULL,
    IsEditable BOOL CHECK(IsEditable IN (0,1)) NOT NULL,
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id)
);

CREATE TABLE ComicCategory (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ComicId INT NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id),
    FOREIGN KEY (CategoryId)
        REFERENCES Category (Id)
);

CREATE TABLE ComicRating (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AccountId INT NOT NULL,
    ComicId INT NOT NULL,
    Rating DECIMAL(2,1) NOT NULL CHECK (Rating >= 0.0 AND Rating <= 5.0),
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id)
);

CREATE TABLE Comment (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AccountId INT NOT NULL,
    ComicId INT NOT NULL,
    Content TEXT NOT NULL,
    IsMainCmt BIT NOT NULL,
    MainCmtId INT NULL,
    CmtTime DATETIME NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id)
);

CREATE TABLE Favorite (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AccountId INT NOT NULL,
    ComicId INT NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id)
);

CREATE TABLE History (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AccountId INT NOT NULL,
    ChapterId INT NOT NULL,
    ReadTime DATETIME NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ChapterId)
        REFERENCES Chapter (Id)
);

CREATE TABLE Leaderboard (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ComicId INT NOT NULL,
    TypeId INT NOT NULL,
    RankNo INT NOT NULL,
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id),
    FOREIGN KEY (TypeId)
        REFERENCES LeaderBoardType (Id)
);

CREATE TABLE Interaction (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AccountId INT NOT NULL,
    CommentId INT NOT NULL,
    TypeId INT NOT NULL,
    ReactTime DATETIME NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (CommentId)
        REFERENCES Comment (Id),
    FOREIGN KEY (TypeId)
        REFERENCES InteractionType (Id)
);