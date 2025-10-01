USE OnComics_Database;

#Sample Data For Account Table
INSERT INTO Account 
(Fullname, Email, PasswordHash, Dob, Gender, ImgUrl, IsGoogle, IsVerified, RefreshToken, RefreshExpireTime, FCMToken, Role, Status) 
VALUES
('Admin', 'admin@gmail.com', '5f4dcc3b5aa765d61d8327deb882cf99', '1995-03-15', 'MALE', NULL, 0, 1, NULL, NULL, NULL, 'ADMIN', 'ACTIVE'),
('User', 'user@gmail.com', '6cb75f652a9b52798eb6cf2201057c73', '1998-07-20', 'FEMALE', NULL, 1, 1, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('User2', 'user2@gmail.com', '098f6bcd4621d373cade4e832627b4f6', '1990-12-05', 'MALE', NULL, 0, 0, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('User3', 'user3@gmail.com', 'ad0234829205b9033196ba818f7a872b', '1999-04-22', 'FEMALE', NULL, 1, 1, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('User4', 'user4@gmail.com', '8ad8757baa8564dc136c1e07507f4a98', '1988-01-10', 'MALE', NULL, 0, 1, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('User5', 'user5@gmail.com', '6512bd43d9caa6e02c990b0a82652dca', '1997-09-17', 'FEMALE', NULL, 1, 0, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('User6', 'user6@gmail.com', 'c20ad4d76fe97759aa27a0c99bff6710', '1992-11-30', 'MALE', NULL, 0, 1, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('User7', 'user7@gmail.com', 'c51ce410c124a10e0db5e4b97fc2af39', '1996-02-28', 'MALE', NULL, 1, 1, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('User8', 'user8@gmail.com', 'aab3238922bcc25a6f606eb525ffdc56', '2000-06-14', 'FEMALE', NULL, 0, 0, NULL, NULL, NULL, 'USER', 'ACTIVE'),
('Admin2', 'admin2@gmail.com', '9bf31c7ff062936a96d3c8bd1f8f2ff3', '1994-08-09', 'MALE', NULL, 1, 1, NULL, NULL, NULL, 'ADMIN', 'ACTIVE');

#Sample Data For Comic Table
INSERT INTO Comic 
(Name, Description, Author, ReleaseDate, UpdateTime, ThumbnailUrl, Rating, RateNum, ChapNum, FavoriteNum, DayReadNum, WeekReadNum, MonthReadNum, TotalReadNum, IsNovel, Status) 
VALUES
('Dragon Legend', 'A fantasy story of a young hero destined to fight dragons.', 'Nguyen Van A', '2015-06-10', '2025-09-05 12:00:00', 'https://example.com/thumbs/dragon.jpg', 8.7, 1520, 120, 540, 320, 2100, 9000, 125000, 0, 'ONGOING'),
('Love in the Rain', 'A romantic novel set in Hanoi during the rainy season.', 'Tran Thi B', '2018-02-14', '2025-08-28 09:30:00', 'https://example.com/thumbs/love.jpg', 9.2, 2040, 45, 780, 220, 1500, 6500, 98500, 1, 'COMPLETED'),
('Cyber Shadows', 'A sci-fi action comic about hackers in a futuristic city.', 'Le Van C', '2020-11-01', '2025-09-01 18:45:00', 'https://example.com/thumbs/cyber.jpg', 8.1, 870, 80, 330, 140, 890, 4200, 62000, 0, 'ONGOING'),
('Eternal Blossom', 'A poetic novel of love and loss through time.', 'Pham Thi D', '2016-04-22', '2025-09-02 14:10:00', 'https://example.com/thumbs/blossom.jpg', 9.5, 3100, 60, 1250, 500, 3000, 12000, 180000, 1, 'COMPLETED'),
('Shadow Hunt', 'A dark fantasy about hunters fighting demons in the night.', 'Hoang Van E', '2017-10-30', '2025-09-06 20:20:00', 'https://example.com/thumbs/shadow.jpg', 7.9, 650, 95, 410, 260, 1700, 7200, 91000, 0, 'ONGOING'),
('City Beats', 'A slice-of-life story about youth and music in Saigon.', 'Nguyen Thi F', '2019-08-12', '2025-08-30 16:00:00', 'https://example.com/thumbs/city.jpg', 8.3, 1200, 30, 600, 180, 1200, 5100, 70300, 0, 'UPCOMING'),
('War of Gods', 'Epic battles between gods and mortals across realms.', 'Do Van G', '2014-12-05', '2025-09-03 11:40:00', 'https://example.com/thumbs/gods.jpg', 8.9, 2760, 200, 1450, 600, 4100, 18500, 250000, 0, 'COMPLETED'),
('Silent Melody', 'A heartwarming romance novel about two musicians.', 'Pham Van H', '2021-03-25', '2025-09-07 08:15:00', 'https://example.com/thumbs/melody.jpg', 9.0, 980, 25, 320, 150, 1100, 4800, 65500, 1, 'ONGOING'),
('Galaxy Riders', 'Adventures of a space crew exploring unknown galaxies.', 'Vu Thi I', '2013-07-19', '2025-09-04 19:50:00', 'https://example.com/thumbs/galaxy.jpg', 7.5, 430, 150, 290, 200, 1350, 5700, 80200, 0, 'ONGOING'),
('Forgotten Realm', 'A high fantasy novel with kingdoms, wars, and magic.', 'Nguyen Van J', '2012-01-08', '2025-09-08 10:25:00', 'https://example.com/thumbs/realm.jpg', 9.4, 3520, 310, 2120, 800, 5000, 22000, 330000, 1, 'COMPLETED');

#Sample Data For Category Table
INSERT INTO Category (Name, Description, Status) 
VALUES
('Action', NULL, 'ACTIVE'),
('Comedy', NULL, 'ACTIVE'),
('Romance', NULL, 'ACTIVE'),
('Fantasy', NULL, 'ACTIVE'),
('Sci-Fi', NULL, 'ACTIVE'),
('Horror', NULL, 'ACTIVE'),
('Drama', NULL, 'ACTIVE'),
('Adventure', NULL, 'ACTIVE'),
('Slice of Life', NULL, 'ACTIVE'),
('Sports', NULL, 'ACTIVE');

#Sample Data For InteractionType Table
INSERT INTO InteractionType (Name, ImgUrl, Status) 
VALUES
('LIKE', NULL, 'ACTIVE'),
('DISLIKE', NULL, 'ACTIVE'),
('LOVE', NULL, 'ACTIVE'),
('LAUGH', NULL, 'ACTIVE'),
('ANGRY', NULL, 'ACTIVE'),
('SAD', NULL, 'ACTIVE');

#Sample Data For Chapter Table
INSERT INTO Chapter (ComicId, ChapNo, Name, ReadNum, ReleaseTime, Status) 
VALUES
(1, 1, 'The Awakening', 1200, '2015-06-10 10:00:00', 'ACTIVE'),
(1, 2, 'Dragon’s Roar', 950, '2015-07-01 10:00:00', 'ACTIVE'),
(2, 1, 'First Encounter', 2100, '2018-02-14 09:00:00', 'ACTIVE'),
(2, 2, 'Rainy Streets', 1850, '2018-03-01 09:00:00', 'ACTIVE'),
(3, 1, 'Into the Grid', 870, '2020-11-01 18:00:00', 'ACTIVE'),
(3, 2, 'Hackers Unite', 650, '2020-11-15 18:00:00', 'ACTIVE'),
(4, 1, 'Petals of Time', 3100, '2016-04-22 14:00:00', 'ACTIVE'),
(5, 1, 'Darkness Falls', 600, '2017-10-30 20:00:00', 'ACTIVE'),
(5, 2, 'Hunter’s Oath', 450, '2017-11-15 20:00:00', 'ACTIVE'),
(6, 1, 'First Beat', 1200, '2019-08-12 16:00:00', 'ACTIVE');

#Sample Data For ChapterSource Table
INSERT INTO ChapterSource (ComicId, SrcUrl, Arrangement, IsEditable) 
VALUES
(1, 'https://example.com/comic1/source1', 1, 1),
(1, 'https://example.com/comic1/source2', 2, 0),
(2, 'https://example.com/comic2/source1', 1, 1),
(3, 'https://example.com/comic3/source1', 1, 0),
(3, 'https://example.com/comic3/source2', 2, 1),
(4, 'https://example.com/comic4/source1', 1, 1),
(5, 'https://example.com/comic5/source1', 1, 0),
(6, 'https://example.com/comic6/source1', 1, 1),
(6, 'https://example.com/comic6/source2', 2, 0),
(6, 'https://example.com/comic6/source3', 3, 1);

#Sample Data For ComicCategory Table
INSERT INTO ComicCategory (ComicId, CategoryId) 
VALUES
(1, 1),
(1, 4),
(2, 3),
(2, 7),
(3, 1),
(3, 5),
(4, 3),
(4, 7),
(5, 1),
(5, 6),
(6, 2),
(6, 8);

#Sample Data For ComicRating Table
INSERT INTO ComicRating (AccountId, ComicId, Rating) 
VALUES
(1, 1, 4.5),
(2, 1, 4.7),
(3, 2, 5.0),
(4, 2, 4.8),
(5, 3, 3.9),
(6, 3, 3.5),
(7, 4, 5.0),
(8, 5, 3.8),
(9, 6, 4.1),
(10, 6, 4.3);

-- Sample Data For Comment Table
INSERT INTO Comment (AccountId, ComicId, Content, IsEdited, IsMainCmt, MainCmtId, CmtTime, InteractionNum) 
VALUES
(1, 1, 'This comic is amazing! Can’t wait for the next chapter.', 0, 1, NULL, '2025-09-01 10:15:00', 2),
(2, 1, 'I love the dragon fight scenes.', 0, 1, NULL, '2025-09-01 10:30:00', 0),
(3, 1, 'Totally agree with you!', 0, 0, 1, '2025-09-01 10:45:00', 0),
(4, 2, 'Such a touching romance story ❤️', 0, 1, NULL, '2025-09-02 09:10:00', 0),
(5, 2, 'The ending made me cry.', 0, 1, NULL, '2025-09-02 09:25:00', 1),
(6, 2, 'Same here, I was in tears.', 0, 0, 5, '2025-09-02 09:40:00', 0),
(7, 3, 'Cyberpunk vibes are strong in this one.', 0, 1, NULL, '2025-09-03 14:20:00', 1),
(8, 3, 'The hacking scenes are epic!', 0, 0, 7, '2025-09-03 14:35:00', 0),
(9, 4, 'Eternal Blossom is a masterpiece.', 0, 1, NULL, '2025-09-04 11:50:00', 0),
(10, 5, 'Shadow Hunt has such a dark atmosphere, I love it.', 0, 1, NULL, '2025-09-05 20:05:00', 0);

#Sample Data For Favorite Table
INSERT INTO Favorite (AccountId, ComicId) 
VALUES
(1, 1),
(2, 1),
(3, 2),
(4, 2),
(5, 3),
(6, 3),
(7, 4),
(8, 5),
(9, 6),
(10, 6);

#Sample Data For History Table
INSERT INTO History (AccountId, ChapterId, ReadTime) 
VALUES
(1, 1, '2025-09-01 09:10:00'),
(1, 2, '2025-09-01 09:45:00'),
(2, 3, '2025-09-01 10:15:00'),
(3, 4, '2025-09-01 10:50:00'),
(4, 5, '2025-09-02 08:30:00'),
(5, 6, '2025-09-02 09:00:00'),
(6, 7, '2025-09-02 09:30:00'),
(7, 8, '2025-09-02 10:00:00'),
(8, 9, '2025-09-03 11:15:00'),
(9, 10, '2025-09-03 12:00:00');

#Sample Data For Interaction Table
INSERT INTO Interaction (AccountId, CommentId, TypeId, ReactTime) 
VALUES
(1, 1, 1, '2025-09-01 10:20:00'),
(2, 1, 2, '2025-09-01 10:25:00'),
(3, 2, 1, '2025-09-01 10:35:00'),
(4, 3, 3, '2025-09-01 10:50:00'),
(5, 4, 1, '2025-09-02 09:15:00'),
(6, 5, 2, '2025-09-02 09:35:00'),
(7, 6, 1, '2025-09-02 09:55:00'),
(8, 7, 4, '2025-09-03 14:25:00'),
(9, 8, 1, '2025-09-03 14:45:00'),
(10, 9, 2, '2025-09-04 12:05:00');








