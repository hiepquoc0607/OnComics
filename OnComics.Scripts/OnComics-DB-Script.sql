DROP DATABASE IF EXISTS OnComics_Database;

CREATE DATABASE IF NOT EXISTS OnComics_Database;

USE OnComics_Database;

DROP TABLE IF EXISTS Interaction,
                     History,
                     Favorite, 
                     Comment, 
                     ComicRating,
                     ComicCategory,
                     ChapterSource,
                     Chapter,
                     InteractionType,
                     Category,
                     Comic,
                     Account;

CREATE TABLE Account (
    Id BINARY(16) PRIMARY KEY,
    Fullname NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash TEXT NULL,
    Dob DATE NULL,
    Gender VARCHAR(10) NULL,
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
    Id BINARY(16) PRIMARY KEY,
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
    Id BINARY(16) PRIMARY KEY,
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Description TEXT NULL,
    Status VARCHAR(10) NOT NULL
);

CREATE TABLE InteractionType (
    Id BINARY(16) PRIMARY KEY,
    Name NVARCHAR(100) UNIQUE NOT NULL,
    ImgUrl TEXT NULL,
    Status VARCHAR(10) NOT NULL
);

CREATE TABLE Chapter (
    Id BINARY(16) PRIMARY KEY,
    ComicId BINARY(16) NOT NULL,
    ChapNo INT NOT NULL,
    Name NVARCHAR(100) NULL,
    ReadNum INT NOT NULL,
    ReleaseTime DATETIME NOT NULL,
    Status VARCHAR(10) NOT NULL,
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id),
    UNIQUE (ComicId , ChapNo)
);

CREATE TABLE ChapterSource (
    Id BINARY(16) PRIMARY KEY,
    ChapterId BINARY(16) NOT NULL,
    SrcUrl TEXT NOT NULL,
    ViewUrl TEXT NULL,
    Arrangement INT NOT NULL,
    IsImage BOOL CHECK(IsImage IN (0,1)) NOT NULL,
    FOREIGN KEY (ChapterId)
        REFERENCES Chapter (Id)
);

CREATE TABLE ComicCategory (
    Id BINARY(16) PRIMARY KEY,
    ComicId BINARY(16) NOT NULL,
    CategoryId BINARY(16) NOT NULL,
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id),
    FOREIGN KEY (CategoryId)
        REFERENCES Category (Id),
	UNIQUE (ComicId, CategoryId)
);

CREATE TABLE ComicRating (
    Id BINARY(16) PRIMARY KEY,
    AccountId BINARY(16) NOT NULL,
    ComicId BINARY(16) NOT NULL,
    Rating DECIMAL(2,1) NOT NULL CHECK (Rating >= 0.0 AND Rating <= 5.0),
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id),
	UNIQUE (AccountId, ComicId)
);

CREATE TABLE Comment (
    Id BINARY(16) PRIMARY KEY,
    AccountId BINARY(16) NOT NULL,
    ComicId BINARY(16) NOT NULL,
    Content TEXT NOT NULL,
    IsEdited BOOL CHECK(IsEdited IN (0,1)) NOT NULL,
    IsMainCmt BOOL CHECK(IsMainCmt IN (0,1)) NOT NULL,
    MainCmtId BINARY(16) NULL,
    CmtTime DATETIME NOT NULL,
    InteractionNum INT NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id),
	FOREIGN KEY (MainCmtId)
        REFERENCES Comment (Id)
);

CREATE TABLE Attachment (
    Id BINARY(16) PRIMARY KEY,
    CommentId BINARY(16) NOT NULL,
    StorageUrl TEXT NOT NULL,
    FOREIGN KEY (CommentId)
        REFERENCES Comment (Id)
);

CREATE TABLE Favorite (
    Id BINARY(16) PRIMARY KEY,
    AccountId BINARY(16) NOT NULL,
    ComicId BINARY(16) NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ComicId)
        REFERENCES Comic (Id),
	UNIQUE (AccountId, ComicId)
);

CREATE TABLE History (
    Id BINARY(16) PRIMARY KEY,
    AccountId BINARY(16) NOT NULL,
    ChapterId BINARY(16) NOT NULL,
    ReadTime DATETIME NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (ChapterId)
        REFERENCES Chapter (Id),
	UNIQUE (AccountId, ChapterId)
);

CREATE TABLE Interaction (
    Id BINARY(16) PRIMARY KEY,
    AccountId BINARY(16) NOT NULL,
    CommentId BINARY(16) NOT NULL,
    TypeId BINARY(16) NOT NULL,
    ReactTime DATETIME NOT NULL,
    FOREIGN KEY (AccountId)
        REFERENCES Account (Id),
    FOREIGN KEY (CommentId)
        REFERENCES Comment (Id),
    FOREIGN KEY (TypeId)
        REFERENCES InteractionType (Id),
	UNIQUE (AccountId, CommentId)
);