
CREATE TABLE Product (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL
);


CREATE TABLE Auction (
    Id INT IDENTITY(1,1) PRIMARY KEY,        -- separate PK
    ProductId INT NOT NULL,                  -- FK to Product

    StartingPrice DECIMAL(18,2) NOT NULL,
    CurrentPrice DECIMAL(18,2) NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,

    BidCount INT NOT NULL DEFAULT(0),
    WinnerUserId NVARCHAR(450) NULL,         -- Identity user key (string)
    RunnerUpUserId NVARCHAR(450) NULL,

    Status NVARCHAR(20) NOT NULL
);


ALTER TABLE Auction
ADD CONSTRAINT FK_Auction_Product
FOREIGN KEY (ProductId) REFERENCES Product(Id)
ON DELETE CASCADE;


CREATE TABLE Bid (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    AuctionId INT NOT NULL,
    UserId NVARCHAR(450) NOT NULL,           -- Identity user FK (string)
    Amount DECIMAL(18,2) NOT NULL,
    BidTime DATETIME NOT NULL
);


ALTER TABLE Bid
ADD CONSTRAINT FK_Bid_Auction
FOREIGN KEY (AuctionId) REFERENCES Auction(Id)
ON DELETE CASCADE;

ALTER TABLE Bid
ADD CONSTRAINT UQ_Bid_UserPerAuction UNIQUE (AuctionId, UserId);

-- Sample data 

INSERT INTO Product (Name, Category, Description) VALUES
('Dior Sauvage', 'Perfumes', 'Fresh and powerful fragrance'),
('Dyson Vacuum V11', 'Home Appliances', 'Strong suction cordless vacuum'),
('Honda Civic 2020 Model', 'Automobile', 'Used car in a great condition'),
('Zara Denim Jacket', 'Fashion Outfit', 'Premium blue denim jacket'),
('MacBook Pro 2023', 'Laptops', 'M2 Pro model, excellent performance'),
('Rolex Submariner', 'Luxury Watches', 'Classic luxury dive watch');


INSERT INTO Auction (ProductId, StartingPrice, CurrentPrice, StartTime, EndTime, BidCount, WinnerUserId, RunnerUpUserId, Status)
VALUES
(1, 5000, 5000, DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,1,GETDATE()), 0, NULL, NULL, 'active'),
(2, 15000, 15000, GETDATE(), DATEADD(DAY,2,GETDATE()), 0, NULL, NULL, 'active'),
(3, 2000000, 2000000, DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,3,GETDATE()), 0, NULL, NULL, 'active'),
(4, 3500, 3500, GETDATE(), DATEADD(DAY,5,GETDATE()), 0, NULL, NULL, 'active'),
(5, 250000, 250000, DATEADD(HOUR,-5,GETDATE()), DATEADD(DAY,1,GETDATE()), 0, NULL, NULL, 'active'),
(6, 1200000, 1200000, GETDATE(), DATEADD(DAY,4,GETDATE()), 0, NULL, NULL, 'active');

