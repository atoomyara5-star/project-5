CREATE DATABASE LibraryD;
GO
USE LibraryD;
GO

-------------------------------------------------
-- Users
-------------------------------------------------
CREATE TABLE Users(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE NOT NULL,
    CONSTRAINT chk_Emailaddress CHECK (Email LIKE '%@ses.yu.edu.jo'),
    Password VARCHAR(300) NOT NULL,
    Phone VARCHAR(20),
    Role VARCHAR(20) DEFAULT 'Student',
    Profile_Picture VARCHAR(MAX)
);

-------------------------------------------------
-- Category
-------------------------------------------------
CREATE TABLE Category(
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL
);

-------------------------------------------------
-- Status
-------------------------------------------------
CREATE TABLE Status(
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName VARCHAR(50)
);

-------------------------------------------------
-- Books
-------------------------------------------------
CREATE TABLE Books(
    BookId INT IDENTITY(1,1) PRIMARY KEY,
    BookName VARCHAR(200) NOT NULL,
    Author VARCHAR(150),
    CategoryId INT,
    StatusId INT,
picture varchar(500),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    FOREIGN KEY (StatusId) REFERENCES Status(StatusId)
);

-------------------------------------------------
-- Rooms
-------------------------------------------------
CREATE TABLE Rooms(
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    RoomName VARCHAR(100) NOT NULL,
    Capacity INT NOT NULL,
    StatusId INT,

    FOREIGN KEY (StatusId) REFERENCES Status(StatusId)
);

-------------------------------------------------
-- Borrowings
-------------------------------------------------
CREATE TABLE Borrowings(
    BorrowId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    BookId INT,
    BorrowDate DATETIME DEFAULT GETDATE(),
    ReturnDate DATETIME,
    StatusId INT,

    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (StatusId) REFERENCES Status(StatusId)
);

-------------------------------------------------
-- Room Reservations
-------------------------------------------------
CREATE TABLE RoomReservations(
    ReservationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    RoomId INT,
    Reservation_Date DATE,
    Start_Time TIME,
    End_Time AS DATEADD(HOUR,2,Start_Time),
    StatusId INT,

    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId),
    FOREIGN KEY (StatusId) REFERENCES Status(StatusId)
);

-------------------------------------------------
-- Feedback
-------------------------------------------------
CREATE TABLE Feedback(
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    Message VARCHAR(500),
    Date DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-------------------------------------------------
-- Blacklist
-------------------------------------------------
CREATE TABLE Blacklist(
    BlacklistId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    Reason VARCHAR(300),
    DateAdded DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(UserId)
); 
CREATE TABLE Notifications(
Id INT IDENTITY PRIMARY KEY,
UserId INT,
Message VARCHAR(300),
IsRead BIT DEFAULT 0,
Date DATETIME DEFAULT GETDATE()
);

INSERT INTO Status(StatusName)
VALUES ('Available'), ('Borrowed'), ('Pending'), ('Confirmed');
